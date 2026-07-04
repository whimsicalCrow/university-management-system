#Requires -Version 5.1
<#
.SYNOPSIS
    Starts the University Thesis Collaboration Portal for demo / presentation use.

.DESCRIPTION
    Runs the full startup sequence from a clean terminal:
      1. Verifies .NET SDK availability
      2. Restores NuGet packages
      3. Builds the solution in Release
      4. Checks port 5118 for conflicts
      5. Launches the app (http://localhost:5118)

    SQL Server (LocalDB or Docker) must already be running before calling this script.
    See README.md > Quick Start for database setup instructions.

.EXAMPLE
    # From repo root:
    .\scripts\start-demo.ps1
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ─── Helpers ────────────────────────────────────────────────────────────────

function Write-Step   { param($msg) Write-Host "`n▶  $msg" -ForegroundColor Cyan }
function Write-Ok     { param($msg) Write-Host "   ✓  $msg" -ForegroundColor Green }
function Write-Fail   { param($msg) Write-Host "`n✗  $msg" -ForegroundColor Red }
function Write-Warn   { param($msg) Write-Host "   ⚠  $msg" -ForegroundColor Yellow }

# ─── Resolve repo root (script lives in scripts/) ───────────────────────────

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

# ─── Banner ─────────────────────────────────────────────────────────────────

Write-Host ""
Write-Host "╔══════════════════════════════════════════════════════╗" -ForegroundColor Magenta
Write-Host "║   University Thesis Collaboration Portal — Demo      ║" -ForegroundColor Magenta
Write-Host "╚══════════════════════════════════════════════════════╝" -ForegroundColor Magenta

# ─── Step 0: DB reminder ────────────────────────────────────────────────────

Write-Step "Pre-flight: database check"
Write-Warn "Ensure SQL Server is reachable BEFORE this script starts the app."
Write-Host ""
Write-Host "   Option A — Docker SQL Server (appsettings.Development.json):" -ForegroundColor DarkCyan
Write-Host "     Host : 127.0.0.1,1433   DB : UniversityDB   User : sa" -ForegroundColor DarkCyan
Write-Host "     Start: docker run -e ACCEPT_EULA=Y -e SA_PASSWORD=YourStrong!Passw0rd ``" -ForegroundColor DarkCyan
Write-Host "                        -p 1433:1433 --name sql -d mcr.microsoft.com/mssql/server:2022-latest" -ForegroundColor DarkCyan
Write-Host ""
Write-Host "   Option B — LocalDB (appsettings.json, Visual Studio default):" -ForegroundColor DarkCyan
Write-Host "     Server=(localdb)\mssqllocaldb   DB: UniversityCollaborationDb" -ForegroundColor DarkCyan

# ─── Step 1: .NET SDK ────────────────────────────────────────────────────────

Write-Step "Checking .NET SDK"
$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) {
    Write-Fail ".NET SDK not found. Install from https://dotnet.microsoft.com/download"
    exit 1
}
$sdkVersion = & dotnet --version 2>&1
Write-Ok ".NET SDK $sdkVersion"

# ─── Step 2: Restore ─────────────────────────────────────────────────────────

Write-Step "Restoring packages  (dotnet restore UniversitySystem.sln)"
& dotnet restore UniversitySystem.sln
if ($LASTEXITCODE -ne 0) {
    Write-Fail "dotnet restore failed (exit $LASTEXITCODE). Check NuGet connectivity and retry."
    exit 1
}
Write-Ok "Packages restored"

# ─── Step 3: Build ───────────────────────────────────────────────────────────

Write-Step "Building solution in Release  (dotnet build -c Release --no-restore)"
& dotnet build UniversitySystem.sln -c Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Fail "dotnet build failed (exit $LASTEXITCODE). Fix compile errors before running the demo."
    exit 1
}
Write-Ok "Build succeeded"

# ─── Step 4: Port conflict check ─────────────────────────────────────────────

Write-Step "Checking port 5118"
$occupied = Get-NetTCPConnection -LocalPort 5118 -State Listen -ErrorAction SilentlyContinue
if ($occupied) {
    $pid_ = ($occupied | Select-Object -First 1).OwningProcess
    $proc = Get-Process -Id $pid_ -ErrorAction SilentlyContinue
    $procName = if ($proc) { $proc.ProcessName } else { "unknown" }
    Write-Fail "Port 5118 is already in use by PID $pid_ ($procName)."
    Write-Host "   Run the following to free it, then re-run this script:" -ForegroundColor Yellow
    Write-Host "     Stop-Process -Id $pid_ -Force" -ForegroundColor Yellow
    exit 1
}
Write-Ok "Port 5118 is free"

# ─── Step 5: Print demo credentials ──────────────────────────────────────────

Write-Host ""
Write-Host "┌─────────────────────────────────────────────────────────┐" -ForegroundColor Cyan
Write-Host "│  App URL  :  http://localhost:5118                      │" -ForegroundColor Cyan
Write-Host "│  Login at :  http://localhost:5118/login                │" -ForegroundColor Cyan
Write-Host "│                                                         │" -ForegroundColor Cyan
Write-Host "│  Demo accounts (password: Password123!)                 │" -ForegroundColor Cyan
Write-Host "│                                                         │" -ForegroundColor Cyan
Write-Host "│  Student   →  student1@univ.edu  → /dashboard          │" -ForegroundColor Cyan
Write-Host "│  Professor →  prof1@univ.edu     → / (professor home)  │" -ForegroundColor Cyan
Write-Host "└─────────────────────────────────────────────────────────┘" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Starting app... (Ctrl+C to stop)" -ForegroundColor Green
Write-Host ""

# ─── Step 6: Run ─────────────────────────────────────────────────────────────

& dotnet run --project University.Web/University.Web.csproj --launch-profile http --no-build
if ($LASTEXITCODE -ne 0) {
    Write-Fail "App exited with code $LASTEXITCODE."
    exit $LASTEXITCODE
}
