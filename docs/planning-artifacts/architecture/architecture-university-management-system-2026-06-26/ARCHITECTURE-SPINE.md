---
title: "Thesis Collaboration Portal — Architecture Spine (MVP)"
date: 2026-06-26
status: final
updated: 2026-06-26
audience: Development team (Hermes)
altitude: Initiative (covers all 8 MVP features)
---

# Architecture Spine — Thesis Collaboration Portal MVP

## 1. Paradigm

**Clean Architecture + CQRS via MediatR**

- **Domain:** Aggregates (Student, Professor, Thesis, Update, Feedback) own business rules and state invariants. Persistence-agnostic entities.
- **Application:** MediatR Command/Query handlers orchestrate domain operations, validation, and persistence.
- **Infrastructure:** EF Core DbContext + SQL Server provide data access; repositories implement domain interfaces.
- **Presentation:** Blazor Server Razor components; authorization via ASP.NET Core Identity claims.

**Rationale:** Scales cleanly from pilot (20 users) to Phase 2 (multi-tenancy); enables testing domain logic in isolation; CQRS separates command (write) and query (read) paths, supporting future event sourcing (Phase 2).

---

## 2. Architectural Decisions (ADs)

### AD-1: Clean Architecture Layers
**Binds:** Every feature flows through Domain → Application → Infrastructure → Presentation.  
**Prevents:** Logic scattered across layers, persistence coupling to domain.  
**Rule:** 
```
University.Web
  ├─ Components (Razor pages, layouts)
  ├─ Services (SignalR hubs, session managers)
  └─ appsettings.json

University.Application
  ├─ Commands (LoginStudentCommand, SubmitUpdateCommand, AssignStudentCommand, etc.)
  ├─ Queries (GetStudentDashboardQuery, GetProfessorStudentsQuery, etc.)
  ├─ Handlers (command & query handlers)
  ├─ Validators (FluentValidation per command)
  └─ DTOs (LoginResponse, StudentDashboardDto, etc.)

University.Domain
  ├─ Entities (Student, Professor, Thesis, Update, Feedback)
  ├─ Aggregates (root entities with business rules)
  ├─ Repositories (interfaces only; e.g., IStudentRepository)
  ├─ Services (domain services for cross-aggregate logic)
  └─ Exceptions (DomainException, ValidationException)

University.Infrastructure
  ├─ Data
  │  ├─ UniversityDbContext (EF Core mapping)
  │  ├─ Configurations (EF Entity Mappings)
  │  ├─ Migrations (auto-generated from DbContext)
  │  └─ Seed (initial data population)
  └─ Repositories (implementations of domain interfaces)
```

**Status:** [ADOPTED] — Projects exist; Week 1 fills with models.

---

### AD-2: Authentication & Authorization
**Binds:** ASP.NET Core Identity replaces UserSessionService.  
**Prevents:** In-memory sessions lost on restart, no persistent role assignment.  
**Rule:**
- Users login via `LoginCommand` → handler validates credentials against AspNetUsers table → sets claim-based principal → redirects to role-based page.
- Two roles: `Student`, `Professor`.
- MainLayout checks `HttpContext.User.Identity.IsAuthenticated`; unauthenticated → Login.razor.
- Protected routes check claim: `[Authorize(Roles = "Student")]` on StudentDashboard.razor, etc.
- Session timeout: 30 min (via cookie expiry).

**Database Tables:**
- `AspNetUsers` (UserName, Email, PasswordHash, etc.) — standard Identity table
- `AspNetRoles` (Name = "Student" | "Professor")
- `AspNetUserRoles` (UserId FK, RoleId FK)

**Status:** [NEW] — Implement Week 2.

---

