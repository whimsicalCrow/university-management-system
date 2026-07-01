# Addendum: Implementation Status & Integration Roadmap

## Summary of Gap Analysis

The original brief (archived as `docs/brief.md`) describes a **fully-wired MVP with backend integration, CQRS handlers, and database persistence**. The current state is a **UI-only Blazor prototype** with mock data and client-side state only.

### What This Means

**Original Brief Claims:**
- ✓ Login flow working (end-to-end)
- ✓ Thesis updates wired to database with draft/review workflow
- ✓ Meeting scheduling with overlap detection and API persistence
- ✓ Supervisor assignment with access control policies
- ✓ Thesis topics with professor approval workflow
- ✓ Data persists across sessions

**Actual Current State:**
- ✓ Login UI with role picker (no persistence)
- ✗ Thesis updates captured in component state only (no save)
- ✗ Meeting scheduling uses hardcoded mock calendars
- ✗ Supervisor assignment uses mock data; no real assignment persistence
- ✗ Thesis topics use local form state; no database
- ✗ All data lost on page refresh

---

## Implications

### For Stakeholders (Professor Panel, Project Sponsor)

- **Good news:** The UI/UX is substantially complete and demonstrates the intended workflows.
- **Important caveat:** This is a **design mockup, not a functional system**. To move forward, backend integration must happen.
- **Timeline impact:** MVP (functional POC) now requires 2–3 weeks of backend work instead of being "already mostly done."

### For Developer (You)

**Advantages of Current Prototype:**
- UI structure is locked in; pages won't need redesigns once backend is wired.
- Bootstrap layout is responsive and accessible-ready (WCAG 2.1 AA structure in place).
- Role-based navigation is proven; identity integration is a clean swap.

**Work Ahead (MVP Phase):**
1. Design domain models from the UI flows (reverse-engineer from Razor pages).
2. Create EF Core DbContext and migrations.
3. Implement CQRS handlers (MediatR already installed).
4. Replace mock data in Blazor pages with API calls.
5. Replace `UserSessionService` with ASP.NET Core Identity (with feature toggle for safety).

---

## Recommended Path Forward

### Phase 0: Validation (Days 1–2)
- **Action:** Share the reworked brief with your sponsor/professor.
- **Ask:** Confirm the MVP scope, demo date, and authentication approach (test accounts vs. LDAP).

### Phase 1: Domain Design (Days 3–4)
- **Create:** `University.Domain/Entities/` with aggregates: `Student`, `Professor`, `Thesis`, `ThesisTopic`, `ThesisUpdate`, `Meeting`, `ActionItem`.
- **Apply:** Clean Architecture invariants (e.g., "a student can only propose topics to their assigned thesis").
- **Deliverable:** Domain models pass design review; no database yet.

### Phase 2: Persistence (Days 5–7)
- **Create:** `University.Infrastructure/Data/UniversityDbContext.cs`.
- **Apply:** EF Core migrations; seed test data (3–5 sample theses with students/professors).
- **Deliverable:** Database schema visible in SQL Server / Azure SQL; migrations runnable.

### Phase 3: CQRS Layer (Days 8–12)
- **Create:** MediatR commands and queries (e.g., `CreateThesisTopicCommand`, `GetApprovedTopicsQuery`).
- **Add:** FluentValidation validators for each command.
- **Wire:** MediatR pipeline (logging, exception handling).
- **Deliverable:** Commands/queries tested with in-memory database; no UI changes yet.

### Phase 4: API Endpoints & UI Integration (Days 13–18)
- **Create:** Controller or minimal API endpoints for each domain flow.
- **Replace:** Mock data in Blazor pages with HTTP calls to endpoints.
- **Wire:** ASP.NET Core Identity; replace `UserSessionService` (feature toggle).
- **Deliverable:** End-to-end flow (e.g., "student submits topic → saved to DB → professor sees it → professor approves → student sees approval status").

### Phase 5: Polish & Demo (Days 19–21)
- **Test:** Integration tests for happy paths.
- **Audit:** Accessibility check (Axe DevTools).
- **Demo:** Live walkthrough on agreed date.

---

## Decision Points Needing Input

### 1. Authentication Strategy
- **Option A:** Use test accounts (hardcoded or seeded in database). **Pros:** Fast MVP, no dependency on IT. **Cons:** Not real-world.
- **Option B:** Integrate with university LDAP/Shibboleth. **Pros:** Production-ready. **Cons:** Adds 1–2 days; requires IT collaboration.
- **Recommendation for MVP:** Option A (test accounts with feature toggle to LDAP later).

### 2. Notification Delivery
- **Option A:** Queue notifications in database; surface in UI only (no email). **Pros:** Fast, self-contained. **Cons:** No out-of-app notifications.
- **Option B:** Wire SendGrid or SMTP for email confirmations. **Pros:** More professional. **Cons:** Adds setup and requires SMTP server or API key.
- **Recommendation for MVP:** Option A (queued, UI only; Phase 2 adds email).

### 3. Data Retention & GDPR
- **Question:** How long should thesis content be kept after a course ends? (Regulatory requirement? Archival policy?)
- **Impact:** Affects soft-delete strategy, audit logging.
- **Recommendation:** Defer to Phase 2 if not yet defined; use no-delete policy for MVP.

### 4. Load Test Baseline
- **Question:** Expected concurrent users for performance target?
- **Impact:** Shapes database indexing, SignalR tuning, caching strategy.
- **Recommendation:** 50 concurrent users as MVP target; scale post-pilot based on real usage.

---

## Known Technical Debt

### Immediate (Should Fix Before MVP Demo)

1. **Bootstrap Icons CDN integrity hash is broken** — The `App.razor` references a Bootstrap Icons CDN with an invalid `integrity` hash. This causes the stylesheet to be blocked in strict browsers. **Fix:** Update the hash or use the local `lib/` copy.

2. **`UserSessionService` has no multi-device support** — Current in-memory session is per-Blazor-circuit, so a user logging in on two browsers will have two separate sessions. **Fix:** Acceptable for MVP; ASP.NET Core Identity will handle multi-device naturally.

### Soon (Phase 1–2)

1. **Test files are empty** — xUnit projects exist but contain no tests. **Fix:** Write integration tests for CQRS handlers and domain logic once implementations exist.

2. **SignalR is scaffolded but not used** — Blazor Server has SignalR built-in, but no real-time update logic exists. **Fix:** Post-MVP, wire SignalR for live notifications ("Professor approved your topic!").

3. **No data seeding strategy** — Demo will need sample data. **Fix:** Create a seeder in `Infrastructure/Data/SeedData.cs` run at startup.

---

## Rollover from Original Brief

**Sections Preserved:**
- Problem Statement (still accurate).
- Target Users (unchanged).
- Proposed Solution pillars (same, but backend now explicit).
- Success Metrics (adjusted for MVP instead of aspirational).
- Post-MVP Vision (Phases 2+).

**Sections Updated:**
- Executive Summary: Reframed as "UI prototype moving to MVP."
- Technical Considerations: Added current state vs. deferred state.
- MVP Scope: Spelled out domain models, CQRS, API endpoints.
- Roadmap: Added "Prototype" and "MVP" phases with timelines.

**Sections Removed:**
- References to "Restore real authentication" as a next step (now explicitly part of MVP).
- Claims that flows are "end-to-end wired" (they are not yet).

---

## Ready for Next Steps

The reworked brief is now honest about the current state and clear about the path to MVP. It serves as a communication tool for stakeholders and a roadmap for implementation. 

**Recommended action:** Share with sponsor and confirm timeline before beginning Phase 1 (Domain Design).
