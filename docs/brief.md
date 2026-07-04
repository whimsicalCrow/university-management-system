# Project Brief: Thesis Collaboration Portal

## Executive Summary

**Thesis Collaboration Portal** is a web-based platform that connects university students, supervisors, and department administrators throughout the full lifecycle of a thesis project. It solves the coordination gap inherent in academic thesis supervision — scattered emails, missed deadlines, and opaque progress — by providing a structured workspace for milestone tracking, progress updates, meeting scheduling, and supervisor assignment. The primary target is Greek-language university departments (initially ECE-type programmes), with a proof-of-concept for the University of Peloponnese Department of Electrical & Computer Engineering.

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
- **Meeting Scheduling** — students propose slots; supervisors accept, decline, or counter-propose; overlap enforcement built in; calendar/ICS export available post-MVP.
- **Supervisor Assignment UI** — administrators search students and professors and bind them to a thesis; access claims are enforced at the API layer.
- **Action Item Tracking** — per-meeting tasks with live status updates.
- **Thesis Topic Management** — professors propose, draft, submit for approval, approve/reject, and archive topics; students can browse approved topics, express interest, and suggest new topics (which go straight to Pending Approval); Draft and Archived entries are hidden from the student view.
- **Accessibility** — WCAG 2.1 AA support via an integrated self-hosted accessibility widget (AccessYes by CookieYes).

**Differentiator:** purpose-built for the Greek academic thesis lifecycle rather than adapted from a general project-management tool; default Greek UI copy; role model matches university organisational structure.

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

### Business Objectives

- Deliver a functional POC acceptable for presentation to a professor panel by the end of the current academic sprint.
- Reduce supervisor–student communication cycles (measured by coordination round-trips per milestone) by ≥ 30% in a pilot cohort.
- Enable zero email-only thesis coordination for a pilot of ≥ 5 students.

### User Success Metrics

- Students submit at least one structured update per week without a manual reminder.
- Supervisors review and respond to updates within 5 business days (visible in status timestamps).
- Meeting scheduling is confirmed within 2 round-trips.

### Key Performance Indicators (KPIs)

- **Weekly active users per thesis project:** target ≥ 3 interactions/week per active thesis.
- **Average time-to-feedback:** update submitted → supervisor status change; target < 5 business days.
- **Meeting no-show rate:** target < 10% of confirmed meetings.
- **Topic approval cycle time:** draft → approved; target < 7 days.

---

## MVP Scope

### Core Features (Must Have)

- **Role-Pick Login Page:** single-click role selector (Student / Professor) as the app entry point; scoped in-memory session (`UserSessionService`) drives all role-gating for the POC without requiring ASP.NET Core Identity.
- **Role-Based Navigation:** navbar and page content adapt per role. Students: Home, Student Dashboard, Thesis Updates, Meetings, Thesis Topics (browse/interest/suggest). Professors: Home, Thesis Updates, Meetings, Supervisor Workflow, Thesis Topics (full CRUD + approve/reject/archive).
- **Student Dashboard:** thesis status overview, upcoming milestones, recent supervisor feedback, quick-nav links to updates/meetings.
- **Thesis Update Timeline:** Markdown editor with autosave, status workflow (Draft → Pending Review → Accepted / Needs Revision), supervisor filtered read-only view.
- **Meeting Scheduling:** propose/accept/decline/counter-propose slots, professor overlap guard, action item CRUD with status badges.
- **Supervisor Assignment UI:** administrator assigns students ↔ professors to thesis projects with confirmation dialogs.
- **Thesis Topic Management:** professor propose, draft, submit for approval, approve/reject, archive; student browse (Approved + Pending visible, Draft + Archived hidden), express interest (per-session tracking), suggest new topic (submitted as Pending Approval).
- **Accessibility Widget:** integrated, self-hosted WCAG 2.1 AA toolbar with keyboard shortcut (Alt+A).

### Out of Scope for MVP

- File/attachment upload pipeline (US-021 — deferred to Phase 2)
- Production CI/CD pipeline (US-003 — scaffolded, not activated)
- Production Kubernetes deployment (`k8s/` manifests exist but not deployed)
- Email/notification delivery (notification domain modelled; delivery service not wired)
- Load testing and OWASP security hardening passes (US-040, US-041 — post-MVP)
- Calendar integration / ICS export (US-031 — post-MVP)
- Mobile-native application

### MVP Success Criteria

A professor can open the portal, pick the Professor role on the login screen, see all topics they have proposed, approve a pending topic, view a student's update timeline, accept a meeting request, and create an action item — all without leaving the browser and without any backend errors — in a live demo session. A student can pick the Student role, browse approved thesis topics, express interest, and submit a new topic suggestion.

---

## Post-MVP Vision

### Phase 2 Features

- Attachment storage pipeline (Azure Blob Storage, scoped per thesis project).
- Email and in-app notification delivery for all status-change events.
- Calendar export (ICS) and optional video conference link on confirmed meetings.
- Student self-registration and thesis-topic application flow.

### Long-term Vision

Within 1–2 years, the platform should support:

- Multi-department / multi-university tenancy with isolated data boundaries.
- AI-assisted feedback suggestions for supervisors reviewing thesis updates.
- Analytics dashboard for department heads: cohort progress heat-maps, bottleneck detection.
- Integration with institutional identity providers (Shibboleth / LDAP / eduGAIN).

### Expansion Opportunities

- Other supervised programme types: internship supervision, capstone projects, postgraduate dissertations.
- Export compliance with European Academic Calendar and ECTS standards.
- Open API for third-party integrations (plagiarism checkers, reference managers).

---

## Technical Considerations

### Platform Requirements

