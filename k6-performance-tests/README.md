# Performance Tests — University Thesis Portal

Load tests for the golden demo flow using [k6](https://k6.io).

## Install k6

```powershell
# Windows — winget
winget install k6 --source winget --accept-source-agreements --accept-package-agreements

# Windows — Chocolatey
choco install k6

# macOS
brew install k6
```

## Prerequisites

1. App running at `http://localhost:5118`:
   ```powershell
   .\scripts\start-demo.ps1
   ```
2. SQL Server (Docker or LocalDB) reachable — the app auto-migrates and seeds accounts on startup.
3. Demo accounts available: `student1@univ.edu` / `prof1@univ.edu` / `TempPass123!`

## Run

```powershell
# From repo root
k6 run k6-performance-tests/golden-flow.js

# Override base URL (e.g. staging)
k6 run -e BASE_URL=http://localhost:5118 k6-performance-tests/golden-flow.js
```

## Thresholds

| Metric | Target |
|---|---|
| Login p95 | < 500 ms |
| Page-load p95 | < 1 500 ms |
| Auth errors | 0 |
| Overall error rate | < 1 % |
| HTTP failures (`http_req_failed`) | < 1 % |

## Scenarios

| Scenario | VUs | Flow |
|---|---|---|
| `studentFlow` | 5 | login → `/dashboard` → `/updates` → `/thesis-topics` |
| `professorFlow` | 5 | login → `/` → `/thesis-topics` → `/updates` |

Total: 10 VUs, 40 s (10 s ramp-up + 20 s hold + 10 s ramp-down).

## Fallback (k6 not available)

If k6 cannot be installed, run the PowerShell timing script instead:

```powershell
.\k6-performance-tests\measure-fallback.ps1
```

Results are written to `docs/implementation-artifacts/pr-07-performance-report.md`.
