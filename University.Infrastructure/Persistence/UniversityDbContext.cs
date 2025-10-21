using Microsoft.EntityFrameworkCore;
using University.Domain.Entities;

namespace University.Infrastructure.Persistence;

public class UniversityDbContext : DbContext
{
    public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Professor> Professors => Set<Professor>();
    public DbSet<ThesisProject> ThesisProjects => Set<ThesisProject>();
    public DbSet<ThesisUpdate> ThesisUpdates => Set<ThesisUpdate>();
    public DbSet<Meeting> Meetings => Set<Meeting>();
    public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(s => s.Email).IsUnique();

            entity.HasOne<Professor>()
                .WithMany()
                .HasForeignKey(s => s.SupervisorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ThesisProject>(entity =>
        {
            entity.HasOne<Student>()
                .WithMany()
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Professor>()
                .WithMany()
                .HasForeignKey(p => p.ProfessorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.Updates)
                .WithOne()
                .HasForeignKey(u => u.ThesisProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.Meetings)
                .WithOne()
                .HasForeignKey(m => m.ThesisProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ThesisUpdate>(entity =>
        {
            entity.Property(u => u.Notes).HasMaxLength(6000);
            entity.Property(u => u.AuthorRole).HasMaxLength(50);
        });

        modelBuilder.Entity<Meeting>(entity =>
        {
            entity.Property(m => m.Location).HasMaxLength(200);
            entity.Property(m => m.Agenda).HasMaxLength(2000);
        });
    }
}