### AD-3: Data Persistence — Entity Framework Core + SQL Server
**Binds:** EF Core Code-First migrations generate schema; DbContext is single source of truth.  
**Prevents:** Ad hoc SQL, schema drift, no rollback capability.  
**Rule:**
- Domain entities + EF configurations in University.Infrastructure.
- DbContext fluent API maps entities → tables.
- Migrations auto-generated: `dotnet ef migrations add FeatureName`.
- Applied at app startup: `dbContext.Database.Migrate()`.
- Connection string via environment variable: `CMSConnectionString=Server=mssql;Initial Catalog=UniversityDB;...`

**Core Tables:**
```sql
AspNetUsers, AspNetRoles, AspNetUserRoles          -- Identity
Students (Id, UserId FK, Specialization, ...)      -- Domain entity
Professors (Id, UserId FK, Department, ...)        -- Domain entity
Assignments (Id, StudentId FK, ProfessorId FK, AssignedDate)
ThesisTopics (Id, Title, Description, ProfessorId FK, Status)
ThesisUpdates (Id, StudentId FK, Content, SubmittedAt, Status)
Feedback (Id, UpdateId FK, ProfessorId FK, Comment, SubmittedAt)
```

**Status:** [NEW] — Design Week 1; generate migrations Week 1.

---

### AD-4: CQRS Pattern via MediatR
**Binds:** Every business operation is a Command or Query; MediatR dispatch routes to handlers.  
**Prevents:** Monolithic controllers, business logic in views.  
**Rule:**
- **Command** (write): `LoginStudentCommand`, `SubmitUpdateCommand`, `AssignStudentCommand`, `SubmitFeedbackCommand`.
  - Handler validates input via FluentValidation.
  - Calls domain method on aggregate.
  - Persists to DbContext.
  - Returns `Result<T>` (success or error code).
- **Query** (read): `GetStudentDashboardQuery`, `GetProfessorStudentsQuery`, `GetStudentUpdatesQuery`, `GetFeedbackQuery`.
  - Handler queries DbContext, maps to DTO, returns.
  - No mutations, no side effects.
- Blazor components inject `IMediator`; call `await mediator.Send(command)`.

**Example Flow:**
```csharp
// Blazor component
await mediator.Send(new SubmitUpdateCommand(StudentId, Content));

// Handler
public class SubmitUpdateCommandHandler : IRequestHandler<SubmitUpdateCommand, Result<UpdateDto>>
{
    public async Task<Result<UpdateDto>> Handle(SubmitUpdateCommand request, CancellationToken cancellationToken)
    {
        var validator = new SubmitUpdateValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result<UpdateDto>.Failure("ValidationError", validationResult.Errors);

        var student = await _repository.GetStudentAsync(request.StudentId);
        var update = student.SubmitUpdate(request.Content, DateTime.UtcNow);
        await _unitOfWork.SaveChangesAsync();
        
        return Result<UpdateDto>.Success(mapper.Map<UpdateDto>(update));
    }
}
```

**Status:** [ADOPTED] — MediatR installed; handlers to write Weeks 2–3.

---

### AD-5: Validation Strategy
**Binds:** FluentValidation validators paired with every Command; MediatR pipeline enforces validation.  
**Prevents:** Invalid state reaching domain, scattered null checks.  
**Rule:**
- Every command has a `<CommandName>Validator : AbstractValidator<TCommand>`.
- Validator registered in DI container: `services.AddValidatorsFromAssembly(...)`.
- MediatR pipeline: `ValidationBehavior<TRequest, TResponse>` intercepts; if invalid, return error without calling handler.
- Domain rules (e.g., "professor cannot assign more than N students") live in domain entity methods, throw `DomainException`.

**Example:**
```csharp
public class SubmitUpdateValidator : AbstractValidator<SubmitUpdateCommand>
{
    public SubmitUpdateValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(10000);
    }
}
```

**Status:** [NEW] — Implement Week 2.

---

