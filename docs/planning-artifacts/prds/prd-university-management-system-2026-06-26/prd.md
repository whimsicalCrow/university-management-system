---
title: "Thesis Collaboration Portal — MVP PRD"
date: 2026-06-26
status: draft
audience: Developer (Hermes)
project: university-management-system
phase: MVP
deadline: 2026-07-31
---

# Thesis Collaboration Portal — MVP Product Requirements Document

## 1. Vision & Objectives

**Vision Statement:**  
A working MVP that enables students to submit thesis updates, supervisors to review and provide feedback, and professors to self-assign students—all running in Docker containers with SQL Server backend. Deployable and demonstrable as a graduation project by end of July 2026.

**Core Objectives:**
- ✅ Replace mock data with persistent SQL Server database in Docker
- ✅ Implement ASP.NET Core Identity for role-based authentication (Student, Professor)
- ✅ Wire MediatR CQRS pattern for core domain operations
- ✅ Enable professor-driven student assignment (professor self-assigns students, not admin)
- ✅ Support thesis update submission and review workflow
- ✅ Deploy & demonstrate locally via docker-compose
- ✅ Set foundation for Phase 2+ (attachments, email, calendar)

---

## 2. MVP Feature Set (By Actor)

### 2.1 Student Flows

#### F1: Student Authentication & Login
**User Story:** As a student, I log in with a username/password so I can access my thesis workspace.

- Simple login form (username, password)
- Role auto-assigned: `Student`
- Persisted in ASP.NET Core Identity + database
- [ASSUMPTION] No self-registration in MVP; professors create student accounts
- Session expires after 30 minutes of inactivity
- **Acceptance Criteria:**
  - Login form validates credentials against database
  - Session established with claim-based identity
  - Redirect to StudentDashboard on successful login
  - Error message on failed login

#### F2: Student Dashboard
**User Story:** As a student, I see my thesis assignment status and can navigate to my actions.

- Display current supervisor name (if assigned)
- Display thesis topic (if assigned)
- Display "Pending Assignment" message if not yet assigned
- Quick-access buttons to: Submit Update, View Feedback, View Meetings (Phase 2)
- **Acceptance Criteria:**
  - Dashboard loads student's assignment from database
  - Shows supervisor name or "Awaiting assignment" state
  - All navigation links work

#### F3: Submit Thesis Update
**User Story:** As a student, I write a Markdown update and submit it to my supervisor for review.

- Reuse existing Markdig preview UI (already in StudentDashboard.razor)
- Submit button creates `ThesisUpdate` record in database
- Status: `Draft` on creation
- Store update in database with timestamp
- Show "Update submitted" confirmation
- **Acceptance Criteria:**
  - Update saves to database with student ID, timestamp, markdown content
  - Autosave placeholder → real save to database
  - Supervisor can retrieve update from database (F5)
  - [ASSUMPTION] File attachments are Phase 2; MVP is text-only

#### F4: View Feedback from Supervisor
**User Story:** As a student, I read my supervisor's feedback on my update.

- Fetch latest feedback from database for this student
- Display in read-only format with professor name and feedback timestamp
- [ASSUMPTION] Real-time notifications are Phase 2; students check manually
- **Acceptance Criteria:**
  - Feedback loads from database
  - Displays professor name, feedback timestamp, and comment text

---

### 2.2 Professor Flows

#### F5: Professor Authentication & Login
**User Story:** As a professor, I log in with a username/password so I can supervise my assigned students.

- Login form (username, password)
- Role auto-assigned: `Professor`
- Persisted in ASP.NET Core Identity + database
- [ASSUMPTION] No self-registration; system admin creates professor accounts
- Session expires after 30 minutes of inactivity
- **Acceptance Criteria:**
  - Login form validates credentials
  - Session established with claim-based identity
  - Redirect to Home.razor (conditional nav to professor-specific pages)

#### F6: Professor Dashboard / My Students
**User Story:** As a professor, I see a list of my assigned students and can manage their thesis updates.

- Table: Student Name | Thesis Topic | Last Update | Status
- Filter by status (In Progress, Awaiting Feedback, Completed)
- Click on a student to view their latest update (F7)
- [ASSUMPTION] No search/pagination in MVP; max 20 students per professor
- **Acceptance Criteria:**
  - Loads professor's assigned students from database
  - Shows student names and thesis topics
  - Links to student update detail page

