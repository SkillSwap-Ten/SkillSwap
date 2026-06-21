# ADR-001: Migration to Clean Architecture for SkillSwap v2.0

| Field           | Value                                                         |
| --------------- | ------------------------------------------------------------- |
| **Status**      | Proposed                                                      |
| **Date**        | 2026-06-18                                                    |
| **Authors**     | David Steven Medina Urrego (`@medi77na`)                      |
| **Reviewers**   | SkillSwap Backend Team (`@Arlexz96`, `@JEscobar07`)           |
| **Supersedes**  | N/A (initial architectural decision for v2)                   |
| **Tags**        | architecture, .NET, backend, breaking-change, v2.0            |

---

## 1. Context

SkillSwap v1.0.0 was delivered as a Web API built with .NET and C#, structured following a **classic layered architecture** (Controllers → Services → Data) with Entity Framework Core against MySQL. The system is currently deployed on Render and serves the Next.js frontend (`Skiller`) deployed on Vercel.

After one year in production and through team retrospectives, the following structural problems have been identified:

### 1.1 Architectural pain points

- **Tight coupling between layers.** Services depend directly on `DbContext` and on framework-specific types. Business logic cannot be unit-tested without spinning up the entire infrastructure stack.
- **No clear separation between business rules and delivery mechanism.** Domain logic is scattered across `Services/` and `Controllers/`, making it impossible to reuse the domain in different delivery contexts (background jobs, message queues, CLI tools).
- **DTOs and entities are not properly isolated.** Models leak from the persistence layer into the API surface, increasing the risk of accidentally exposing internal fields.
- **Validation logic is duplicated** across Controllers, DTOs (via DataAnnotations), and Services.
- **No test suite.** There is no `Tests/` project. Coverage is effectively 0%. Regressions can only be detected manually.

### 1.2 Operational and quality gaps

- **No CI/CD pipeline.** No GitHub Actions workflows exist for build verification, tests, or deployment automation.
- **Hardcoded demo credentials in the public README.** This is a security incident waiting to happen.
- **No structured logging.** Application logs use the default `ILogger` defaults without structured output, correlation IDs, or external sinks.
- **No observability.** No health checks, no metrics, no distributed tracing.
- **Secrets management is implicit.** `appsettings.json` and `.env` files coexist without a clear policy.
- **No API versioning beyond URL prefix.** The `Controllers/V1/` folder is convention-based and not enforced at the framework level.

### 1.3 Forces

The team is composed of three backend engineers with mid-to-senior level experience. The codebase is small enough that a rewrite is feasible within a single iteration cycle. The frontend (`Skiller`) is the only known consumer and is owned by the same organization, which means **breaking-change tolerance is high** — coordinated migration is acceptable.

The team has also recently adopted **BMAD methodology** (v6.8.0) including Test Architect (v1.19.0) and BMad Builder (v2.0.0), creating a structured process to support the rewrite.

---

## 2. Decision

We will rewrite the SkillSwap backend as **v2.0** using **Clean Architecture** (also known as Onion / Hexagonal Architecture), organized as a multi-project .NET solution with **strict dependency rules** enforced by project references.

The v2 will live in an **orphan Git branch** (`v2`) within the same repository as v1, preserving the v1 history under tag `v1.0.0` and on `main`. The v2 will become the new `main` only when feature-complete and validated.

We explicitly accept that **v2 will introduce breaking changes** to the API surface where required by code quality, security, or domain correctness. The frontend (`Skiller`) will be adapted in a coordinated migration after v2 reaches a stable contract. We will not preserve v1 endpoint shapes that we already know are flawed.

### 2.1 Solution structure

