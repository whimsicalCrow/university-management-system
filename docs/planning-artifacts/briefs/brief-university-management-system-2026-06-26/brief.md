---
title: "Project Brief: Thesis Collaboration Portal (UI Prototype → MVP)"
status: draft
created: "2026-06-26"
updated: "2026-06-26"
---

# Project Brief: Thesis Collaboration Portal

## Executive Summary

**Thesis Collaboration Portal** is a web-based platform designed to connect university students, supervisors, and department administrators throughout the full lifecycle of a thesis project. It solves the coordination gap inherent in academic thesis supervision — scattered emails, missed deadlines, and opaque progress — by providing a structured workspace for milestone tracking, progress updates, meeting scheduling, and supervisor assignment.

**Current State (as of 2026-06-26):** A **UI-only Blazor prototype** with 8 fully-rendered Razor pages, mock data, and client-side state. The prototype demonstrates the intended user flows and visual design but contains no backend, no database, and no persistent data. **Next Phase:** wire end-to-end flows (domain models, application services, database, API) to move from prototype to functional MVP.

**Target:** Greek-language university departments (initially ECE-type programmes), with a proof-of-concept for the University of Peloponnese Department of Electrical & Computer Engineering.

---

## Problem Statement

Academic thesis supervision in Greek university departments currently relies on ad-hoc email chains, shared drives, and informal meetings. This creates:

- **No single source of truth** for thesis status, feedback history, or upcoming deadlines.
- **Delayed feedback loops:** supervisors receive draft chapters via email and may take weeks to respond with no tracking.
- **Scheduling friction:** arranging progress meetings requires multiple email exchanges with no conflict detection.
- **Access control gaps:** there is no mechanism that ensures only assigned participants can view sensitive thesis content.
- **Administrative overhead:** assigning supervisors and co-supervisors requires manual coordination between department staff and professors.

These problems worsen as thesis cohorts grow. Existing general-purpose tools (Google Classroom, email, Teams) lack domain-specific structure, so adoption and compliance remain low.

---

## Proposed Solution

The portal introduces role-aware workspaces tied to individual thesis projects. Key pillars:

- **Thesis Update Timeline** — structured Markdown updates with autosave, draft/review status workflow, and a supervisor read-only view with filters.
- **Meeting Scheduling** — students propose slots; supervisors accept, decline, or counter-propose; overlap enforcement built in.
- **Supervisor Assignment UI** — administrators search students and professors and bind them to a thesis; access claims are enforced at the API layer.
- **Action Item Tracking** — per-meeting tasks with live status updates.
- **Thesis Topic Management** — professors propose, draft, submit for approval, approve/reject, and archive topics; students browse approved topics, express interest, and suggest new topics.
- **Accessibility** — WCAG 2.1 AA support via an integrated self-hosted accessibility widget (AccessYes by CookieYes).

**Differentiator:** purpose-built for the Greek academic thesis lifecycle; default Greek UI copy; role model matches university organisational structure.

---

## Current Implementation Status

### ✅ What Exists (UI Prototype)

**8 fully-rendered Blazor pages:**
1. **Login.razor** — Dual-role selector (Student / Professor); routes to role-specific dashboard. Uses `UserSessionService` (in-memory session, no persistence).
2. **Home.razor** — Role-based landing page; conditionally redirects students to dashboard.
3. **StudentDashboard.razor** — Thesis status overview layout; placeholder data ("Thesis details will appear here once data integration is complete").
4. **ThesisTopics.razor** — Form-driven UI for proposing/editing thesis topics; local component state only; mock data support.
5. **Meetings.razor** — Calendar grid UI showing meeting slots; hardcoded mock data for multiple calendars; role-based slot display.
6. **Assignments.razor** — Supervisor assignment workflow with dual search/select UI; 5 mock students, 4 mock supervisors; no persistence.
7. **ThesisUpdates.razor** — Markdown editor with draft autosave indicator, formatting toolbar, file attachment input field; no backend.
8. **Layout Components** — MainLayout (auth guard), NavMenu (role-conditional navigation), LoginLayout (bare shell).

