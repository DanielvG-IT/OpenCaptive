# OpenCaptive — Product Context

> The canonical "why" behind OpenCaptive. `AGENTS.md` carries a condensed always-loaded
> summary of this; read this file when you need the full mental model behind a feature.

## Mission

OpenCaptive is a B2B SaaS platform that lets organizations create, manage, and analyze the
complete guest Wi-Fi experience across any network vendor. It intentionally owns the
**business layer** and delegates infrastructure operations to vendor integrations.

Think of OpenCaptive as **the operating system for guest Wi-Fi**, not a network controller.

> Whenever evaluating a feature, ask: *does this improve the guest experience, operational
> management, or business insights?* If not, it probably doesn't belong in OpenCaptive.

The goal is **not** to become another UniFi management dashboard. The goal is to become the
software that sits **between the guest and the internet**.

---

## Core Principle

> **OpenCaptive manages the guest experience, not the network infrastructure.**

Controllers (UniFi, Omada, MikroTik, Cisco, Aruba…) are implementation details. Hotels,
restaurants, schools and businesses should be able to change network vendors without
rebuilding their guest experience. Everything is designed around OpenCaptive's own
capabilities, never around vendor APIs.

---

## Product Areas

Vendor controllers simply expose the operations OpenCaptive needs; everything business-shaped
lives in one of these eight areas.

- **Identity** — Users, Organizations, Memberships, Permissions, Invitations.
  Controls who may access OpenCaptive.
- **Site Management** — Sites, Networks, Site Integrations.
  Represents the customer's physical infrastructure.
- **Portal Management** — Portal, PortalVersions, Themes, Builder, Publishing.
  Defines what guests experience before internet access.
- **Guest Access** — Authentication Methods, Authorization, Guest Sessions.
  Securely connects guests to the internet.
- **Analytics** — Visitors, Sessions, Conversions, Campaigns.
  Turns guest traffic into business insights.
- **Communications** — Emails, Notifications, Invitations.
  Talks to administrators and guests.
- **Integrations** — UniFi, Omada, MikroTik, Cisco, …
  Bridges OpenCaptive to external infrastructure.
- **Platform** — Audit Logs, API, Webhooks, Background Jobs, Monitoring.
  Everything required to operate OpenCaptive as a SaaS.

When picking up a task, place it in one of these areas first — it clarifies what "done" looks
like and what it shouldn't touch. See `TODO.md` for which areas are implemented today.

---

## Primary Customer

Organizations with one or more physical locations — e.g. hotels, holiday parks, campings,
restaurants, cafés, libraries, municipalities, schools, universities, hospitals, shopping
malls, stadiums, airports, MSPs.

---

## Mental Model

```
Organization
├── Site (Amsterdam Hotel)
│   ├── Network (Guest WiFi)
│   │   └── Portal
│   ├── Network (Conference)
│   └── Site Integration
├── Site (Rotterdam Hotel)
└── Site (London Hotel)
```

- Organizations own Sites.
- Sites represent real physical locations.
- A Site can have multiple Networks.
- Networks represent guest Wi-Fi SSIDs.
- A Network has exactly one active Portal.
- A Portal defines the guest experience.

---

## What is a Site?

A physical location (Hotel Amsterdam, Camping Zeeland). A Site owns Networks and
Integrations, and is the organizational boundary for operational management.

## What is a Network?

One guest SSID (Hotel Guest, Staff, Conference, VIP). Belongs to exactly one Site.
Synchronized from integrations.

## What is a Portal?

The website shown before internet access is granted — splash screen, T&Cs, social/voucher/
email login, sponsor ads, branding, campaigns. A portal is **not** HTML; it is a *versioned
document*. Publishing creates a new active version.

## What is an Integration?

The connection between OpenCaptive and external infrastructure (UniFi, Omada, MikroTik,
Cisco). Integrations expose OpenCaptive capabilities instead of leaking vendor APIs.
OpenCaptive never asks "how does UniFi authenticate a guest?" — it says "authenticate this
guest."

---

## Core Product Loop — the heart of the product

Everything in OpenCaptive ultimately supports this loop; every feature should strengthen one
or more of its steps.

1. Administrator configures Sites.
2. Administrator connects Site Integrations.
3. Administrator creates Networks.
4. Administrator designs Portals.
5. Administrator publishes a Portal.
6. Guest connects to Wi-Fi.
7. Network redirects guest to OpenCaptive.
8. Guest authenticates.
9. Integration authorizes internet access.
10. `GuestSession` is created.
11. Analytics are collected.
12. Organization improves the portal using the collected insights — closing the loop.

### Guest Session

One period of internet access: start time, end time, network, authentication method,
optional guest identity, analytics. GuestSessions are historical records and the **source of
truth** for analytics.

### Analytics

Derived from GuestSessions — daily guests, peak hours, returning visitors, average session
duration, authentication conversion, campaign performance.

---

## Portal Builder (flagship feature)

Drag & drop, versioned, themeable, multi-language, mobile-first, reusable components.
Publishing creates immutable `PortalVersion`s; only one version is active at a time.

## Authentication Methods

Accept Terms, Voucher, Password, Email verification, SMS, Social Login, PMS Integration,
External API. These are **OpenCaptive** features; integrations merely execute the final
authorization on the controller.

---

## Architecture Principles (product-level)

> Implementation-level architecture rules (error handling, EF Core, multi-tenant IDOR,
> testing) live in `AGENTS.md`. This section is the product-shaped reasoning behind them.

- **Clean Architecture** — Domain never depends on Infrastructure; Application never depends
  on ASP.NET; Infrastructure implements abstractions.
- **Rich Domain Model** — entities expose business behavior (`Portal.Publish()`,
  `GuestSession.Authenticate()`, `GuestSession.EndSession()`, `Site.Enable()`,
  `Integration.MarkConnected()`), never persistence operations. There are no `Delete()`
  methods on entities — deletion belongs in the Application layer.
- **Hard Deletes** — no `IsDeleted`/`DeletedAt`. Deletion rules are enforced through explicit
  dependency checks and foreign keys.
- **Multi-tenancy** — every resource belongs to exactly one Organization; Application
  services always scope queries to the current Organization; no cross-org access.
- **Vendor Independence** — the Application layer never branches on UniFi/Omada/MikroTik; it
  asks for OpenCaptive capabilities.

## Development Philosophy

Explicit code over clever abstractions. Avoid speculative architecture. Build today's
requirements while leaving a clean path for tomorrow. Duplicate once; abstract after
repetition becomes obvious.

---

## Roadmap

- **Foundation** — Authentication, Organizations, Sites, Portal versions, Integrations,
  Networks, Guest sessions.
- **Management** — Site / Network / Portal CRUD, Integration management.
- **Portal** — visual builder, components, themes, publishing.
- **Guest Flow** — authentication, authorization, session creation.
- **Analytics** — dashboard, reports, trends.
- **Future** — multi-controller support, mobile app, MSP features, white-labeling, campaign
  engine, API integrations.

---

> **OpenCaptive is not a network controller. It is a platform for designing, managing, and
> analyzing the entire guest Wi-Fi experience across any network vendor.**
