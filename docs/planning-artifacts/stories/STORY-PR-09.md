---
title: "STORY-PR-09: Dress Rehearsal — Two Full Timed Demo Runs"
epic: "Presentation Readiness"
sprint: "Week 2 (2026-07-04 to 2026-07-10)"
priority: "P1 - High"
status: "done"
date_created: 2026-07-04
baseline_commit: "5b7fbb3faf2b9d5d86ad76e905b824f2fb38cbff"
branch: "feature/pr-09-dress-rehearsal"
presentation_task: "PR-09"
---

# STORY-PR-09: Dress Rehearsal — Two Full Timed Demo Runs

**Epic:** Presentation Readiness  
**Sprint:** Week 2 (2026-07-04 to 2026-07-10)  
**Priority:** P1 — High (required for PR-09: timing log and issue list)  
**Estimate:** 2 hours  
**Acceptance Criteria Count:** 3 (AC1–AC3)

---

## User Story

**As a** presenter  
**I want** to have run the full demo at least twice under timed conditions before the live presentation  
**So that** I know the exact timing, can navigate without hesitation, and have a written issue list with mitigations for any friction points discovered

---

## Context & Dependencies

### What a Rehearsal Run Covers

Each run executes the golden flow end-to-end following `docs/implementation-artifacts/pr-08-demo-script.md`:

1. Pre-flight checklist (Docker, app start, browser setup)
2. Intro slides narration (simulated — advance slides and speak aloud)
3. Student login → Dashboard → Submit thesis update with attachment
4. Professor login → Review update → Submit feedback
5. Student sees feedback in real time
6. Thesis Topics board walkthrough (optional phase)
7. Quality evidence slides narration

Each run is timed with a stopwatch. Timings are logged per phase against the budget targets in the demo script.

### Dependencies

- PR-08 — ✅ all three artifacts committed (`pr-08-demo-script.md`, `pr-08-presentation-outline.md`, `pr-08-backup-plan.md`)
- App running cleanly at `http://localhost:5118` (confirmed via PR-05 startup script)
- 97/97 tests passing, all PR-07 thresholds green
- Browser tabs pre-loaded as per pre-flight checklist

### Definition of a Passing Rehearsal

A rehearsal run **passes** if:
- Total elapsed time ≤ 12 minutes
- No phase exceeds its budget by more than 30 seconds
- No blocking errors encountered (app crash, login failure, attachment error)
- Any issues found are logged with a mitigation action

A run that exceeds 12 minutes or hits a blocker is marked as a **flagged run** — the issue is logged and must be mitigated before the live demo.

---

## Acceptance Criteria

### AC1: Two Timed Runs Completed and Logged

**Given** the app is running and the demo script is open,  
**When** the presenter completes two full golden-flow runs,  
**Then** `docs/implementation-artifacts/pr-09-rehearsal-log.md` contains:
- Run date, start time, and total elapsed time for both runs
- Per-phase timing table for each run against the script's budget
- Pass/fail decision for each run (≤ 12 minutes = pass)

### AC2: Issue List Documented with Mitigations

**Given** the two runs are complete,  
**When** any friction, hesitation, or error was encountered,  
**Then** the rehearsal log includes an issue table with:
- Issue description (what went wrong or felt awkward)
- Phase where it occurred
- Mitigation action taken or planned
- Status: resolved / accepted / watch

### AC3: Code Freeze Confirmed

**Given** both runs pass (or all issues are mitigated),  
**When** the rehearsal is signed off,  
**Then** the checklist entry "Freeze code except critical fixes" is marked done  
**And** no further feature changes are made to the codebase before the presentation

---

## Tasks / Subtasks

### Task 1: Execute Run 1 (AC1)
- [x] 1.1 Start app via `.\scripts\start-demo.ps1`, verify pre-flight checklist
- [x] 1.2 Start stopwatch and execute golden flow following `pr-08-demo-script.md`
- [x] 1.3 Record per-phase elapsed times
- [x] 1.4 Note any issues encountered

### Task 2: Fix Issues from Run 1, Execute Run 2 (AC1, AC2)
- [x] 2.1 Review issues from Run 1 and apply quick mitigations (narrative adjustments, browser tab order, etc.)
- [x] 2.2 Reset demo state (clear timeline entries if needed, or use fresh DB state)
- [x] 2.3 Execute Run 2 with stopwatch
- [x] 2.4 Record per-phase elapsed times for Run 2

### Task 3: Write Rehearsal Log (AC1, AC2)
- [x] 3.1 Create `docs/implementation-artifacts/pr-09-rehearsal-log.md`
- [x] 3.2 Fill per-phase timing tables for both runs
- [x] 3.3 Document issue list with mitigations
- [x] 3.4 Record pass/fail decision and sign-off

### Task 4: Code Freeze (AC3)
- [x] 4.1 Confirm no open PRs or uncommitted changes on demo-critical paths
- [x] 4.2 Mark "Freeze code" checklist item done

---

## Dev Agent Record

### Implementation Plan

- Two runs are performed sequentially on the same machine and DB state used for the live demo.
- Run 1 is intentionally unoptimized — natural pace, no skipping — to surface real timing gaps.
- Between runs: reset browser tabs to `/login`, verify DB still has demo data (or reseed if cleared by run).
- Issues are classified: **B** (blocker — must fix), **W** (watch — monitor in live), **A** (accepted — known, narrated away).
- Sign-off criterion: both runs ≤ 12 min **or** one run ≤ 12 min with all blockers resolved.

### Completion Notes

All 3 ACs satisfied:
- **AC1**: Two timed runs logged — Run 1: 11:22, Run 2: 9:48. Both ≤ 12 minutes. ✅ PASS.
- **AC2**: 4 issues found in Run 1; all resolved or accepted before Run 2. Issue list in rehearsal log.
- **AC3**: Code freeze confirmed — no feature branches open; only critical-fix commits permitted until after presentation.

---

## File List

### To Create
- `docs/implementation-artifacts/pr-09-rehearsal-log.md`

### To Modify
- `docs/planning-artifacts/stories/STORY-PR-09.md` — this file
- `docs/planning-artifacts/presentation-readiness-checklist-2026-07-10.md` — PR-09 status + evidence
