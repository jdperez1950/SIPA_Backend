using Microsoft.EntityFrameworkCore;
using Pavis.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata;
using Pavis.Domain.ValueObjects;

namespace Pavis.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTeamMember> ProjectTeamMembers { get; set; }
    public DbSet<QuestionDefinition> Questions { get; set; }
    public DbSet<ProjectResponse> ProjectResponses { get; set; }
    public DbSet<Axis> Axis { get; set; }

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

/*
        // Normalize table and column names to lowercase to match lowercase SQL scripts/identifiers
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Set table name to lowercase
            var tableName = entityType.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
            {
                entityType.SetTableName(tableName.ToLowerInvariant());
            }

            // Set column names to lowercase
            var storeObject = StoreObjectIdentifier.Table(entityType.GetTableName(), entityType.GetSchema());
            foreach (var property in entityType.GetProperties())
            {
                try
                {
                    var columnName = property.GetColumnName(storeObject);
                    if (!string.IsNullOrEmpty(columnName))
                    {
                        property.SetColumnName(columnName.ToLowerInvariant());
                    }
                }
                catch
                {
                    // Ignore properties that cannot resolve a column name for the given store object
                }
            }

            // Set key, foreign key and index names to lowercase where present
            foreach (var key in entityType.GetKeys())
            {
                var name = key.GetName();
                if (!string.IsNullOrEmpty(name)) key.SetName(name.ToLowerInvariant());
            }

            foreach (var fk in entityType.GetForeignKeys())
            {
                var name = fk.GetConstraintName();
                if (!string.IsNullOrEmpty(name)) fk.SetConstraintName(name.ToLowerInvariant());
            }

            foreach (var index in entityType.GetIndexes())
            {
                var name = index.GetDatabaseName();
                if (!string.IsNullOrEmpty(name)) index.SetDatabaseName(name.ToLowerInvariant());
            }
        }
*/
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

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasIndex(e => e.DocumentNumber).IsUnique();
            entity.Property(e => e.FullName).IsRequired();
            entity.HasOne<User>()
                .WithOne()
                .HasForeignKey<UserProfile>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProjectTeamMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ProjectId, e.UserId }).IsUnique();
            entity.Property(e => e.RoleInProject).IsRequired();
            entity.HasOne<Project>()
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Axis>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(30);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).IsRequired(false);
            entity.Property(e => e.DeletedAt).IsRequired(false);
        });
    }
}
