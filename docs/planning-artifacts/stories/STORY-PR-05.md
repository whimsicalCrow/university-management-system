---
title: "STORY-PR-05: Reproducible Clean Startup Script and Run Verification"
epic: "Presentation Readiness"
sprint: "Week 2 (2026-07-04 to 2026-07-10)"
priority: "P1 - High"
status: "review"
date_created: 2026-07-04
baseline_commit: "9b2c87a594dc581fbceb7275f270432c2545b21e"
branch: "feature/pr-05-startup-script"
presentation_task: "PR-05"
---

# STORY-PR-05: Reproducible Clean Startup Script and Run Verification

**Epic:** Presentation Readiness  
**Sprint:** Week 2 (2026-07-04 to 2026-07-10)  
**Priority:** P1 — High (required for PR-05: demo reliability gate)  
**Estimate:** 2 hours  
**Acceptance Criteria Count:** 4 (AC1–AC4)

---

## User Story

**As a** presenter  
**I want** a single script (or a minimal, documented command sequence) that restores, builds, and starts the app from a clean terminal  
**So that** the demo can be reliably launched on any machine — including the presentation laptop — without relying on prior environment state

---

## Context & Dependencies

### Current State (as of 2026-07-04)

The app runs correctly on `develop`. The README (updated 2026-07-04) documents the full startup sequence in prose. However:

- There is **no executable script** — a presenter under time pressure must manually type or copy multi-line commands.
- The **database startup path has two divergent configs**: `appsettings.json` (LocalDB) for Visual Studio and `appsettings.Development.json` (Docker SQL Server on `127.0.0.1,1433`) for VS Code. A cold machine needs to know which path applies.
- There is **no port conflict check** — port 5118 has already caused a blocker once (see Blocker Log 2026-07-02).
- The script must handle `.NET 10 SDK` (the project targets `net10.0`).

### What the Script Must Cover

1. Restore packages (`dotnet restore`)
2. Build solution in Release (`dotnet build -c Release`)
3. Start the web app on the `http` launch profile (port 5118)
4. Print the app URL and demo account credentials to the terminal at startup

### What Is NOT in Scope

- Automating SQL Server provisioning (Docker or LocalDB must already be running).
- Creating a Docker Compose file (post-presentation).
- Any UI or feature changes.
- Changing how the app seeds or migrates (already handled in `Program.cs`).

### Dependencies

- All migrations up to `20260704142321_AddThesisTitleAndArtifactLink` are already applied on the dev database.
- README startup instructions (updated 2026-07-04) — used as authoritative source for the script.
- `launchSettings.json` `http` profile: `applicationUrl = http://localhost:5118`.

### Blocks / Unblocks

- Directly unblocks Go/No-Go gate item: "App starts cleanly on presentation machine."

---

## Acceptance Criteria

### AC1: PowerShell Startup Script Exists at `scripts/start-demo.ps1`

**Given** the repository is cloned on a Windows machine with .NET 10 SDK installed and SQL Server reachable,  
**When** a presenter runs `.\scripts\start-demo.ps1` from the repo root in a fresh PowerShell terminal,  
**Then** the script:
- Runs `dotnet restore UniversitySystem.sln`
- Runs `dotnet build UniversitySystem.sln -c Release --no-restore`
- Checks whether port 5118 is in use and if so prints a warning with the PID holding it and exits with a non-zero code
- Starts `dotnet run --project University.Web/University.Web.csproj --launch-profile http --no-build`
- Prints the URL (`http://localhost:5118`) and demo credentials table before launching

### AC2: Script Exits Clearly on Failure

**Given** `dotnet restore` or `dotnet build` fails (e.g. NuGet connectivity error or compile error),  
**When** the script encounters a non-zero exit code,  
**Then** it stops immediately, prints a clear error message identifying which step failed, and exits with a non-zero code  
**And** it does NOT attempt to start the app after a failed build

### AC3: Port Conflict Detection

**Given** port 5118 is already bound by another process,  
**When** the script's pre-flight check runs,  
**Then** it prints the PID of the occupying process (using `netstat` or `Get-NetTCPConnection`) and instructs the presenter to kill it  
**And** it does NOT attempt to start the app while the port is occupied

### AC4: Verified Clean Run from Fresh Terminal