- **Target Platforms:** Web (desktop-first, responsive to tablet viewports).
- **Browser / OS Support:** Chrome 120+, Firefox 120+, Edge 120+, Safari 17+; Windows and macOS.
- **Performance Requirements:** page load < 2 s on a standard university LAN; Blazor Server SignalR circuit reconnect within 5 s.

### Technology Preferences

- **Frontend:** Blazor Server (.NET 8), Bootstrap 5 (local), Bootstrap Icons, Markdig (Markdown rendering).
- **Backend:** ASP.NET Core 8, CQRS via MediatR, FluentValidation, SignalR (scaffolded for live updates).
- **Database:** Entity Framework Core with SQL Server / Azure SQL (baseline `.bacpac` provided).
- **Hosting / Infrastructure:** Docker (Dockerfile present), Kubernetes manifests scaffolded (`k8s/`); Azure Container Apps target for production.

### Architecture Considerations

- **Repository Structure:** monorepo — `University.Domain`, `University.Application`, `University.Infrastructure`, `University.Web`, test projects.
- **Service Architecture:** Clean Architecture — domain aggregates own business rules; application layer orchestrates via handlers; infrastructure provides persistence and external services.
- **Integration Requirements:** notification service interface defined; email delivery (SendGrid or SMTP) to be wired post-MVP.
- **Security / Compliance:** ASP.NET Core Identity with role + claim-based policies; HTTPS enforced; antiforgery middleware enabled; WCAG 2.1 AA; GDPR — no user data collected by accessibility widget.

---

## Constraints & Assumptions

### Constraints

- **Budget:** Academic / personal project — no paid cloud services at MVP/POC stage.
- **Timeline:** POC demo milestone is imminent within the current academic sprint.
- **Resources:** Solo developer; single Blazor Server project; no dedicated QA.
- **Technical:** No production database available for POC demo; in-memory stubs replace EF Core / MediatR in the current build.

### Key Assumptions

- Department structure maps cleanly to roles: one student → one primary supervisor, optionally one co-supervisor.
- Greek is the default display language; the accessibility widget auto-detects the page language (`lang` attribute).
- The professor presenting the POC has authority to approve new thesis topics in the demo scenario.
- Real email delivery is not required for the POC phase.
- A single tenant (one university department) is sufficient for MVP; multi-tenancy is a post-MVP concern.

---

## Risks & Open Questions

### Key Risks

- **In-memory stub divergence:** POC stubs may drift from the intended backend contract, making real backend integration harder later. *Mitigation:* keep stub types aligned with domain model shapes; write interface contracts even for stubs.
- **Scope creep before demo:** adding UI features without a wired backend creates a fragile demo. *Mitigation:* freeze feature scope, mark remaining work as post-demo.
- **UI-only role gating:** the current POC enforces roles in the Blazor UI layer via `UserSessionService` only — there is no server-side auth policy preventing a determined user from navigating directly to a restricted URL. *Mitigation:* acceptable for demo; must be replaced with ASP.NET Core Identity + claim policies before any real-data pilot.
- **Bootstrap Icons CDN integrity failure:** the current `App.razor` integrity hash for the Bootstrap Icons CDN is invalid, causing the stylesheet to be blocked in strict browsers. *Mitigation:* update the hash or switch to the local `lib/` copy.

### Open Questions

- Will the department require integration with the university's existing identity provider (LDAP / Shibboleth)?
- Is there a formal data-retention or GDPR policy for student thesis content hosted on the portal?
- Should Meeting Scheduling support recurring meeting series or only one-off slots for MVP?
- What is the expected maximum number of concurrent thesis projects per department for performance baseline?

### Areas Needing Further Research

- Greek university thesis regulation requirements (formal submission deadlines, examination committee composition rules, ECTS mapping).
- Accessibility audit results once authentication and full data flow are restored to the Blazor build.
- Competitive landscape: existing Greek university portal tools (e.g., eClass, department-specific systems) and their thesis-management gaps.

---

## Next Steps

1. ~~Freeze POC scope and confirm demo date with professor.~~ *(scope frozen; login + role views complete)*
2. Fix the Bootstrap Icons CDN integrity hash in `University.Web/Components/App.razor`.
3. Restore real authentication (ASP.NET Core Identity + role seeds) behind a feature toggle for the pilot build, replacing the current `UserSessionService` POC session.
4. Wire at least one end-to-end flow (Thesis Update Timeline) to the database to demonstrate real persistence in the demo.
5. Hand off this brief to the PM agent to generate the PRD.

---

## Appendices

### C. References

- `docs/developer-guide.md` — CQRS conventions, pipeline behaviours, naming standards.
- `docs/user stories/` — 17 user stories across UI, Backend, DevOps, Infrastructure, Integration, Security, Documentation, Architecture.
- `University.Web/Components/Pages/` — POC Blazor pages (StudentDashboard, ThesisUpdates, Meetings, Assignments, ThesisTopics, Home, Login).
- `University.Web/Components/Layout/` — MainLayout (auth guard), NavMenu (role-conditional), LoginLayout (bare shell for login page).
- `University.Web/Services/UserSessionService.cs` — scoped in-memory session service (UserName, Role, IsAuthenticated, OnChange event).
- `.vscode/launch.json` — configured `coreclr` launch profiles for http (`localhost:5118`) and https (`localhost:7173`) with auto-open browser.
- `k8s/` — Kubernetes deployment and service manifests (scaffolded).
- `tests/` — Unit tests (MeetingTests, ValidationBehaviorTests, LoggingBehaviorTests) and integration test stubs.

---

*This Project Brief provides the full context for the Thesis Collaboration Portal. The next step is PRD Generation Mode: review this brief thoroughly to produce the PRD section by section, asking for any necessary clarification or suggesting improvements.*
