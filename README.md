<div align="center">

# 🛜 OpenCaptive

**An open-source, enterprise-grade captive-portal platform with ultimate customization.**

[![Backend CI](https://github.com/DanielvG-IT/OpenCaptive/actions/workflows/backend.yml/badge.svg)](https://github.com/DanielvG-IT/OpenCaptive/actions/workflows/backend.yml)
[![Frontend CI](https://github.com/DanielvG-IT/OpenCaptive/actions/workflows/frontend.yml/badge.svg)](https://github.com/DanielvG-IT/OpenCaptive/actions/workflows/frontend.yml)
[![CodeQL](https://github.com/DanielvG-IT/OpenCaptive/actions/workflows/codeql.yml/badge.svg)](https://github.com/DanielvG-IT/OpenCaptive/actions/workflows/codeql.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](./LICENSE)

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
│   ├── Backend/                          # .NET 10 — Clean Architecture
│   │   ├── OpenCaptive.Api/              # ASP.NET Core HTTP API (composition root)
│   │   ├── OpenCaptive.Application/      # Use cases / application services
│   │   ├── OpenCaptive.Domain/          # Core domain model & business rules
│   │   ├── OpenCaptive.Contracts/       # Shared DTOs / API contracts
│   │   ├── OpenCaptive.Infrastructure/  # Persistence & cross-cutting concerns
│   │   ├── OpenCaptive.Integrations.Abstractions/  # Integration interfaces
│   │   └── OpenCaptive.Integrations.UniFi/         # UniFi controller integration
│   └── Frontend/
│       └── apps/
│           ├── admin/                    # Next.js admin dashboard
│           └── portal/                   # Next.js end-user captive portal
└── OpenCaptive.slnx                      # .NET solution
```

## 🚀 Getting Started

### Prerequisites

| Tool        | Version |
| ----------- | ------- |
| .NET SDK    | 10.0+   |
| Node.js     | 20+     |
| npm         | 10+     |

### Backend

```bash
# Restore & build the solution
dotnet restore OpenCaptive.slnx
dotnet build OpenCaptive.slnx

# Run the API
dotnet run --project src/Backend/OpenCaptive.Api
```

### Frontend

Each app under `src/Frontend/apps` is a standalone Next.js application.

```bash
# Admin dashboard
cd src/Frontend/apps/admin
npm install
npm run dev

# End-user portal
cd src/Frontend/apps/portal
npm install
npm run dev
```

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

Released under the **MIT License**. See [LICENSE](./LICENSE) for details.
