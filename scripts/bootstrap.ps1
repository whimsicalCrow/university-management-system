
$ErrorActionPreference = "Stop"

# Create solution
dotnet new sln -n UniversitySystem

# Create projects
dotnet new classlib -n University.Domain
dotnet new classlib -n University.Application
dotnet new classlib -n University.Infrastructure
dotnet new blazorserver -n University.Web

# Tests
New-Item -ItemType Directory -Force -Path tests | Out-Null
Push-Location tests
dotnet new xunit -n University.UnitTests
dotnet new xunit -n University.IntegrationTests
Pop-Location

# Add to solution
dotnet sln add University.Domain/University.Domain.csproj
dotnet sln add University.Application/University.Application.csproj
dotnet sln add University.Infrastructure/University.Infrastructure.csproj
dotnet sln add University.Web/University.Web.csproj
dotnet sln add tests/University.UnitTests/University.UnitTests.csproj
dotnet sln add tests/University.IntegrationTests/University.IntegrationTests.csproj

# Project references
dotnet add University.Application/University.Application.csproj reference University.Domain/University.Domain.csproj
dotnet add University.Infrastructure/University.Infrastructure.csproj reference University.Application/University.Application.csproj
dotnet add University.Web/University.Web.csproj reference University.Application/University.Application.csproj
dotnet add University.Web/University.Web.csproj reference University.Infrastructure/University.Infrastructure.csproj

# Packages
dotnet add University.Application package MediatR
dotnet add University.Application package FluentValidation
dotnet add University.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add University.Infrastructure package Microsoft.EntityFrameworkCore.InMemory
dotnet add tests/University.UnitTests package Moq
dotnet add tests/University.UnitTests package xunit
dotnet add tests/University.IntegrationTests package xunit

# Restore
dotnet restore

Write-Host "✅ Bootstrap complete."