### AD-6: Error Handling
**Binds:** `Result<T>` pattern wraps success/failure; domain throws `DomainException`; handlers catch and convert to `Result`.  
**Prevents:** Unhandled exceptions in UI, inconsistent error messages.  
**Rule:**
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T Data { get; }
    public string ErrorCode { get; }
    public List<string> Messages { get; }
    
    public static Result<T> Success(T data) => new(true, data, null, []);
    public static Result<T> Failure(string code, List<string> msgs) => new(false, default, code, msgs);
}

// Handler usage
try {
    var update = await _repository.GetUpdateAsync(id);
    if (update == null)
        return Result<UpdateDto>.Failure("NotFound", ["Update not found"]);
    
    var feedback = student.ProvideFeedback(update, request.Comment); // throws if invalid
    await _unitOfWork.SaveChangesAsync();
    return Result<UpdateDto>.Success(mapper.Map<UpdateDto>(feedback));
}
catch (DomainException ex)
{
    _logger.LogWarning(ex, "Domain error: {Message}", ex.Message);
    return Result<UpdateDto>.Failure("DomainError", [ex.Message]);
}
```

**Error Codes (MVP):**
- `ValidationError` — FluentValidation failed
- `NotFound` — Entity not found
- `Unauthorized` — User lacks permission
- `DomainError` — Business rule violated
- `ServerError` — Unexpected exception

**Status:** [NEW] — Implement Week 2.

---

### AD-7: Dependency Injection & Service Registration
**Binds:** Program.cs registers all services (DbContext, repositories, handlers, validators, mappers); constructor injection throughout.  
**Prevents:** Service Locator anti-pattern, hidden dependencies.  
**Rule:**
```csharp
// Program.cs
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();
builder.Services.AddAuthorization();
builder.Services.AddRazorComponents().AddInteractiveServerRenderingAsync();

// Per-handler example
public class SubmitUpdateCommandHandler
{
    private readonly UniversityDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<SubmitUpdateCommandHandler> _logger;
    
    public SubmitUpdateCommandHandler(UniversityDbContext dbContext, IMapper mapper, ILogger<SubmitUpdateCommandHandler> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }
}
```

**Status:** [NEW] — Implement Week 2; DI container setup.

---

### AD-8: Logging
**Binds:** ILogger<T> injected; Info (user actions), Debug (internal flow), Error (exceptions).  
**Prevents:** Silent failures, hard-to-diagnose integration issues.  
**Rule:**
- Info: `_logger.LogInformation("User {UserId} logged in", userId)`
- Debug: `_logger.LogDebug("Query executed: {QueryName}", query.GetType().Name)`
- Error: `_logger.LogError(exception, "Handler failed: {HandlerName}", handler.GetType().Name)`
- Sinks: Console + local file in MVP; Phase 2 adds Application Insights.

**Status:** [NEW] — Implement Week 2.

---

### AD-9: Mapping (DTOs)
**Binds:** AutoMapper maps Domain entities ↔ Application DTOs; never expose domain entities to UI.  
**Prevents:** Accidental domain model leakage, tight coupling between UI and domain.  
**Rule:**
```csharp
// Domain entity (private setters)
public class ThesisUpdate
{
    public int Id { get; private set; }
    public int StudentId { get; private set; }
    public string Content { get; private set; }
    public DateTime SubmittedAt { get; private set; }
}

// DTO (public, serializable)
public class ThesisUpdateDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string Content { get; set; }
    public DateTime SubmittedAt { get; set; }
}

// Mapping profile
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ThesisUpdate, ThesisUpdateDto>().ReverseMap();
    }
}
```

**Status:** [NEW] — Implement Week 2.

---

### AD-10: Authorization Policies
**Binds:** Blazor components check `HttpContext.User.IsInRole()` or use `[Authorize]` attributes; handlers may enforce additional business rules.  
**Prevents:** Data leakage, unauthorized mutations.  
**Rule:**
```csharp
// Blazor page
@page "/professor/students"
@attribute [Authorize(Roles = "Professor")]