```
SkillSwap.sln
├── src/
│   ├── SkillSwap.Domain/             ← Entities, Value Objects, Domain Events, Domain Exceptions
│   ├── SkillSwap.Application/        ← Use Cases (CQRS), Interfaces (Ports), DTOs, Validators
│   ├── SkillSwap.Infrastructure/     ← EF Core, Repositories (Adapters), External Services
│   ├── SkillSwap.API/                ← Controllers, Middleware, Swagger, Composition Root
│   └── SkillSwap.Shared/             ← Cross-cutting concerns (Result<T>, Errors, Helpers)
├── tests/
│   ├── SkillSwap.Domain.UnitTests/
│   ├── SkillSwap.Application.UnitTests/
│   ├── SkillSwap.Infrastructure.IntegrationTests/
│   └── SkillSwap.API.FunctionalTests/
├── docs/
│   ├── adr/                          ← Architecture Decision Records
│   ├── architecture/                 ← Diagrams, briefs, technical specs
│   └── api/                          ← OpenAPI specs, contract documentation
├── .github/
│   └── workflows/
│       ├── ci.yml                    ← Build + test + coverage on every PR
│       └── cd.yml                    ← Deploy on merge to main
├── Dockerfile                        ← Multi-stage build (slim runtime image)
├── docker-compose.yml                ← Local dev orchestration (API + MySQL + Seq for logs)
├── .gitignore
├── .editorconfig                     ← Code style enforcement
├── Directory.Build.props             ← Shared MSBuild config (nullable, treat warnings as errors)
└── README.md
```

### 2.2 Dependency rules (enforced via project references)

```
SkillSwap.API ──────────► SkillSwap.Application ──────────► SkillSwap.Domain
       │                          │                                ▲
       │                          ▼                                │
       └──────► SkillSwap.Infrastructure ───────────────────────────┘
                          │
                          ▼
                  SkillSwap.Shared (referenced by all layers)
```

**The dependency arrow always points inward.** `Domain` has zero external dependencies. `Application` only depends on `Domain` and abstractions. `Infrastructure` implements those abstractions. `API` wires everything together at the composition root.

### 2.3 Architectural patterns adopted

| Pattern                | Purpose                                                                                                                                                     |
| ---------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **CQRS** (via MediatR) | Separate write paths (`Commands`) from read paths (`Queries`). Commands return `Result<T>`; queries return projections directly.                            |
| **Repository + UoW**   | Abstract persistence behind interfaces in `Application`. Implementations in `Infrastructure`.                                                               |
| **Result Pattern**     | Replace exception-based control flow with `Result<T>` and `Error` types. Exceptions are reserved for truly exceptional conditions.                          |
| **Pipeline Behaviors** | Cross-cutting concerns (validation, logging, transactions, caching) as MediatR pipeline behaviors instead of decorators or middleware in the wrong layer.   |
| **Domain Events**      | Capture state changes in aggregates. Dispatched after persistence via outbox pattern (Phase 2).                                                              |
| **Value Objects**      | Eliminate primitive obsession. Email, UserId, SkillRating, etc. are typed value objects, not raw strings/ints.                                              |
| **Specification**      | Encapsulate query filters as composable specifications. Reduces leakage of `IQueryable` into the Application layer.                                         |

---

## 3. Technology Stack

| Concern                   | Choice                                              | Rationale                                                                                  |
| ------------------------- | --------------------------------------------------- | ------------------------------------------------------------------------------------------ |
| **Runtime**               | .NET 8 LTS                                          | Long-term support until November 2026; mature ecosystem; aligns with team experience.       |
| **ORM**                   | EF Core 8 + `Pomelo.EntityFrameworkCore.MySql`      | Preserve MySQL compatibility; avoid data migration cost; team is already familiar with EF. |
| **Mediator / CQRS**       | MediatR                                             | De-facto standard; enables pipeline behaviors and clean handler separation.                |
| **Validation**            | FluentValidation                                    | Decouples validation from DTOs; testable in isolation; composable.                         |
| **Object Mapping**        | Mapster                                             | Faster than AutoMapper; compile-time mappings; less reflection magic.                      |
| **Authentication**        | ASP.NET Core Identity + JWT with refresh tokens     | Standard, battle-tested, supports hashing best practices (PBKDF2).                         |
| **Password Hashing**      | Argon2id (via `Konscious.Security.Cryptography`)    | OWASP-recommended; resistant to GPU/ASIC attacks.                                          |
| **Logging**               | Serilog with sinks (Console, File, Seq in dev)      | Structured logs with correlation IDs; queryable in Seq during development.                 |
| **Observability**         | OpenTelemetry (traces + metrics)                    | Vendor-neutral; future-proof for moving to Honeycomb / Grafana / Azure Monitor.            |
| **Health Checks**         | `Microsoft.Extensions.Diagnostics.HealthChecks`     | Required by Render for liveness probes; exposed at `/health` and `/health/ready`.          |
| **Rate Limiting**         | Built-in `Microsoft.AspNetCore.RateLimiting`        | Native in .NET 7+; no extra dependency.                                                    |
| **API Versioning**        | `Asp.Versioning.Mvc` + `Asp.Versioning.ApiExplorer` | Explicit, framework-level versioning instead of folder conventions.                        |
| **API Documentation**     | Swashbuckle (Swagger / OpenAPI 3.0)                 | Industry standard; consumed by frontend tooling.                                           |
| **Testing — Unit**        | xUnit + FluentAssertions + NSubstitute              | xUnit is the .NET community standard; FluentAssertions makes failures readable.            |
| **Testing — Integration** | `WebApplicationFactory` + Testcontainers (MySQL)    | Real MySQL instance in Docker for tests; no mock divergence.                               |
| **Code Coverage**         | Coverlet + ReportGenerator                          | Integrated into `dotnet test`; reports consumed in CI.                                     |
| **CI/CD**                 | GitHub Actions                                      | Already on GitHub; free for public repos; matrix builds across .NET versions.              |
| **Static Analysis**       | SonarCloud (free tier for public repos)             | Detects bugs, code smells, security hotspots; required PR gate.                            |
| **Code Style**            | `.editorconfig` + analyzers + `dotnet format`       | Enforced in CI; failing format fails the build.                                            |
| **Container**             | Docker with multi-stage build                       | Smaller production images; clear separation of build vs runtime.                           |
| **Secrets (dev)**         | .NET User Secrets                                   | Per-user, never in source control.                                                         |
| **Secrets (prod)**        | Environment variables injected by Render            | Standard for containerized deployments; supports rotation.                                 |

