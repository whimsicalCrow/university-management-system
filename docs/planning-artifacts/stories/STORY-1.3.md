# Story 1.3: Integrate ASP.NET Core Identity

**Epic:** Epic 1 — Authentication & Data Foundation  
**Sprint:** Week 1 (2026-06-27 to 2026-07-03)  
**Priority:** Critical (blocks all feature work)  
**Estimate:** 2–3 hours  
**Acceptance Criteria Count:** 6 (AC1–AC6)

---

## User Story

**As a** system architect  
**I want to** integrate ASP.NET Core Identity for user authentication and authorization  
**So that** login/logout flows are secure, persistent, and role-aware (Student vs. Professor)

---

## Acceptance Criteria

### AC1: Identity Services Configured in Program.cs
**Given** the web app is starting up  
**When** Program.cs runs  
**Then** ASP.NET Core Identity is registered with:
- `AddIdentityApiEndpoints<IdentityUser>()`
- `AddEntityFrameworkStores<UniversityDbContext>()`
- `AddDefaultTokenProviders()`
- Authentication middleware (`AddAuthentication()`)
- Authorization middleware (`app.UseAuthentication(); app.UseAuthorization();`)

### AC2: Seed Data Created (Professors + Students)
**Given** the application runs for the first time after migration  
**When** `dotnet ef database update` completes  
**Then** the UniversityDbContext seed method populates:
- 5 professor users (prof1@univ.edu through prof5@univ.edu, password: "TempPass123!")
- 15 student users (student1@univ.edu through student15@univ.edu, password: "TempPass123!")
- 3 assignment records linking students to professors
- All users created with correct roles (Student / Professor)

### AC3: Login Command Handler Created
**Given** a user submits credentials via Login.razor  
**When** `LoginCommand` is dispatched to MediatR  
**Then** the handler:
- Validates email/password against Identity user store
- Returns success with user identity (email, role) or error
- Sets secure session cookie (default: 30 min timeout)
- Logs the login event (log level: Information)

### AC4: Login.razor Updated (UI → Handler Flow)
**Given** a user is on the Login page  
**When** they select a role and click "Login"  
**Then** instead of calling `UserSessionService.SetSession()`:
- Collect email + password from form
- Dispatch `LoginCommand` via MediatR
- On success: redirect to role-appropriate dashboard (Student → StudentDashboard, Professor → ProfessorDashboard)
- On failure: display error message ("Invalid email or password")

### AC5: Logout Implemented
**Given** a user is authenticated  
**When** they click "Logout" in NavMenu  
**Then**:
- Session cookie is cleared
- User is redirected to Login page
- All in-memory session state is disposed

### AC6: Authentication Guard on Routes
**Given** an unauthenticated user  
**When** they attempt to access `/student-dashboard`, `/professor-students`, or other protected pages  
**Then** they are redirected to `/login` with a return URL parameter so they can be redirected after login

---

## Technical Context

### Current State (Story 1.2 Complete)
- ✅ Domain entities created (Student, Professor, Assignment, ThesisUpdate, Feedback)
- ✅ UniversityDbContext configured with all relationships
- ✅ InitialCreate migration generated (9 tables including AspNetUsers, AspNetRoles, etc.)
- ✅ Program.cs has DbContext registered
- ❌ Identity services NOT yet configured
- ❌ Seed data NOT yet created
- ❌ Login handlers NOT yet implemented
- ❌ UI still uses mock UserSessionService

### Architecture Constraints
1. **CQRS Pattern:** All login logic flows through MediatR handlers
2. **Clean Architecture:** Login command handlers in University.Application layer
3. **Role-Based Access Control (RBAC):** Two roles in MVP: "Student", "Professor"
4. **Claim-Based Authorization:** Use ASP.NET Core's [Authorize(Roles = "Student")] attributes
5. **No Custom Role Store:** Use IdentityRole directly (no custom role entity)
6. **Seed Data:** Use OnModelCreating() + HasData() in EF Core (not separate seed method)

### Dependencies
- Story 1.2 ✅ (database schema complete)
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (10.0.0) — already in Infrastructure.csproj
- MediatR (v11.1.0) — already in Application.csproj

### Blocking
- Story 1.4 (DI configuration for all layers) waits for this

---

## Implementation Guide

### Step 1: Update Program.cs — Identity Configuration
**File:** `University.Web/Program.cs`

Add after `AddDbContext<UniversityDbContext>`:
```csharp
// Add ASP.NET Core Identity
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<UniversityDbContext>()
    .AddDefaultTokenProviders();

// Add Authentication & Authorization
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("StudentOnly", policy => policy.RequireRole("Student"))
    .AddPolicy("ProfessorOnly", policy => policy.RequireRole("Professor"));
```