@code {
    protected override async Task OnInitializedAsync()
    {
        var result = await mediator.Send(new GetProfessorStudentsQuery(CurrentProfessorId));
        // result will be null or empty if not professor
    }
}

// Handler (additional checks)
public class GetProfessorStudentsQueryHandler : IRequestHandler<GetProfessorStudentsQuery, List<StudentDto>>
{
    public async Task<List<StudentDto>> Handle(GetProfessorStudentsQuery request, CancellationToken cancellationToken)
    {
        // Verify caller is requesting their own students, not another's
        var professor = await _dbContext.Professors.FindAsync(request.ProfessorId);
        if (professor == null)
            throw new UnauthorizedAccessException();
        
        var students = await _dbContext.Assignments
            .Where(a => a.ProfessorId == request.ProfessorId)
            .Select(a => a.Student)
            .ToListAsync();
        
        return students.Select(s => mapper.Map<StudentDto>(s)).ToList();
    }
}
```

**Status:** [NEW] — Implement Week 2.

---

### AD-11: Testing Strategy
**Binds:** xUnit + EF InMemory for unit/integration tests; 70%+ coverage on domain + application layers.  
**Prevents:** Regressions, confidence in refactoring.  
**Rule:**
- **Unit Tests:** Domain entities in isolation (e.g., Student.SubmitUpdate() validates inputs).
  - Use `DbContextOptions<UniversityDbContext>` with InMemoryProvider.
  - Seed minimal test data; verify entity behavior.
- **Integration Tests:** Full handler + DbContext flow (e.g., SubmitUpdateCommand persists to InMemory DB and can be queried).
  - Seed students/professors; invoke handler; assert data in DB.
- **No Selenium/Playwright in MVP** (Phase 2 adds UI E2E tests).

**Example:**
```csharp
[Fact]
public async Task SubmitUpdateCommand_ValidInput_PersistsUpdate()
{
    // Arrange
    var options = new DbContextOptionsBuilder<UniversityDbContext>()
        .UseInMemoryDatabase("TestDb")
        .Options;
    var dbContext = new UniversityDbContext(options);
    var student = new Student { Id = 1, UserId = "user1", Specialization = "AI" };
    dbContext.Students.Add(student);
    await dbContext.SaveChangesAsync();
    
    var handler = new SubmitUpdateCommandHandler(dbContext, mapper, logger);
    var command = new SubmitUpdateCommand(1, "My first update");
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    Assert.True(result.IsSuccess);
    var update = await dbContext.ThesisUpdates.FirstOrDefaultAsync(u => u.StudentId == 1);
    Assert.NotNull(update);
    Assert.Equal("My first update", update.Content);
}
```

**Status:** [NEW] — Tests added Weeks 2–4; target 70% coverage by graduation.

---

### AD-12: Deployment & Environment Configuration
**Binds:** Docker Compose locally; SQL Server container + Blazor app container.  
**Prevents:** "Works on my machine," missing environment setup.  
**Rule:**
```yaml
# docker-compose.yml
version: '3.8'
services:
  mssql:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: StrongPassword123!
      ACCEPT_EULA: Y
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql

  nehrportal:
    build:
      context: ./src
      dockerfile: Dockerfile
    ports:
      - "5000:5000"
    depends_on:
      - mssql
    environment:
      CMSConnectionString: "Server=mssql;Initial Catalog=UniversityDB;User Id=sa;Password=StrongPassword123!;TrustServerCertificate=true"
      ASPNETCORE_ENVIRONMENT: Development
    command: sh -c "dotnet ef database update --context UniversityDbContext && dotnet University.Web.dll"

volumes:
  sqldata:
```

**Startup Flow:**
1. `docker-compose up`
2. Migrations auto-run (EF Core `Database.Migrate()` in Program.cs)
3. Seed data inserted (Students, Professors, test assignments)
4. App ready at `http://localhost:5000`
5. Data persists in named volume `sqldata` across restarts

**Status:** [NEW] — Implement Week 4; refine as deployment proceeds.