---

## 4. Consequences

### 4.1 Positive

- **Testability.** Domain and Application logic can be unit-tested in isolation, without infrastructure. Coverage target ≥ 80% on Domain + Application.
- **Maintainability.** Each layer has a single, well-defined responsibility. Changes in persistence don't ripple into business logic.
- **Scalability of the codebase.** New features follow a predictable vertical slice pattern; onboarding new engineers is faster.
- **Security posture.** Centralized authentication, proper password hashing, rate limiting, secret management — all foundational, not retrofitted.
- **Observability.** Structured logs + traces + metrics from day one, enabling production diagnostics that v1 cannot provide.
- **CI/CD enforcement.** Quality gates (tests, coverage, static analysis) prevent regressions at PR time.
- **Senior-level signaling.** The codebase becomes a strong portfolio artifact demonstrating architecture maturity.

### 4.2 Negative

- **Initial overhead.** Setting up 5 projects, the test pyramid, and CI/CD is heavier than a single-project layered solution. Estimated 1–2 sprints before the first feature ships.
- **Learning curve.** Team members unfamiliar with CQRS, MediatR, Result pattern, or pipeline behaviors will need to ramp up. Mitigated by pair programming and the BMAD Test Architect process.
- **More files.** A simple CRUD operation now touches 6–8 files (Entity, Command, Handler, Validator, Endpoint, Mapper, Test) instead of 2–3. This is a deliberate trade-off for testability and clarity.
- **Breaking changes for the frontend.** `Skiller` will need to be updated to consume the new contract. Coordinated migration plan required.

### 4.3 Neutral

- **MySQL is retained.** No database engine change, no data migration cost in this phase.
- **Render deployment is retained.** No infrastructure change in this phase. A future ADR may evaluate Azure App Service or Container Apps.
- **The repository remains a monolith.** No microservices in v2.0. A future ADR may evaluate modular decomposition.

---

## 5. Alternatives Considered

### 5.1 Continue with layered architecture and refactor incrementally

**Rejected.** The structural debt is too pervasive — Services are coupled to `DbContext`, Controllers contain business logic, there is no test seam to safely refactor against. An incremental refactor would take longer than the rewrite and produce inferior results.

### 5.2 Vertical Slice Architecture (VSA)

**Considered, not adopted as the primary pattern.** VSA is excellent for high-velocity teams and reduces the file count per feature. However, it does not enforce the dependency rule as strictly as Clean Architecture, and the team is using v2 partly as a learning exercise where explicit boundaries provide more pedagogical value. We will, however, **adopt VSA-style folder organization inside `Application/`** (folders by feature, not by technical type).

### 5.3 Modular Monolith

