namespace University.Infrastructure.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using University.Domain.Entities;

/// <summary>
/// Entity Framework Core DbContext for the Thesis Collaboration Portal.
/// Manages persistence for domain entities (Student, Professor, Assignment, ThesisUpdate, Feedback)
/// and includes ASP.NET Core Identity tables for user and role management.
/// </summary>
public class UniversityDbContext : IdentityDbContext<IdentityUser>
{
    /// <summary>
    /// Initializes a new instance of the UniversityDbContext.
    /// </summary>
    /// <param name="options">DbContext configuration options provided by dependency injection.</param>
    public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the collection of students in the system.
    /// </summary>
    public DbSet<Student> Students { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of professors in the system.
    /// </summary>
    public DbSet<Professor> Professors { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of assignments (student-professor relationships).
    /// </summary>
    public DbSet<Assignment> Assignments { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of thesis updates.
    /// </summary>
    public DbSet<ThesisUpdate> ThesisUpdates { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of feedback on thesis updates.
    /// </summary>
    public DbSet<Feedback> Feedback { get; set; } = null!;

    /// <summary>
    /// Gets or sets the persisted thesis topic assignments.
    /// </summary>
    public DbSet<ThesisTopicAssignment> ThesisTopicAssignments { get; set; } = null!;

    /// <summary>
    /// Gets or sets persisted thesis artifact binaries.
    /// </summary>
    public DbSet<ThesisArtifact> ThesisArtifacts { get; set; } = null!;

    /// <summary>
    /// Configures the model relationships and constraints using Fluent API.
    /// Ensures proper foreign key mappings, cascade delete rules, and index configuration.
    /// </summary>
    /// <param name="modelBuilder">The ModelBuilder instance.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Student entity relationships
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Supervisor)
            .WithMany()
            .HasForeignKey(s => s.SupervisorId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Student>()
            .HasMany(s => s.ThesisUpdates)
            .WithOne(tu => tu.Student)
            .HasForeignKey(tu => tu.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Professor entity relationships
        modelBuilder.Entity<Professor>()
            .HasMany(p => p.AssignedStudents)
            .WithOne(a => a.Professor)
            .HasForeignKey(a => a.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Professor>()
            .HasMany(p => p.FeedbackProvided)
            .WithOne(f => f.Professor)
            .HasForeignKey(f => f.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Assignment entity relationships
        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Student)
            .WithMany()
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure ThesisUpdate entity relationships
        modelBuilder.Entity<ThesisUpdate>()
            .HasMany(tu => tu.Feedback)
            .WithOne(f => f.Update)
            .HasForeignKey(f => f.UpdateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ThesisTopicAssignment>()
            .HasOne(t => t.Student)
            .WithMany()
            .HasForeignKey(t => t.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ThesisTopicAssignment>()
            .HasOne(t => t.Professor)
            .WithMany()
            .HasForeignKey(t => t.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ThesisArtifact>()
            .HasOne(a => a.Student)
            .WithMany()
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Feedback entity relationships (all configured above)

        // Configure indexes for performance
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.UserId)
            .IsUnique();

        modelBuilder.Entity<Professor>()
            .HasIndex(p => p.UserId)
            .IsUnique();

        modelBuilder.Entity<Assignment>()
            .HasIndex(a => new { a.StudentId, a.ProfessorId })
            .IsUnique();

        modelBuilder.Entity<ThesisUpdate>()
            .HasIndex(tu => tu.StudentId);

        modelBuilder.Entity<Feedback>()
            .HasIndex(f => f.UpdateId);

        modelBuilder.Entity<Feedback>()
            .HasIndex(f => f.ProfessorId);

        modelBuilder.Entity<ThesisTopicAssignment>()
            .HasIndex(t => t.TopicId)
            .IsUnique();

        modelBuilder.Entity<ThesisTopicAssignment>()
            .HasIndex(t => t.StudentId);

        modelBuilder.Entity<ThesisTopicAssignment>()
            .HasIndex(t => t.ProfessorId);

        modelBuilder.Entity<ThesisArtifact>()
            .HasIndex(a => a.ArtifactId)
            .IsUnique();

        modelBuilder.Entity<ThesisArtifact>()
            .HasIndex(a => a.StudentId);

        modelBuilder.Entity<ThesisArtifact>()
            .HasIndex(a => a.ThesisId);

        // Seed Identity Roles
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "Professor", NormalizedName = "PROFESSOR" },
            new IdentityRole { Id = "2", Name = "Student", NormalizedName = "STUDENT" }
        );

        // Seed Identity Users (Professors) and Students
        var professorUsers = new[]
        {
            new IdentityUser { Id = "prof-1", UserName = "prof1@univ.edu", Email = "prof1@univ.edu", EmailConfirmed = true, NormalizedUserName = "PROF1@UNIV.EDU", NormalizedEmail = "PROF1@UNIV.EDU", SecurityStamp = "prof-1-stamp", ConcurrencyStamp = "prof-1-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "prof-2", UserName = "prof2@univ.edu", Email = "prof2@univ.edu", EmailConfirmed = true, NormalizedUserName = "PROF2@UNIV.EDU", NormalizedEmail = "PROF2@UNIV.EDU", SecurityStamp = "prof-2-stamp", ConcurrencyStamp = "prof-2-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "prof-3", UserName = "prof3@univ.edu", Email = "prof3@univ.edu", EmailConfirmed = true, NormalizedUserName = "PROF3@UNIV.EDU", NormalizedEmail = "PROF3@UNIV.EDU", SecurityStamp = "prof-3-stamp", ConcurrencyStamp = "prof-3-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "prof-4", UserName = "prof4@univ.edu", Email = "prof4@univ.edu", EmailConfirmed = true, NormalizedUserName = "PROF4@UNIV.EDU", NormalizedEmail = "PROF4@UNIV.EDU", SecurityStamp = "prof-4-stamp", ConcurrencyStamp = "prof-4-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "prof-5", UserName = "prof5@univ.edu", Email = "prof5@univ.edu", EmailConfirmed = true, NormalizedUserName = "PROF5@UNIV.EDU", NormalizedEmail = "PROF5@UNIV.EDU", SecurityStamp = "prof-5-stamp", ConcurrencyStamp = "prof-5-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" }
        };

        var studentUsers = new[]
        {
            new IdentityUser { Id = "student-1", UserName = "student1@univ.edu", Email = "student1@univ.edu", EmailConfirmed = true, NormalizedUserName = "STUDENT1@UNIV.EDU", NormalizedEmail = "STUDENT1@UNIV.EDU", SecurityStamp = "student-1-stamp", ConcurrencyStamp = "student-1-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "student-2", UserName = "student2@univ.edu", Email = "student2@univ.edu", EmailConfirmed = true, NormalizedUserName = "STUDENT2@UNIV.EDU", NormalizedEmail = "STUDENT2@UNIV.EDU", SecurityStamp = "student-2-stamp", ConcurrencyStamp = "student-2-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "student-3", UserName = "student3@univ.edu", Email = "student3@univ.edu", EmailConfirmed = true, NormalizedUserName = "STUDENT3@UNIV.EDU", NormalizedEmail = "STUDENT3@UNIV.EDU", SecurityStamp = "student-3-stamp", ConcurrencyStamp = "student-3-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "student-4", UserName = "student4@univ.edu", Email = "student4@univ.edu", EmailConfirmed = true, NormalizedUserName = "STUDENT4@UNIV.EDU", NormalizedEmail = "STUDENT4@UNIV.EDU", SecurityStamp = "student-4-stamp", ConcurrencyStamp = "student-4-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "student-5", UserName = "student5@univ.edu", Email = "student5@univ.edu", EmailConfirmed = true, NormalizedUserName = "STUDENT5@UNIV.EDU", NormalizedEmail = "STUDENT5@UNIV.EDU", SecurityStamp = "student-5-stamp", ConcurrencyStamp = "student-5-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "student-6", UserName = "student6@univ.edu", Email = "student6@univ.edu", EmailConfirmed = true, NormalizedUserName = "STUDENT6@UNIV.EDU", NormalizedEmail = "STUDENT6@UNIV.EDU", SecurityStamp = "student-6-stamp", ConcurrencyStamp = "student-6-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "student-7", UserName = "student7@univ.edu", Email = "student7@univ.edu", EmailConfirmed = true, NormalizedUserName = "STUDENT7@UNIV.EDU", NormalizedEmail = "STUDENT7@UNIV.EDU", SecurityStamp = "student-7-stamp", ConcurrencyStamp = "student-7-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "student-8", UserName = "student8@univ.edu", Email = "student8@univ.edu", EmailConfirmed = true, NormalizedUserName = "STUDENT8@UNIV.EDU", NormalizedEmail = "STUDENT8@UNIV.EDU", SecurityStamp = "student-8-stamp", ConcurrencyStamp = "student-8-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "student-9", UserName = "student9@univ.edu", Email = "student9@univ.edu", EmailConfirmed = true, NormalizedUserName = "STUDENT9@UNIV.EDU", NormalizedEmail = "STUDENT9@UNIV.EDU", SecurityStamp = "student-9-stamp", ConcurrencyStamp = "student-9-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "student-10", UserName = "student10@univ.edu", Email = "student10@univ.edu", EmailConfirmed = true, NormalizedUserName = "STUDENT10@UNIV.EDU", NormalizedEmail = "STUDENT10@UNIV.EDU", SecurityStamp = "student-10-stamp", ConcurrencyStamp = "student-10-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "student-11", UserName = "student11@univ.edu", Email = "student11@univ.edu", EmailConfirmed = true, NormalizedUserName = "STUDENT11@UNIV.EDU", NormalizedEmail = "STUDENT11@UNIV.EDU", SecurityStamp = "student-11-stamp", ConcurrencyStamp = "student-11-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "student-12", UserName = "student12@univ.edu", Email = "student12@univ.edu", EmailConfirmed = true, NormalizedUserName = "STUDENT12@UNIV.EDU", NormalizedEmail = "STUDENT12@UNIV.EDU", SecurityStamp = "student-12-stamp", ConcurrencyStamp = "student-12-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "student-13", UserName = "student13@univ.edu", Email = "student13@univ.edu", EmailConfirmed = true, NormalizedUserName = "STUDENT13@UNIV.EDU", NormalizedEmail = "STUDENT13@UNIV.EDU", SecurityStamp = "student-13-stamp", ConcurrencyStamp = "student-13-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "student-14", UserName = "student14@univ.edu", Email = "student14@univ.edu", EmailConfirmed = true, NormalizedUserName = "STUDENT14@UNIV.EDU", NormalizedEmail = "STUDENT14@UNIV.EDU", SecurityStamp = "student-14-stamp", ConcurrencyStamp = "student-14-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" },
            new IdentityUser { Id = "student-15", UserName = "student15@univ.edu", Email = "student15@univ.edu", EmailConfirmed = true, NormalizedUserName = "STUDENT15@UNIV.EDU", NormalizedEmail = "STUDENT15@UNIV.EDU", SecurityStamp = "student-15-stamp", ConcurrencyStamp = "student-15-concurrency", PasswordHash = "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=" }
        };

        var allUsers = professorUsers.Concat(studentUsers).ToArray();
        modelBuilder.Entity<IdentityUser>().HasData(allUsers);

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

        // Seed Domain Professors (using new object initializer syntax for read-only properties)
        modelBuilder.Entity<Professor>().HasData(
            new { Id = 1, UserId = "prof-1", Name = "Χριστοδούλου", Department = "Computer Science", Expertise = "Machine Learning" },
            new { Id = 2, UserId = "prof-2", Name = "Πετρέλλης",   Department = "Mathematics",      Expertise = "Algebra" },
            new { Id = 3, UserId = "prof-3", Name = "Χαραλαμπάκος", Department = "Physics",          Expertise = "Quantum" },
            new { Id = 4, UserId = "prof-4", Name = "Κούτρας",     Department = "Chemistry",        Expertise = "Organic" },
            new { Id = 5, UserId = "prof-5", Name = "Τζήμας",      Department = "Biology",          Expertise = "Genetics" }
        );

        // Seed Domain Students
        var seedDate = new DateTime(2026, 6, 26, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Student>().HasData(
            new { Id = 1, UserId = "student-1", Specialization = "AI", EnrollmentDate = seedDate.AddMonths(-11), SupervisorId = 1 },
            new { Id = 2, UserId = "student-2", Specialization = "Theoretical", EnrollmentDate = seedDate.AddMonths(-10), SupervisorId = 2 },
            new { Id = 3, UserId = "student-3", Specialization = "Nuclear", EnrollmentDate = seedDate.AddMonths(-9), SupervisorId = 3 },
            new { Id = 4, UserId = "student-4", Specialization = "Synthetic", EnrollmentDate = seedDate.AddMonths(-8), SupervisorId = 4 },
            new { Id = 5, UserId = "student-5", Specialization = "Molecular", EnrollmentDate = seedDate.AddMonths(-7), SupervisorId = 5 },
            new { Id = 6, UserId = "student-6", Specialization = "AI", EnrollmentDate = seedDate.AddMonths(-6), SupervisorId = 1 },
            new { Id = 7, UserId = "student-7", Specialization = "Theoretical", EnrollmentDate = seedDate.AddMonths(-5), SupervisorId = 2 },
            new { Id = 8, UserId = "student-8", Specialization = "Nuclear", EnrollmentDate = seedDate.AddMonths(-4), SupervisorId = 3 },
            new { Id = 9, UserId = "student-9", Specialization = "Synthetic", EnrollmentDate = seedDate.AddMonths(-3), SupervisorId = 4 },
            new { Id = 10, UserId = "student-10", Specialization = "Molecular", EnrollmentDate = seedDate.AddMonths(-2), SupervisorId = 5 },
            new { Id = 11, UserId = "student-11", Specialization = "AI", EnrollmentDate = seedDate.AddMonths(-1), SupervisorId = 1 },
            new { Id = 12, UserId = "student-12", Specialization = "Theoretical", EnrollmentDate = seedDate, SupervisorId = 2 },
            new { Id = 13, UserId = "student-13", Specialization = "Nuclear", EnrollmentDate = seedDate, SupervisorId = 3 },
            new { Id = 14, UserId = "student-14", Specialization = "Synthetic", EnrollmentDate = seedDate, SupervisorId = 4 },
            new { Id = 15, UserId = "student-15", Specialization = "Molecular", EnrollmentDate = seedDate, SupervisorId = 5 }
        );

        // Seed Assignments
        modelBuilder.Entity<Assignment>().HasData(
            new { Id = 1, StudentId = 1, ProfessorId = 1, AssignedDate = seedDate },
            new { Id = 2, StudentId = 2, ProfessorId = 2, AssignedDate = seedDate },
            new { Id = 3, StudentId = 3, ProfessorId = 3, AssignedDate = seedDate }
        );
    }
}
