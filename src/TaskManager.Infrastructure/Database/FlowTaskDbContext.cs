using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;

namespace TaskManager.Infrastructure.Database
{
    public class FlowTaskDbContext : DbContext
    {
        public DbSet<TaskItem> TaskItems { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<Workspace> Workspaces { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<TaskTag> TaskTags { get; set; } = null!;
        public DbSet<TaskDependency> TaskDependencies { get; set; } = null!;
        public DbSet<TimeEntry> TimeEntries { get; set; } = null!;
        public DbSet<FocusSession> FocusSessions { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Activity> Activities { get; set; } = null!;
        public DbSet<SavedFilter> SavedFilters { get; set; } = null!;
        public DbSet<DashboardWidget> DashboardWidgets { get; set; } = null!;
        public DbSet<UserPreference> UserPreferences { get; set; } = null!;

        public FlowTaskDbContext()
        {
        }

        public FlowTaskDbContext(DbContextOptions<FlowTaskDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var appDir = System.IO.Path.Combine(folder, "FlowTask");
                System.IO.Directory.CreateDirectory(appDir);
                var dbPath = System.IO.Path.Combine(appDir, TaskManager.Core.Constants.AppConstants.DatabaseName);
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                var dateTimeOffsetConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTimeOffset, long>(
                    v => v.UtcTicks,
                    v => new DateTimeOffset(v, TimeSpan.Zero));

                var nullableDateTimeOffsetConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTimeOffset?, long?>(
                    v => v.HasValue ? v.Value.UtcTicks : null,
                    v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);

                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var properties = entityType.ClrType.GetProperties()
                        .Where(p => p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?));

                    foreach (var property in properties)
                    {
                        if (property.PropertyType == typeof(DateTimeOffset))
                        {
                            modelBuilder.Entity(entityType.Name).Property(property.Name)
                                .HasConversion(dateTimeOffsetConverter);
                        }
                        else if (property.PropertyType == typeof(DateTimeOffset?))
                        {
                            modelBuilder.Entity(entityType.Name).Property(property.Name)
                                .HasConversion(nullableDateTimeOffsetConverter);
                        }
                    }
                }
            }



            // TaskItem
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
                
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.DueDate);
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.WorkspaceId);
                entity.HasIndex(e => e.ParentTaskId);
                entity.HasIndex(e => e.IsArchived);
                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(e => e.Project)
                      .WithMany()
                      .HasForeignKey(e => e.ProjectId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Workspace)
                      .WithMany()
                      .HasForeignKey(e => e.WorkspaceId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ParentTask)
                      .WithMany(e => e.SubTasks)
                      .HasForeignKey(e => e.ParentTaskId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Project
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                
                entity.HasIndex(e => e.WorkspaceId);
                entity.HasIndex(e => e.IsArchived);

                entity.HasOne(e => e.Workspace)
                      .WithMany()
                      .HasForeignKey(e => e.WorkspaceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Workspace
            modelBuilder.Entity<Workspace>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            });

            // Tag
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // TaskTag
            modelBuilder.Entity<TaskTag>(entity =>
            {
                entity.HasKey(e => new { e.TaskId, e.TagId });

                entity.HasOne(e => e.Task)
                      .WithMany(e => e.Tags)
                      .HasForeignKey(e => e.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Tag)
                      .WithMany()
                      .HasForeignKey(e => e.TagId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TaskDependency
            modelBuilder.Entity<TaskDependency>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.TaskId);
                entity.HasIndex(e => e.DependsOnTaskId);

                entity.HasOne(e => e.Task)
                      .WithMany(e => e.Dependencies)
                      .HasForeignKey(e => e.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.DependsOnTask)
                      .WithMany()
                      .HasForeignKey(e => e.DependsOnTaskId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // TimeEntry
            modelBuilder.Entity<TimeEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.TaskId);
                entity.HasIndex(e => e.StartTime);

                entity.HasOne(e => e.Task)
                      .WithMany(e => e.TimeEntries)
                      .HasForeignKey(e => e.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // FocusSession
            modelBuilder.Entity<FocusSession>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.TaskId);
                entity.HasIndex(e => e.StartTime);

                entity.HasOne(e => e.Task)
                      .WithMany()
                      .HasForeignKey(e => e.TaskId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.IsRead);
                entity.HasIndex(e => e.CreatedAt);
            });

            // Activity
            modelBuilder.Entity<Activity>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.TaskId);
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.CreatedAt);
            });

            // SavedFilter
            modelBuilder.Entity<SavedFilter>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            });

            // DashboardWidget
            modelBuilder.Entity<DashboardWidget>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.WidgetType);
            });

            // UserPreference
            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Key).IsRequired().HasMaxLength(200);
                
                entity.HasIndex(e => e.Key).IsUnique();
            });
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            var now = DateTimeOffset.UtcNow;

            foreach (var entry in entries)
            {
                var createdAtProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedAt");
                var updatedAtProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");

                if (entry.State == EntityState.Added && createdAtProperty != null)
                {
                    createdAtProperty.CurrentValue = now;
                }

                if (updatedAtProperty != null)
                {
                    updatedAtProperty.CurrentValue = now;
                }
            }
        }
    }
}
