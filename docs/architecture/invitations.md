# Organization Invitations

Design decision record. Converged after review; captured so it isn't re-litigated later.

## Purpose

Organization invitations let administrators invite new members into an organization. An
invitation is a **first-class domain concept** (`OrganizationInvitation`) — never a disabled
`ApplicationUser` or a "pending" `OrganizationMembership`. `ApplicationUser` means a real
user; `OrganizationMembership` means a confirmed member. Pending state lives only on the
invitation.

## Lifecycle

Stored `Status`: `Pending → Accepted` or `Pending → Revoked`.

`Expired` is **not** a stored state — it is derived: `IsExpired => ExpiresAt <= UtcNow`
(same as `RefreshToken`). No sweeper job, no state to keep in sync with the clock.

`Accept`, `Revoke`, and `Resend` are all only valid from `Pending`.

## Security (mirrors `RefreshToken`)

- Cryptographically secure random **opaque** token (not a JWT — JWTs can't be revoked before
  expiry, leak claims, and persist in email/logs forever).
- Only the **SHA-256 hash** of the token is stored; the raw token exists only in the email.
  SHA-256 (a fast hash) is correct here because the token is already high-entropy — unlike
  passwords, it needs no slow hashing.
- Tokens are single-use, revocable, and time-limited (default expiry ~7 days). Short expiry
  is deliberate: the token proves the recipient controlled the mailbox *at least once*, not
  that they still do — a property that decays with time, so the window is kept small.
- The public URL carries the token, not the database id. A `<lookup-id>.<secret>` split is
  preferred so the id can be logged/indexed while the secret stays out of logs.

## Acceptance

A single transaction that:

1. validates the invitation (`Pending`, not expired, verified email matches),
2. creates the `OrganizationMembership`,
3. marks the invitation `Accepted`.

The **domain** expresses the rules (`Accept(userId, verifiedEmail)` guards internally).
**Database constraints** protect against a concurrent double-accept race — the unique index
on membership plus the filtered-pending unique index. A constraint violation surfaces as
`DbUpdateException` (inner `PostgresException`, since this path uses `SaveChangesAsync`) and
is mapped to an `AlreadyAccepted` outcome. That mapping is **concurrency recovery, not
business logic** — it should almost never fire.

## Email binding

Membership is created only for the **verified** email tied to the invitation. Proof of email
control differs by flow but yields the same invariant:

- **New user:** possession of a valid token (delivered to the invited inbox) is the proof.
- **Existing user:** their authenticated, already-verified session email must equal the
  invitation email.

`Email` (and `OrganizationId`, `Role`) are **immutable** on the aggregate. Changing the
invitee = revoke + create a new invitation. Mutating the email on a live token would silently
re-point an already-sent link at a different address, breaking the binding.

## Authorization

`CanAssignRole(inviterRole, invitedRole)` gates invitation **creation** — you cannot invite
someone to a role you couldn't grant yourself (prevents privilege escalation via the
onboarding path, e.g. an Admin minting an Owner).

Authority is evaluated **once, at creation** (snapshot-at-creation). A pending invitation is a
snapshot of the inviter's authority; later demotion/removal of the inviter does **not**
invalidate it. This is an intentional product decision — the escalation window is bounded by
expiry (~7 days), and it keeps member-management from having to reach into invitations on
every role change. Matches GitHub/Slack/Notion behavior.

> **Still open:** the concrete `CanAssignRole` table (Owner-invites-whom, Admin-invites-whom,
> whether Manager/Editor may invite at all) is not yet decided. It gates only the create
> path, so it does not block the aggregate, token, accept, revoke, or resend work. Fill it in
> before wiring `CreateAsync`'s authorization.

## Aggregate responsibilities

`OrganizationInvitation` owns:

- `Accept(userId, verifiedEmail)`, `Revoke(...)`, `Resend(newTokenHash)`
- `IsActive` (= `Pending && !IsExpired`) and `IsExpired`

`Resend` internally rotates the token (`RotateToken(newTokenHash)`), bumps `ResendCount`, sets
`LastSentAt`, and extends expiry — no external caller rotates a token without resending, so
`RotateToken` is not a standalone public verb.

The aggregate **never** generates, hashes, base64s, or URL-encodes a token — it only ever
receives a hash. Token generation and hashing live in Infrastructure, exactly as with
`RefreshToken`.

Audit fields: `CreatedByUserId`, `AcceptedAt`, `AcceptedByUserId`, `RevokedAt`,
`RevokedByUserId`, `LastSentAt`, `ResendCount`. Any IP/User-Agent capture is subject to GDPR
(EU tenants) — collect only with a named security purpose.

## Out of scope (for now)

Current schema assumes **one organization per user** — `OrganizationMembership` has a unique
index on `UserId` alone. This means the **existing-user, cross-organization** accept flow
cannot be built yet (it would violate that constraint). Ship the **new-user** flow now;
existing-user invitations land alongside a dedicated multi-organization project that relaxes
the index to `(UserId, OrganizationId)` and revisits the "one org from the JWT" assumption.
