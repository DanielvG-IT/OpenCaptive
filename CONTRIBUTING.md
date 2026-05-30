# Contributing to OpenCaptive

First off — thank you for taking the time to contribute! 🎉

This document explains how to get set up and what we expect from contributions.

## 📋 Code of Conduct

This project and everyone participating in it is governed by our
[Code of Conduct](./CODE_OF_CONDUCT.md). By participating, you are expected to
uphold it.

## 🛠️ Development Setup

See the [Getting Started](./README.md#-getting-started) section of the README for
prerequisites and how to run the backend and frontend apps.

## 🌱 Workflow

1. **Fork** the repository and clone your fork.
2. **Create a branch** off `main` using a descriptive name:
   - `feat/unifi-guest-auth`
   - `fix/portal-redirect-loop`
   - `docs/readme-setup`
3. **Make your changes** with clear, focused commits.
4. **Push** and open a **Pull Request** against `main`.
5. Make sure CI is green and address review feedback.

## ✅ Commit Messages

We follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(optional scope): <description>

[optional body]
```

Common types: `feat`, `fix`, `docs`, `refactor`, `test`, `chore`, `ci`.

Example: `feat(integrations): add MikroTik controller adapter`

## 🧪 Quality Checks

Before opening a PR, please run the relevant checks locally:

**Backend**

```bash
dotnet build OpenCaptive.slnx
dotnet test OpenCaptive.slnx
```

**Frontend** (run inside the app you changed, e.g. `src/Frontend/apps/admin`)

```bash
npm run lint
npm run build
```

## 🧩 Adding a New Integration

Network-controller integrations implement the interfaces in
`OpenCaptive.Integrations.Abstractions`. To add one:

1. Create a new project `OpenCaptive.Integrations.<Name>` under `src/Backend`.
2. Reference `OpenCaptive.Integrations.Abstractions` and implement the contracts.
3. Register it in the API's composition root.
4. Add documentation and, where possible, tests.

## 🐛 Reporting Bugs & Requesting Features

Use the [issue templates](https://github.com/DanielvG-IT/OpenCaptive/issues/new/choose).
Please include as much detail as possible — versions, steps to reproduce, and logs.

## 📄 License

By contributing, you agree that your contributions will be licensed under the
[MIT License](./LICENSE).
