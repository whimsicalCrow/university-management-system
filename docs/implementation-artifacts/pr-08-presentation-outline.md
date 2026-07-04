# PR-08 Presentation Outline — University Thesis Management System

**Event:** Diploma Thesis Defense (Παρουσίαση Διπλωματικής Εργασίας)  
**Department:** ΤΜΗΜΑ ΗΜΜΥ — Πανεπιστήμιο Πελοποννήσου  
**Target duration:** 10–12 minutes + Q&A  
**Format:** Slides → Live Demo → Slides → Q&A

---

## Slide 1 — Title

**Title:** Σύστημα Διαχείρισης Πτυχιακών Εργασιών  
**Subtitle:** University Thesis Management System  
**Author:** Hermes  
**Supervisor:** [Επιβλέπων Καθηγητής]  
**Institution:** Τμήμα Ηλεκτρολόγων Μηχανικών και Μηχανικών Υπολογιστών — Πανεπιστήμιο Πελοποννήσου  
**Date:** Ιούλιος 2026

**Speaker note:** Pause 5 seconds, let the title slide settle. Greet the committee. *"Good morning. My diploma thesis presents a platform for managing the full lifecycle of a thesis assignment — from topic publication through student progress tracking to professor feedback."*

---

## Slide 2 — Problem Statement

**Title:** The Problem

**Content:**
- Thesis supervision is currently managed via email, shared drives, and informal messages
- No single view of thesis status, feedback history, or submitted artifacts
- Students and professors lose context across the semester
- No audit trail for feedback exchanges or file submissions

**Visual:** Simple before/after diagram — left: email threads and shared folders; right: unified portal

**Speaker note:** *"The goal is to replace ad-hoc communication with a structured, auditable workflow that both students and professors can access from a browser."*

---

## Slide 3 — Architecture

**Title:** Clean Architecture

**Content (four-layer onion diagram):**
- **Domain** — Entities: `ThesisAssignment`, `ThesisUpdate`, `ThesisArtifact`, `FeedbackEntry`; no framework dependencies
- **Application** — MediatR CQRS: commands (`SubmitUpdateCommand`, `SubmitReviewCommand`), queries, FluentValidation validators
- **Infrastructure** — EF Core 10 + SQL Server, repository implementations, local/Azure Blob storage service, no-op virus scan placeholder
- **Web (UI)** — ASP.NET Core 10 Blazor Server; interactive SSR; ASP.NET Core Identity; cookie auth

**Visual:** Concentric rings or left-to-right dependency arrows

**Speaker note:** *"Each layer depends only inward. The Domain has zero framework references. The Application layer defines interfaces; Infrastructure implements them. This makes the core logic fully unit-testable without a database."*

---

## Slide 4 — Technology Stack

**Title:** Technology Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10 / ASP.NET Core |
| UI | Blazor Server (Interactive SSR) |
| CQRS / Mediator | MediatR 11 |
| Validation | FluentValidation 11 |
| ORM | EF Core 10 + SQL Server |
| Identity / Auth | ASP.NET Core Identity |
| Markdown rendering | Markdig (HTML sanitized) |
| Tests | xUnit, bunit, EF Core InMemory |
| Containers | Docker (SQL Server 2022) |

**Speaker note:** *"The full stack is open-source and runs on a single machine for the demo. The storage backend is pluggable — local filesystem in dev, Azure Blob Storage in production via a single config flag."*

---

## Slide 5 — Core Features

**Title:** Four Core Features

1. **Thesis Topics Board** — Professors publish topics; students express interest; topic lifecycle (Draft → Open → Assigned → Archived)
2. **Progress Updates + Attachments** — Students submit timestamped updates with file attachments (PDF, DOCX, PPTX, ZIP ≤ 20 MB); signed download tokens with 15-minute expiry
3. **Professor Feedback Loop** — Professors review updates, set status (Approved / Needs Revision), write comments; feedback is persisted and visible to the student in real time
4. **Student Dashboard** — Aggregated view of thesis status, supervisor, pending feedback, and uploaded artifacts

**Visual:** Four-quadrant icon grid or feature screenshot thumbnails

