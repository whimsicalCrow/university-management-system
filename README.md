# University Management System

A clean-architecture, modular .NET 8 Blazor Server application scaffold.
Includes CQRS (MediatR), FluentValidation, EF Core, unit/integration test projects, Docker, GitHub Actions, and Kubernetes manifests.

## Quick start

```bash
# 1) Ensure .NET 8 SDK and (optionally) Docker are installed
dotnet --info

# 2) Generate solution + add projects + references + restore packages
./scripts/bootstrap.sh
# or on Windows PowerShell
./scripts/bootstrap.ps1

# 3) Run the web app
dotnet run --project University.Web/University.Web.csproj
```

## Projects
- **University.Domain** – Entities and domain interfaces
- **University.Application** – CQRS handlers, DTOs, validators
- **University.Infrastructure** – EF Core DbContext, repositories
- **University.Web** – Blazor Server app
- **tests** – Unit & integration tests

## CI/CD
- GitHub Actions build + test + Docker image push (configure `DOCKER_USERNAME`/`DOCKER_PASSWORD` secrets).

## Kubernetes
Apply manifests in `k8s/` after pushing your image:
```bash
kubectl apply -f k8s/
```
