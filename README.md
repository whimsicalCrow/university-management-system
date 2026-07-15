# University Management System

A web platform for universities to coordinate diploma thesis workflows between students and supervising professors — topic proposals, progress updates, file submissions, and feedback, all in one place.

Built with **Blazor Server** (.NET 10), **EF Core**, and **ASP.NET Core Identity**.

---

## What it does

| Role | Capabilities |
|---|---|
| **Professor** | Propose thesis topics, review student updates, leave feedback, assign students to topics, receive interest notifications |
| **Student** | Browse open topics, express interest, submit progress updates with file attachments (PDF, DOCX, PPTX, ZIP up to 20 MB), track feedback |

---

## Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) — provides the SQL Server database
- Git

> **No Docker?** Install Visual Studio 2022 (which includes SQL Server LocalDB) and skip the Docker step — the default connection string in `appsettings.json` targets LocalDB automatically.

### 1. Clone the repository

```bash
git clone https://github.com/your-org/university-management-system.git
cd university-management-system
```

### 2. Start the database

#### Option A: Docker (recommended)

```bash
docker build -t university-sql .
docker run -d \
  --name university-sql \
  -p 1433:1433 \
  -e MSSQL_SA_PASSWORD=YourStrong!Passw0rd \
  -m 2g \
  university-sql
```

> On Windows PowerShell, replace `\` with a backtick `` ` `` for line continuation.

**Troubleshooting:**
- **Container exits immediately (exit code 137):** Increase Docker Desktop memory in settings to at least 4GB total, or allocate more with `-m 4g`
- **Can't connect to port 1433:** Check `docker ps` to verify the container is running, and `docker logs university-sql` for errors
- **Persist data between runs:** Add `-v university-db:/var/opt/mssql` to the docker run command

To stop/restart between sessions:
```bash
docker stop university-sql
docker start university-sql
```

#### Option B: SQL Server LocalDB (Windows only)

Skip Docker and use the local connection string in `appsettings.json` — no additional setup needed.

### 3. Run the application

```bash
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

## Running Tests

### Unit and Integration Tests

```bash
dotnet test UniversitySystem.sln
# Expected: 97 tests, 0 failures (82 unit + 15 integration)
```

### Performance Tests (k6)

Load tests for the golden demo flow live under `k6-performance-tests/`.

#### Install k6

```powershell
# Windows — winget
winget install k6 --source winget --accept-source-agreements --accept-package-agreements

# Windows — Chocolatey
choco install k6

# macOS
brew install k6
```

#### Run

```bash
# Start the app first (or use: .\scripts\start-demo.ps1)
dotnet run --project University.Web/University.Web.csproj

# Then in a second terminal:
cd k6-performance-tests
k6 run golden-flow.js
```

---

## Project Structure

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
├── scripts/
│   └── start-demo.ps1           One-shot demo launcher
├── Dockerfile                   SQL Server 2022 image with health check
└── .dockerignore                Build context exclusions
```

---

## Tech Stack

.NET 10 · Blazor Server · EF Core 10 · SQL Server · ASP.NET Core Identity · MediatR · FluentValidation · xUnit · bunit · Docker

---

## Database & Storage

### SQL Server

Default: SQL Server 2022 running in Docker (see Quick Start). Connection string targets `localhost,1433`.

Health check verifies the database is ready to accept connections:
```bash
docker ps --format "{{.Names}}\t{{.Status}}"
```

Once status shows `healthy`, the app is ready to start.

### Attachment Storage

By default, files are saved locally under `wwwroot/attachments/`. To switch to Azure Blob Storage, set `Attachments:StorageProvider` to `AzureBlob` in `appsettings.json` and provide your connection string.

---

## CI/CD

Azure DevOps (`pipeline-cd.yaml`): restore → build → test → SonarQube → Docker image → Azure Container Apps.

---

## Environment Variables

When running the Docker container, override defaults:

```bash
docker run -d \
  -e MSSQL_SA_PASSWORD=MySecurePassword123! \
  -e MSSQL_AGENT_ENABLED=true \
  university-sql
```

- `ACCEPT_EULA=Y` — Required to accept SQL Server EULA (set automatically)
- `MSSQL_SA_PASSWORD` — SA account password (default: `YourStrong!Passw0rd`)
- `MSSQL_AGENT_ENABLED=true` — Enables SQL Server Agent (optional)

---

## Connecting with SQL Tools

Use any SQL Server client (SSMS, Azure Data Studio, DBeaver) to connect directly:

- **Server:** `localhost,1433`
- **Username:** `sa`
- **Password:** (your `MSSQL_SA_PASSWORD` value)

Or via command line:
```bash
sqlcmd -S localhost -U sa -P YourStrong!Passw0rd
```
