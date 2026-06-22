# Handoff #2 — Solution Scaffolding (Cowork → Claude Code + BMAD Builder)

> **Cómo usar este documento:**
> 1. Asegurate de estar en `~/Personal/SkillSwap/` y en la rama `v2`.
> 2. Lanzá Claude Code (nueva sesión, no reutilizar la del HANDOFF-01).
> 3. Copiá y pegá **el bloque completo del prompt** como tu primer mensaje.
> 4. Claude Code te va a pedir go/no-go por bloque, igual que en HANDOFF-01.

---

## Contexto del scope

Este handoff materializa la **estructura física** del v2 conforme al ADR-001 §2.1 (project layout) y §2.2 (dependency rules). **NO incluye lógica de negocio** — eso pertenece a los vertical slices (Tarea #8 y #9 del roadmap).

El objetivo final del HANDOFF-02 es:
- `dotnet build` pasa con cero warnings sobre toda la solution
- `dotnet test` pasa (0 tests, 0 failures) — la pirámide de testing existe vacía
- Estructura lista para que el primer vertical slice (`Users.Register`) se construya sin tocar configuración

---

## Prompt para Claude Code

```
Hola Claude Code. Soy Steven (medi77na), backend lead de SkillSwap.
Vamos con el HANDOFF-02: scaffolding completo de la solution v2 conforme
al ADR-001 §2.1 y §2.2.

CONTEXTO PREVIO:
- HANDOFF-01 está completado. Estamos en rama v2, working tree solo tiene
  .gitignore + docs/ commiteados + BMAD untracked preservado.
- Tag v1.0.0 protege el estado anterior. Si algo sale mal, podemos resetear.
- El ADR-001 (docs/adr/ADR-001-clean-architecture-migration.md) es la
  fuente de verdad. Leelo COMPLETO antes de empezar — necesito que tengas
  presentes las decisiones de §2.2 (dependency rules), §3 (tech stack)
  y §7 (testing strategy).

REGLAS DE TRABAJO (iguales que HANDOFF-01):
- Go/no-go por bloque, no por comando individual.
- STOP obligatorio si encontrás estado inesperado.
- No commitees nada hasta el final (Bloque I).
- Conventional Commits.
- Si una decisión técnica no está en el ADR, preguntame ANTES de asumir.

INVOCACIÓN DE BMAD BUILDER (opcional pero recomendado):
Si tenés disponible el agente "BMad Builder" en este entorno, podés
invocarlo para que asista en la generación del scaffolding. Si no, ejecutá
los comandos directamente con `dotnet` CLI.

---

PLAN DE EJECUCIÓN (10 bloques)

═════════════════════════════════════════════════════════════════
BLOQUE A — Pre-flight (no destructivo)
═════════════════════════════════════════════════════════════════

A.1. Verificar que estás en rama v2 y que está sincronizada con origin/v2:
        git branch --show-current     # debe decir: v2
        git status                    # working tree solo con BMAD untracked
        git log --oneline -3          # primer commit: 836f9ed o el equivalente

A.2. Verificar que tenés .NET 8 SDK instalado:
        dotnet --list-sdks            # debe aparecer alguna 8.x.x
   Si NO está, parate y reportá. Si está 9.x, también sirve pero confirmame antes.

A.3. STOP — reportame el output de A.1 y A.2 antes de continuar.

═════════════════════════════════════════════════════════════════
BLOQUE B — global.json + estructura de carpetas
═════════════════════════════════════════════════════════════════

B.1. Crear global.json en la raíz fijando la versión del SDK
     (replicabilidad — todos los devs usan la misma versión):

     {
       "sdk": {
         "version": "8.0.0",
         "rollForward": "latestFeature",
         "allowPrerelease": false
       }
     }

     Ajustá el "version" al SDK 8.x.x que tengas instalado.

B.2. Crear estructura de carpetas raíz:
        mkdir -p src tests docs/architecture docs/api .github/workflows

═════════════════════════════════════════════════════════════════
BLOQUE C — Solution + 5 proyectos src
═════════════════════════════════════════════════════════════════

C.1. Crear la solution en la raíz:
        dotnet new sln -n SkillSwap

C.2. Crear los 5 proyectos src conforme al ADR-001 §2.1:

     cd src
     dotnet new classlib -n SkillSwap.Domain         -f net8.0
     dotnet new classlib -n SkillSwap.Application    -f net8.0
     dotnet new classlib -n SkillSwap.Infrastructure -f net8.0
     dotnet new webapi   -n SkillSwap.API            -f net8.0 --use-controllers
     dotnet new classlib -n SkillSwap.Shared         -f net8.0
     cd ..

     IMPORTANTE: SkillSwap.API debe usar --use-controllers (no minimal APIs).
     Esto se alinea con el versioning explícito de Asp.Versioning del ADR §3.

C.3. Eliminar los archivos boilerplate que .NET genera y no necesitamos:

     rm src/SkillSwap.Domain/Class1.cs
     rm src/SkillSwap.Application/Class1.cs
     rm src/SkillSwap.Infrastructure/Class1.cs
     rm src/SkillSwap.Shared/Class1.cs
     rm src/SkillSwap.API/WeatherForecast.cs 2>/dev/null
     rm src/SkillSwap.API/Controllers/WeatherForecastController.cs 2>/dev/null

C.4. Agregar los 5 proyectos a la solution:

     dotnet sln SkillSwap.sln add \
       src/SkillSwap.Domain/SkillSwap.Domain.csproj \
       src/SkillSwap.Application/SkillSwap.Application.csproj \
       src/SkillSwap.Infrastructure/SkillSwap.Infrastructure.csproj \
       src/SkillSwap.API/SkillSwap.API.csproj \
       src/SkillSwap.Shared/SkillSwap.Shared.csproj

═════════════════════════════════════════════════════════════════
BLOQUE D — Project references (la regla de dependencias del ADR §2.2)
═════════════════════════════════════════════════════════════════

LA REGLA ES INVIOLABLE (§2.2 del ADR):
   API ──► Application ──► Domain
    │           │             ▲
    │           ▼             │
    └──► Infrastructure ──────┘
                │
                ▼
             Shared (referenciado por todos)

D.1. Domain NO REFERENCIA NADA (excepto Shared en el último paso).
     Aplicación de regla.

D.2. Application referencia Domain:
        dotnet add src/SkillSwap.Application reference src/SkillSwap.Domain

D.3. Infrastructure referencia Application (para implementar sus interfaces) y Domain:
        dotnet add src/SkillSwap.Infrastructure reference \
          src/SkillSwap.Application src/SkillSwap.Domain

D.4. API referencia Application e Infrastructure (composición en startup):
        dotnet add src/SkillSwap.API reference \
          src/SkillSwap.Application src/SkillSwap.Infrastructure

D.5. Shared es referenciado por todos:
        dotnet add src/SkillSwap.Domain         reference src/SkillSwap.Shared
        dotnet add src/SkillSwap.Application    reference src/SkillSwap.Shared
        dotnet add src/SkillSwap.Infrastructure reference src/SkillSwap.Shared
        dotnet add src/SkillSwap.API            reference src/SkillSwap.Shared

D.6. VERIFICACIÓN CRÍTICA — confirmar que ningún proyecto rompe la regla.
     Mostrame el output de:
        grep -r "ProjectReference" src/*/*.csproj

     Confirmá que:
     - Domain solo referencia Shared
     - Application referencia Domain + Shared
     - Infrastructure referencia Application + Domain + Shared
     - API referencia Application + Infrastructure + Shared
     - Shared no referencia a nadie

═════════════════════════════════════════════════════════════════
BLOQUE E — 4 proyectos de tests
═════════════════════════════════════════════════════════════════

E.1. Crear los 4 proyectos de test conforme al ADR-001 §7:

     cd tests
     dotnet new xunit -n SkillSwap.Domain.UnitTests            -f net8.0
     dotnet new xunit -n SkillSwap.Application.UnitTests       -f net8.0
     dotnet new xunit -n SkillSwap.Infrastructure.IntegrationTests -f net8.0
     dotnet new xunit -n SkillSwap.API.FunctionalTests         -f net8.0
     cd ..

E.2. Eliminar UnitTest1.cs boilerplate de cada uno:
        rm tests/SkillSwap.Domain.UnitTests/UnitTest1.cs
        rm tests/SkillSwap.Application.UnitTests/UnitTest1.cs
        rm tests/SkillSwap.Infrastructure.IntegrationTests/UnitTest1.cs
        rm tests/SkillSwap.API.FunctionalTests/UnitTest1.cs

E.3. Agregar los 4 a la solution:
        dotnet sln SkillSwap.sln add \
          tests/SkillSwap.Domain.UnitTests/SkillSwap.Domain.UnitTests.csproj \
          tests/SkillSwap.Application.UnitTests/SkillSwap.Application.UnitTests.csproj \
          tests/SkillSwap.Infrastructure.IntegrationTests/SkillSwap.Infrastructure.IntegrationTests.csproj \
          tests/SkillSwap.API.FunctionalTests/SkillSwap.API.FunctionalTests.csproj

E.4. Referencias: cada test referencia el proyecto bajo test:

     dotnet add tests/SkillSwap.Domain.UnitTests \
       reference src/SkillSwap.Domain

     dotnet add tests/SkillSwap.Application.UnitTests \
       reference src/SkillSwap.Application src/SkillSwap.Domain

     dotnet add tests/SkillSwap.Infrastructure.IntegrationTests \
       reference src/SkillSwap.Infrastructure src/SkillSwap.Application src/SkillSwap.Domain

     dotnet add tests/SkillSwap.API.FunctionalTests \
       reference src/SkillSwap.API

═════════════════════════════════════════════════════════════════
BLOQUE F — NuGet packages base (por proyecto)
═════════════════════════════════════════════════════════════════

F.1. SkillSwap.Domain — sin paquetes (debe estar libre de dependencias externas).

F.2. SkillSwap.Application:
     dotnet add src/SkillSwap.Application package MediatR
     dotnet add src/SkillSwap.Application package FluentValidation
     dotnet add src/SkillSwap.Application package FluentValidation.DependencyInjectionExtensions
     dotnet add src/SkillSwap.Application package Mapster
     dotnet add src/SkillSwap.Application package Mapster.DependencyInjection
     dotnet add src/SkillSwap.Application package Microsoft.Extensions.Logging.Abstractions

F.3. SkillSwap.Infrastructure:
     dotnet add src/SkillSwap.Infrastructure package Microsoft.EntityFrameworkCore
     dotnet add src/SkillSwap.Infrastructure package Microsoft.EntityFrameworkCore.Design
     dotnet add src/SkillSwap.Infrastructure package Pomelo.EntityFrameworkCore.MySql
     dotnet add src/SkillSwap.Infrastructure package Microsoft.Extensions.Configuration.Abstractions
     dotnet add src/SkillSwap.Infrastructure package Konscious.Security.Cryptography.Argon2

F.4. SkillSwap.API:
     dotnet add src/SkillSwap.API package Asp.Versioning.Mvc
     dotnet add src/SkillSwap.API package Asp.Versioning.Mvc.ApiExplorer
     dotnet add src/SkillSwap.API package Swashbuckle.AspNetCore
     dotnet add src/SkillSwap.API package Serilog.AspNetCore
     dotnet add src/SkillSwap.API package Serilog.Sinks.Console
     dotnet add src/SkillSwap.API package Serilog.Sinks.File
     dotnet add src/SkillSwap.API package Serilog.Sinks.Seq
     dotnet add src/SkillSwap.API package OpenTelemetry.Extensions.Hosting
     dotnet add src/SkillSwap.API package OpenTelemetry.Instrumentation.AspNetCore
     dotnet add src/SkillSwap.API package OpenTelemetry.Instrumentation.Http
     dotnet add src/SkillSwap.API package OpenTelemetry.Exporter.Console
     dotnet add src/SkillSwap.API package Microsoft.AspNetCore.Authentication.JwtBearer
     dotnet add src/SkillSwap.API package AspNetCore.HealthChecks.MySql

F.5. SkillSwap.Shared — sin paquetes inicialmente (utilities propios).

F.6. Tests:
     # Common a todos los tests:
     for proj in tests/SkillSwap.*; do
       dotnet add $proj package FluentAssertions
       dotnet add $proj package NSubstitute
     done

     # Integration & Functional (DB real + WebApplicationFactory):
     dotnet add tests/SkillSwap.Infrastructure.IntegrationTests \
       package Testcontainers.MySql
     dotnet add tests/SkillSwap.Infrastructure.IntegrationTests \
       package Microsoft.EntityFrameworkCore.Design

     dotnet add tests/SkillSwap.API.FunctionalTests \
       package Microsoft.AspNetCore.Mvc.Testing

F.7. STOP — mostrame el output de `dotnet list package` para todos los
     proyectos. Quiero verificar que todas las versiones son compatibles
     con .NET 8 antes de seguir.

═════════════════════════════════════════════════════════════════
BLOQUE G — Directory.Build.props + .editorconfig + global.json
═════════════════════════════════════════════════════════════════

G.1. Crear Directory.Build.props en la raíz con configuración compartida:

     <Project>
       <PropertyGroup>
         <TargetFramework>net8.0</TargetFramework>
         <Nullable>enable</Nullable>
         <ImplicitUsings>enable</ImplicitUsings>
         <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
         <WarningsAsErrors />
         <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
         <AnalysisLevel>latest-recommended</AnalysisLevel>
         <LangVersion>latest</LangVersion>
         <NoWarn>$(NoWarn);CS1591</NoWarn>
       </PropertyGroup>
     </Project>

     Esto aplica nullable + warnings-as-errors a TODOS los proyectos
     automáticamente (la convención lo absorbe sin tener que repetir
     en cada .csproj).

G.2. Crear .editorconfig en la raíz con las reglas estándar de C#.
     Usá el template oficial de Microsoft como base; si no lo tenés a
     mano, generá uno con reglas mínimas para:
     - C# style (var preferences, brace style, expression-bodied members)
     - Naming conventions (interfaces I-prefix, async Async-suffix, etc.)
     - File organization (system usings first, alphabetical)

     Si tenés dudas sobre alguna regla específica, dejame ver el
     contenido propuesto antes de escribir el archivo.

═════════════════════════════════════════════════════════════════
BLOQUE H — Dockerfile + docker-compose.yml
═════════════════════════════════════════════════════════════════

H.1. Crear Dockerfile multi-stage en la raíz:

     # syntax=docker/dockerfile:1.7
     # ──── Build stage ────
     FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
     WORKDIR /src
     COPY ["global.json", "./"]
     COPY ["Directory.Build.props", "./"]
     COPY ["SkillSwap.sln", "./"]
     COPY ["src/SkillSwap.Domain/SkillSwap.Domain.csproj", "src/SkillSwap.Domain/"]
     COPY ["src/SkillSwap.Application/SkillSwap.Application.csproj", "src/SkillSwap.Application/"]
     COPY ["src/SkillSwap.Infrastructure/SkillSwap.Infrastructure.csproj", "src/SkillSwap.Infrastructure/"]
     COPY ["src/SkillSwap.API/SkillSwap.API.csproj", "src/SkillSwap.API/"]
     COPY ["src/SkillSwap.Shared/SkillSwap.Shared.csproj", "src/SkillSwap.Shared/"]
     RUN dotnet restore "src/SkillSwap.API/SkillSwap.API.csproj"

     COPY src/ src/
     RUN dotnet publish "src/SkillSwap.API/SkillSwap.API.csproj" \
         -c Release -o /app/publish --no-restore /p:UseAppHost=false

     # ──── Runtime stage ────
     FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
     WORKDIR /app
     EXPOSE 8080
     ENV ASPNETCORE_URLS=http://+:8080
     ENV ASPNETCORE_ENVIRONMENT=Production
     RUN groupadd -r app && useradd -r -g app app
     USER app
     COPY --from=build --chown=app:app /app/publish .
     HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
       CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1
     ENTRYPOINT ["dotnet", "SkillSwap.API.dll"]

H.2. Crear .dockerignore en la raíz para excluir bin/, obj/, tests/, docs/,
     .git/, BMAD folders, etc.

H.3. Crear docker-compose.yml en la raíz con 3 servicios:
     - api (build desde Dockerfile, puerto 8080:8080)
     - mysql 8.x (volumen persistente, env vars MYSQL_ROOT_PASSWORD desde .env)
     - seq (datalust/seq:latest para logs estructurados en dev, puerto 5341)

     Las env vars vienen de .env (no commitear). Crear .env.example con
     placeholders sin valores reales.

═════════════════════════════════════════════════════════════════
BLOQUE I — README.md + LICENSE + verificación final
═════════════════════════════════════════════════════════════════

I.1. Crear README.md minimal en la raíz. CRÍTICO: NO incluir credenciales
     hardcodeadas (lección aprendida del v1, ADR §1.2). Incluir:
     - Título + descripción breve
     - Status badge: "v2.0 - In Development"
     - Quick start con docker-compose
     - Link al ADR-001 y a docs/
     - Sección "Testing" con `dotnet test`
     - Sección "Contributing" con link a Conventional Commits

I.2. Crear LICENSE — copiar MIT License con año 2026 y holder "SkillSwap-Ten".

I.3. Crear .env.example con variables placeholder (DB connection, JWT,
     Seq URL). Sin valores reales.

I.4. Verificación final — TODO debe pasar:
        dotnet restore
        dotnet build --no-restore
        dotnet test --no-build --verbosity normal

     Resultados esperados:
     - Build: 0 errors, 0 warnings
     - Tests: 0 tests, 0 failures (proyectos vacíos)

     Si CUALQUIER warning aparece, parate y reportá. La idea es que
     v2 arranque con cero deuda.

═════════════════════════════════════════════════════════════════
BLOQUE J — Commit + push
═════════════════════════════════════════════════════════════════

J.1. Verificar git status:
        git status

     Esperado: untracked en la mayoría de los archivos nuevos +
     BMAD folders (que deben seguir ignoradas por .gitignore).

J.2. Stage selectivo (NO uses `git add .`):
        git add SkillSwap.sln global.json Directory.Build.props \
                .editorconfig Dockerfile .dockerignore docker-compose.yml \
                README.md LICENSE .env.example .gitignore \
                src/ tests/ docs/handoff/HANDOFF-02-*.md

     OBSERVACIÓN: Si el HANDOFF-02 no está en docs/handoff/, omitilo
     y reportame.

J.3. Verificar lo staged:
        git status --short
        git diff --cached --stat

     STOP — mostrame el output. Confirmá conmigo antes del commit.

J.4. Commit:
        git commit -m "feat: scaffold clean architecture solution v2.0

        - Add solution with 5 src projects (Domain, Application,
          Infrastructure, API, Shared) following ADR-001 §2.1
        - Add 4 test projects (Unit, Integration, Functional) per §7
        - Enforce dependency rules from §2.2 via project references
        - Configure base NuGet packages: MediatR, FluentValidation,
          Mapster, EF Core + Pomelo, Serilog, OpenTelemetry, xUnit,
          FluentAssertions, NSubstitute, Testcontainers
        - Add Directory.Build.props (nullable + warnings-as-errors)
        - Add .editorconfig with C# style rules
        - Add multi-stage Dockerfile + docker-compose.yml with MySQL + Seq
        - Add README without hardcoded credentials (security fix from v1)
        - First build: 0 errors, 0 warnings

        Refs: docs/adr/ADR-001-clean-architecture-migration.md §2, §3, §7"

J.5. Push:
        git push origin v2

═════════════════════════════════════════════════════════════════
BLOQUE K — Reporte final (Bloque F del HANDOFF-01 equivalente)
═════════════════════════════════════════════════════════════════

K.1. Reportame en un mismo mensaje:
     - `git log --oneline -5`
     - `dotnet sln list`
     - `tree -L 3 -I 'bin|obj|_bmad*|.agent*|.claude'` (si tenés tree)
     - Una tabla con: cada proyecto + cantidad de paquetes NuGet + sus referencias
     - Cualquier desviación del plan que tuviste que hacer

K.2. Confirmá que la build pasa con 0 warnings y que los tests corren
     (aunque sean 0).

K.3. Una vez confirmado, paso al siguiente handoff (HANDOFF-03: CI/CD
     con GitHub Actions, que es la Tarea #5 del roadmap).
```

---

## Después de que Claude Code termine

Cuando Claude Code te entregue el reporte final del Bloque K, regresá a Cowork y pegamelo. Verifico el estado en disco (igual que hice con HANDOFF-01) y avanzamos al **HANDOFF-03** para configurar CI/CD con GitHub Actions sobre la solution ya scaffolded.

---

## Notas de seguridad

- ⚠️ **Si la build falla con warnings**, no fuerces el commit. Investigá la causa primero — pueden ser versiones de paquetes incompatibles, o C# 12 features bloqueadas por nullable.
- ✅ **El tag `v1.0.0` sigue siendo tu red de seguridad.** Si todo este scaffolding queda mal, podés resetear v2 a `836f9ed` (el commit del HANDOFF-01) y reintentar.
- 💡 **Si BMAD Builder está disponible**, podés delegar bloques específicos (especialmente F y H) y dejar que él genere los archivos. Tu rol es validar que respeta las decisiones del ADR.
