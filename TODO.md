# TODO

## 1. Site-deletion redesign — DONE
- [x] Flip these four `OnDelete(...)` from `Cascade` → `Restrict`:
  - `NetworkConfiguration.cs:36-39` (Site → Network)
  - `SiteIntegrationConfiguration.cs:36-39` (Site → SiteIntegration)
  - `PortalConfiguration.cs:33-36` (Network → Portal)
  - `SiteConfiguration.cs:36-39` (Organization → Site) — double-check intent before migrating: this means deleting an org with any sites will 409 instead of cascading, not just Site-level deletes changing behavior.
- [x] Decide explicitly on `NetworkConfiguration.cs:41-44` (Network → SiteIntegration) — stays `Cascade` or not.
- [x] Generate + apply the EF Core migration for those FK changes. (`20260703151101_RestrictConfigurationDeletes`)
- [x] Add dependency pre-checks to `SiteService.DeleteAsync` (Networks/SiteIntegrations exist?) so `SiteErrors.CannotDeleteWithActiveDependencies` becomes reachable again.

## 4. Stubbed Organization features (`NotImplementedException`)
Each feature is two-sided: the endpoint currently throws directly instead of calling the
service, so wire the endpoint → service call *and* implement the service method. Apply the
usual multi-tenant rule — derive the org from `ICurrentUser.OrganizationId`, and a mismatch
against the route `id` returns `NotFound` (see `GetAsync` for the established pattern).

- [ ] **Update organization** — `OrganizationEndpoints.cs:44` (UpdateOrganization) + `OrganizationService.cs:34` (UpdateAsync)
- [ ] **Delete organization** — `OrganizationEndpoints.cs:53` (DeleteOrganization) + `OrganizationService.cs:39` (DeleteAsync)
- [ ] **Get members** — `OrganizationEndpoints.cs:61` (GetMembers) + `OrganizationService.cs:44` (GetMembersAsync)
- [ ] **Add member** — `OrganizationEndpoints.cs:69` (AddMember) + `OrganizationService.cs:49` (AddMemberAsync)
- [ ] **Remove member** — `OrganizationEndpoints.cs:78` (RemoveMember) + `OrganizationService.cs:54` (RemoveMemberAsync)

## 2. Docs
- [ ] Add the "Domain Entity Design" section to `AGENTS.md` (finalized version, `EndSession()` corrected).

## 3. Tracing gap
- [ ] Add `EnrichDiagnosticContext` to `UseSerilogRequestLogging()` in `Program.cs` so `TraceId` gets attached to the request-summary log event.
- [ ] Add a Serilog Seq sink (package + `Serilog:WriteTo` entry + `Seq:ServerUrl` in User Secrets pointing at `localhost:5341`) so structured logs actually land in the running Seq container.