**Navigation & Styling:**
- Bootstrap 5 local (no CDN); Bootstrap Icons; Markdig (Markdown preview library).
- Role-based navigation menu (students see student links, professors see professor links).
- WCAG 2.1 AA widget placeholder (not yet integrated).

**Authentication:**
- `UserSessionService` (scoped): in-memory per-Blazor-circuit; enum roles (Student/Professor); no multi-device support.
- Login workflow: role selection → username (optional, defaults to "Student User" / "Professor User") → dashboard.

### ❌ What Does NOT Exist (Deferred)

| Component | Status | Reason |
|-----------|--------|--------|
| **Domain Models** | Not implemented | No `.cs` files in `University.Domain/` |
| **Application Services** | Not implemented | No `.cs` files in `University.Application/`; CQRS planned but not coded |
| **Database** | Not implemented | No EF Core DbContext, migrations, or SQL schema defined |
| **API / Endpoints** | Not implemented | No controller or minimal API endpoints |
| **Data Persistence** | Not implemented | No Entity Framework integration; mock data lives in component state only |
| **Real Authentication** | Not implemented | ASP.NET Core Identity not wired; session lives in memory |
| **Backend Validation** | Not implemented | No FluentValidation validators or MediatR pipeline behaviors |
| **Tests** | Not implemented | Test projects exist (xUnit, EF InMemory) but contain no test files |
| **Notification Service** | Not implemented | Notification domain pattern defined in brief but no service wired |
| **File Storage** | Not implemented | US-021 (attachment pipeline) deferred to Phase 2 |
| **Kubernetes Deployment** | Scaffolded but not deployed | Manifests exist in `k8s/` but not activated |

---

## Target Users

### Primary User Segment: Students

- Undergraduate and graduate students actively completing a thesis project.
- **Goals:** track their own progress, submit structured updates, schedule meetings, see supervisor feedback in one place.
- **Current behaviours:** email chains with supervisors, shared Google Docs for drafts, WhatsApp for meeting coordination.
- **Pain points:** uncertainty about thesis status, no visibility into the supervisor's review queue, missed deadlines due to poor communication.

### Secondary User Segment: Professors / Supervisors

- Academic staff supervising 2–10 students simultaneously.
- **Goals:** review updates efficiently, confirm or reschedule meetings, approve/reject thesis topics, give structured and trackable feedback.
- **Pain points:** high email volume from students, unclear which student is waiting for review, overlapping meeting commitments.

### Tertiary User Segment: Department Administrators

- Staff responsible for assigning students to supervisors and managing the topic pool.
- **Goals:** assign and re-assign participants quickly, oversee the active thesis cohort.
- **Pain points:** manual spreadsheet tracking of who supervises whom, no enforcement of access restrictions.

---

## Goals & Success Metrics

### Prototype Phase Goals (Current ✅)

- ✅ Demonstrate intended user flows via UI mockup.
- ✅ Validate role-based navigation and page layout.
- ✅ Communicate design to stakeholders (professor panel, project team).
- ✅ Establish baseline accessibility (WCAG 2.1 AA structure in place).

### MVP Phase Goals (Next 🔄)

- Deliver a **functionally complete POC** with end-to-end data flows (add thesis topic → save to DB → retrieve for student browsing → professor approves → student notified).
- Reduce supervisor–student communication cycles by ≥ 30% in a **pilot cohort of ≥ 5 students**.
- Enable **zero email-only** thesis coordination for the pilot.

### User Success Metrics (MVP-phase target)

- Students submit at least one structured update per week without manual reminder.
- Supervisors review and respond to updates within 5 business days (visible in status timestamps).
- Meeting scheduling is confirmed within 2 round-trips.

### Key Performance Indicators (MVP-phase target)

- **Weekly active users per thesis project:** ≥ 3 interactions/week per active thesis.
- **Average time-to-feedback:** update submitted → supervisor status change; target < 5 business days.
- **Meeting no-show rate:** target < 10% of confirmed meetings.
- **Topic approval cycle time:** draft → approved; target < 7 days.

