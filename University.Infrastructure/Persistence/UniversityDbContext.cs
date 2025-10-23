using Microsoft.EntityFrameworkCore;
using University.Domain.Aggregates.Theses;

namespace University.Infrastructure.Persistence;

public sealed class UniversityDbContext : DbContext
{
    public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
        : base(options)
    {
    }

    public DbSet<ThesisProject> ThesisProjects => Set<ThesisProject>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UniversityDbContext).Assembly);

        modelBuilder.Entity<ThesisProject>(builder =>
        {
            builder.HasKey(thesis => thesis.Id);

            builder.Property(thesis => thesis.Title)
                .HasMaxLength(200);

            builder.Property(thesis => thesis.Summary)
                .HasMaxLength(2000);

            builder.Property(thesis => thesis.Status)
                .HasMaxLength(100);

            builder.OwnsMany(thesis => thesis.Updates, updatesBuilder =>
            {
                updatesBuilder.WithOwner().HasForeignKey("ThesisProjectId");
                updatesBuilder.HasKey(update => update.Id);
                updatesBuilder.Property(update => update.Id).ValueGeneratedNever();

                updatesBuilder.Property(update => update.Note)
                    .HasMaxLength(4000);

                updatesBuilder.Property(update => update.OccurredOn)
                    .HasPrecision(0);

                updatesBuilder.Property(update => update.Status)
                    .HasMaxLength(50);

                updatesBuilder.Property(update => update.LastModifiedOn)
                    .HasPrecision(0);

                updatesBuilder.OwnsMany(update => update.Attachments, attachmentsBuilder =>
                {
                    attachmentsBuilder.WithOwner().HasForeignKey("ThesisUpdateId");
                    attachmentsBuilder.HasKey(attachment => attachment.Id);
                    attachmentsBuilder.Property(attachment => attachment.Id).ValueGeneratedNever();

                    attachmentsBuilder.Property(attachment => attachment.FileName)
                        .HasMaxLength(255);

                    attachmentsBuilder.Property(attachment => attachment.ContentType)
                        .HasMaxLength(150);

                    attachmentsBuilder.Property(attachment => attachment.BlobName)
                        .HasMaxLength(512);
                });
            });
        });
    }
}