Add before `app.Run()`:
```csharp
app.UseAuthentication();
app.UseAuthorization();
```

### Step 2: Seed Data via EF Core HasData()
**File:** `University.Infrastructure/Data/UniversityDbContext.cs`

In `OnModelCreating()` method, add at the end:

```csharp
// Seed Identity Users (Professors)
var professorUsers = new List<IdentityUser>();
for (int i = 1; i <= 5; i++)
{
    var prof = new IdentityUser
    {
        Id = Guid.NewGuid().ToString(),
        UserName = $"prof{i}@univ.edu",
        Email = $"prof{i}@univ.edu",
        EmailConfirmed = true,
        NormalizedUserName = $"PROF{i}@UNIV.EDU",
        NormalizedEmail = $"PROF{i}@UNIV.EDU"
    };
    professorUsers.Add(prof);
}

// Seed Identity Users (Students)
var studentUsers = new List<IdentityUser>();
for (int i = 1; i <= 15; i++)
{
    var student = new IdentityUser
    {
        Id = Guid.NewGuid().ToString(),
        UserName = $"student{i}@univ.edu",
        Email = $"student{i}@univ.edu",
        EmailConfirmed = true,
        NormalizedUserName = $"STUDENT{i}@UNIV.EDU",
        NormalizedEmail = $"STUDENT{i}@UNIV.EDU"
    };
    studentUsers.Add(student);
}

modelBuilder.Entity<IdentityUser>().HasData(professorUsers.Concat(studentUsers));

// Seed Roles
modelBuilder.Entity<IdentityRole>().HasData(
    new IdentityRole { Id = "1", Name = "Professor", NormalizedName = "PROFESSOR" },
    new IdentityRole { Id = "2", Name = "Student", NormalizedName = "STUDENT" }
);

// Seed User-Role Mappings
var userRoles = new List<IdentityUserRole<string>>();
foreach (var prof in professorUsers)
{
    userRoles.Add(new IdentityUserRole<string> { UserId = prof.Id, RoleId = "1" });
}
foreach (var student in studentUsers)
{
    userRoles.Add(new IdentityUserRole<string> { UserId = student.Id, RoleId = "2" });
}
modelBuilder.Entity<IdentityUserRole<string>>().HasData(userRoles);

// Seed Domain Professors
var professors = new List<Professor>();
for (int i = 0; i < professorUsers.Count; i++)
{
    professors.Add(new Professor
    {
        Id = i + 1,
        UserId = professorUsers[i].Id,
        Department = new[] { "Computer Science", "Mathematics", "Physics", "Chemistry", "Biology" }[i],
        Expertise = new[] { "Machine Learning", "Algebra", "Quantum", "Organic", "Genetics" }[i]
    });
}
modelBuilder.Entity<Professor>().HasData(professors);

// Seed Domain Students
var students = new List<Student>();
for (int i = 0; i < studentUsers.Count; i++)
{
    students.Add(new Student
    {
        Id = i + 1,
        UserId = studentUsers[i].Id,
        Specialization = new[] { "AI", "Theoretical", "Nuclear", "Synthetic", "Molecular" }[i % 5],
        EnrollmentDate = DateTime.UtcNow.AddMonths(-i % 12),
        SupervisorId = (i % 5) + 1  // Assign first 5 students to prof1, next 5 to prof2, etc.
    });
}
modelBuilder.Entity<Student>().HasData(students);

// Seed Assignments (3 assignments: student 1–3 with professor 1–3)
modelBuilder.Entity<Assignment>().HasData(
    new Assignment { Id = 1, StudentId = 1, ProfessorId = 1, AssignedDate = DateTime.UtcNow },
    new Assignment { Id = 2, StudentId = 2, ProfessorId = 2, AssignedDate = DateTime.UtcNow },
    new Assignment { Id = 3, StudentId = 3, ProfessorId = 3, AssignedDate = DateTime.UtcNow }
);
```

**Note:** This requires a new migration after updating OnModelCreating(). Run:
```bash
dotnet ef migrations add AddIdentityAndSeedData --project University.Infrastructure --startup-project University.Web
```

### Step 3: Create Login Command Handler
**File:** `University.Application/Commands/LoginCommand.cs`

```csharp
namespace University.Application.Commands;

using MediatR;

public class LoginCommand : IRequest<LoginResult>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }  // "Student" or "Professor"
}

public class LoginResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? UserId { get; set; }
    public string? Role { get; set; }
}
```

**File:** `University.Application/Commands/LoginCommandHandler.cs`

