# Architecture & Design Docs

Durable design documentation for OpenCaptive's non-obvious, security-critical, or
much-debated subsystems. These are **decision records** — they explain *why* something is the
way it is, so a decision reached after real deliberation doesn't have to be rediscovered in
three months.

## What belongs here

- Design decisions that took more than a moment to reach (token models, transaction
  boundaries, authorization invariants, aggregate lifecycles).
- The *why* behind a subsystem, not a restatement of the code. If the code already makes it
  obvious, it doesn't need a doc.

Keep each doc **concise**. When a decision changes, update the doc in the same PR — a stale
architecture doc is worse than none.

## Where other documentation lives

| You want…                                   | Look in            |
| ------------------------------------------- | ------------------ |
| What the product is and who it's for        | `PRODUCT.md`       |
| Repo layout, setup, how to run it           | `README.md`        |
| How to contribute, branch, commit           | `CONTRIBUTING.md`  |
| How to report a vulnerability               | `SECURITY.md`      |
| Conventions for AI agents / codegen tools   | `AGENTS.md`        |
| **Why a subsystem is built the way it is**  | **here**           |

## Index

- [authentication.md](./authentication.md) — access/refresh token model, rotation with reuse
  detection, MFA, email-verification and password-reset tokens.
- [multi-tenancy.md](./multi-tenancy.md) — tenant isolation model: org-from-JWT, per-query
  scoping, wrong-org → 404, and why the discipline is load-bearing.
- [invitations.md](./invitations.md) — organization invitation aggregate, opaque token
  security, acceptance transaction, snapshot-at-creation authorization.
