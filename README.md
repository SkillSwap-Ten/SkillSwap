# SkillSwap

[![CI](https://github.com/SkillSwap-Ten/SkillSwap/actions/workflows/ci.yml/badge.svg?branch=v2)](https://github.com/SkillSwap-Ten/SkillSwap/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET 8 LTS](https://img.shields.io/badge/.NET-8.0%20LTS-blue.svg)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![Status](https://img.shields.io/badge/v2.0-in--development-orange.svg)](docs/adr/ADR-001-clean-architecture-migration.md)

> A skill-exchange platform where members trade what they know for what they want to learn.

SkillSwap is being rewritten from scratch (**v2.0**) on Clean Architecture using .NET 8. The previous version is preserved under tag [`v1.0.0`](../../releases/tag/v1.0.0) and on the `main` branch; active development happens on the `v2` branch until cutover.

---

## Status

| Item                  | State                                                               |
|-----------------------|---------------------------------------------------------------------|
| Architecture          | Clean Architecture — see [ADR-001](docs/adr/ADR-001-clean-architecture-migration.md) |
| Runtime               | .NET 8 LTS (support until November 2026)                            |
| Frontend (`Skiller`)  | Still on v1 contract; migration scheduled post-cutover              |
| CI/CD                 | GitHub Actions — build, format, test + coverage (see [§ CI/CD](#cicd)) |
| First feature slice   | Pending — `Users.Register` will be the foundation                   |

---

## Quick start (local development)

You need: **.NET 8 SDK**, **Docker** (Desktop / OrbStack / Colima), and **Git**.

```bash
# 1. Clone and switch to the v2 branch
git clone https://github.com/<org>/SkillSwap.git
cd SkillSwap
git checkout v2

# 2. Create your local environment from the template
cp .env.example .env
# Then edit .env and replace every __change_me__ with a real value.

# 3. Start the stack (API + MySQL + Seq for logs)
docker compose up --build

# 4. Verify
#    API:  http://localhost:8080
#    Seq:  http://localhost:5341
#    MySQL: localhost:3306  (credentials from your .env)
```

> ⚠️ **Never commit `.env`.** It is gitignored. Only `.env.example` (with `__change_me__` placeholders) is tracked.

---

## Solution structure

The v2 follows strict Clean Architecture dependency rules. See [ADR-001 §2](docs/adr/ADR-001-clean-architecture-migration.md) for the full rationale.

```
src/
├── SkillSwap.Domain/            Entities, Value Objects, Domain Events, Domain Exceptions
├── SkillSwap.Application/       Use Cases (CQRS), Interfaces, DTOs, Validators
├── SkillSwap.Infrastructure/    EF Core, Repositories, External Services
├── SkillSwap.API/               Controllers, Middleware, Swagger, Composition Root
└── SkillSwap.Shared/            Cross-cutting concerns (Result<T>, Errors, Helpers)

tests/
├── SkillSwap.Domain.UnitTests/
├── SkillSwap.Application.UnitTests/
├── SkillSwap.Infrastructure.IntegrationTests/    real MySQL via Testcontainers
└── SkillSwap.API.FunctionalTests/                full stack via WebApplicationFactory
```

The dependency arrow always points inward: `API → Application → Domain`, with `Infrastructure` and `Shared` placed in the diagram as shown in [ADR-001 §2.2](docs/adr/ADR-001-clean-architecture-migration.md).

---

## Testing

```bash
# Restore, build, and run all tests
dotnet restore
dotnet build --no-restore
dotnet test --no-build --verbosity normal
```

Coverage targets enforced in CI ([ADR-001 §7](docs/adr/ADR-001-clean-architecture-migration.md)):

| Layer            | Minimum coverage |
|------------------|------------------|
| Domain           | ≥ 90%            |
| Application      | ≥ 85%            |
| Infrastructure   | ≥ 70%            |
| API              | ≥ 60%            |
| Overall          | ≥ 80%            |

---

## CI/CD

Every push and pull request to `v2` or `main` triggers the [CI workflow](.github/workflows/ci.yml): `dotnet format` check → restore → build (Release, warnings-as-errors) → test with coverage → coverage report uploaded as artifact. Coverage thresholds from [ADR-001 §7](docs/adr/ADR-001-clean-architecture-migration.md) (Domain ≥ 90, Application ≥ 85, Infrastructure ≥ 70, API ≥ 60, Overall ≥ 80) currently run as **informational** — they will flip to hard-fail starting with the first vertical slice (HANDOFF-04), once real tests exist to back them.

The [CD workflow](.github/workflows/cd.yml) is **parked**: manual-only (`workflow_dispatch`) and gated behind a `DEPLOY` confirmation string and the `RENDER_DEPLOY_HOOK_URL` secret. It activates once the first endpoints land in `SkillSwap.API` (post HANDOFF-04).

---

## Contributing

- Branching: feature branches off `v2` (`feature/*`), squash-merge into `v2` via PR.
- Commits: [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) (`feat:`, `fix:`, `refactor:`, `docs:`, `test:`, `chore:`).
- Code style: enforced by `.editorconfig` and `TreatWarningsAsErrors=true` (see `Directory.Build.props`).
- Every PR runs the full test suite. A failing build or a coverage regression blocks the merge.

---

## Documentation

| Document             | Purpose                                                       |
|----------------------|---------------------------------------------------------------|
| [`docs/adr/`](docs/adr/)               | Architecture Decision Records — including ADR-001 |
| [`docs/architecture/`](docs/architecture/) | Diagrams, technical specifications                |
| [`docs/api/`](docs/api/)               | OpenAPI contracts and per-slice API documentation |
| [`docs/handoff/`](docs/handoff/)       | Migration handoff notes (internal team only)      |

---

## License

[MIT](LICENSE) © 2026 SkillSwap-Ten
