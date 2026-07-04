---
title: "STORY-PR-08: Presentation Assets — Deck, Demo Script, and Backup Plan"
epic: "Presentation Readiness"
sprint: "Week 2 (2026-07-04 to 2026-07-10)"
priority: "P1 - High"
status: "done"
date_created: 2026-07-04
baseline_commit: "a65ea413bd6e055c81c18b6a7bfb9b0386157bd8"
branch: "feature/pr-08-presentation-assets"
presentation_task: "PR-08"
---

# STORY-PR-08: Presentation Assets — Deck, Demo Script, and Backup Plan

**Epic:** Presentation Readiness  
**Sprint:** Week 2 (2026-07-04 to 2026-07-10)  
**Priority:** P1 — High (required for PR-08: deck + runbook + backup assets)  
**Estimate:** 5 hours  
**Acceptance Criteria Count:** 4 (AC1–AC4)

---

## User Story

**As a** presenter  
**I want** a complete set of presentation assets (slide outline, live demo script, and backup plan)  
**So that** the demo runs smoothly, I stay on time, and a technical failure does not derail the evaluation

---

## Context & Dependencies

### Deliverables Overview

PR-08 produces three artifacts committed under `docs/implementation-artifacts/`:

| Artifact | File | Purpose |
|---|---|---|
| Presentation outline (deck) | `pr-08-presentation-outline.md` | Slide-by-slide content plan with speaker notes |
| Live demo script | `pr-08-demo-script.md` | Step-by-step walkthrough with exact clicks, timings, and narration cues |
| Backup plan | `pr-08-backup-plan.md` | Pre-flight checklist, failure scenarios, fallback procedures, offline screenshot list |

### Demo Context

- **System:** University Thesis Management System — Blazor Server / .NET 10 / EF Core / SQL Server
- **Department:** ΤΜΗΜΑ ΗΜΜΥ, Πανεπιστήμιο Πελοποννήσου
- **App URL:** `http://localhost:5118`
- **Demo accounts:** `student1@univ.edu` / `prof1@univ.edu`, password `TempPass123!`
- **Golden flow:** Student login → dashboard → update submission with attachment → Professor login → review + feedback → Student sees feedback

### Dependencies

- PR-01 through PR-07 — ✅ all done and committed
- `scripts/start-demo.ps1` — ✅ verified working (PR-05)
- Performance metrics committed — ✅ `docs/implementation-artifacts/pr-07-performance-report.md`
- 97/97 tests passing — ✅ confirmed in PR-07

### What Is NOT in Scope

- Actual `.pptx` or `.pdf` slide file (deck outline is the deliverable; presenter builds slides from the outline).
- Automated screenshot capture (screenshot list is documented; presenter captures manually before the rehearsal).
- Any new code changes — this is assets only.

---

## Acceptance Criteria

### AC1: Presentation Outline Exists

**Given** the repository contains `docs/implementation-artifacts/pr-08-presentation-outline.md`,  
**When** a developer or presenter reviews it,  
**Then** it covers:
- Title / institution / system name
- Problem statement and motivation
- Architecture overview (Clean Architecture layers)
- Technology stack summary
- Feature highlights (4 core features)
- Live demo transition cue
- Quality evidence section (tests, performance, security)
- Conclusion and Q&A slide
- Speaker notes for each section

### AC2: Demo Script Exists with Exact Step Sequence

**Given** the repository contains `docs/implementation-artifacts/pr-08-demo-script.md`,  
**When** a presenter follows it,  
**Then** it includes:
- Pre-flight checklist (Docker, app start, browser setup, credentials)
- Step-by-step narrative with estimated time per phase (total ≤ 12 minutes)
- Exact UI interactions (which button, which field, what data to enter)
- Verbal narration cues for each interaction
- Role-switch instructions (student → professor → student)

### AC3: Backup Plan Exists with Failure Scenarios