**Deferred to a future ADR.** The current feature scope (Users, Auth, Requests, Reports) does not yet justify the operational complexity of module boundaries. Once the v2 is stable, we may revisit this for modules like Notifications or Reporting.

### 5.4 Microservices

**Rejected for v2.0.** Team size (3 backend engineers) and feature scope do not justify the operational overhead. Distributed systems introduce failure modes that a small team cannot adequately operate.

---

## 6. Migration Strategy

### 6.1 Git workflow

1. Tag the current state of `main` as `v1.0.0` (preservation).
2. Rename existing `develop` branch to `v1-archive` (preserve any in-flight v1 work).
3. Create an **orphan branch** `v2` from `main`, then `git rm -rf .` to start with an empty working tree.
4. Develop v2 on `v2` with feature branches off of it (`feature/*`).
5. When v2 is feature-complete and validated, force `main` to point to `v2` and tag `v2.0.0`. Old `main` is preserved in the `v1.0.0` tag.

### 6.2 Feature migration order (vertical slices)

Each slice is a self-contained PR that touches all 5 layers for one feature:

1. **Foundation slice** — solution scaffolding, CI/CD pipeline, shared infrastructure (DI, logging, error handling), health checks. No feature code.
2. **Users.Register** — proves the architecture end-to-end with a simple write path.
3. **Users.Authenticate** — establishes JWT + refresh token flow.
4. **Users.GetProfile** — proves the read path with projections.
5. **Skills.\*** — domain core (CRUD on skills catalog).
6. **Requests.\*** — exchange request lifecycle (the heart of the product).
7. **Reports.\*** — reporting and moderation features.
8. **Cutover** — frontend integration, DNS switch, decommission of v1.

### 6.3 Frontend coordination

Each vertical slice publishes its OpenAPI contract under `docs/api/v2/`. The frontend team consumes these contracts and updates `Skiller` in parallel. A staging environment will run v2 against a staging instance of the frontend before production cutover.

---

## 7. Quality and Testing Strategy

The testing pyramid for v2.0 follows ISTQB / BMAD best practices:

```
                      ▲
                     ╱ ╲     ~5%   E2E / Functional tests
                    ╱   ╲           (WebApplicationFactory, full stack)
                   ╱─────╲
                  ╱       ╲  ~20%  Integration tests
                 ╱         ╲        (real MySQL via Testcontainers)
                ╱───────────╲
               ╱             ╲ ~75%  Unit tests
              ╱               ╲      (Domain + Application, no infra)
             ─────────────────
```

**Coverage gates enforced in CI:**

- Domain: ≥ 90%
- Application: ≥ 85%
- Infrastructure: ≥ 70%
- API: ≥ 60%
- Overall: ≥ 80%

The **BMAD Test Architect** will be invoked to produce the formal Test Strategy document as a companion to this ADR.

---

## 8. Open Questions

The following items are tracked for future ADRs:

- **ADR-002**: Authentication & Authorization strategy (Identity vs IdentityServer vs Custom)
- **ADR-003**: Domain modeling — aggregate boundaries for Skills, Requests, Users
- **ADR-004**: Outbox pattern for domain events and reliable messaging
- **ADR-005**: Multitenancy strategy (if/when needed)
- **ADR-006**: Frontend contract evolution and breaking change policy

---

## 9. References

- Robert C. Martin, *Clean Architecture: A Craftsman's Guide to Software Structure and Design* (2017)
- Vaughn Vernon, *Implementing Domain-Driven Design* (2013)
- Microsoft Learn — [Common web application architectures](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- Jason Taylor — [Clean Architecture Template for .NET](https://github.com/jasontaylordev/CleanArchitecture)
- Martin Fowler — [StranglerFigApplication](https://martinfowler.com/bliki/StranglerFigApplication.html)
- OWASP — [Password Storage Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html)

---

## 10. Decision Log

| Date       | Decision                                                                                | By            |
| ---------- | --------------------------------------------------------------------------------------- | ------------- |
| 2026-06-18 | Initial draft proposed — Clean Architecture, .NET 8, breaking-change tolerance accepted | `@medi77na`   |
| TBD        | Team review                                                                             | Backend team  |
| TBD        | Accepted / Rejected / Amended                                                           | Backend team  |

---

*This ADR follows the format proposed by Michael Nygard, adapted for the SkillSwap project.*
