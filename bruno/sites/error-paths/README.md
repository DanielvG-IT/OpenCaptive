# Not yet testable via API

**Delete a site that owns a network → expect 409** (empirical test for the `PostgresException` catch
in `SiteService.DeleteAsync`) can't be built as a Bruno request yet — `Network` exists in the
Domain/Infrastructure layers but has no endpoints (`NetworkEndpoints.cs` doesn't exist). To run this
test tonight, insert a Network row directly against the site via `psql` or a DB client, then run
`Delete Site` and confirm 409, not 500.

**Force a bug → expect 500 via `GlobalExceptionHandler`** also isn't a Bruno request — it needs a
temporary `throw` added to any endpoint, tested, then reverted. Not something to commit to the collection.
