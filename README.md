# Thesis Management System

A web platform for universities to coordinate diploma thesis workflows between students and supervising professors — topic proposals, progress updates, file submissions, and feedback, all in one place.

Built with **Blazor Server** (.NET 10), **EF Core**, and **ASP.NET Core Identity**.

---

## What it does

| Role | Capabilities |
|---|---|
| **Professor** | Propose thesis topics, review student updates, leave feedback, assign students to topics, receive interest notifications |
| **Student** | Browse open topics, express interest, submit progress updates with file attachments (PDF, DOCX, PPTX, ZIP up to 20 MB), track feedback |

---

## Quick start

### 1. Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) — provides the SQL Server database

> **No Docker?** Install Visual Studio 2022 (which includes SQL Server LocalDB) and skip the Docker step — the default connection string in `appsettings.json` targets LocalDB automatically.

### 2. Start the database (Docker)

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" \
  -p 1433:1433 --name sql -d mcr.microsoft.com/mssql/server:2022-latest
```

> On Windows PowerShell, replace `\` with a backtick `` ` `` for line continuation.

To stop/restart between sessions:
```bash
docker stop sql
docker start sql
```

### 3. Run the app

```bash
git clone https://github.com/your-org/university-management-system.git
cd university-management-system

dotnet restore UniversitySystem.sln
dotnet run --project University.Web/University.Web.csproj
```

Open **http://localhost:5118** in your browser.

On first run, EF Core automatically applies migrations and seeds all demo accounts — no manual database setup needed.

### 4. Log in with a demo account

**Password for all accounts: `TempPass123!`**

| Role | Email |
|---|---|
| Professor | `prof1@univ.edu` … `prof5@univ.edu` |
| Student | `student1@univ.edu` … `student15@univ.edu` |

---

## Running the tests

```bash
dotnet test UniversitySystem.sln
# Expected: 97 tests, 0 failures (82 unit + 15 integration)
```

---

## Performance tests (k6)

Load tests for the golden demo flow live under `k6-performance-tests/`.

### Install k6

```powershell
# Windows — winget
winget install k6 --source winget --accept-source-agreements --accept-package-agreements

# Windows — Chocolatey
choco install k6

# macOS
brew install k6
```

### Run

```bash
# Start the app first (or use: .\scripts\start-demo.ps1)
dotnet run --project University.Web/University.Web.csproj

# Then in a second terminal:
cd k6-performance-tests
k6 run golden-flow.js
```

---

## Project structure

```
UniversitySystem.sln
├── University.Domain/           Entities, enums, domain logic
├── University.Application/      CQRS commands & handlers (MediatR), validators
├── University.Infrastructure/   EF Core DbContext, repositories, storage services
├── University.Web/              Blazor Server app — pages, layout, components
├── tests/
│   ├── University.UnitTests/
│   └── University.IntegrationTests/
├── k6-performance-tests/        k6 load-test scripts
├── k8s/                         Kubernetes deployment & service manifests
└── scripts/
    └── start-demo.ps1           One-shot demo launcher
```

---

## Tech stack

.NET 10 · Blazor Server · EF Core 10 · SQL Server · ASP.NET Core Identity · MediatR · FluentValidation · xUnit · bunit

---

## Attachment storage

By default, files are saved locally under `wwwroot/attachments/`. To switch to Azure Blob Storage, set `Attachments:StorageProvider` to `AzureBlob` in `appsettings.json` and provide your connection string.

---

## CI/CD

Azure DevOps (`pipeline-cd.yaml`): restore → build → test → SonarQube → Docker image → Azure Container Apps.

