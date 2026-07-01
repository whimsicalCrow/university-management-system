---
title: "STORY-1.1: Design and Create EF Core Domain Model"
epic: "Epic 1: Authentication & Data Foundation"
sprint: "Week 1 (2026-06-27 to 2026-07-03)"
priority: "P0 - Critical Path"
status: "ready-for-dev"
date_created: 2026-06-26
acceptance_criteria_met: false
---

# STORY-1.1: Design and Create EF Core Domain Model

## Story Overview

**As a developer,**  
**I want to define all domain entities (Student, Professor, Assignment, ThesisUpdate, Feedback) with proper relationships,**  
**So that the database schema is ready for Code-First migrations.**

---

## Acceptance Criteria

### AC1: Student Entity Structure
**Given** a clean University.Domain project,  
**When** I create a Student entity class,  
**Then** it has the following properties:
- `Id` (int, primary key)
- `UserId` (string, FK to AspNetUser)
- `Specialization` (string)
- `EnrollmentDate` (DateTime)
- Navigation property: `Supervisor` (Professor, nullable)
- Navigation property: `ThesisUpdates` (ICollection<ThesisUpdate>)
**And** the entity includes domain methods:
- `void AssignSupervisor(int professorId)` — validates supervisor not already assigned
- `ThesisUpdate SubmitUpdate(string content, DateTime submittedAt)` — creates update, validates content
**And** domain methods throw `DomainException` on invalid state

### AC2: Professor Entity Structure
**Given** the Student entity from AC1,  
**When** I create a Professor entity class,  
**Then** it has the following properties:
- `Id` (int, primary key)
- `UserId` (string, FK to AspNetUser)
- `Department` (string)
- `Expertise` (string)
- Navigation property: `AssignedStudents` (ICollection<Assignment>)
- Navigation property: `FeedbackProvided` (ICollection<Feedback>)

### AC3: Assignment Entity Structure
**Given** Student and Professor entities,  
**When** I create an Assignment entity class,  
**Then** it has the following properties:
- `Id` (int, primary key)
- `StudentId` (int, FK to Student)
- `ProfessorId` (int, FK to Professor)
- `AssignedDate` (DateTime)
- Navigation properties: `Student` (Student), `Professor` (Professor)
**And** foreign key constraints cascade on delete (Student/Professor deletion removes assignments)

### AC4: ThesisUpdate Entity Structure
**Given** Student entity,  
**When** I create a ThesisUpdate entity class,  
**Then** it has the following properties:
- `Id` (int, primary key)
- `StudentId` (int, FK to Student)
- `Content` (string, max 10000 chars)
- `SubmittedAt` (DateTime)
- `Status` (enum: Draft, Submitted, Reviewed)
- Navigation property: `Feedback` (ICollection<Feedback>)
- Navigation property: `Student` (Student)

### AC5: Feedback Entity Structure
**Given** ThesisUpdate and Professor entities,  
**When** I create a Feedback entity class,  
**Then** it has the following properties:
- `Id` (int, primary key)
- `UpdateId` (int, FK to ThesisUpdate)
- `ProfessorId` (int, FK to Professor)
- `Comment` (string, max 5000 chars)
- `SubmittedAt` (DateTime)
- Navigation properties: `Update` (ThesisUpdate), `Professor` (Professor)

### AC6: Domain Exception Handling
**Given** all entity classes,  
**When** a domain rule is violated (e.g., empty update content, professor already assigned),  
**Then** entity methods throw `DomainException` with descriptive message
**And** `DomainException` is created in University.Domain.Exceptions namespace

### AC7: Value Objects & Enums
**Given** the domain model,  
**When** I define enums,  
**Then** `UpdateStatus` enum exists with values: Draft, Submitted, Reviewed
**And** all entities use proper value object patterns where applicable

### AC8: Unit Tests for Domain Model
**Given** xUnit configured in University.UnitTests,  
**When** I create tests for domain entities,  
**Then** tests cover:
- Student.AssignSupervisor() validates supervisor not already assigned
- Student.SubmitUpdate() validates content is not empty
- ThesisUpdate() validates StudentId is valid
- Feedback() validates comment is not empty
**And** all tests pass and run in < 2 seconds
**And** domain layer has ≥70% line coverage

---

## Technical Context

