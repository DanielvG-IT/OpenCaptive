<div align="center">

# 🛜 OpenCaptive

**An open-source, enterprise-grade captive-portal platform with ultimate customization.**

[![Backend CI](https://github.com/DanielvG-IT/OpenCaptive/actions/workflows/backend.yml/badge.svg)](https://github.com/DanielvG-IT/OpenCaptive/actions/workflows/backend.yml)
[![Frontend CI (Admin)](https://github.com/DanielvG-IT/OpenCaptive/actions/workflows/frontend-admin.yml/badge.svg)](https://github.com/DanielvG-IT/OpenCaptive/actions/workflows/frontend-admin.yml)
[![Frontend CI (Portal)](https://github.com/DanielvG-IT/OpenCaptive/actions/workflows/frontend-portal.yml/badge.svg)](https://github.com/DanielvG-IT/OpenCaptive/actions/workflows/frontend-portal.yml)
[![CodeQL](https://github.com/DanielvG-IT/OpenCaptive/actions/workflows/codeql.yml/badge.svg)](https://github.com/DanielvG-IT/OpenCaptive/actions/workflows/codeql.yml)
[![License: Proprietary](https://img.shields.io/badge/License-Proprietary-red.svg)](#)

</div>

---

## ✨ Overview

OpenCaptive is a self-hostable captive-portal platform that gives you full control
over the guest Wi-Fi experience. Build branded onboarding flows, plug in your own
authentication and network controllers, and keep ownership of your data — without
vendor lock-in.

It ships with a pluggable integration model, with first-class support for
**UniFi** network controllers out of the box.

## 🏗️ Architecture

The repository is a monorepo split into a .NET backend and a Next.js frontend.

```
OpenCaptive/
├── src/
│   ├── Backend/                                        # .NET 10 — Clean Architecture
│   │   ├── OpenCaptive.Api/                            # ASP.NET Core HTTP API (composition root)
│   │   ├── OpenCaptive.Application/                    # Use cases / application services
│   │   ├── OpenCaptive.Domain/                         # Core domain model & business rules
│   │   ├── OpenCaptive.Infrastructure/                 # Persistence & cross-cutting concerns
│   │   ├── OpenCaptive.Integrations.Abstractions/      # Integration interfaces
│   │   └── OpenCaptive.Integrations.UniFi/             # UniFi controller integration
│   └── Frontend/
│       └── apps/
│           ├── admin/                                  # Next.js admin dashboard
│           └── portal/                                 # Next.js end-user captive portal
├── docs/architecture/                                  # Design decision records
├── docker-compose.yml                                  # Local services (Postgres, Papercut)
├── Makefile                                            # Shortcuts for the commands below
└── OpenCaptive.slnx                                    # .NET solution
```

## 🚀 Getting Started

### Prerequisites

| Tool        | Version | Notes                          |
| ----------- | ------- | ------------------------------ |
| .NET SDK    | 10.0+   |                                |
| Node.js     | 22+     |                                |
| npm         | 10+     |                                |
| Docker      | 24+     | with Compose v2                |
| GNU Make    | 3.81+   | optional — shortcuts only      |

### Local services

`docker-compose.yml` at the repo root runs everything the API talks to locally:
Postgres, and [Papercut](https://github.com/ChangemakerStudios/Papercut-SMTP) as an SMTP
catcher so no development mail ever leaves the machine.

```bash
# Start the containers in the background
docker compose up -d          # make up
```

| Service  | Local address                                              |
| -------- | ---------------------------------------------------------- |
| Postgres | `localhost:5432` — db `opencaptive`, `postgres`/`postgres` |
| Papercut | SMTP on `localhost:25`, web UI on http://localhost:37408   |

### Backend

The API ships without local secrets — `ConnectionStrings:Postgres`,
`Authentication:Jwt:SigningKey` and the `Email:Smtp` block in `appsettings.json` are
intentionally blank. Supply them through [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets)
so they never reach the repo:

```bash
cd src/Backend/OpenCaptive.Api
dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Port=5432;Database=opencaptive;Username=postgres;Password=postgres"
dotnet user-secrets set "Authentication:Jwt:SigningKey" "$(openssl rand -base64 48)"
dotnet user-secrets set "Email:Smtp:Host" "localhost"
dotnet user-secrets set "Email:Smtp:Port" "25"
```

Then build, apply the migrations, and run (migrations need the EF Core CLI —
`dotnet tool install --global dotnet-ef`):

```bash
# Restore & build the solution
dotnet restore OpenCaptive.slnx          # make restore
dotnet build OpenCaptive.slnx            # make build

# Apply EF Core migrations (the API does not migrate on startup)
dotnet ef database update \
  --project src/Backend/OpenCaptive.Infrastructure \
  --startup-project src/Backend/OpenCaptive.Api    # make db-update

# Run the API — https://localhost:8001, http://localhost:8000
dotnet run --project src/Backend/OpenCaptive.Api   # make api
```

### Frontend

Each app under `src/Frontend/apps` is a standalone Next.js application with its own
lockfile — there is no workspace root, so install per app.

```bash
# Admin dashboard
cd src/Frontend/apps/admin
npm install
npm run dev                   # make dev-admin

# End-user portal
cd src/Frontend/apps/portal
npm install
npm run dev                   # make dev-portal
```

### Make shortcuts

The `Makefile` wraps the commands above — it is convenience only, never a different
build. Run `make` (or `make help`) for the full list.

| Target                 | Does                                             |
| ---------------------- | ------------------------------------------------ |
| `up` / `down` / `logs` | Manage the local service containers              |
| `restore`/`build`/`test` | The .NET solution                              |
| `api`                  | Run the API                                      |
| `db-update`            | Apply pending EF Core migrations                 |
| `migration NAME=…`     | Add an EF Core migration                         |
| `install` / `lint` / `build-frontend` | Both frontend apps                |
| `dev-admin` / `dev-portal` | Run one frontend dev server                  |
| `check`                | Everything CI checks, before opening a PR        |

## 🐳 Container images

Each deployable has its own multi-stage Dockerfile. The API builds from the repo root
(its project graph spans `src/Backend`); each frontend builds from its own directory.

```bash
docker build -f src/Backend/OpenCaptive.Api/Dockerfile -t opencaptive/api .   # make image-api
docker build -t opencaptive/admin  src/Frontend/apps/admin                     # make image-admin
docker build -t opencaptive/portal src/Frontend/apps/portal                    # make image-portal
make images IMAGE_TAG=0.1.0                                                    # all three
```

All three run as a non-root user, expose a `HEALTHCHECK`, and listen on `8080` (API) or
`3000` (frontends).

The API reads configuration from environment variables using `__` as the section
separator — the same keys as `appsettings.json`:

| Variable                          | Required | Example                                                         |
| --------------------------------- | -------- | --------------------------------------------------------------- |
| `ConnectionStrings__Postgres`     | yes      | `Host=db;Port=5432;Database=opencaptive;Username=…;Password=…`   |
| `Authentication__Jwt__SigningKey` | yes      | 32+ bytes of base64 randomness                                   |
| `Email__Smtp__Host` / `__Port`    | yes      | `smtp.example.com` / `587`                                       |
| `Frontend__ApplicationUrl` / `__PortalUrl` | yes | public URLs used in outgoing email links                     |
| `ASPNETCORE_ENVIRONMENT`          | no       | `Production` (default when unset)                                |

Three things to know before deploying:

- **Migrations are not applied on startup.** Run `dotnet ef database update` against the
  target database as a separate deploy step.
- **`NEXT_PUBLIC_*` values are baked in at frontend build time**, not read at runtime — an
  API URL that differs per environment means one image build per environment.
- **The Data Protection key ring lives in Postgres** (`DataProtectionKeys` table), so
  Identity-issued email-verification and password-reset tokens survive restarts and are
  accepted by every replica. This needs the migration applied — an instance that cannot
  reach the table falls back to an ephemeral in-memory ring.

## 🧩 Integrations

OpenCaptive talks to network controllers through the
`OpenCaptive.Integrations.Abstractions` interfaces, so adding a new controller is
a matter of implementing those contracts.

- **UniFi** — `OpenCaptive.Integrations.UniFi`

Want to add another controller (Cisco, MikroTik, OPNsense, …)? See
[CONTRIBUTING.md](./CONTRIBUTING.md).

## 🤝 Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](./CONTRIBUTING.md) and our
[Code of Conduct](./CODE_OF_CONDUCT.md) before opening a pull request.

## 🔐 Security

Found a vulnerability? Please **do not** open a public issue — follow the process in
our [Security Policy](./SECURITY.md).

## 📄 License

All rights reserved. License TBD.
