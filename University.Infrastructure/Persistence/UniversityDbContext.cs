using Microsoft.EntityFrameworkCore;
using University.Domain.Aggregates.Theses;
using University.Domain.Notifications;

namespace University.Infrastructure.Persistence;

public sealed class UniversityDbContext : DbContext
{
    public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
        : base(options)
    {
    }

    public DbSet<ThesisProject> ThesisProjects => Set<ThesisProject>();

    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();

    public DbSet<NotificationRecord> NotificationRecords => Set<NotificationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UniversityDbContext).Assembly);

        modelBuilder.Entity<NotificationPreference>(builder =>
        {
            builder.HasKey(preference => preference.Id);

            builder.HasIndex(preference => preference.UserId)
                .IsUnique();

            builder.Property(preference => preference.DeliveryMode)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(preference => preference.UpdatedOn)
                .HasPrecision(0);
        });

        modelBuilder.Entity<NotificationRecord>(builder =>
        {
            builder.HasKey(record => record.Id);

            builder.HasIndex(record => record.UserId);

            builder.Property(record => record.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(record => record.Message)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(record => record.Reference)
                .HasMaxLength(512);

            builder.Property(record => record.CreatedOn)
                .HasPrecision(0);

            builder.Property(record => record.ReadOn)
                .HasPrecision(0);
        });

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

                updatesBuilder.OwnsMany(update => update.Comments, commentsBuilder =>
                {
                    commentsBuilder.WithOwner().HasForeignKey("ThesisUpdateId");
                    commentsBuilder.HasKey(comment => comment.Id);
                    commentsBuilder.Property(comment => comment.Id).ValueGeneratedNever();

                    commentsBuilder.Property(comment => comment.Content)
                        .HasMaxLength(4000)
                        .IsRequired();

                    commentsBuilder.Property(comment => comment.CreatedOn)
                        .HasPrecision(0);

                    commentsBuilder.Property(comment => comment.LastEditedOn)
                        .HasPrecision(0);
                });

                updatesBuilder.OwnsMany(update => update.AuditTrail, auditBuilder =>
                {
                    auditBuilder.WithOwner().HasForeignKey("ThesisUpdateId");
                    auditBuilder.HasKey(entry => entry.Id);
                    auditBuilder.Property(entry => entry.Id).ValueGeneratedNever();

                    auditBuilder.Property(entry => entry.Action)
                        .HasMaxLength(200)
                        .IsRequired();

                    auditBuilder.Property(entry => entry.Details)
                        .HasMaxLength(2000);

                    auditBuilder.Property(entry => entry.FromStatus)
                        .HasMaxLength(50);

                    auditBuilder.Property(entry => entry.ToStatus)
                        .HasMaxLength(50);

                    auditBuilder.Property(entry => entry.OccurredOn)
                        .HasPrecision(0);
                });
            });
        });
    }
}
