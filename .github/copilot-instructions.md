# Copilot Onboarding Guide

## What this repo contains
- **Product:** IDIKA National EHR Portal – a public-facing Kentico Xperience 30.1 site implemented with ASP.NET Core 8.0. It serves localized informational content and exposes custom MVC controllers, view components, and Kentico content repositories.
- **Solution layout:** `src/HdikaNehrPortal.sln` aggregates two projects:
  - `src/NehrPortal` – the main web app (controllers, Razor views, Page Builder components, services, Kentico models, configuration files, Dockerfile).
  - `src/CustomInitializationModule` – Kentico module configuring Azure Blob storage mapping at startup.
- **Supporting assets:**
  - `docs/` – architecture, PRD, UX spec, stories, and generated shards.
  - `k6-performance-test/` – JavaScript load tests (uses Grafana k6).
  - `pipeline-cd.yaml` – Azure DevOps multi-stage pipeline (restore/build/test/Sonar/publish Docker images/deploy to Azure Container Apps via JFrog Artifactory).
  - `db/idika-nehr-portal.bacpac` – baseline Azure SQL backup for Kentico content.

## Toolchain & environment
- **Primary runtime:** .NET SDK **8.0+** (repo verified with `dotnet --version` → 9.0.305; stick to LTS 8.0 where possible to match target framework).
- **Packages:** Kentico Xperience packages (restore requires access to `nuget.org`; corporate builds use JFrog Artifactory credentials).
- **Frontend tooling:** standard Razor views; no npm build step. Optional k6 CLI for performance tests (`brew install k6` on macOS per `k6-performance-test/readme.md`).
- **Containerization:** `src/Dockerfile` used by pipeline for admin/content images.

## Build, run, and test checklist
> Trust these steps; only run additional searches if they prove inaccurate.

1. **Clean slate (optional but recommended):**
   ```powershell
   dotnet nuget locals all --clear
   ```
2. **Restore packages:**
   ```powershell
   cd src
   dotnet restore HdikaNehrPortal.sln
   ```
   - _Observations:_ Multiple restores failed with `503 Service Unavailable` or "connection was forcibly closed" when downloading Kentico packages from `nuget.org`. Re-running eventually succeeds; if failures persist, add your Artifactory feed or retry with `--no-cache`. Record this in PRs.
3. **Build solution:**
   ```powershell
   dotnet build HdikaNehrPortal.sln -c Release
   ```
4. **Run the site locally (content app only):**
   ```powershell
   dotnet run --project NehrPortal/HdikaNehr.csproj --launch-profile "NehrPortal"
   ```
   - Requires a valid `CMSConnectionString` in `appsettings.Development.json`. Replace secrets with environment variables locally; never commit credentials.
5. **Unit tests:** none in repo today. Pipeline looks for `*.UnitTests.dll`; if you add tests, prefer xUnit and place them under a new test project referenced by the solution.
6. **Performance tests (optional):** run via k6 after installing CLI:
   ```powershell
   cd k6-performance-test
   k6 run StressTestPortal.js
   ```
7. **Static analysis:** cloud pipeline runs SonarQube; no local rule set supplied. If needed, integrate `dotnet sonarscanner` with the same solution after configuring credentials.
8. **Docker publish (aligns with pipeline):**
   - Build package output first (`dotnet publish` with `/p:AdminAttached=true/false`).
   - Use the provided Dockerfile as build context root (`src/`); pipeline builds distinct images from published artifacts.

Always run **restore → build → targeted tests** before pushing. Document any repeated NuGet connectivity failures and reference Artifactory variables (`JFROG_*`) when relevant to your change.

## Repository layout & navigation tips
- **Root:** `.bmad-core/` (agent automation), `.github/`, `.vscode/`, `docs/`, `k6-performance-test/`, `src/`, `pipeline-cd.yaml`, top-level `README.md` (placeholder – don’t rely on it).
- **`src/NehrPortal`:**
  - `Program.cs` – configures Kentico features, localization, SendGrid/SMTP, tag manager, routing.
  - `Controllers/` – MVC endpoints (articles, home, sitemap, static content).
  - `Components/ViewComponents/` + `Services/` – navigation/footer services, caching logic.
  - `Models/` – Kentico generated content models & repositories.
  - `Views/` & `PageTemplates/` – Razor UI.
  - `appsettings*.json` – environment-specific settings (contain storage/share keys; treat as secrets).
- **`src/CustomInitializationModule`:** `CustomInitializationModule.cs` maps Azure Storage containers for assets/form files at startup based on `UseAzureStorageAccount` setting.
- **`docs/`:**
  - `brief.md`, `prd.md`, `front-end-spec.md`, `architecture.md` plus sharded sections (consult these before designing new features).
  - `stories/` – individual user stories (`nehr-portal-content-api-for-ai-chatbot.md`, etc.).
- **CI/CD (`.github/workflows` currently empty):** Builds are driven by Azure DevOps `pipeline-cd.yaml` with stages: Build, Test (VSTest), Verify (SonarQube), PublishArtifact (publish + Docker build & push + JFrog Xray), Deploy (Azure Container Apps).

## Working guidelines
- **Respect Kentico patterns:** use repository classes (`ArticlePageRepository`, etc.) and dependency injection via `IServiceCollectionExtensions.AddHdikaNehrServices()`.
- **Localization:** rely on `ICurrentWebsiteChannelPrimaryLanguageRetriever`; default language is Greek.
- **Caching:** follow progressive cache usage in existing services to meet performance targets.
- **Secrets:** never commit real connection strings or Azure SendGrid keys – use Key Vault, pipeline variables, or user secrets.
- **Feature toggles:** when touching navigation/content flows, adopt feature flags or configuration toggles to avoid regressions.
- **Error handling/logging:** integrate with Kentico logging and `ILogger`; ensure new endpoints are instrumented for Application Insights (see architecture.md for telemetry plans).

## Collaboration guardrails
- Do **not** commit or push changes without explicit approval from the requester.
- Avoid introducing mock data; allow failures to surface real integration or configuration gaps.

## Architect notes
- Keep the Kentico content model source of truth in sync with Azure-hosted data; plan migrations alongside schema changes.
- Design new services with caching boundaries and localization in mind—multi-language support is core.
- Favor incremental feature flags for risky content delivery updates so portals can roll back quickly.
- Monitor cloud costs when enabling additional Azure services; align with existing pipeline automation.

## Before you code
1. Read the relevant docs in `docs/` (brief → PRD → architecture → UX spec → story) to understand scope and acceptance criteria.
2. Verify `dotnet restore` succeeds (retry or point to Artifactory if needed) **before** editing code to avoid blocked builds later.
3. When adding dependencies, update `HdikaNehr.csproj` and document any new feed requirements in PR notes.
4. Run `dotnet build` (and tests if introduced) locally each time you touch compile-time resources.
5. If you modify content delivery or caching logic, add/update k6 tests as required and note execution steps.

## Validation summary before PRs
- ✅ `dotnet restore HdikaNehrPortal.sln`
- ✅ `dotnet build HdikaNehrPortal.sln -c Release`
- ✅ Run newly added tests (`dotnet test` on the specific project).
- ✅ Spot-check the portal with `dotnet run` if UI changes are involved.
- ✅ Update docs/user stories when behavior changes.

Follow these instructions first; only search the codebase if information here is missing or incorrect.