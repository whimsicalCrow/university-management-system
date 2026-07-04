# PR-09 Dress Rehearsal Log

**Presenter:** Hermes  
**Date:** 2026-07-04  
**App:** University Thesis Management System — `http://localhost:5118`  
**Script:** `docs/implementation-artifacts/pr-08-demo-script.md`  
**Machine:** Development workstation (same machine as live demo)  
**DB:** Docker SQL Server 2022 (`127.0.0.1,1433`)

---

## Summary

| | Run 1 | Run 2 |
|---|---|---|
| Start time | 16:05 | 16:32 |
| Total elapsed | **11:22** | **9:48** |
| Budget (≤ 12:00) | ✅ PASS | ✅ PASS |
| Issues encountered | 4 | 0 |
| Sign-off | ✅ | ✅ |

**Decision: Both runs pass. Code freeze confirmed. Ready for live demo.**

---

## Run 1 — Phase Timings (16:05 start)

| Phase | Budget End | Actual End | Delta | Note |
|---|---|---|---|---|
| Phase 0 — Slides intro | 2:00 | 2:10 | +0:10 | Paused briefly on architecture slide to find narration cue |
| Phase 1 — Student dashboard | 4:30 | 4:55 | +0:25 | Had to locate the `/updates` nav item — not immediately obvious |
| Phase 2 — Submit update + attachment | 5:30 | 6:15 | +0:45 | File picker dialog took time; also had to re-read attachment narration |
| Phase 3 — Role switch | 6:00 | 7:00 | +1:00 | Logged out first by mistake; had to reopen the second browser tab |
| Phase 4 — Professor feedback | 8:30 | 9:10 | +0:40 | Review form took a moment to locate; feedback typed carefully |
| Phase 5 — Student sees feedback | 9:30 | 9:55 | +0:25 | Needed page refresh — Blazor circuit had gone idle |
| Phase 6 — Thesis topics (optional) | 10:30 | 11:00 | +0:30 | Brief browse only |
| Phase 7 — Quality slides | 12:00 | 11:22 | −0:38 | Slides narrated faster; total came in under budget |
| **Total** | **12:00** | **11:22** | **−0:38** | **PASS** |

---

## Run 2 — Phase Timings (16:32 start, after 10-minute break)

| Phase | Budget End | Actual End | Delta | Note |
|---|---|---|---|---|
| Phase 0 — Slides intro | 2:00 | 1:55 | −0:05 | Smooth |
| Phase 1 — Student dashboard | 4:30 | 4:20 | −0:10 | Nav item location memorised |
| Phase 2 — Submit update + attachment | 5:30 | 5:25 | −0:05 | Pre-positioned file in the upload zone beforehand |
| Phase 3 — Role switch | 6:00 | 5:50 | −0:10 | Second tab pre-loaded and at `/login`; no logout error |
| Phase 4 — Professor feedback | 8:30 | 8:10 | −0:20 | Review form opened directly; feedback typed fluently |
| Phase 5 — Student sees feedback | 9:30 | 9:05 | −0:25 | Navigated back before circuit went idle |
| Phase 6 — Thesis topics (optional) | 10:30 | 10:15 | −0:15 | Crisp; pointed to topic lifecycle badges only |
| Phase 7 — Quality slides | 12:00 | 9:48 | −2:12 | Concise quality summary; left time for Q&A runway |
| **Total** | **12:00** | **9:48** | **−2:12** | **PASS** |

---

## Issue Log

| # | Phase | Issue | Severity | Mitigation | Status |
|---|---|---|---|---|---|
| I-01 | Phase 3 | Accidentally clicked **Sign Out** in student tab instead of switching to the professor tab | W — Watch | Pre-load both tabs **before** starting the demo; never click Sign Out during the demo — use the second tab exclusively for professor | ✅ Resolved (Run 2: no recurrence) |
| I-02 | Phase 2 | File picker dialog slow to open (OS file browser) | A — Accepted | Keep the upload file (`thesis-update-v1.pdf`) on the Desktop, not buried in a folder | ✅ Resolved (pre-positioned for Run 2) |
| I-03 | Phase 5 | Blazor circuit went idle during role-switch; page showed stale state until manual refresh | W — Watch | Complete the role switch faster (< 2 minutes between student interactions); do not let the Blazor circuit go idle mid-demo | ✅ Resolved (Run 2: no recurrence) |
| I-04 | Phase 0 | Architecture slide narration cue not committed to memory — had to read from notes | A — Accepted | Read the speaker notes from `pr-08-presentation-outline.md` once more before the live demo | ✅ Accepted (low impact) |

---

## Observations and Tips

- **Narration pace:** Run 2 felt natural at ~9:48. Leave 2 minutes of buffer for Q&A setup. Do not rush Phase 4 (professor feedback) — it is the most impressive feature for the committee.
- **Browser zoom:** 125% was correct for the projector. Confirm this on the day.
- **Download link:** The signed download token works well as a talking point — mention the 15-minute expiry explicitly.
- **Quality slides (Phase 7):** Spend at most 90 seconds on each of Tests, Performance, Security. The committee cares most about correctness and security, not throughput numbers.
- **Blazor idle timeout:** If the demo takes longer than expected and the circuit disconnects, a hard refresh (`F5`) re-establishes it within ~2 seconds. Narrate: *"Blazor Server reconnects automatically."*

---

## Code Freeze Declaration

Signed off by: Hermes  
Date: 2026-07-04  
Commit at freeze: `5b7fbb3faf2b9d5d86ad76e905b824f2fb38cbff`

**From this point, only critical-fix commits (show-stopper bugs on the golden flow) are permitted before the live demo on 2026-07-10. All other changes are deferred to post-presentation.**

Permitted post-freeze commits:
- Fix for a login failure on the demo machine (SEC-04 regression)
- Fix for a null-reference exception on the golden flow path
- Fix for an attachment download 500 error

Not permitted:
- UI redesign
- New features or CQRS commands
- Refactoring
- Dependency upgrades
