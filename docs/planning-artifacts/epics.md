---
stepsCompleted: [1]
inputDocuments: 
  - docs/planning-artifacts/prds/prd-university-management-system-2026-06-26/prd.md
  - docs/planning-artifacts/architecture/architecture-university-management-system-2026-06-26/ARCHITECTURE-SPINE.md
---

# university-management-system - Epic Breakdown

## Overview

This document provides the complete epic and story breakdown for the Thesis Collaboration Portal MVP, decomposing the requirements from the PRD and Architecture spine into implementable stories for graduation delivery (2026-07-31).

---

## Requirements Inventory

### Functional Requirements

FR1: Student can log in with username/password and be assigned Student role
FR2: Student can view dashboard showing thesis assignment status and supervisor name
FR3: Student can write and submit Markdown thesis updates
FR4: Student can view feedback from supervisor on submitted updates
FR5: Professor can log in with username/password and be assigned Professor role
FR6: Professor can view list of students assigned to them with thesis topics
FR7: Professor can review student updates and submit written feedback
FR8: Professor can search for unassigned students and self-assign them

### Non-Functional Requirements

NFR1: All data must persist in SQL Server database (survive app restart)
NFR2: Authentication must use ASP.NET Core Identity with persistent role storage
NFR3: System must support 10–20 concurrent students + 5 professors without timeout
NFR4: Session timeout: 30 minutes of inactivity
NFR5: Database schema must be managed via EF Core Code-First migrations
NFR6: Unit & integration test coverage: 70%+ on domain & application layers
NFR7: MVP must deploy locally via docker-compose in < 5 minutes
NFR8: All data must be queryable and persisted after docker restart

### Additional Requirements (Architecture)

AR1: Clean Architecture layers: Domain (rules), Application (orchestration), Infrastructure (persistence), Presentation (UI)
AR2: CQRS pattern via MediatR: Commands (write) and Queries (read) with handlers
AR3: Validation via FluentValidation: Input validation in handlers before execution
AR4: Error handling via Result<T> pattern: No bare exceptions to UI
AR5: Dependency Injection: All services registered in DI container via Program.cs
AR6: Domain-Driven Design: Student, Professor, Update entities encapsulate state transitions
AR7: Testing: xUnit + EF InMemory for unit/integration tests
AR8: Deployment: Docker Compose with mssql + app services; migrations auto-run at startup
AR9: Database schema: AspNetUsers/Roles + Students/Professors/Assignments/ThesisUpdates/Feedback tables
AR10: Authorization: Role-based access control (Student/Professor); no admin role in MVP

### FR Coverage Map

{{requirements_coverage_map}}

---

## Epic List

### Epic 1: Authentication & Data Foundation
Students and professors can log in securely; all data persists in SQL Server via EF Core migrations and identity tables.  
**FRs covered:** FR1 (student login), FR5 (professor login)

### Epic 2: Student Thesis Workflow
Students can view their thesis assignments, submit thesis updates, and read supervisor feedback.  
**FRs covered:** FR2 (dashboard), FR3 (submit update), FR4 (view feedback)

### Epic 3: Professor Thesis Management
Professors can view assigned students, provide feedback on updates, and self-assign students from the unassigned pool.  
**FRs covered:** FR6 (dashboard), FR7 (feedback), FR8 (self-assign)

### Epic 4: Integration & Graduation-Ready Deployment
All 8 features work end-to-end; system deploys via Docker; ready for thesis demo.  
**FRs covered:** All FRs (E2E validation)

---

## Epic 1: Authentication & Data Foundation

**Epic Goal:** Students and professors can log in securely; all data persists in SQL Server via EF Core migrations and identity tables.

**FRs Covered:** FR1, FR5  
**NFRs Addressed:** NFR1 (persistence), NFR2 (Identity), NFR4 (timeout), NFR5 (migrations)  
**Technical Scope:** DbContext, domain models, Identity integration, DI configuration

### Story 1.1: Design and Create EF Core Domain Model

As a developer,
I want to define all domain entities (Student, Professor, Assignment, ThesisUpdate, Feedback) with proper relationships,
So that the database schema is ready for Code-First migrations.

**Acceptance Criteria:**

**Given** a clean University.Domain project,
**When** I create entity classes for Student, Professor, Assignment, ThesisUpdate, Feedback,
**Then** each entity has proper properties (IDs, foreign keys, timestamps) and relationships are defined via navigation properties
**And** all entities inherit from a common base (or use a convention) for consistency
**And** domain rules are enforced in entity methods (e.g., Student.SubmitUpdate() validates inputs)

