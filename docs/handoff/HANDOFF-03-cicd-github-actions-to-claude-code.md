# Handoff #3 — CI/CD with GitHub Actions (Cowork → Claude Code)

> **Cómo usar este documento:**
> 1. Estás en `~/Personal/SkillSwap/` y en rama `v2`.
> 2. Nueva sesión de Claude Code (no reutilizar la del HANDOFF-02).
> 3. Copiá y pegá el bloque del prompt como tu primer mensaje.
> 4. Claude Code te va a pedir go/no-go por bloque.

---

## Contexto del scope

Este handoff configura el pipeline de **CI/CD** sobre la solution ya scaffolded, conforme al ADR-001 §7 (testing strategy + coverage gates). El objetivo es que **cada PR a `v2` y `main` pase obligatoriamente por**:
1. **Format check** (`dotnet format --verify-no-changes`)
2. **Build con cero warnings** (warnings-as-errors ya activo en `Directory.Build.props`)
3. **Tests pasando** (xUnit, 0 failures requeridos)
4. **Coverage report** generado y publicado como artifact

**Lo que SÍ se configura en HANDOFF-03:**
- `coverlet.runsettings` (collector config)
- `.github/workflows/ci.yml` (PR gate principal)
- `.github/workflows/cd.yml` (deployment, draft — Render se configura cuando v2 tenga endpoints reales)
- `.github/dependabot.yml` (auto-PRs para actualizar NuGet semanalmente)
- `.github/pull_request_template.md`
- Badges en `README.md`

**Lo que NO entra en HANDOFF-03 (parqueado intencionalmente):**
- Gates de cobertura **activos** — no tiene sentido validarlos contra 0 tests; van a quedar configurados pero como *informational* hasta el HANDOFF-04 (primer vertical slice).
- SonarCloud — opcional. Si lo querés más adelante, lo agregamos en otro handoff cuando tengamos código real.
- Branch protection en GitHub Settings — es manual UI, te doy instrucciones al final.
- Despliegue real a Render — necesita endpoints + deploy hook configurado.

---

## Prompt para Claude Code

