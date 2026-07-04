# TODO

## 1. Site-deletion redesign
- [ ] Flip these four `OnDelete(...)` from `Cascade` → `Restrict`:
  - `NetworkConfiguration.cs:36-39` (Site → Network)
  - `SiteIntegrationConfiguration.cs:36-39` (Site → SiteIntegration)
  - `PortalConfiguration.cs:33-36` (Network → Portal)
  - `SiteConfiguration.cs:36-39` (Organization → Site) — double-check intent before migrating: this means deleting an org with any sites will 409 instead of cascading, not just Site-level deletes changing behavior.
- [ ] Decide explicitly on `NetworkConfiguration.cs:41-44` (Network → SiteIntegration) — stays `Cascade` or not.
- [ ] Generate + apply the EF Core migration for those FK changes.
- [ ] Add dependency pre-checks to `SiteService.DeleteAsync` (Networks/SiteIntegrations exist?) so `SiteErrors.CannotDeleteWithActiveDependencies` becomes reachable again.

## 2. Docs
- [ ] Add the "Domain Entity Design" section to `AGENTS.md` (finalized version, `EndSession()` corrected).

## 3. Tracing gap
- [ ] Add `EnrichDiagnosticContext` to `UseSerilogRequestLogging()` in `Program.cs` so `TraceId` gets attached to the request-summary log event.
- [ ] Add a Serilog Seq sink (package + `Serilog:WriteTo` entry + `Seq:ServerUrl` in User Secrets pointing at `localhost:5341`) so structured logs actually land in the running Seq container.