### Architecture Decision References
- **AD-1:** Clean Architecture Layers — Domain owns business rules, persistence-agnostic
- **AD-6:** Domain-Driven Design — Student, Professor, Update entities encapsulate state
- **AD-9:** Layered Folder Structure — University.Domain contains entities only, no persistence logic

### Key Constraints
- 🚫 **NO** EF Core-specific attributes (DbSet, etc.) in domain entities — keep persistence-agnostic
- 🚫 **NO** navigation properties with circular references
- ✅ **DO** use private setters on entity properties (immutable after creation)
- ✅ **DO** include validation logic in entity methods (not just data holders)

### Related Stories
- **Blocks:** Story 1.2 (Database Migrations), Story 2.1 (Student Queries), Story 3.1 (Professor Queries)
- **Depends On:** None (first story in epic)

---

## Files to Create/Modify

### Create (University.Domain/)

```
University.Domain/
├── Entities/
│   ├── Student.cs
│   ├── Professor.cs
│   ├── Assignment.cs
│   ├── ThesisUpdate.cs
│   ├── Feedback.cs
│   └── BaseEntity.cs (optional, for common Id)
├── Exceptions/
│   └── DomainException.cs
└── Enums/
    └── UpdateStatus.cs
```

### Create (University.UnitTests/)

```
University.UnitTests/
├── Domain/
│   ├── StudentTests.cs
│   ├── ProfessorTests.cs
│   ├── AssignmentTests.cs
│   ├── ThesisUpdateTests.cs
│   └── FeedbackTests.cs
```

---

## Implementation Notes

### Entity Design Pattern

Each entity should follow this pattern:

```csharp
public class Student
{
    // Private setters for immutability
    public int Id { get; private set; }
    public string UserId { get; private set; }
    
    // Domain methods (public, with validation)
    public void AssignSupervisor(int professorId)
    {
        if (SupervisorId.HasValue)
            throw new DomainException("Student already has a supervisor");
        SupervisorId = professorId;
    }
    
    // Factory method for creation
    public static Student Create(string userId, string specialization)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("UserId cannot be empty");
        
        return new Student 
        { 
            UserId = userId, 
            Specialization = specialization,
            EnrollmentDate = DateTime.UtcNow
        };
    }
}
```

### Test Design Pattern

```csharp
[Fact]
public void AssignSupervisor_WhenNoExistingAssignment_Succeeds()
{
    // Arrange
    var student = Student.Create("user123", "AI");
    
    // Act
    student.AssignSupervisor(1);
    
    // Assert
    Assert.Equal(1, student.SupervisorId);
}

[Fact]
public void AssignSupervisor_WhenAlreadyAssigned_ThrowsException()
{
    // Arrange
    var student = Student.Create("user123", "AI");
    student.AssignSupervisor(1);
    
    // Act & Assert
    var ex = Assert.Throws<DomainException>(() => student.AssignSupervisor(2));
    Assert.Contains("already has a supervisor", ex.Message);
}
```

---

## Success Checklist

- [ ] All 5 entity classes created (Student, Professor, Assignment, ThesisUpdate, Feedback)
- [ ] `DomainException` created in Exceptions folder
- [ ] `UpdateStatus` enum created
- [ ] All navigation properties defined (FK relationships)
- [ ] Domain methods implemented with validation
- [ ] All unit tests created and passing
- [ ] Domain layer coverage ≥ 70%
- [ ] No EF Core dependencies in University.Domain
- [ ] Code compiles without warnings
- [ ] All acceptance criteria verified

---

## Development Time Estimate

**Estimated:** 2–3 hours (including tests)  
**Timeline:** Should be completed by 2026-06-28 (Day 2 of Week 1)

---

## Notes for Development Log

When you complete this story, update [DEVELOPMENT-LOG.md](../../../DEVELOPMENT-LOG.md) with:

- **Date Completed:** [date]
- **Time Spent:** [hours]
- **Key Decisions:** Any entity design choices, naming conventions, validation patterns
- **Blockers Encountered:** Any issues with patterns or architecture decisions
- **Tests Passing:** Confirmation of test count + coverage percentage

---

## Definition of Done

✅ Story complete when:
1. All 5 entities implemented with proper validation
2. All 15+ unit tests passing
3. Domain layer coverage ≥ 70%
4. Code review passed (no EF Core dependencies in Domain)
5. DEVELOPMENT-LOG.md updated
6. Story marked as `status: completed`

