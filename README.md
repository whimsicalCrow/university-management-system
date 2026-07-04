# University Management System

A thesis-collaboration platform built with Clean Architecture on .NET 10, Blazor Server, CQRS (MediatR), EF Core, and ASP.NET Core Identity.

## Features

- **Thesis Topics** — professors create and manage topics; students express interest; assignment closes the topic
- **Thesis Updates** — students submit progress updates with optional file attachments (PDF, DOCX, PPTX, ZIP — up to 20 MB)
- **Professor Feedback Loop** — professors review updates, change status (Approved / Needs Revision), and leave written comments; feedback is persisted and visible to students in real time
- **Student Dashboard** — aggregated view of thesis status, pending feedback comments, and uploaded artifacts
- **Role-based navigation** — Professor and Student roles enforced via ASP.NET Core Identity policies
- **Pluggable attachment storage** — Local filesystem (default) or Azure Blob Storage, switched via configuration
- **Signed download tokens** — time-limited, tamper-proof URLs for artifact downloads (15-minute expiry)
- **Auto-migration** — EF Core migrations run on startup; demo accounts seeded automatically

## Tech stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10 / ASP.NET Core |
| UI | Blazor Server (interactive SSR) |
| CQRS | MediatR 11 |
| Validation | FluentValidation 11 |
| ORM | EF Core 10 + SQL Server |
| Identity | ASP.NET Core Identity |
| Markdown rendering | Markdig |
| Tests | xUnit, bunit, EF Core InMemory |

## Project layout

```
UniversitySystem.sln
├── University.Domain/          Entities, domain methods, enums, exceptions
├── University.Application/     MediatR commands + handlers, interfaces, validators
├── University.Infrastructure/  EF Core DbContext, repositories, storage services
├── University.Web/             Blazor Server app (pages, layout, services)
└── tests/
    ├── University.UnitTests/       82 xUnit + bunit tests
    └── University.IntegrationTests/ 15 EF InMemory integration tests
```

## Quick start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (recommended) **or** SQL Server LocalDB (included with Visual Studio)

### Docker Desktop setup (recommended)

1. Install and start [Docker Desktop](https://www.docker.com/products/docker-desktop/).
2. Pull and run SQL Server 2022:

```powershell
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" `
  -p 1433:1433 --name sql -d mcr.microsoft.com/mssql/server:2022-latest
```

3. Verify the container is running:

```powershell
docker ps   # should show container "sql", port 0.0.0.0:1433->1433/tcp
```

4. To stop and restart the container between sessions:

```powershell
docker stop sql
docker start sql
```

The `appsettings.Development.json` connection string is pre-configured for this container — no further changes needed.

### Run

```powershell
# Restore and build
dotnet restore UniversitySystem.sln
dotnet build  UniversitySystem.sln -c Release

# Start the app (auto-migrates and seeds accounts on first run)
dotnet run --project University.Web/University.Web.csproj
```

Open **http://localhost:5118** (or **https://localhost:7173**).

### Connection string

The default `appsettings.json` targets LocalDB:

```
Server=(localdb)\mssqllocaldb;Database=UniversityCollaborationDb;Trusted_Connection=True;
```

For a Docker SQL Server instance, override in `appsettings.Development.json` (already pre-configured):

```
Server=127.0.0.1,1433;Database=UniversityDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;
```

### Demo accounts

Accounts are seeded automatically on first run. **Password for all accounts: `TempPass123!`**

- 5 professors: `prof1@univ.edu` – `prof5@univ.edu` (Role: `Professor`)
- 15 students: `student1@univ.edu` – `student15@univ.edu` (Role: `Student`)

See [docs/demo-users.md](docs/demo-users.md) for full names, research areas, and student specializations.

### Attachment storage

| Setting | Value | Effect |
|---|---|---|
| `Attachments:StorageProvider` | `Local` (default) | Saves files under `wwwroot/attachments/` |
| `Attachments:StorageProvider` | `AzureBlob` | Uses `AzureBlobAttachmentStorageService` |
| `Attachments:MaxFileSizeBytes` | `20971520` | 20 MB upload limit |
| `Attachments:AllowedExtensions` | `.pdf .docx .pptx .zip` | Blocked types rejected at upload |

## CI/CD

Builds run via Azure DevOps (`pipeline-cd.yaml`): restore → build → test → SonarQube → Docker publish → deploy to Azure Container Apps.

> GitHub Actions (`.github/workflows/`) is not yet configured.

## Kubernetes

```powershell
# After pushing your image, update k8s/deployment.yaml with your image tag, then:
kubectl apply -f k8s/
```

```powershell
dotnet test UniversitySystem.sln
# Expected: 97 tests, 0 failures (82 unit + 15 integration)
```

## Configuration reference

### Attachment storage

| Setting | Value | Effect |
|---|---|---|
| `Attachments:StorageProvider` | `Local` (default) | Saves files under `wwwroot/attachments/` |
| `Attachments:StorageProvider` | `AzureBlob` | Uses `AzureBlobAttachmentStorageService` |
| `Attachments:MaxFileSizeBytes` | `20971520` | 20 MB upload limit |
| `Attachments:AllowedExtensions` | `.pdf .docx .pptx .zip` | Blocked types rejected at upload |