---

## MVP Scope (Next Phase: Backend Integration)

### Core Features (Must Have for MVP)

#### 1. Domain & Persistence
- **Domain Models:** `Student`, `Professor`, `Thesis`, `ThesisTopic`, `ThesisUpdate`, `Meeting`, `ActionItem` aggregates with invariants.
- **Database Schema:** SQL Server / Azure SQL with EF Core migrations.
- **DbContext:** Maps entities to tables; seeded with test data (3–5 sample theses, students, professors for demo).

#### 2. Authentication & Authorization
- **ASP.NET Core Identity** replacing `UserSessionService` (with feature toggle for smooth migration).
- **Role-based policies:** Student role restricts to own thesis; Professor role sees assigned students; Admin role has full CRUD on assignments.
- **Access enforcement:** HTTP 403 if unauthorized; API endpoints validate claims server-side.

#### 3. Thesis Topic Management (End-to-End)
- **Professor CRUD:** Create, edit, submit for approval, archive topic.
- **Student Browsing:** View approved topics; express interest (click "I'm interested" → persist in DB); suggest new topic (form submission → insert as Pending Approval).
- **Admin Approval:** Review pending topics → approve/reject (visible to professor; rejected reasons logged).
- **API Endpoints:** `POST /api/topics`, `GET /api/topics/{id}`, `PATCH /api/topics/{id}/approve`, etc.

#### 4. Thesis Updates Timeline
- **Student writes:** Markdown text in editor → `POST /api/thesis/{id}/updates` → saved with `status: Draft`.
- **Student submits for review:** `PATCH /api/thesis/{id}/updates/{updateId}/submit` → status: Pending Review.
- **Professor reviews:** `GET /api/thesis/{id}/updates` (filtered read-only view) → feedback form.
- **Professor provides feedback:** `PATCH /api/thesis/{id}/updates/{updateId}/review` with feedback text → status: Accepted / Needs Revision.
- **Autosave (client-side):** Every 30 seconds while editing; no server-side autosave yet.

#### 5. Meeting Scheduling
- **Student proposes:** Select date/time from calendar → `POST /api/meetings/propose` → status: Proposed.
- **Professor accepts/declines/counter-proposes:** `PATCH /api/meetings/{id}/respond` → status: Confirmed / Declined / CounterProposed.
- **Overlap enforcement:** Professor's calendar backend validates no double-booking; client-side warning also shown.
- **Action items:** After meeting confirmed, student/professor create action items → `POST /api/meetings/{id}/action-items` → tracked with status (Open, Closed).

#### 6. Supervisor Assignment (Admin Workflow)
- **Admin UI:** Search students (by name/ID) and professors (by name/title); link via assignment form.
- **Persist:** `POST /api/assignments` → create `StudentProfessorAssignment` record.
- **Access Control:** Assignment claim (e.g., "supervisor_for_thesis_123") attached to professor's identity; used in authorization policies.
- **Confirmation:** On creation, both student and professor notified (notification queued; not yet delivered in MVP).

#### 7. Accessibility (WCAG 2.1 AA)
- **Widget Integration:** AccessYes self-hosted widget activated on all pages via Alt+A shortcut.
- **Semantic HTML:** Proper heading hierarchy, ARIA labels, color contrast.
- **Keyboard Navigation:** Tab/Shift+Tab through all interactive elements; Enter/Space to activate.
- **Testing:** Axe DevTools audit on key flows; critical issues fixed before pilot.

### Out of Scope for MVP

- File/attachment upload pipeline (US-021 — deferred to Phase 2).
- Email/notification delivery (notification domain modelled; delivery service queued but not sent).
- Calendar integration / ICS export (Phase 2).
- Mobile-native app.
- Load testing and OWASP security hardening (post-MVP audit).
- Multi-department / multi-university tenancy (Phase 2).
- AI-assisted feedback suggestions (Phase 2).

### MVP Success Criteria