```
Hola Claude Code. Soy Steven (medi77na), backend lead de SkillSwap.
HANDOFF-03: configurar CI/CD con GitHub Actions sobre la solution
ya scaffolded por HANDOFF-02.

CONTEXTO PREVIO:
- Rama actual: v2. Último commit: bf9eeb7 (scaffold clean architecture).
- Solution con 9 proyectos (5 src + 4 tests), build 0/0, test 0/0.
- ADR-001 (docs/adr/ADR-001-clean-architecture-migration.md) define
  los coverage targets en §7: Domain 90 / App 85 / Infra 70 / API 60
  / Total 80. Pero como aún no hay tests reales, los gates van como
  informational en esta primera iteración — vamos a flippearlos
  a hard-fail cuando lleguen los vertical slices.

REGLAS DE TRABAJO (iguales que HANDOFF-01 y 02):
- Go/no-go por bloque.
- STOP obligatorio si encontrás estado inesperado.
- Conventional Commits.
- No tocar GitHub UI (branch protection, secrets) — eso lo hace Steven manualmente.
- Si una decisión técnica no está en el ADR ni en este handoff, preguntá.

═════════════════════════════════════════════════════════════════
BLOQUE A — Pre-flight
═════════════════════════════════════════════════════════════════

A.1. Verificá estado:
        git branch --show-current             # debe decir: v2
        git status                            # working tree clean
        git log --oneline -2                  # bf9eeb7 + 836f9ed

A.2. Verificá que .github/workflows/ existe (lo creó HANDOFF-02):
        ls -la .github/

A.3. STOP — reportame A.1 y A.2 antes de continuar.

═════════════════════════════════════════════════════════════════
BLOQUE B — coverlet.runsettings
═════════════════════════════════════════════════════════════════

B.1. Crear coverlet.runsettings en la raíz. Contenido completo:

     <?xml version="1.0" encoding="utf-8"?>
     <RunSettings>
       <DataCollectionRunSettings>
         <DataCollectors>
           <DataCollector friendlyName="XPlat code coverage">
             <Configuration>
               <Format>cobertura,opencover,json</Format>
               <Exclude>[*.Tests?]*,[xunit.*]*,[*]*.Migrations.*</Exclude>
               <ExcludeByAttribute>Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute</ExcludeByAttribute>
               <ExcludeByFile>**/Migrations/*.cs,**/Program.cs</ExcludeByFile>
               <SingleHit>false</SingleHit>
               <UseSourceLink>true</UseSourceLink>
               <IncludeTestAssembly>false</IncludeTestAssembly>
               <SkipAutoProps>true</SkipAutoProps>
               <DeterministicReport>true</DeterministicReport>
             </Configuration>
           </DataCollector>
         </DataCollectors>
       </DataCollectionRunSettings>
     </RunSettings>

     Notas para vos (no editar):
     - Excluimos test assemblies y migrations del coverage.
     - Excluimos Program.cs (composition root con boilerplate de .NET).
     - DeterministicReport=true → outputs reproducibles en CI.

═════════════════════════════════════════════════════════════════
BLOQUE C — Workflow principal de CI (.github/workflows/ci.yml)
═════════════════════════════════════════════════════════════════

C.1. Crear .github/workflows/ci.yml con este contenido:

     name: CI

     on:
       push:
         branches: [main, v2]
       pull_request:
         branches: [main, v2]
       workflow_dispatch:

     env:
       DOTNET_VERSION: '8.0.x'
       DOTNET_NOLOGO: true
       DOTNET_CLI_TELEMETRY_OPTOUT: true
       DOTNET_SKIP_FIRST_TIME_EXPERIENCE: true
       NUGET_XMLDOC_MODE: skip

     jobs:
       build-and-test:
         name: Build & Test
         runs-on: ubuntu-latest
         timeout-minutes: 15

         steps:
           - name: Checkout
             uses: actions/checkout@v4
             with:
               fetch-depth: 0

           - name: Setup .NET ${{ env.DOTNET_VERSION }}
             uses: actions/setup-dotnet@v4
             with:
               dotnet-version: ${{ env.DOTNET_VERSION }}

           - name: Cache NuGet packages
             uses: actions/cache@v4
             with:
               path: ~/.nuget/packages
               key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj','**/global.json') }}
               restore-keys: |
                 ${{ runner.os }}-nuget-

           - name: Restore dependencies
             run: dotnet restore SkillSwap.sln

           - name: Verify code format
             run: dotnet format SkillSwap.sln --verify-no-changes --severity error --no-restore

           - name: Build (Release, warnings-as-errors)
             run: dotnet build SkillSwap.sln --configuration Release --no-restore

           - name: Test with coverage
             run: |
               dotnet test SkillSwap.sln \
                 --configuration Release \
                 --no-build \
                 --logger "trx;LogFileName=test-results.trx" \
                 --logger "console;verbosity=normal" \
                 --collect:"XPlat Code Coverage" \
                 --settings coverlet.runsettings \
                 --results-directory ./TestResults

           - name: Generate coverage report
             if: success() || failure()
             uses: danielpalme/ReportGenerator-GitHub-Action@5.3.11
             with:
               reports: '**/coverage.cobertura.xml'
               targetdir: 'coveragereport'
               reporttypes: 'HtmlInline;Cobertura;MarkdownSummaryGithub;TextSummary'
               historydir: 'coverage-history'

           - name: Append coverage summary to job output
             if: success() || failure()
             run: |
               if [ -f coveragereport/SummaryGithub.md ]; then
                 echo "## Coverage Report" >> $GITHUB_STEP_SUMMARY
                 cat coveragereport/SummaryGithub.md >> $GITHUB_STEP_SUMMARY
               fi

           - name: Upload coverage artifacts
             if: success() || failure()
             uses: actions/upload-artifact@v4
             with:
               name: coverage-report
               path: coveragereport
               retention-days: 30

           - name: Upload test results
             if: success() || failure()
             uses: actions/upload-artifact@v4
             with:
               name: test-results
               path: TestResults
               retention-days: 14

           # === Coverage gates (informational hasta HANDOFF-04) ===
           # Cuando aterricen los primeros tests, activá estos thresholds
           # convirtiendo el `continue-on-error: true` en `false`.
           # Targets del ADR-001 §7:
           #   Domain ≥90 / Application ≥85 / Infrastructure ≥70
           #   API ≥60 / Total ≥80
           - name: Enforce coverage thresholds (informational only)
             if: success()
             continue-on-error: true
             run: |
               echo "::notice::Coverage gates are informational in HANDOFF-03."
               echo "::notice::Will be enforced starting HANDOFF-04 (first vertical slice)."

C.2. STOP — mostrame el archivo creado para review antes de seguir
     al bloque D.

═════════════════════════════════════════════════════════════════
BLOQUE D — Workflow de CD (draft) (.github/workflows/cd.yml)
═════════════════════════════════════════════════════════════════

D.1. Crear .github/workflows/cd.yml con un workflow PARQUEADO
     (solo trigger manual hasta que tengamos endpoints reales).
     Contenido:

     name: CD (staging)

     # IMPORTANTE: este workflow está intencionalmente PARQUEADO.
     # No corre en push automático hasta que:
     #   1. Existan endpoints reales en SkillSwap.API
     #   2. El secret RENDER_DEPLOY_HOOK_URL esté configurado en GitHub
     # Por ahora solo se puede disparar manualmente (workflow_dispatch).
     # Activación esperada: post HANDOFF-04 (primer vertical slice).

     on:
       workflow_dispatch:
         inputs:
           confirm:
             description: 'Type DEPLOY to confirm'
             required: true
             type: string

     env:
       DOTNET_VERSION: '8.0.x'
       DOTNET_NOLOGO: true
       DOTNET_CLI_TELEMETRY_OPTOUT: true

     jobs:
       guard:
         name: Confirmation guard
         runs-on: ubuntu-latest
         steps:
           - name: Verify confirmation input
             if: github.event.inputs.confirm != 'DEPLOY'
             run: |
               echo "::error::Confirmation string did not match. Aborting."
               exit 1

       deploy:
         name: Deploy to Render (staging)
         needs: guard
         runs-on: ubuntu-latest
         timeout-minutes: 10
         environment: staging
         steps:
           - name: Trigger Render deploy hook
             env:
               HOOK: ${{ secrets.RENDER_DEPLOY_HOOK_URL }}
             run: |
               if [ -z "$HOOK" ]; then
                 echo "::error::RENDER_DEPLOY_HOOK_URL secret not configured."
                 echo "::error::Configure it in GitHub Settings > Environments > staging > Secrets."
                 exit 1
               fi
               curl -fsSL -X POST "$HOOK"
               echo "Deploy triggered."

═════════════════════════════════════════════════════════════════
BLOQUE E — Dependabot (.github/dependabot.yml)
═════════════════════════════════════════════════════════════════

E.1. Crear .github/dependabot.yml:

     version: 2
     updates:
       # NuGet packages
       - package-ecosystem: "nuget"
         directory: "/"
         schedule:
           interval: "weekly"
           day: "monday"
           time: "09:00"
           timezone: "America/Bogota"
         open-pull-requests-limit: 5
         target-branch: "v2"
         labels:
           - "dependencies"
           - "nuget"
         commit-message:
           prefix: "chore(deps)"
           include: "scope"
         groups:
           microsoft:
             patterns:
               - "Microsoft.*"
               - "System.*"
           opentelemetry:
             patterns:
               - "OpenTelemetry.*"
           serilog:
             patterns:
               - "Serilog.*"
           testing:
             patterns:
               - "xunit*"
               - "FluentAssertions"
               - "NSubstitute"
               - "Testcontainers.*"
               - "coverlet.*"

       # GitHub Actions
       - package-ecosystem: "github-actions"
         directory: "/"
         schedule:
           interval: "weekly"
         target-branch: "v2"
         labels:
           - "dependencies"
           - "github-actions"
         commit-message:
           prefix: "chore(actions)"

       # Docker
       - package-ecosystem: "docker"
         directory: "/"
         schedule:
           interval: "weekly"
         target-branch: "v2"
         labels:
           - "dependencies"
           - "docker"
         commit-message:
           prefix: "chore(docker)"

═════════════════════════════════════════════════════════════════
BLOQUE F — PR template (.github/pull_request_template.md)
═════════════════════════════════════════════════════════════════

F.1. Crear .github/pull_request_template.md:

     ## Summary
     <!-- Breve descripción de qué cambia y por qué -->

     ## Type of change
     - [ ] feat — new feature
     - [ ] fix — bug fix
     - [ ] refactor — code change without behavior change
     - [ ] perf — performance improvement
     - [ ] test — adding/updating tests
     - [ ] docs — documentation only
     - [ ] chore — tooling, deps, config
     - [ ] BREAKING CHANGE

     ## Related
     - ADR: <!-- e.g. docs/adr/ADR-002-... -->
     - Closes #
     - Refs #

     ## Checklist
     - [ ] My code follows the style of this project (`dotnet format` passes)
     - [ ] Build passes with 0 warnings (`dotnet build`)
     - [ ] Tests pass (`dotnet test`)
     - [ ] I added tests covering my changes (unit + integration as applies)
     - [ ] Coverage thresholds from ADR-001 §7 are respected
     - [ ] Public API changes are documented in `docs/api/v2/`
     - [ ] If this introduces a breaking change, the frontend team has been notified
     - [ ] No secrets, credentials, or PII in source

     ## Notes for reviewer
     <!-- Cualquier contexto adicional, decisiones tomadas, alternativas evaluadas -->

═════════════════════════════════════════════════════════════════
BLOQUE G — README badges + sección CI/CD
═════════════════════════════════════════════════════════════════

G.1. Editá README.md para agregar badges en la parte superior
     (después del título principal). Los badges van con estos URLs:

     [![CI](https://github.com/SkillSwap-Ten/SkillSwap/actions/workflows/ci.yml/badge.svg?branch=v2)](https://github.com/SkillSwap-Ten/SkillSwap/actions/workflows/ci.yml)
     [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
     [![.NET 8 LTS](https://img.shields.io/badge/.NET-8.0%20LTS-blue.svg)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
     [![Status](https://img.shields.io/badge/v2.0-in--development-orange.svg)](docs/adr/ADR-001-clean-architecture-migration.md)

G.2. Agregá una sección "## CI/CD" en el README explicando:
     - Cada PR a `v2` o `main` dispara el workflow CI
     - Steps: format check → build → test → coverage report
     - Coverage gates están INFORMATIONAL hasta HANDOFF-04
     - Targets finales (del ADR-001 §7): Domain 90 / App 85 / Infra 70 / API 60 / Total 80
     - El workflow CD está parqueado, se activa post HANDOFF-04

     Mantenelo conciso, 2 párrafos máximo.

═════════════════════════════════════════════════════════════════
BLOQUE H — Stage + commit + push
═════════════════════════════════════════════════════════════════

H.1. Verificá git status:
        git status --short

     Esperado: untracked en .github/workflows/, .github/dependabot.yml,
     .github/pull_request_template.md, coverlet.runsettings; modified
     en README.md; untracked también HANDOFF-03 si lo dejaste en docs/handoff/.

H.2. Stage selectivo:
        git add .github/workflows/ci.yml \
                .github/workflows/cd.yml \
                .github/dependabot.yml \
                .github/pull_request_template.md \
                coverlet.runsettings \
                README.md \
                docs/handoff/HANDOFF-03-*.md

H.3. STOP — mostrame `git diff --cached --stat` antes de commitear.

H.4. Commit:
        git commit -m "ci: add GitHub Actions workflows + coverage tooling

        - Add ci.yml: format check, build, test, coverage on PRs to main/v2
        - Add cd.yml (parked): manual-only Render deploy hook, activates
          after first vertical slice (HANDOFF-04)
        - Add coverlet.runsettings with exclusions for migrations and Program.cs
        - Add dependabot.yml: weekly NuGet + actions + docker updates
        - Add pull_request_template.md with ADR-aligned checklist
        - Add CI badges and CI/CD section to README

        Coverage gates from ADR-001 §7 are wired as informational until
        first real tests land in HANDOFF-04.

        Refs: docs/adr/ADR-001-clean-architecture-migration.md §7"

H.5. Push:
        git push origin v2

═════════════════════════════════════════════════════════════════
BLOQUE I — Verificar la primera corrida de CI
═════════════════════════════════════════════════════════════════

I.1. Después del push, GitHub Actions debería disparar el workflow CI
     automáticamente. Esperá 30 segundos y verificá:

        gh run list --branch v2 --limit 1

     (si no tenés gh CLI, abrí https://github.com/SkillSwap-Ten/SkillSwap/actions
     en el navegador)

I.2. Si la corrida está running, esperá a que termine (~3-5 min).
     Cuando termine, mostrame:
        gh run view --log <run-id>           # si tenés gh
     o el output relevante del browser.

I.3. RESULTADO ESPERADO:
        - Format check: ✅ pass
        - Build: ✅ pass (0 warnings, 0 errors)
        - Test: ✅ pass (0 tests, 0 failures — esperable, sin tests reales todavía)
        - Coverage report: ✅ generado (mostrará 0% pero existe el artefacto)

     Si CUALQUIER step falla, parate y reportame el error completo.
     Las causas comunes:
        - Format error si el .editorconfig encuentra algo
        - Cache miss si las versiones de paquetes no resuelven
        - ReportGenerator versión incorrecta

═════════════════════════════════════════════════════════════════
BLOQUE J — Reporte final
═════════════════════════════════════════════════════════════════

J.1. Reportame:
     - `git log --oneline -3`
     - URL de la corrida CI exitosa
     - Tabla con cada step del workflow + duración + status
     - Cualquier desviación del plan
     - Cualquier warning del log de la corrida (aunque no falle, quiero verlos)

J.2. Confirmá que el badge CI del README ya carga verde en GitHub
     (puede tardar 1-2 min después del primer run exitoso).

J.3. ⚠️ AVISO PARA STEVEN: después de tu confirmación, hay TRES tareas
     manuales en GitHub UI que NO puedo hacer yo:

     (a) Branch protection en `main`:
         Settings → Branches → Add rule → Branch name pattern: main
         - Require a pull request before merging
         - Require approvals: 1
         - Require status checks: CI / Build & Test
         - Require conversation resolution
         - Do not allow bypassing

     (b) Branch protection en `v2` (similar pero más laxo durante dev):
         - Require status checks: CI / Build & Test
         - Allow force pushes by admins (útil durante el dev fase)

     (c) Habilitar Dependabot:
         Settings → Code security → Dependabot version updates → Enable
         (puede que ya esté on por default si el repo es público)

     Estos pasos los hace Steven (que tiene acceso admin al repo).
```

---

## Después de que Claude Code termine

Cuando Claude Code te entregue el reporte del Bloque J, pegámelo. Yo:
1. Verifico en disco que los archivos quedaron como deben.
2. Visito la URL del CI run para confirmar que pasó.
3. Te paso instrucciones detalladas para los 3 pasos manuales de GitHub UI.
4. Marco la Tarea #5 como completed y arrancamos con la **Tarea #6: ADR-002 Auth & Authorization** — y acá **sí entra BMAD Architect Agent** finalmente.

---

## Notas de seguridad

- ⚠️ **El primer run del CI puede fallar** por razones triviales (caché frío, formato de YAML). No te asustes — investigamos y corregimos en un commit follow-up.
- ✅ **El workflow CD está parqueado intencionalmente.** No va a intentar deployar nada hasta que vos lo dispares manualmente con `workflow_dispatch`.
- ✅ **Coverage 0% en el primer run es esperable** — no significa que algo está mal. Los tests reales aterrizan en HANDOFF-04.
- ⚠️ **Si Claude Code ofrece configurar branch protection vía gh CLI** (lo puede hacer), permítelo solo si lo querés automatizado. Mi preferencia es que vos lo hagas manualmente la primera vez para entender qué estás activando.
