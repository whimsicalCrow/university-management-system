# Presentation Readiness Checklist (Deadline: 2026-07-10)

## Goal
Deliver a reliable, end-to-end thesis workflow demo with supporting quality evidence (security, performance, and documentation) by 2026-07-10.

## Scope Lock (Do Not Expand)
- Golden flow only:
  1. Student login -> dashboard -> thesis update submission with attachment
  2. Professor login -> review update -> submit feedback
  3. Optional meeting view (non-critical)
- Out of scope until after presentation: new major features, deep refactors, non-demo UI redesign.

## Status Legend
- not-started
- in-progress
- blocked
- done

## Master Task Board

| ID | Workstream | Task | Owner | Estimate (h) | Due | Status | Blocker | Evidence Required |
|---|---|---|---|---:|---|---|---|---|
| PR-01 | Scope | Freeze presentation scope and non-goals | Hermes | 1 | 2026-07-02 | done | None | Scope note committed in docs |
| PR-02 | Core Feature | Complete US-021 attachment persistence end-to-end | Hermes | 6 | 2026-07-03 | not-started | None | Upload + retrieve demo + tests |
| PR-03 | Core Feature | Complete US-022 feedback loop visibility | Hermes | 5 | 2026-07-04 | not-started | None | Professor feedback visible to student |
| PR-04 | UX Hardening | Fix critical UI states for demo path (errors, empty states, role nav) | Hermes | 4 | 2026-07-05 | in-progress | None | Manual test checklist signed |
| PR-05 | Reliability | Create clean startup/run script and verify from fresh terminal | Hermes | 2 | 2026-07-06 | in-progress | None | Successful clean run commands |
| PR-06 | Security | Run OWASP-focused checks and patch high-severity findings | Hermes | 4 | 2026-07-07 | not-started | None | Security findings table |
| PR-07 | Performance | Execute load tests on critical paths and capture metrics | Hermes | 4 | 2026-07-08 | not-started | None | Response time and error-rate report |
| PR-08 | Presentation | Build final deck, demo script, backup script, and fallback screenshots | Hermes | 5 | 2026-07-09 | not-started | None | Deck + runbook + backup assets |
| PR-09 | Dress Rehearsal | Run full demo twice under timed conditions | Hermes | 2 | 2026-07-09 | not-started | None | Timing log and issue list |
| PR-10 | Go/No-Go | Final readiness check and lock release candidate | Hermes | 1 | 2026-07-10 | not-started | None | Go decision record |

## Daily Execution Plan

### 2026-07-02 (Today)
- [x] PR-01 scope lock completed.
- [x] Define acceptance criteria for golden flow.
- [x] Confirm no new scope additions accepted.

### 2026-07-03
- [ ] PR-02 attachment pipeline complete.
- [ ] Verify uploaded files appear in File Library and timeline.
- [ ] Add or update tests for upload/retrieval/failure.

### 2026-07-04
- [ ] PR-03 feedback loop complete.
- [ ] Verify professor feedback persistence and student visibility.
- [ ] Add minimal notification/audit indicator if missing.

### 2026-07-05
- [ ] PR-04 UX hardening complete.
- [ ] Fix role navigation edge cases.
- [ ] Remove temporary debug artifacts.

### 2026-07-06
- [ ] PR-05 reproducible startup workflow documented and tested.
- [ ] Confirm build -> run -> demo flow works from clean terminal.

### 2026-07-07
- [ ] PR-06 security pass complete.
- [ ] Patch high-priority vulnerabilities.
- [ ] Document accepted residual risks.

### 2026-07-08
- [ ] PR-07 performance pass complete.
- [ ] Capture and store metrics artifacts.

### 2026-07-09
- [ ] PR-08 deck and demo assets finalized.
- [ ] PR-09 two full rehearsals completed.
- [ ] Freeze code except critical fixes.

### 2026-07-10
- [ ] PR-10 go/no-go checklist completed.
- [ ] Launch with release-candidate build only.

## Go/No-Go Gate (Presentation Day)
All must be true:
- [ ] App starts cleanly on presentation machine.
- [ ] Demo accounts authenticate successfully.
- [ ] Attachment upload and retrieval work in live demo.
- [ ] Professor feedback path works end-to-end.
- [ ] No blocking runtime exceptions on demo flow.
- [ ] Slide deck and fallback screenshots available offline.

## Blocker Log
| Date | Task ID | Blocker | Impact | Owner | Mitigation | Status |
|---|---|---|---|---|---|---|
| 2026-07-02 | PR-05 | Port 5118 conflict (`address already in use`) | Could block local demo startup | Hermes | Killed stale process and relaunched app on expected port | closed |

## Evidence Index
- 2026-07-02: Scope lock and presentation checklist created in this file.
- 2026-07-02: Login UI updated and verified at `/login` (new subtitle and neutral panel).
- 2026-07-02: Build succeeded (`University.Web.csproj`, latest task run).
- Security report path: fill on 2026-07-07.
- Performance report path: fill on 2026-07-08.
- Deck and runbook path: fill on 2026-07-09.

## Change Control
Any proposed task added after 2026-07-05 requires:
1. Explicit impact statement to demo reliability.
2. Time trade-off against an existing task.
3. Approval to replace, not expand, scope.
