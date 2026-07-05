# Multi-Tenancy & Tenant Isolation

OpenCaptive is multi-tenant: every organization's data must be invisible to every other
organization. This document explains *how* that isolation works and — more importantly — *why*
it's built this way, because the isolation is enforced by **discipline in every query**, not by
a single mechanism you can rely on to catch mistakes. If you're adding a feature that touches
tenant-owned data, read this first.

## The tenant is the Organization, and it comes from the token

Every resource (Site, Network, Portal, Integration, membership, invitation…) belongs to
exactly one `Organization`. The tenant boundary *is* the organization.

Here's the part that matters: **the current organization is never supplied by the client.** It
is read from the `org_id` claim inside the signed JWT access token, surfaced through
`ICurrentUser.OrganizationId` (see `ClaimsCurrentUser`). The claim is baked into the token at
login by `JwtAccessTokenGenerator` and protected by the token's signature.

Why this matters: a value the client can't set is a value the client can't forge. Because the
organization is derived from a signed token rather than a request field, a whole class of
attacks — "call the API but pass someone else's org id" — is impossible *by construction*.
There's nothing to tamper with.

This is why you'll see the rule stated bluntly elsewhere: **services take `ICurrentUser` by
constructor injection and derive the org from it; they never accept an organization id as a
method parameter, route segment, or request-body field.** An org id in a request signature is
a code smell — it means the door that the JWT closed has been propped back open.

## The core rule: scope every query, never query by id alone

Deriving the org isn't enough — you have to *use* it in every read and write. The rule is:

> Combine the entity id with `OrganizationId` in the query. Never look up an entity by its id
> alone.

For example, `SiteService` deletes with `x.Id == siteId && x.OrganizationId == currentOrgId`,
not just `x.Id == siteId`. The organization predicate is what makes the row invisible to
other tenants.

The threat this defends against is **IDOR** (Insecure Direct Object Reference). Ids in this
system are GUIDs, but treat them as guessable/enumerable anyway — defence in depth. Imagine
Alice is authenticated for her own org and calls `GET /sites/{someSiteId}` with a site id that
belongs to Bob's organization. If the query were `WHERE Id = someSiteId`, she'd get Bob's
site. Because it's `WHERE Id = someSiteId AND OrganizationId = <Alice's org from her JWT>`, she
gets nothing. Alice's own token is what scopes the query; she can't widen it.

## Nested resources scope through their parent

Resources form a hierarchy: `Organization → Site → Network → Portal`, and
`Site → Integration`. A Network doesn't carry an `OrganizationId` directly — it belongs to a
Site, which belongs to an Organization. So scoping *cascades through the parent*: to touch a
Network, first confirm its Site belongs to the caller's org, then confirm the Network belongs
to that Site. The org check still happens; it just happens at the top of the chain. Never skip
straight to `WHERE NetworkId = x` — that bypasses the tenant boundary entirely.

## Wrong-org returns 404, not 403

When a caller asks for a resource that exists but belongs to another organization, the answer
is **`NotFound` (404)** — the same response as for a resource that doesn't exist at all. This
is intentional. A `403 Forbidden` would confirm "this id exists, you just can't have it,"
which leaks the existence of other tenants' data. Collapsing "doesn't exist" and "not yours"
into one indistinguishable response means an attacker learns nothing from probing.

## There is no safety net — the discipline is load-bearing

This is the single most important thing for a contributor to internalize: **nothing
automatically scopes your queries.** There is no EF Core global query filter that silently
appends `OrganizationId = …` to every query. (The project also uses hard deletes, so there's
no soft-delete filter either — see `AGENTS.md`.) Every scoped query is scoped because a human
wrote the `OrganizationId` predicate by hand.

That's a deliberate choice — global filters are easy to forget you're depending on, and they
interact badly with the raw-SQL `ExecuteUpdate`/`ExecuteDelete` paths this codebase favors —
but it means the cost is eternal vigilance. Forget the `OrganizationId` clause in one query and
you have a silent cross-tenant data leak that no test will catch unless you wrote it to. When
reviewing or writing any data-access code, the reflex should be: *where is the tenant scope in
this query?* If you can't point to it, it isn't there.

## Current limitation: one organization per user

Today, `OrganizationMembership` has a **unique index on `UserId` alone** — a user belongs to
exactly one organization, and the JWT carries that single `org_id`. This keeps "the current
org" unambiguous.

When multi-organization support lands, several things change together: the membership index
relaxes to `(UserId, OrganizationId)`, the token/session model has to represent *which* of a
user's orgs is active (org-switching), and every "the org from the JWT" assumption in this
document has to be revisited. It's a coordinated project, not a config flip — which is exactly
why the [invitation design](./invitations.md) explicitly scopes the existing-user,
cross-org flow *out* until that project happens.