#### F7: Review Student Update
**User Story:** As a professor, I read a student's thesis update and provide written feedback.

- Fetch update from database
- Display student name, update timestamp, markdown content (read-only preview)
- Text area for supervisor feedback
- "Submit Feedback" button saves feedback to database
- Show confirmation: "Feedback submitted"
- **Acceptance Criteria:**
  - Update loads from database
  - Feedback textarea accepts text
  - Feedback saves with professor ID, timestamp, update ID
  - Student can retrieve feedback (F4)

#### F8: Self-Assign Students
**User Story:** As a professor, I search for unassigned students and assign them to myself.

- **Current State:** Assignments.razor has mock data (hardcoded 5 students, 4 professors)
- **MVP Behavior:**
  - Page: "Assign Students"
  - Search box: searches unassigned students by name
  - Results: table of unassigned students
  - "Assign to Me" button next to each student
  - Button triggers: `Student.SupervisorId = CurrentProfessor.Id`, saves to database
  - Confirmation: "Student assigned to you"
- **Acceptance Criteria:**
  - Search returns unassigned students from database
  - "Assign to Me" button persists assignment in database
  - Student appears in professor's dashboard (F6) immediately
  - [ASSUMPTION] Professor can reassign (change supervisor); Phase 2 locks reassignment after first update

---

## 3. Out of Scope (Phase 2+)

- ❌ File attachments (thesis documents, code, etc.)
- ❌ Email notifications
- ❌ Calendar/meeting scheduling
- ❌ Admin dashboard or bulk import
- ❌ Multi-tenancy (single university)
- ❌ AI feedback suggestions
- ❌ Export to PDF/Word
- ❌ Real-time notifications/SignalR
- ❌ Mobile app

---

## 4. Success Metrics

| Metric | Target | Rationale |
|--------|--------|-----------|
| **MVP Deployment** | Runs locally via `docker-compose up` | Demonstration requirement |
| **Database Persistence** | All data survives app restart | Validates SQL Server integration |
| **Core Flows** | All 8 features functional end-to-end | Thesis defense demo |
| **Auth Integration** | Login + role-based nav working | Proves Identity wiring |
| **Pilot Scale** | 10–20 students + 5 professors, zero errors | Load test baseline |
| **Code Coverage** | 70%+ on domain/application layers | Foundation for Phase 2 |
| **Deployment Time** | Spin up demo environment in < 5 min | Time-to-thesis-defense matters |

---

## 5. Architecture & Technical Approach

### 5.1 Database Schema (SQL Server in Docker)

**Core Tables:**
- `AspNetUsers` (ASP.NET Core Identity)
- `AspNetRoles` (Student, Professor)
- `AspNetUserRoles` (link users to roles)
- `Students` (StudentId, UserId FK, Specialization, EnrollmentDate)
- `Professors` (ProfessorId, UserId FK, Department, Expertise)
- `Assignments` (AssignmentId, StudentId FK, ProfessorId FK, AssignedDate)
- `ThesisTopics` (TopicId, Title, Description, ProfessorId FK, Status)
- `ThesisUpdates` (UpdateId, StudentId FK, Content, SubmittedAt, Status)
- `Feedback` (FeedbackId, UpdateId FK, ProfessorId FK, Comment, SubmittedAt)

**[ASSUMPTION]** Entity Framework Core Code-First migrations will generate schema.

### 5.2 Architecture Decisions

| Layer | Pattern | Implementation |
|-------|---------|-----------------|
| **Presentation** | Blazor Server components | Existing 8 pages, refactored for backend integration |
| **Application** | MediatR CQRS | Commands: LoginStudent, SubmitUpdate, AssignStudent; Queries: GetStudentDashboard, GetStudentUpdates |
| **Domain** | Clean Architecture aggregates | Student, Professor, Thesis, Update, Feedback entities with business rules |
| **Infrastructure** | EF Core + SQL Server | DbContext, repositories, migrations in Docker |
| **Authentication** | ASP.NET Core Identity | Role-based policies for Student/Professor access |

### 5.3 Docker Compose