**Speaker note:** *"These four features cover the full supervision lifecycle. I'll demonstrate all four in the live demo."*

---

## Slide 6 — Live Demo (Transition)

**Title:** Live Demo

**Content (large, centered text):**
> Student login → Dashboard → Submit update with attachment  
> Professor login → Review → Feedback  
> Student sees feedback in real time

**Speaker note:** *"Let me switch to the browser."*  
→ **Switch to browser. Follow `pr-08-demo-script.md`.**

---

## Slide 7 — Testing

**Title:** Quality — 97 Automated Tests

**Content:**
- **82 unit tests** — xUnit + bunit; domain logic, command handlers, validators, Blazor components; run without database
- **15 integration tests** — EF Core InMemory provider; repository round-trips, cascade deletes, migration integrity
- **0 failures** on develop branch
- Coverage: all CQRS command and query handlers, all repository methods, attachment upload/download pipeline, feedback persistence

**Visual:** Simple table or bar — Unit 82 / Integration 15 / Total 97 / Failed 0

**Speaker note:** *"The test suite runs in under 10 seconds locally. The CI pipeline gates every push on a clean build and all 97 tests passing."*

---

## Slide 8 — Performance

**Title:** Performance — Load Test Results

**Thresholds and Results:**

| Metric | Target | Actual (p95) | Result |
|---|---|---|---|
| Login response time | < 500 ms | 89 ms | ✅ PASS |
| Page-load response time | < 1 500 ms | 175 ms | ✅ PASS |
| Overall error rate | < 1 % | 0 % | ✅ PASS |
| Auth errors (401/403) | 0 | 0 | ✅ PASS |

**Setup:** 10 simulated virtual users, 40-second run, PowerShell Stopwatch fallback (k6 measured via HTTP timing)

**Speaker note:** *"All four thresholds green under 10 concurrent users on a local developer machine. The system responds well within the perceivable latency window for a live demo."*

---

## Slide 9 — Security

**Title:** Security — OWASP Audit

**Findings patched (4 HIGH):**

| ID | Category | Fix Applied |
|---|---|---|
| SEC-01 | Stored XSS | Markdig `.DisableHtml()` — raw HTML in markdown stripped |
| SEC-02 | Broken Access Control | Removed `?role=supervisor` query parameter; role from session only |
| SEC-03 | IDOR | `?authorId=N` gated on server-side supervisor check |
| SEC-04 | Missing Authorization | `@attribute [Authorize]` on all 5 protected pages + `LoginPath="/login"` |

**Accepted risk (4 MEDIUM):** Security headers, antiforgery on JSON endpoint, NoOp virus scan, dev password in `appsettings.Development.json`

**Speaker note:** *"An OWASP Top 10 code review was conducted before the demo. All four high-severity findings — XSS, two access control gaps, and missing authorization attributes — were patched. Four medium findings are accepted and documented with rationale."*

---

## Slide 10 — Conclusions and Future Work

**Title:** Conclusions

**Delivered:**
- End-to-end thesis workflow platform (Clean Architecture, Blazor Server, .NET 10)
- Attachment pipeline with pluggable storage and signed download tokens
- Real-time professor feedback loop persisted to SQL Server
- 97 automated tests, OWASP security pass, load-tested performance

**Future Work:**
- WebSocket / SignalR real-time notifications (push feedback to student without refresh)
- Azure Blob Storage production configuration and virus scan integration
- Email notifications on status transitions
- Export thesis history as PDF report

**Speaker note:** *"Thank you. I'm happy to take questions."* → Advance to Q&A placeholder slide.

---

## Appendix Slides (Q&A Backup)

### A1 — EF Core Migration List
List of all applied migrations with dates and descriptions.

### A2 — Attachment Storage Architecture
Sequence diagram: upload request → validation → storage service → DB record → signed token generation → download.

### A3 — Authentication Flow
Sequence diagram: browser → `/api/auth/login` JSON POST → Identity `SignInManager` → cookie set → protected page → `[Authorize]` → 200.

### A4 — Demo Accounts Reference
Full table from `docs/demo-users.md` — professors and students with roles and specializations.