---

### Story 1.2: Create and Verify EF Core Migrations

As a developer,
I want to generate Code-First migrations for all domain entities and Identity tables,
So that the database schema is version-controlled and can be rolled forward/backward.

**Acceptance Criteria:**

**Given** domain entities from Story 1.1 and ASP.NET Core Identity configured,
**When** I run `dotnet ef migrations add InitialCreate`,
**Then** a migration file is generated with Create Table statements for all entities
**And** Foreign key constraints are defined with cascade delete where appropriate
**And** migrations run without errors: `dotnet ef database update`
**And** SQL Server contains all 9 tables: AspNetUsers, AspNetRoles, AspNetUserRoles, Students, Professors, Assignments, ThesisTopics, ThesisUpdates, Feedback

---

### Story 1.3: Implement ASP.NET Core Identity Integration

As a developer,
I want to replace UserSessionService with ASP.NET Core Identity and create a working login flow,
So that users have persistent, role-based authentication across restarts.

**Acceptance Criteria:**

**Given** ASP.NET Core Identity configured in Program.cs,
**When** a user enters username and password on Login.razor,
**Then** credentials are validated against AspNetUsers table
**And** a claim-based principal is created with role claims (Student or Professor)
**And** session cookie is set with 30-min timeout
**And** MainLayout redirects unauthenticated users to Login.razor
**And** authenticated users are routed to role-appropriate landing page (StudentDashboard for Student, Home for Professor)

---

### Story 1.4: Configure Dependency Injection and Program.cs

As a developer,
I want to register all services (DbContext, repositories, MediatR handlers, validators, mappers) in the DI container,
So that the application has a consistent, maintainable service architecture.

**Acceptance Criteria:**

**Given** a clean Program.cs,
**When** I configure services (AddDbContext, AddMediatR, AddFluentValidationAutoValidation, AddAutoMapper, AddAuthentication),
**Then** DbContext is registered with SQL Server connection string from environment variable
**And** MediatR is registered and auto-discovers handlers from University.Application assembly
**And** FluentValidation is registered with pipeline behavior for command validation
**And** AutoMapper is configured with profiles from University.Application
**And** Blazor Server rendering is configured with interactive server mode
**And** Authentication/Authorization middleware is added to the pipeline
**And** Application starts without dependency injection errors

---

## Epic 2: Student Thesis Workflow

**Epic Goal:** Students can view their thesis assignments, submit thesis updates, and read supervisor feedback.

**FRs Covered:** FR2, FR3, FR4  
**NFRs Addressed:** NFR6 (70% test coverage)  
**Technical Scope:** MediatR queries/commands, domain logic, Razor component refactoring

### Story 2.1: Refactor StudentDashboard to Load Data from Database

As a student,
I want my dashboard to show my current supervisor and thesis topic fetched from the database,
So that I see my real assignment instead of mock data.

**Acceptance Criteria:**

**Given** a student is logged in,
**When** StudentDashboard.razor loads,
**Then** a GetStudentDashboardQuery is dispatched with the current student ID
**And** the query returns student name, supervisor name (or "Awaiting assignment" if null), and thesis topic
**And** the dashboard displays this data in the UI
**And** if no assignment exists, a "Pending Assignment" message is shown
**And** unit tests cover GetStudentDashboardQuery with 10–20 students in InMemory DB

---

### Story 2.2: Implement Thesis Update Submission

As a student,
I want to write a Markdown thesis update and submit it so my supervisor can review it,
So that I can document my thesis progress.

**Acceptance Criteria:**

**Given** a student is logged in and has a supervisor assigned,
**When** the student enters Markdown content and clicks "Submit Update",
**Then** a SubmitUpdateCommand is dispatched with student ID, content, and timestamp
**And** SubmitUpdateValidator validates that content is not empty and ≤10,000 characters
**And** the command handler calls student.SubmitUpdate(content) on the domain entity
**And** a ThesisUpdate record is created in the database with status = "Submitted"
**And** a success message is displayed: "Update submitted"
**And** unit + integration tests verify the flow end-to-end with InMemory DB

---

### Story 2.3: Display Student Thesis Updates and Supervisor Feedback

As a student,
I want to see my submitted updates and the feedback my supervisor has written,
So that I can track my progress and respond to supervisor guidance.

**Acceptance Criteria:**