A professor can:
1. Open the portal and log in with role assignment (no dual-role selector; linked to department AD or test account).
2. Propose a new thesis topic via form → submit for approval.
3. View a student's update timeline, read an existing update, and leave feedback → update status changes.
4. See a meeting proposal from a student, accept or decline it, and create an action item.
5. Perform all of the above **without leaving the browser, without API errors, and with data persisting across page refresh/logout-login cycles**.

A student can:
1. Log in to see their assigned thesis.
2. Browse approved topics and express interest.
3. Write a thesis update, save it as draft, then submit for professor review.
4. Propose a meeting slot; see professor's response.

---

## Roadmap: Prototype → MVP → Phase 2+

### Prototype Phase (Current ✅)
- **What:** 8 Blazor pages with mock data; UI-only, no backend.
- **When:** Complete.
- **Output:** Portfolio item, stakeholder walkthrough, design validation.

### MVP Phase (Next 🔄, est. 2–3 weeks with 1 developer)
- **What:** Wire domain models → DB → API endpoints; integrate real authentication; end-to-end flows.
- **When:** TBD (request from project sponsor).
- **Output:** Functional POC deployable to Azure Container Apps; pilot-ready for ≥5 students.

### Phase 2 (Post-MVP, est. 3–4 weeks)
- **What:** File storage pipeline (US-021); notification delivery; calendar export; student self-registration.
- **When:** After pilot feedback.
- **Output:** Enhanced MVP with attachment support and email confirmations.

### Long-term Vision (1–2 years)
- Multi-department / multi-university tenancy.
- AI-assisted feedback suggestions.
- Analytics dashboard for department heads.
- Integration with institutional identity providers (Shibboleth / LDAP).

---

## Technical Considerations

### Current Architecture

**Monorepo Structure:**
- `University.Domain/` — (empty; ready for domain models)
- `University.Application/` — (empty; ready for CQRS handlers)
- `University.Infrastructure/` — (empty; ready for EF Core DbContext, repositories)
- `University.Web/` — Blazor Server frontend with 8 pages; `UserSessionService` for session.
- `tests/` — xUnit projects (empty; ready for unit/integration tests).

**Technology Stack (In Place):**
- **Frontend:** Blazor Server (.NET 8), Bootstrap 5 (local), Markdig, SignalR (scaffolded).
- **Backend:** ASP.NET Core 8, MediatR, FluentValidation (NuGet packages installed but not used).
- **Database:** SQL Server / Azure SQL (not yet connected).
- **Infrastructure:** Docker (Dockerfile present), Kubernetes (manifests scaffolded), Azure Container Apps (target).

### Design Patterns (To Be Implemented in MVP)

- **Clean Architecture:** Domain layer owns business rules; Application layer orchestrates via MediatR handlers; Infrastructure provides persistence.
- **CQRS:** Commands (Create/Update/Delete thesis topics, meetings, etc.) and Queries (fetch timelines, browse topics) separated.
- **Repository Pattern:** Per-aggregate repositories (e.g., `ThesisTopicRepository`, `MeetingRepository`) in Infrastructure layer.
- **Specification Pattern:** Reusable filters (e.g., "approved topics visible to students").

### Integration Points (To Be Wired)

- **SignalR:** Live update broadcasts (e.g., "Professor has approved your topic") — scaffolded but not active.
- **Notification Service:** Email/SMS stubs defined; SendGrid or SMTP to be wired.
- **Azure Identity:** Optional LDAP / Shibboleth integration for multi-department tenancy (Phase 2).

---

## Constraints & Assumptions

### Constraints

- **Budget:** Academic / personal project — no paid cloud services at MVP/POC stage (use free/trial tiers).
- **Timeline:** MVP phase estimated at 2–3 weeks with one developer.
- **Resources:** Solo developer (you); no dedicated QA or DevOps.
- **Technical:** Baseline database must be seeded with test data; no production data imported yet.

### Key Assumptions