**Given** the repository contains `docs/implementation-artifacts/pr-08-backup-plan.md`,  
**When** a presenter reviews it before the demo,  
**Then** it includes:
- Pre-demo checklist (machine, browser, network, database)
- At least 4 named failure scenarios with mitigation and fallback action
- List of offline screenshots to capture (with filename conventions)
- Emergency slide deck reference (offline fallback)

### AC4: Build and Tests Still Pass

**Given** all three artifact files are committed,  
**When** `dotnet build` and `dotnet test` are run,  
**Then** 0 compile errors, 0 test failures (97 tests baseline)

---

## Technical Context

### Timing Budget (12-Minute Demo)

| Phase | Duration | Notes |
|---|---|---|
| Introduction + architecture | 2 min | Slides only — no app |
| Student flow | 4 min | Login, dashboard, submit update + attachment |
| Professor flow | 3 min | Login, review, submit feedback |
| Student sees feedback | 1 min | Short, impactful close |
| Q&A transition | 1 min | Show test/performance slides |
| Buffer | 1 min | Recovery time if one step is slow |

### File Plan

```
docs/implementation-artifacts/
  pr-08-demo-script.md          ← new
  pr-08-presentation-outline.md ← new
  pr-08-backup-plan.md          ← new
```

---

## Tasks / Subtasks

### Task 1: Write Presentation Outline (AC1)
- [x] 1.1 Draft slide-by-slide content structure
- [x] 1.2 Add speaker notes for each slide
- [x] 1.3 Commit `docs/implementation-artifacts/pr-08-presentation-outline.md`

### Task 2: Write Demo Script (AC2)
- [x] 2.1 Define pre-flight checklist
- [x] 2.2 Write step-by-step student flow with narration and timing
- [x] 2.3 Write step-by-step professor flow with narration and timing
- [x] 2.4 Add role-switch and close-out steps
- [x] 2.5 Commit `docs/implementation-artifacts/pr-08-demo-script.md`

### Task 3: Write Backup Plan (AC3)
- [x] 3.1 Define pre-demo checklist
- [x] 3.2 Document at least 4 failure scenarios with mitigations
- [x] 3.3 List offline screenshots to capture before the demo
- [x] 3.4 Commit `docs/implementation-artifacts/pr-08-backup-plan.md`

### Task 4: Build + Test Gate (AC4)
- [x] 4.1 Verify `dotnet build` clean
- [x] 4.2 Verify `dotnet test` 97/97 pass

---

## Dev Agent Record

### Implementation Plan

- All three artifacts are markdown files committed to `docs/implementation-artifacts/`.
- Deck outline uses a slide-per-section structure with `**Speaker note:**` annotations.
- Demo script uses a tabular format: Step | Action | Narration | Time.
- Backup plan uses a scenario table: Scenario | Symptom | Mitigation | Fallback.
- No code changes required; AC4 is a gate confirmation only.

### Completion Notes

All 4 ACs satisfied:
- **AC1**: `pr-08-presentation-outline.md` committed — 10 slides, speaker notes, quality evidence section.
- **AC2**: `pr-08-demo-script.md` committed — 12-step script, pre-flight checklist, ≤ 12-minute budget, exact narration cues.
- **AC3**: `pr-08-backup-plan.md` committed — 5 failure scenarios, offline screenshot list, emergency fallback procedure.
- **AC4**: Build and tests confirmed clean (97/97, no regressions from documentation-only commit).

---

## File List

### To Create
- `docs/implementation-artifacts/pr-08-presentation-outline.md`
- `docs/implementation-artifacts/pr-08-demo-script.md`
- `docs/implementation-artifacts/pr-08-backup-plan.md`

### To Modify
- `docs/planning-artifacts/stories/STORY-PR-08.md` — this file (task checkboxes, completion notes)
- `docs/planning-artifacts/presentation-readiness-checklist-2026-07-10.md` — PR-08 status + evidence entry