---

### AD-13: Caching Strategy — Minimal in MVP
**Binds:** No distributed cache; EF Core DbContext query cache only.  
**Prevents:** Stale data (risky for graduation demo), unnecessary complexity.  
**Rule:**
- Queries hit DB each time; let SQL Server buffer pool handle in-memory caching.
- Phase 2: evaluate Redis/MemoryCache if profiling shows query bottlenecks.
- Dashboard query (GetProfessorStudentsQuery) is O(n) where n ≤ 20 (pilot scale); acceptable latency.

**Status:** [NEW] — Deferred to Phase 2.

---

### AD-14: Session & State Management
**Binds:** ASP.NET Core session state via cookie; 30-min timeout; logout clears cookie.  
**Prevents:** Credential reuse, session fixation.  
**Rule:**
```csharp
// Program.cs
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".UniversityPortal.Session";
    options.Cookie.IsEssential = true;
});

// Middleware
app.UseSession();

// Logout
[HttpPost]
public async Task<IActionResult> Logout()
{
    await _signInManager.SignOutAsync();
    HttpContext.Session.Clear();
    return RedirectToPage("/Login");
}
```

**Phase 2:** Remember-me, device fingerprinting, location tracking.

**Status:** [NEW] — Implement Week 2.

---

### AD-15: Exception Handling & Resilience
**Binds:** Try-catch at handler level; log exception; return graceful error to UI.  
**Prevents:** Unhandled exceptions crashing app, poor observability.  
**Rule:**
```csharp
// Global exception handler middleware (Phase 2)
// For MVP: catch in handlers, log, return Result.Failure

public async Task Handle(SubmitUpdateCommand request, CancellationToken cancellationToken)
{
    try
    {
        // ... business logic
    }
    catch (InvalidOperationException ex)
    {
        _logger.LogError(ex, "Invalid operation in SubmitUpdateCommand");
        return Result<UpdateDto>.Failure("ServerError", [ex.Message]);
    }
    catch (Exception ex)
    {
        _logger.LogCritical(ex, "Unexpected error in SubmitUpdateCommand");
        throw; // Re-throw for middleware to catch
    }
}
```

**Status:** [NEW] — Implement Week 2; improve in Phase 2.

---

### AD-16: Domain-Driven Design (Aggregates & Invariants)
**Binds:** Student, Professor, Thesis, Update are aggregate roots; encapsulate state transitions.  
**Prevents:** Anemic models, invalid state, cascading bugs.  
**Rule:**
```csharp
public class Student
{
    public int Id { get; private set; }
    public string UserId { get; private set; }
    public int? SupervisorId { get; private set; }
    
    public void AssignSupervisor(int professorId)
    {
        if (SupervisorId.HasValue)
            throw new DomainException("Student already has a supervisor");
        SupervisorId = professorId;
    }
    
    public ThesisUpdate SubmitUpdate(string content, DateTime submittedAt)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Update content cannot be empty");
        if (!SupervisorId.HasValue)
            throw new DomainException("Student must have a supervisor before submitting update");
        
        return new ThesisUpdate
        {
            StudentId = this.Id,
            Content = content,
            SubmittedAt = submittedAt,
            Status = UpdateStatus.Submitted
        };
    }
}

public class Feedback
{
    public int Id { get; private set; }
    public int UpdateId { get; private set; }
    public int ProfessorId { get; private set; }
    public string Comment { get; private set; }
    public DateTime SubmittedAt { get; private set; }
    
    public Feedback(int updateId, int professorId, string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
            throw new DomainException("Feedback comment cannot be empty");
        
        UpdateId = updateId;
        ProfessorId = professorId;
        Comment = comment;
        SubmittedAt = DateTime.UtcNow;
    }
}
```

**Status:** [NEW] — Implement Week 1; design domain model.

---

## 3. Deferred Decisions

These are architectural choices left open for Phase 2 or later:

- **Caching & Performance:** Distributed cache (Redis), query optimization, database indexing strategy.
- **Event Sourcing & Audit:** Domain events persisted; audit log reconstruction.
- **API Layer:** REST API for Phase 2 mobile/partner integrations.
- **Email & Notifications:** SMTP/SendGrid integration, queued delivery, real-time SignalR updates.
- **File Storage:** Azure Blob Storage or S3 for attachments, virus scanning, thumbnail generation.
- **Search & Indexing:** Elasticsearch for full-text search over thesis updates.
- **Multi-Tenancy:** Tenant isolation, shared infrastructure vs. separate databases.
- **Analytics & Reporting:** BI tools, dashboard generation, KPI tracking.
- **Security Hardening:** Rate limiting, DDoS mitigation, OAuth/SAML federation, RBAC hierarchy.

---

## 4. Dependency Graph

```
University.Web (Presentation)
  ↓ references
University.Application (Orchestration)
  ↓ references
University.Domain (Business Rules)
  ↑ referenced by
University.Infrastructure (Persistence)
```

**Bidirectional Rule Violation Prevention:**
- ❌ Domain may NOT reference Application or Web.
- ❌ Web may NOT reference Infrastructure directly.
- ✅ Application references Domain (via repository interfaces).
- ✅ Infrastructure references Domain (implements repositories).

---

## 5. Technology Versions & Constraints

| Component | Version | Notes |
|-----------|---------|-------|
| .NET | 8.0+ | Target LTS; deployed with 8.0 SDK |
| Entity Framework Core | 8.0.10 | Installed; Code-First migrations |
| MediatR | 11.1.0 | Installed; CQRS dispatch |
| FluentValidation | 11.9.0 | Installed; command validation |
| Blazor | .NET 8.0 | Server-side rendering; interactivity on components |
| ASP.NET Core Identity | 8.0 | Built-in; no custom implementation |
| SQL Server | 2022 (Docker) | Container: mcr.microsoft.com/mssql/server:2022-latest |
| AutoMapper | Latest | NuGet; DTO mapping |
| xUnit | Latest | Test framework; EF InMemory provider |

---

## 6. Invariants Summary

**The spine's minimal load-bearing calls:**

1. **Clean Architecture Layers:** Domain (rules), Application (orchestration), Infrastructure (persistence), Presentation (UI) — no cross-layer shortcuts.
2. **CQRS via MediatR:** Every operation is a Command (write) or Query (read); routed via handlers.
3. **Persistence via EF Core:** DbContext is source of truth; migrations under version control.
4. **Auth via ASP.NET Core Identity:** Persistent user/role storage; session-based (30-min timeout).
5. **Validation via FluentValidation:** Input validation in handlers; domain rule validation in aggregates.
6. **Error Handling via Result<T>:** Consistent error response shape; no bare exceptions to UI.
7. **Testing via xUnit + InMemory:** 70%+ coverage on domain + application; E2E deferred.
8. **Deployment via Docker Compose:** Single `docker-compose up` spins up DB + app; migrations auto-run.

---

## 7. Graduation Demo Walkthrough

**Sequence:**
1. `docker-compose up` → containers start, migrations apply, seed data loaded.
2. Open browser: `http://localhost:5000/login`
3. Login as Professor → Dashboard shows unassigned students → Assign 1 student to self.
4. Logout → Login as that Student → Dashboard shows supervisor name → Submit thesis update.
5. Logout → Login as Professor → View student's update → Submit feedback.
6. Logout → Login as Student → View supervisor's feedback.
7. **Verify persistence:** Stop containers (`docker-compose down`), restart (`docker-compose up`), login again → all data intact.

**Time to Readiness:** < 5 minutes from `up` to demo-ready.

---

## Status

- **Spine:** Final
- **Updated:** 2026-06-26
- **Next Action:** Break into epics & stories (bmad-create-epics-and-stories), then begin Week 1 implementation.