```csharp
namespace University.Application.Commands;

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning("Login attempt failed: user not found ({Email})", request.Email);
            return new LoginResult { Success = false, Message = "Invalid email or password" };
        }

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent: true, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Login attempt failed: invalid password ({Email})", request.Email);
            return new LoginResult { Success = false, Message = "Invalid email or password" };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Unknown";

        _logger.LogInformation("User logged in: {Email}, Role: {Role}", user.Email, role);
        return new LoginResult { Success = true, UserId = user.Id, Role = role };
    }
}
```

### Step 4: Update Login.razor
**File:** `University.Web/Components/Pages/Login.razor`

Replace the current login submission logic:

```razor
@code {
    private string selectedRole = "Student";
    private string email = "";
    private string password = "";
    private string errorMessage = "";
    
    [Inject] IMediator Mediator { get; set; }
    [Inject] NavigationManager Navigation { get; set; }

    private async Task HandleLogin()
    {
        errorMessage = "";
        
        var command = new LoginCommand { Email = email, Password = password, Role = selectedRole };
        var result = await Mediator.Send(command);
        
        if (result.Success)
        {
            var redirectUrl = selectedRole == "Student" ? "/student-dashboard" : "/professor-students";
            Navigation.NavigateTo(redirectUrl, forceLoad: true);
        }
        else
        {
            errorMessage = result.Message ?? "Login failed";
        }
    }
}
```

### Step 5: Add Logout Handler
**File:** `University.Web/Components/NavMenu.razor`

Add logout button and handler:

```razor
@code {
    private async Task HandleLogout()
    {
        await SignInManager.SignOutAsync();
        Navigation.NavigateTo("/login", forceLoad: true);
    }
}
```

### Step 6: Protect Routes
Add `@attribute [Authorize]` to top of protected Razor components:

```razor
@attribute [Authorize(Roles = "Student")]
```

---

## Test Scenarios

### Scenario 1: Valid Student Login
1. Navigate to `/login`
2. Email: `student1@univ.edu`, Password: `TempPass123!`, Role: `Student`
3. Expected: Redirect to `/student-dashboard`, authenticated session active
4. Verify: User claim visible in app state

### Scenario 2: Invalid Password
1. Navigate to `/login`
2. Email: `student1@univ.edu`, Password: `WrongPassword`, Role: `Student`
3. Expected: Error message: "Invalid email or password"
4. Verify: User NOT authenticated; session cookie NOT set

### Scenario 3: Invalid Email
1. Navigate to `/login`
2. Email: `nobody@example.com`, Password: `TempPass123!`, Role: `Student`
3. Expected: Error message: "Invalid email or password"

### Scenario 4: Logout
1. Login as student1
2. Click "Logout"
3. Expected: Redirected to `/login`
4. Verify: Session cookie cleared

### Scenario 5: Unauthorized Access
1. Do NOT log in
2. Navigate to `/student-dashboard`
3. Expected: Redirect to `/login?returnUrl=%2Fstudent-dashboard`

### Scenario 6: Professor Login
1. Navigate to `/login`
2. Email: `prof1@univ.edu`, Password: `TempPass123!`, Role: `Professor`
3. Expected: Redirect to `/professor-students` (role-specific dashboard)

---

## Success Checklist

- [ ] AC1: Identity services configured in Program.cs (AddIdentityApiEndpoints, AddEntityFrameworkStores, AddAuthentication, AddAuthorizationBuilder)
- [ ] AC2: Seed data populates 5 professors + 15 students + roles + assignments after `dotnet ef database update`
- [ ] AC3: LoginCommand handler validates credentials, signs in user, returns role
- [ ] AC4: Login.razor calls MediatR handler instead of UserSessionService; displays errors
- [ ] AC5: Logout clears session, redirects to login
- [ ] AC6: Protected routes redirect unauthenticated users to login
- [ ] All 6 test scenarios pass
- [ ] No compilation errors or warnings (except NuGet vulnerability warnings)
- [ ] Code follows Clean Architecture (handlers in Application, services injected, no domain logic leakage)

---

## Implementation Steps

1. Update Program.cs with Identity configuration
2. Create seed data in UniversityDbContext.OnModelCreating()
3. Generate and apply new migration (`AddIdentityAndSeedData`)
4. Create LoginCommand and LoginCommandHandler in Application layer
5. Update Login.razor to use MediatR
6. Add logout handler
7. Add [Authorize] attributes to protected components
8. Test all 6 scenarios
9. Commit with message: "Story 1.3: ASP.NET Core Identity integration"

---

## Notes

- Password hash will be handled automatically by UserManager (using bcrypt by default)
- Session timeout: Default 30 minutes; configured via `CookieAuthenticationOptions` if needed
- MediatR should be registered in Program.cs with `.AddMediatR(typeof(Program))`
- Seed data uses `HasData()` instead of separate seed method for reproducibility
- Roles "Student" and "Professor" are case-sensitive in authorization policies