```yaml
services:
  mssql:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: <strong-password>
      ACCEPT_EULA: Y
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
  
  nehrportal:
    build: ./src
    ports:
      - "5000:5000"
    depends_on:
      - mssql
    environment:
      CMSConnectionString: Server=mssql;Initial Catalog=UniversityDB;User Id=sa;Password=<strong-password>;
```

**[ASSUMPTION]** Connection string injected via environment variable; `dotnet ef migrations` runs at startup.

---

## 6. Implementation Roadmap (4 Weeks to Graduation)

### Week 1: Domain & Database
- [ ] Design database schema (tables listed above)
- [ ] Create EF Core DbContext and entity models
- [ ] Generate & test migrations in Docker
- [ ] Seed test data (5 professors, 10–20 students, 2–3 assignments)

### Week 2: Authentication & Identity
- [ ] Replace UserSessionService with ASP.NET Core Identity
- [ ] Create login endpoint & form (reuse existing Login.razor UI)
- [ ] Implement role-based authorization policies
- [ ] Test login for Student and Professor roles

### Week 3: Core Workflows
- [ ] Implement F1–F8 features (see section 2)
- [ ] Wire MediatR handlers for each feature
- [ ] Refactor existing Razor pages to call MediatR instead of mock data
- [ ] Integration tests for each workflow

### Week 4: Polish & Deployment
- [ ] Docker Compose validation (spin up from scratch)
- [ ] Performance baseline (10–20 users, no timeout)
- [ ] Demo script & dry run
- [ ] Final bug fixes

---

## 7. Testing Strategy

### Unit Tests (EF InMemory + xUnit)
- Domain logic: Student assignment validation, thesis update status transitions
- Application handlers: LoginStudent, SubmitUpdate, GetStudentDashboard
- Coverage target: 70% of domain + application layers

### Integration Tests
- Database round-trip: Create student → assign professor → submit update → fetch feedback
- End-to-end login flow for both roles
- [ASSUMPTION] Selenium/Playwright for UI E2E is Phase 2

### Manual Testing
- Spin up docker-compose, test all 8 features
- Role-based access: professor cannot view other professors' students
- Session timeout after 30 min

---

## 8. Known Constraints & Assumptions

| Assumption | Rationale | Mitigation |
|-----------|-----------|-----------|
| No self-registration | Admin/seeding only for MVP | Seed 5 professors + 10–20 students at startup |
| SQL Server in Docker locally | Avoids external DB dependency | docker-compose includes MSSQL service |
| 30-min session timeout | Reasonable for internal tool | Longer timeouts in Phase 2 |
| No file attachments | Complex blob storage; Phase 2 | MVP is text-only updates |
| Professor assigns only self | Simpler workflow for MVP | Admin bulk-assign in Phase 2 |
| No email notifications | Requires SMTP/SendGrid setup; Phase 2 | Manual check in MVP |
| All data in single database | Matches thesis project scope | Multi-tenancy in Phase 2 |

---

## 9. Graduation Demo Success Criteria

**What You'll Show:**
1. Launch containers: `docker-compose up`
2. Login as professor → assign a student to yourself
3. Login as student → submit a thesis update
4. Login as professor → review update, provide feedback
5. Login as student → view feedback from professor
6. Verify data persists after app restart

**Demo Time:** 15 minutes  
**Risk Mitigation:** Script pre-loaded data in seed file, so demo doesn't depend on manual setup

---

## 10. Next Steps

1. ✅ Confirm this PRD aligns with your vision (no changes needed, or iterate)
2. 🚀 Invoke `bmad-architecture` to design the Clean Architecture blueprint
3. 🚀 Invoke `bmad-create-epics-and-stories` to break down the 8 features into sprint-ready stories
4. 🚀 Begin implementation with `bmad-dev-story` for Week 1 work

---

## Sign-Off

**PRD Status:** Ready for Architecture & Story Planning  
**Created:** 2026-06-26  
**Target Launch:** 2026-07-31  
**Developer:** Hermes  

---

### Document Metadata
- **Deadline Risk Level:** 🔴 **HIGH** — 5 weeks to implement 8 features + Docker + tests. Scope must stay frozen; no feature creep.
- **Technical Risk Level:** 🟡 **MEDIUM** — EF Core migrations + SQL Server in Docker are new to this project; Week 1 schema design is critical path.
- **Demo Risk Level:** 🟢 **LOW** — All features are straightforward CRUD + role-based nav. No novel complexity.