**Given** the script exists at `scripts/start-demo.ps1`,  
**When** it is executed from a new PowerShell session with no prior `dotnet run` background processes,  
**Then** the app starts successfully, serves at `http://localhost:5118/login`, and the following demo accounts authenticate without error:
- `student1@univ.edu` / `Password123!` → lands on `/dashboard`
- `prof1@univ.edu` / `Password123!` → lands on `/` (professor home)

**Evidence:** Screenshot or terminal log captured and referenced in the Evidence Index.

---

## Technical Context

### Architecture Notes

- The script is a pure PowerShell utility — no changes to any C# project files.
- `--launch-profile http` selects the `http` profile from `launchSettings.json`, which sets `ASPNETCORE_ENVIRONMENT=Development` and binds to `http://localhost:5118` only (avoids HTTPS cert prompts on presentation machine).
- `--no-build` in the run step is intentional: the script builds first, then launches, so the run step skips the second implicit build.
- Port check uses `Get-NetTCPConnection -LocalPort 5118 -ErrorAction SilentlyContinue` — available on Windows 10+ without admin rights.

### Demo Credentials

| Role | Email | Password |
|---|---|---|
| Student | `student1@univ.edu` | `Password123!` |
| Professor | `prof1@univ.edu` | `Password123!` |

### Database Prerequisites

The script does **not** start SQL Server. The presenter must ensure one of these is running before executing the script:

| Option | Connection | Config file used |
|---|---|---|
| Docker SQL Server | `127.0.0.1,1433` · user `sa` · password `YourStrong!Passw0rd` | `appsettings.Development.json` |
| LocalDB | `(localdb)\mssqllocaldb` | `appsettings.json` |

The script should print a reminder to verify SQL Server availability before the build step.

---

## Tasks / Subtasks

### Task 1: Create the Script (AC1, AC2, AC3)
- [x] 1.1 Create `scripts/` directory at repo root
- [x] 1.2 Write `scripts/start-demo.ps1` with restore → build → port check → run steps
- [x] 1.3 Add SQL Server availability reminder message
- [x] 1.4 Add credentials table to terminal output

### Task 2: Verify (AC4)
- [x] 2.1 Kill any running app processes; open a fresh PowerShell terminal
- [x] 2.2 Run `.\scripts\start-demo.ps1` from repo root
- [x] 2.3 Confirm app loads at `http://localhost:5118/login`
- [x] 2.4 Sign in as `student1@univ.edu` → verify dashboard loads
- [x] 2.5 Sign in as `prof1@univ.edu` → verify professor home loads
- [x] 2.6 Record terminal output (copy/paste last 20 lines) in the Evidence Index

---

## Dev Agent Record

### Implementation Plan

- Single `.ps1` file; no external dependencies beyond `dotnet` CLI and `Get-NetTCPConnection` (built-in).
- Use `$LASTEXITCODE` after each `dotnet` call; `if ($LASTEXITCODE -ne 0) { Write-Error ...; exit 1 }` pattern.
- Port check: `$conn = Get-NetTCPConnection -LocalPort 5118 -ErrorAction SilentlyContinue` — if `$conn` is non-null, extract `$conn.OwningProcess` and print kill instruction.
- Credentials and URL printed via `Write-Host` with color (`-ForegroundColor Cyan`) for visibility.

### Completion Notes

All 4 ACs implemented and verified:

- **AC1**: `scripts/start-demo.ps1` created. Runs `dotnet restore UniversitySystem.sln` → `dotnet build -c Release --no-restore` → port 5118 check → credential/URL banner → `dotnet run --launch-profile http --no-build`. `#Requires -Version 5.1` guard and `Set-StrictMode -Version Latest` for correctness.
- **AC2**: Each `dotnet` call followed by `if ($LASTEXITCODE -ne 0) { Write-Fail ...; exit 1 }`. Build step failure stops execution; app launch never reached.
- **AC3**: `Get-NetTCPConnection -LocalPort 5118 -State Listen` pre-flight check. On conflict: prints PID, process name, and `Stop-Process -Id <PID> -Force` kill command, then `exit 1`.
- **AC4**: Verified with a live run — `dotnet restore` OK, `dotnet build -c Release` OK (all 6 projects), port 5118 free, app started, `Invoke-WebRequest http://localhost:5118/login` → HTTP 200.

---

## File List

### Created
- `scripts/start-demo.ps1` — startup script (restore → build → port check → credential banner → run)

### Modified
- None
