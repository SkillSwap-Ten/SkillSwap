# Handoff #1 — v2.0 Bootstrap (Cowork → Claude Code)

> **Cómo usar este documento:**
> 1. Abrí tu terminal en la raíz del repo (`cd ~/Personal/SkillSwap/`).
> 2. Lanzá Claude Code (`claude` o desde tu IDE).
> 3. Copiá y pegá **el bloque completo de abajo** como tu primer mensaje a Claude Code.
> 4. Claude Code te irá pidiendo confirmación antes de cada operación destructiva.

---

## Prompt para Claude Code

```
Hola Claude Code. Soy Steven (medi77na), backend lead de SkillSwap.
Estamos arrancando la migración a v2.0 con arquitectura limpia.

CONTEXTO:
- Repo: https://github.com/SkillSwap-Ten/SkillSwap (rama main = v1 desplegada en Render)
- v1 actual: layered architecture, .NET con C#, MySQL, sin tests, sin CI/CD.
- v2 objetivo: Clean Architecture con 5 proyectos (.NET 8 LTS), tests, CI/CD,
  observability. Breaking changes aceptados (el frontend Skiller se adaptará después).

ARTEFACTO DE REFERENCIA:
- Lee primero el ADR-001 que está en docs/adr/ADR-001-clean-architecture-migration.md
- Ese ADR es la fuente de verdad de TODAS las decisiones técnicas.
- Si algo no está en el ADR, preguntame antes de asumir.

TU TAREA EN ESTA SESIÓN:
Ejecutar la SECCIÓN 6.1 del ADR (Git workflow) — el bootstrap de la rama v2.
Concretamente:

1. Verificar que el repo está limpio (git status). Si hay cambios sin commit,
   detenete y reportame para decidir qué hacer.

2. Asegurar que la rama main está actualizada con origin/main.

3. Crear el tag v1.0.0 sobre main con mensaje:
   "SkillSwap v1.0.0 - legacy backend (layered architecture)"
   Pushear el tag al remoto.

4. Renombrar la rama 'develop' actual a 'v1-archive' (preserva el trabajo previo
   por si lo necesitamos como referencia). Pushearla y eliminar la 'develop' vieja
   del remoto.

5. Crear la orphan branch 'v2' partiendo de main:
       git checkout main
       git checkout --orphan v2

6. ANTES de hacer git rm, crear el archivo .gitignore con el siguiente contenido:

   --- INICIO .gitignore ---
   # === Build outputs ===
   bin/
   obj/
   *.user
   *.suo
   .vs/

   # === .NET ===
   *.nupkg
   *.snupkg
   .vscode/*
   !.vscode/settings.json
   !.vscode/launch.json
   !.vscode/extensions.json

   # === Secrets ===
   appsettings.Development.json
   .env
   .env.*
   !.env.example

   # === BMAD methodology (local tooling only) ===
   _bmad/
   _bmad-output/
   .agent/
   .agents/
   .claude/
   .bmad-core/

   # === OS ===
   .DS_Store
   Thumbs.db

   # === IDE ===
   *.swp
   *.swo
   --- FIN .gitignore ---

7. Limpiar el working tree de archivos rastreados de v1:
       git rm -rf --cached .
   (Esto remueve archivos del índice de git, NO del filesystem.)

   Luego eliminar físicamente los archivos de v1 del working tree EXCEPTO:
   - docs/
   - .gitignore (que acabás de crear)
   - .git/

   Usá EXACTAMENTE este comando (preserva docs/, .gitignore, y las carpetas
   locales de BMAD que NO deben ir al repo pero TAMPOCO deben borrarse del disco):

   find . -mindepth 1 -maxdepth 1 \
     ! -name '.git' \
     ! -name '.gitignore' \
     ! -name 'docs' \
     ! -name '_bmad' \
     ! -name '_bmad-output' \
     ! -name '.agent' \
     ! -name '.agents' \
     ! -name '.claude' \
     ! -name '.bmad-core' \
     -exec rm -rf {} +

   ATENCIÓN: este comando es destructivo en el working tree. Pediime confirmación
   explícita ANTES de ejecutarlo. Antes de confirmar, mostrame el output de:
       ls -la
   para que yo verifique qué se va a eliminar y qué se va a preservar.

8. Verificar que docs/adr/ADR-001-clean-architecture-migration.md existe.
   Si NO existe, detenete y reportame.

9. Hacer el primer commit de v2:
       git add .gitignore docs/
       git commit -m "chore: initialize SkillSwap v2 - clean architecture rewrite

       - Add .gitignore with BMAD tooling exclusions
       - Add ADR-001: Clean Architecture migration decision
       - Working tree cleaned, ready for vertical slice scaffolding

       Refs: docs/adr/ADR-001-clean-architecture-migration.md"

10. Pushear la rama v2 al remoto:
        git push -u origin v2

11. Reportame el resultado final con:
    - Output de `git log --oneline -5`
    - Output de `git branch -a`
    - Output de `git tag --list`
    - Tamaño del working tree (`ls -la`)

REGLAS DE TRABAJO:
- Pediime confirmación antes de cualquier operación destructiva (rm, push -f, branch -D, tag -d).
- Si encontrás un estado inesperado, detenete y reportá antes de continuar.
- No alteres el contenido del ADR-001 — solo léelo.
- Todos los commits deben usar Conventional Commits (chore:, feat:, fix:, docs:).
- No instales nada todavía (ni dotnet new, ni paquetes NuGet). Eso es para el próximo handoff.

Cuando termines, sumarizá brevemente qué se hizo y qué quedó pendiente para
el HANDOFF-02 (scaffolding de la solution).
```

---

## Después de que Claude Code termine

Una vez que Claude Code te confirme que la rama `v2` está creada y pusheada, regresá a Cowork (esta sesión) y reportame el resultado. Vamos a producir el **HANDOFF-02** para el scaffolding de los 5 proyectos `.NET`, esta vez delegando a **BMAD Builder** dentro de Claude Code.

---

## Notas de seguridad antes de ejecutar

- ⚠️ **Asegurate de tener todo committeado** en main y develop antes de arrancar. Cualquier cambio sin commit se pierde.
- ⚠️ **Pushea o respaldá la branch `develop` actual** si tenés trabajo no mergeado. El paso 4 la renombra, así que no se pierde, pero verificá antes.
- ✅ **El tag `v1.0.0` es tu red de seguridad.** Aunque algo salga mal, podés volver siempre a ese estado con `git checkout v1.0.0`.
- ✅ La orphan branch **no toca `main`** — la API en Render sigue funcionando sin interrupción durante todo este proceso.