- Department structure maps cleanly to roles: one student → one primary supervisor, optionally one co-supervisor.
- Greek is the default display language; accessibility widget auto-detects page language.
- A single tenant (one university department) is sufficient for MVP.
- ASP.NET Core Identity can replace `UserSessionService` with a feature toggle.
- Test database schema can be auto-generated from EF Core migrations.

### Clarifications Needed (For Kickoff)

- ❓ **Real authentication:** Will the pilot use AD/LDAP or test accounts managed in the app?
- ❓ **Email notifications:** Should confirmation emails be sent on meeting acceptance, or is the portal notification enough for MVP?
- ❓ **Data retention:** How long should thesis content be retained after a course concludes?
- ❓ **Concurrent users:** Expected max concurrent users for performance baseline?
- ❓ **Demo date:** When does the professor panel want to see the MVP demo?

---

## Risks & Mitigations

### Key Risks

| Risk | Impact | Mitigation |
|------|--------|-----------|
| **Scope creep:** Adding features without wired backend creates a fragile demo. | High | Freeze feature list; only refine existing 8 pages; mark future features as post-MVP. |
| **Data model divergence:** Domain models don't match UI assumptions. | Medium | Design domain models first from UI flows; write integration tests early to catch misalignment. |
| **Accessibility audit failures:** Late discovery of WCAG blockers. | Medium | Run Axe DevTools audit on MVP pages weekly; fix critical issues immediately. |
| **Authentication bottleneck:** ASP.NET Core Identity setup delays MVP. | Medium | Start with a simple in-memory test identity provider; integrate real AD later. |
| **Performance under load:** SignalR circuit reconnect issues with many users. | Low (MVP) | Use load testing (k6 in `k6-performance-test/`) post-MVP; acceptable for pilot. |

### Open Questions

- Will the department require integration with existing identity provider (LDAP / Shibboleth)?
- Is there a formal data-retention or GDPR policy for student thesis content?
- Should Meeting Scheduling support recurring meeting series or only one-off slots for MVP?
- What is the expected maximum number of concurrent thesis projects per department?

---

## Next Steps (Transition from Prototype to MVP)

### 1. Validate Scope (This Week)
- [ ] Review this reworked brief with project sponsor.
- [ ] Confirm MVP features and demo date.
- [ ] Answer the "Clarifications Needed" questions above.

### 2. Design Domain Models (Week 1)
- [ ] Sketch `Student`, `Professor`, `Thesis`, `ThesisTopic`, `ThesisUpdate`, `Meeting`, `ActionItem` aggregates with invariants (e.g., "a professor can't approve their own topic").
- [ ] Define repository interfaces (e.g., `IThesisTopicRepository.GetApprovedTopics()`).
- [ ] Generate EF Core migrations from domain models.

### 3. Implement CQRS Layer (Weeks 1–2)
- [ ] Create MediatR commands and queries (e.g., `ApproveThesisTopic`, `GetApprovedTopics`).
- [ ] Implement FluentValidation validators.
- [ ] Wire MediatR pipeline behaviors (logging, exception handling).

### 4. Wire End-to-End Flows (Week 2–3)
- [ ] Replace mock data in each Blazor page with API calls.
- [ ] Create HTTP endpoints (Controllers or minimal API) for each flow.
- [ ] Integrate ASP.NET Core Identity; replace `UserSessionService`.

### 5. Test & Polish (Week 3)
- [ ] Write integration tests (EF InMemory).
- [ ] Accessibility audit and fixes.
- [ ] Live demo prep.

---

## References

- **Existing Brief:** `docs/brief.md` (superseded; kept for archive).
- **Developer Guide:** `docs/developer-guide.md` — CQRS conventions, naming standards.
- **User Stories:** `docs/user stories/` — 17 stories across UI, Backend, DevOps, Infrastructure.
- **UI Prototype:** `University.Web/Components/Pages/` — 8 Blazor pages.
- **Architecture:** `.bmad-core/` — Project guidance and workflows.
- **k6 Performance Tests:** `k6-performance-test/` — Load testing setup (not yet active).

---

**Status:** Draft. Ready for sponsor review and scope confirmation.
