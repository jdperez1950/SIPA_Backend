using Microsoft.EntityFrameworkCore;
using Pavis.Domain.Entities;
using Pavis.Domain.ValueObjects;

namespace Pavis.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<QuestionDefinition> Questions { get; set; }
    public DbSet<ProjectResponse> ProjectResponses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Identifier).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Identifier).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.AdvisorId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.OwnsOne(e => e.Progress, progress =>
            {
                progress.ToJson();
            });
        });

        modelBuilder.Entity<QuestionDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Key).IsUnique();
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Text).IsRequired();

            entity.OwnsOne(e => e.EvidenceConfig, config =>
            {
                config.ToJson();
            });

            entity.Ignore(e => e.Options);
            entity.Ignore(e => e.Dependencies);
        });

        modelBuilder.Entity<ProjectResponse>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<Project>()
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.Value).HasColumnType("jsonb");
            entity.OwnsOne(e => e.Evidence, evidence =>
            {
                evidence.ToJson();
            });
        });
    }
}
