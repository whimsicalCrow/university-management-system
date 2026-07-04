# PR-07 Performance Report — University Thesis Collaboration Portal

## Run Metadata

| Field | Value |
|---|---|
| Date | 2026-07-04 |
| Branch | `feature/pr-07-performance-tests` |
| Commit | `e6be101` (baseline) |
| Environment | Local — Windows, .NET 10, SQL Server 2022 (Docker, `127.0.0.1:1433`) |
| App URL | `http://localhost:5118` |
| Measurement tool | PowerShell `Invoke-WebRequest` + `Stopwatch` (k6 fallback — see note) |
| Samples per endpoint | 20 sequential requests |
| Session handling | `WebRequestSession` cookie jar — Identity cookie carried across page requests |

> **Note on k6:** k6 was not available on this machine (not in PATH; winget install did not complete).
> The PowerShell fallback documented in `STORY-PR-07.md` was used instead.
> The k6 script (`k6-performance-tests/golden-flow.js`) is committed and ready to run when k6 is available.
> Sequential single-user measurements are conservative compared to concurrent k6 VUs; results are expected
> to be representative of single-user demo performance (the relevant scenario for presentation day).

---

## Results

| Endpoint | Role | p50 (ms) | p95 (ms) | avg (ms) | max (ms) | Errors |
|---|---|---:|---:|---:|---:|---:|
| `POST /api/auth/login` | Student | 80 | 88 | 80 | 88 | 0/20 |
| `POST /api/auth/login` | Professor | 81 | 89 | 81 | 89 | 0/20 |
| `GET /login` | Unauthenticated | 13 | 25 | 13 | 25 | 0/20 |
| `GET /dashboard` | Student | 24 | 151 | 31 | 151 | 0/20 |
| `GET /updates` | Student | 27 | 175 | 35 | 175 | 0/20 |
| `GET /thesis-topics` | Student | 24 | 71 | 29 | 71 | 0/20 |
| `GET /` | Professor | 16 | 61 | 19 | 61 | 0/20 |
| `GET /thesis-topics` | Professor | 23 | 89 | 28 | 89 | 0/20 |

---

## Threshold Evaluation

| Threshold | Target | Measured | Result |
|---|---|---|---|
| Login p95 response time | < 500 ms | 89 ms (worst case) | ✅ PASS |
| Page-load p95 response time | < 1 500 ms | 175 ms (worst case — `/updates`) | ✅ PASS |
| HTTP error rate | < 1 % | 0 % (0 errors across 160 requests) | ✅ PASS |
| HTTP 401 / 403 responses | 0 | 0 | ✅ PASS |

**All 4 thresholds PASS.**

---

## Observations

### Login performance (p95 = 88–89 ms)
Login is dominated by bcrypt password verification (~80 ms is typical for bcrypt cost factor 11–12, the ASP.NET Core Identity default). This is by design — bcrypt is intentionally slow to resist brute force. For demo purposes, sub-100 ms login is excellent.

### Page-load performance (p95 = 25–175 ms)
Initial Blazor Server pre-render times are well within target:
- **`/updates` is the slowest page** (p95 = 175 ms) because it executes multiple EF queries: `ResolveStudentIdAsync` + `LoadTimelineAsync` (joins `ThesisUpdates` + `Feedback`). This is acceptable.
- **`/login` is fastest** (p95 = 25 ms) — no DB queries, static form.
- All other pages are under 100 ms p95.

### Zero errors
All 160 requests across 8 endpoints completed with HTTP 200. No auth failures, no server errors.

### What these numbers do NOT capture
- **Blazor SignalR circuit establishment** — the WebSocket handshake and circuit boot add ~500–1 500 ms client-side (browser) on first load. This is not measured here and is the largest component of perceived page-load time during the demo.
- **Concurrent user load** — measurements are sequential. A k6 run with 10 VUs would stress the SignalR hub and SQL connection pool. No degradation is expected at 10 VUs on a local machine, but this is unverified.

---

## Residual Risks

| Risk | Severity | Mitigation |
|---|---|---|
| Blazor cold-start WebSocket latency (~500–1500 ms) not measured | Low | Pre-warm the circuit before the demo by navigating each page once |
| `/updates` is slowest page (p95 175 ms) — may spike on first cold run | Low | Pre-warm by opening the page before the presentation starts |
| Measurements are sequential, not concurrent | Low | Demo is single-user; sequential baseline is representative |
| k6 not run — thresholds validated via PowerShell fallback only | Low | k6 script is committed; run `winget install k6` on the presentation machine if concurrent validation is needed |
