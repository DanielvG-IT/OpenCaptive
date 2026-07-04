# OpenCaptive — Product Context

> The canonical "why" behind OpenCaptive. `AGENTS.md` carries a condensed always-loaded
> summary of this; read this file when you need the full mental model behind a feature.

## Vision

OpenCaptive is an open, modern, multi-tenant captive portal platform. It lets organizations
manage guest Wi-Fi access across one or many physical locations through a single web app.

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

## Product Philosophy

OpenCaptive owns: Organizations, Sites, Networks, Captive portals, Guest authentication,
Guest sessions, Analytics, Branding, Campaigns, Integrations.

Vendor controllers simply expose the operations OpenCaptive needs.

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

## Guest Lifecycle — the heart of the product

```
Guest joins WiFi → Controller redirects → OpenCaptive Portal → Guest authenticates
→ OpenCaptive validates → Integration grants access → GuestSession begins
→ Analytics collected → Guest disconnects → GuestSession ends
```

Everything else exists to support this lifecycle.

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
