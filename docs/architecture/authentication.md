# Authentication & Token Model

How OpenCaptive authenticates users and manages sessions. Security-critical — read this
before touching `AuthService`, the token generators, or the token EF configurations.

## Token types at a glance

The system uses **three different token mechanisms**, deliberately, because they solve
different problems:

| Token                | Form                          | Stored?                     | Lifetime |
| -------------------- | ----------------------------- | --------------------------- | -------- |
| Access token         | JWT (signed)                  | No — stateless              | Short    |
| Refresh token        | Opaque random, SHA-256 hashed | Yes — hash only, in DB      | Sliding  |
| MFA interim token    | JWT (signed)                  | No — stateless              | Very short |
| Email verify / reset | Identity DataProtection token | No — self-contained         | Short    |

The distinction that matters: **refresh (and invitation) tokens are opaque secrets stored as
hashes; email-verification and password-reset tokens are ASP.NET Identity DataProtection
tokens** — encrypted-and-signed, self-contained, and *not* stored. Don't unify them; they
have different revocation and validation properties.

## Access + refresh flow

1. On login (`AuthService`), the user gets a short-lived **JWT access token** and an opaque
   **refresh token**. Only a SHA-256 hash of the refresh token is persisted (`RefreshToken`
   entity, `token_hash` unique); the raw value is returned once and never stored.
2. The access token is a bearer JWT — stateless, validated by signature/expiry, carries the
   user and organization claims. No DB lookup per request.
3. When the access token expires, the client calls `/auth/refresh` with the refresh token.

## Refresh-token rotation with reuse detection

This is the non-obvious part. Every refresh **rotates**: the presented token is revoked and a
new one issued in the same `FamilyId`. Concretely, on `/auth/refresh`:

- Look up the token by its hash. Reject if missing or expired.
- **Reuse detection:** if the token is found but already *revoked*, someone is replaying a
  spent token — treat it as theft and revoke the **entire family**
  (`UserId + FamilyId`), forcing re-login. This is the classic rotation-with-reuse-detection
  pattern (à la Auth0/OAuth BCP).
- **Security-stamp binding:** the refresh token stores the user's `SecurityStamp` at issuance.
  On refresh it's compared to the user's *current* stamp; if they differ (e.g. the password
  was changed, which rotates the stamp), the refresh is rejected. This is how a password
  change invalidates outstanding sessions.
- **Atomic single-use:** the revoke-then-issue uses `ExecuteUpdateAsync` guarded by a
  rows-affected check. If the revoke touches zero rows, another request already consumed this
  token concurrently → revoke the family and reject. The DB is the arbiter under a race; the
  domain expresses intent.
- On success, a new access + refresh token pair is issued in the same family.

Logout revokes the whole family for that user.

### Sliding expiry — a deliberate tradeoff

Each refresh resets the configured refresh-token lifetime (see the note in `AuthService`), so
an **actively refreshed session effectively never expires**. This is a conscious
UX-vs-security tradeoff: convenience for active users, bounded only by the reuse-detection and
security-stamp mechanisms above, not by an absolute session cap. If an absolute cap is ever
wanted, it belongs on the family (earliest-issued-at), not the individual token.

## MFA

When two-factor is enabled, a successful password check does **not** immediately issue
access/refresh tokens. Instead it returns a short-lived **MFA interim JWT**
(`JwtTwoFactorTokenGenerator`), which the client exchanges at `/auth/verify-mfa` with the TOTP
code, or at `/auth/verify-mfa/recovery-code` with a recovery code. Only then are the real
tokens issued. MFA enrolment/management lives under `/profile/mfa/*`.

## Email verification & password reset

These use ASP.NET Identity's `DataProtectorTokenProvider` (see `TokenProviders.cs`), one named
provider each. The token is encrypted + signed + time-limited and carries its own payload — so
it is **not** stored in the database and is validated by decryption, not by hash lookup. This
is a different mechanism from refresh/invitation tokens on purpose: these are low-value,
single-purpose, and don't need independent revocation beyond their short expiry.

## Data Protection key ring

Because those two token types are validated **by decryption rather than by lookup**, they are
only as durable as the Data Protection key ring that encrypts them. The default ring is
in-memory: fine for `dotnet run`, quietly wrong everywhere else — every container restart
would invalidate verification and reset links already sitting in users' inboxes, and two
replicas would reject each other's tokens outright.

So the ring is persisted to Postgres (`OpenCaptiveDbContext` implements
`IDataProtectionKeyContext`; `AddOpenCaptiveDataProtection` wires
`PersistKeysToDbContext`). The database was chosen over a shared volume deliberately: the
tokens are already meaningless without that database, so co-locating the ring adds no failure
domain and no infrastructure.

Two consequences worth remembering:

- **`SetApplicationName("OpenCaptive")` is load-bearing.** It prefixes every purpose string;
  changing it is equivalent to discarding the ring.
- **The ring is stored unencrypted**, so startup logs "No XML encryptor configured". Accepted,
  not overlooked — a wrapping certificate deployed next to the database it protects moves the
  secret rather than securing it. Revisit if a real key vault or HSM ever exists.

## Layering

- **Domain** (`RefreshToken`) owns token *state* and its computed guards (`IsActive`,
  `IsExpired`, `IsRevoked`) — never RNG, hashing, or JWT concerns.
- **Infrastructure** owns secret generation and hashing (`ITokenHasher` / `Sha256TokenHasher`,
  the JWT generators, the Identity providers) and the `AuthService` orchestration.
- **Api** exposes the endpoints (`/auth/*`) and maps `Result` failures to Problem responses.

## Relationship to invitations

Organization invitation tokens (see [invitations.md](./invitations.md)) intentionally mirror
the **refresh-token** design: opaque random value, SHA-256 hash stored, revocable, expiring,
never in plaintext. Same reasoning, same pattern — reuse it, don't reinvent it.