**Given** a student has submitted at least one update (Story 2.2),
**When** the student navigates to "My Updates" or views the dashboard,
**Then** a GetStudentUpdatesQuery is dispatched
**And** the query returns all updates for this student with timestamps and content
**And** for each update, any feedback from the supervisor is displayed below with professor name and timestamp
**And** if no feedback exists, a "No feedback yet" message is shown
**And** updates are displayed in reverse chronological order (newest first)
**And** integration tests verify the query with mock updates and feedback in InMemory DB

---

### Story 2.4: Unit Tests for Student Workflow

As a developer,
I want comprehensive unit tests for all student-side handlers and domain logic,
So that the student workflow is validated and safe to refactor.

**Acceptance Criteria:**

**Given** xUnit + EF InMemory configured in University.UnitTests,
**When** I create tests for SubmitUpdateCommand, GetStudentDashboardQuery, GetStudentUpdatesQuery handlers,
**Then** each handler test seeds test data, executes the handler, and asserts output
**And** domain entity tests (Student.SubmitUpdate, Student.AssignSupervisor) verify business rules
**And** validation tests for SubmitUpdateValidator verify all constraints
**And** test coverage for domain + application layers reaches ≥70%
**And** all tests pass and run in < 5 seconds

---

## Epic 3: Professor Thesis Management

**Epic Goal:** Professors can view assigned students, provide feedback on updates, and self-assign students from the unassigned pool.

**FRs Covered:** FR6, FR7, FR8  
**NFRs Addressed:** NFR3 (10–20 concurrent users)  
**Technical Scope:** MediatR handlers, authorization, Razor components

### Story 3.1: Implement Professor Dashboard to View Assigned Students

As a professor,
I want to see a list of students assigned to me with their thesis topics and last update dates,
So that I know who I'm supervising and can track their progress.

**Acceptance Criteria:**

**Given** a professor is logged in and has students assigned to them (from seed data),
**When** the professor navigates to their dashboard,
**Then** a GetProfessorStudentsQuery is dispatched with the professor ID
**And** the query returns a list of students: Name | Thesis Topic | Last Update Date | Status
**And** [Authorize(Roles = "Professor")] attribute prevents student access
**And** clicking on a student name navigates to the student detail page (Story 3.3)
**And** integration tests verify the query with 5 professors and 15 students in InMemory DB

---

### Story 3.2: Implement Professor Self-Assignment of Students

As a professor,
I want to search for unassigned students and assign them to myself with one click,
So that I can take on new advisees without admin intervention.

**Acceptance Criteria:**

**Given** a professor is logged in,
**When** the professor navigates to "Assign Students",
**Then** a search box allows filtering unassigned students by name
**And** clicking "Assign to Me" next to a student triggers AssignStudentCommand
**And** the command validates that the student is not already assigned
**And** an Assignment record is created with StudentId, ProfessorId, and current date
**And** the student immediately appears in the professor's dashboard (Story 3.1)
**And** a confirmation message is displayed: "Student assigned to you"
**And** integration tests verify the assignment persists in InMemory DB

---

### Story 3.3: Implement Thesis Review and Feedback Submission

As a professor,
I want to review a student's thesis update and submit written feedback,
So that I can guide the student's thesis work and provide constructive guidance.

**Acceptance Criteria:**

**Given** a professor is viewing a student's latest thesis update,
**When** the professor enters feedback text and clicks "Submit Feedback",
**Then** a SubmitFeedbackCommand is dispatched with UpdateId, ProfessorId, and comment text
**And** SubmitFeedbackValidator ensures comment is not empty and ≤5,000 characters
**And** a Feedback record is created in the database with timestamp
**And** a success message is displayed: "Feedback submitted"
**And** the student can immediately see the feedback (Story 2.3)
**And** integration tests verify the feedback round-trip with InMemory DB

---

### Story 3.4: Unit Tests for Professor Workflow

As a developer,
I want comprehensive unit tests for all professor-side handlers and authorization,
So that the professor workflow is validated and safe to refactor.

**Acceptance Criteria:**

**Given** xUnit + EF InMemory configured,
**When** I create tests for GetProfessorStudentsQuery, AssignStudentCommand, SubmitFeedbackCommand handlers,
**Then** each test seeds test data, executes the handler, and asserts output
**And** authorization tests verify that students cannot call professor-only queries
**And** domain entity tests verify that invalid assignments throw DomainException
**And** test coverage for domain + application layers reaches ≥70% across both student + professor flows
**And** all tests pass and run in < 5 seconds

---

## Epic 4: Integration & Graduation-Ready Deployment

