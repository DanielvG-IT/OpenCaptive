# OpenCaptive

An open‑source, enterprise‑grade captive‑portal platform with ultimate customization.  
Built with a .NET Core Web API backend, Supabase (PostgreSQL) database, and a Next.js frontend featuring a WYSIWYG editor for look‑and‑feel configuration.

---

## 🚀 Features

- **Modular Microservices**

  - **Portal Service**: renders captive portal UI from JSON models
  - **Admin Service**: Next.js‑based WYSIWYG customization & tenant management
  - **Vendor Adapters**: UniFi, Cisco ISE, Netgear, OpenWISP, and more

- **Rich Customization**

  - Drag‑&‑drop layout via GrapesJS
  - Style manager for colors, fonts, images
  - JSON‑backed component definitions

- **Multi‑Tenant & On‑Premise**

  - Tenant isolation with row‑level security
  - Containerized (Docker, Kubernetes) for on‑prem or SaaS
  - Scalable, stateless services with API Gateway

- **Open Source & Extensible**
  - MIT‑licensed monorepo
  - Plugin architecture for new vendor adapters
  - CI/CD with GitHub Actions

---

## 📦 Tech Stack

| Layer              | Framework / Tool             |
| ------------------ | ---------------------------- |
| **Backend API**    | ASP.NET Core 9 (C#)          |
| **Database**       | Supabase (PostgreSQL + Auth) |
| **Frontend**       | Next.js (React + TypeScript) |
| **WYSIWYG Editor** | GrapesJS / Craft.js          |
| **State Mgmt**     | React Context / SWR          |
| **Auth**           | Supabase Auth / JWT          |
| **Container**      | Docker, Kubernetes           |
| **CI/CD**          | GitHub Actions               |

---

## 🔧 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) & npm/yarn
- [Docker](https://www.docker.com/) & [kubectl](https://kubernetes.io/) (optional)
- Supabase account or self‑hosted instance

---

## ⚙️ Usage

1. **Register & Login** via Supabase‑powered Auth.
2. **Create Tenant** in Admin Center (`/admin`).
3. **Customize Portal** with drag‑&‑drop editor.
4. **Save & Publish**—portal lives at `https://portal.your-domain.com/{tenant}`.
5. **Connect Routers** by pointing vendor captive‑portal URL to your Portal Service endpoint.

---

## 🤝 Contributing

1. Fork the repo
2. Create a feature branch (`git checkout -b feat/my-adapter`)
3. Commit & push
4. Open a Pull Request

Please follow our [CONTRIBUTING.md](./CONTRIBUTING.md) for code style and review guidelines.

---

## 📄 License

Released under the **MIT License**. See [LICENSE](./LICENSE) for details.