**Epic Goal:** All 8 features work end-to-end; system deploys via Docker; ready for thesis demo.

**FRs Covered:** All FRs (E2E validation)  
**NFRs Addressed:** NFR7 (docker <5 min), NFR8 (persistence), NFR6 (70% coverage)  
**Technical Scope:** Docker Compose, seed data, E2E integration tests, demo script

### Story 4.1: Create Docker Compose Configuration

As a developer,
I want a single docker-compose.yml that spins up SQL Server and the Blazor app,
So that the system can be deployed with `docker-compose up` in < 5 minutes from cold start.

**Acceptance Criteria:**

**Given** a docker-compose.yml in the project root,
**When** I run `docker-compose up`,
**Then** a SQL Server 2022 container starts with SA_PASSWORD and ACCEPT_EULA
**And** the Blazor app container builds from ./src/Dockerfile
**And** the app waits for SQL Server to be ready before starting
**And** EF Core migrations run automatically at app startup: `dotnet ef database update`
**And** seed data is inserted (5 professors, 15 students, 3 assignments)
**And** the app is accessible at http://localhost:5000 within 5 minutes
**And** data persists in a named volume `sqldata` across `docker-compose down` and `docker-compose up`

---

### Story 4.2: Create Seed Data and Integration Tests

As a developer,
I want seed data (professors, students, assignments) to be inserted at app startup,
So that the demo can proceed without manual data entry.

**Acceptance Criteria:**

**Given** a DbContext seed method,
**When** the app starts for the first time (or migrations run),
**Then** seed data is inserted: 5 professors, 15 students, 3 professor-student assignments
**And** each seed user has a login credential for demo purposes
**And** integration tests load this seed data into InMemory DB and verify all 8 features work end-to-end
**And** tests cover full flows: login → assign → submit update → review feedback
**And** tests verify role-based access (students cannot see professor pages, etc.)
**And** all integration tests pass

---

### Story 4.3: End-to-End Demo Validation and Script

As a developer,
I want to validate all 8 features work in sequence and create a demo script,
So that I can demonstrate the system to the thesis committee with confidence.

**Acceptance Criteria:**

**Given** a running docker-compose environment,
**When** I follow the demo script,
**Then** Login as Professor → Dashboard shows unassigned students → Assign a student → Logout
**And** Login as Student → Dashboard shows supervisor → Submit thesis update → Logout
**And** Login as Professor → Dashboard shows assigned student → Review update → Submit feedback → Logout
**And** Login as Student → Dashboard shows feedback from professor
**And** Stop containers (`docker-compose down`), restart (`docker-compose up`), verify all data persists
**And** Total demo time from cold start to all features validated: ≤ 15 minutes
**And** Demo script is documented in README.md with step-by-step instructions

---

## Requirements Coverage Map

| Requirement | Epic | Story | Notes |
|------------|------|-------|-------|
| FR1: Student login | Epic 1 | 1.3 | ASP.NET Core Identity |
| FR2: Student dashboard | Epic 2 | 2.1 | Load assignment from DB |
| FR3: Submit thesis update | Epic 2 | 2.2 | MediatR SubmitUpdateCommand |
| FR4: View feedback | Epic 2 | 2.3 | GetStudentUpdatesQuery + feedback display |
| FR5: Professor login | Epic 1 | 1.3 | ASP.NET Core Identity (same flow) |
| FR6: Professor dashboard | Epic 3 | 3.1 | GetProfessorStudentsQuery |
| FR7: Submit feedback | Epic 3 | 3.3 | SubmitFeedbackCommand |
| FR8: Self-assign students | Epic 3 | 3.2 | AssignStudentCommand |
| NFR1: Database persistence | Epic 1 | 1.2, 1.4 | EF Core migrations, SQL Server |
| NFR2: ASP.NET Core Identity | Epic 1 | 1.3 | Role-based authentication |
| NFR3: Concurrent users | Epic 3 | 3.4 | Load testing (seed 15 users) |
| NFR4: 30-min timeout | Epic 1 | 1.3 | Cookie expiry configured |
| NFR5: EF Core migrations | Epic 1 | 1.2 | Code-First + version control |
| NFR6: 70% test coverage | Epic 2, 3, 4 | 2.4, 3.4, 4.2 | Unit + integration tests |
| NFR7: Docker < 5 min | Epic 4 | 4.1 | docker-compose.yml |
| NFR8: Data persistence post-restart | Epic 4 | 4.1, 4.3 | Named volume + verification |



