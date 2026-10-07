using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;

namespace TaskManager.Infrastructure.Database;

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly FlowTaskDbContext _context;

    public DatabaseInitializer(FlowTaskDbContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync()
    {
        // Ensures the database is created
        await _context.Database.EnsureCreatedAsync();

        if (!_context.Workspaces.Any())
        {
            var now = DateTimeOffset.UtcNow;

            // 1. Workspaces
            var wsDev = new Workspace { Id = Guid.NewGuid(), Name = "Development", Description = "Software Engineering & Architecture", Icon = "code", Color = "#0078D4", CreatedAt = now, UpdatedAt = now };
            var wsPersonal = new Workspace { Id = Guid.NewGuid(), Name = "Personal", Description = "Life, Health & Hobbies", Icon = "home", Color = "#107C10", CreatedAt = now, UpdatedAt = now };
            _context.Workspaces.AddRange(wsDev, wsPersonal);

            // 2. Projects
            var pGallery = new Project { Id = Guid.NewGuid(), WorkspaceId = wsDev.Id, Name = "Gallery Web App", Description = "Next-gen creative showcase platform", Color = "#0078D4", CreatedAt = now, UpdatedAt = now };
            var pMobile = new Project { Id = Guid.NewGuid(), WorkspaceId = wsDev.Id, Name = "FlowTask Mobile App", Description = "Cross-platform mobile companion", Color = "#8E56FF", CreatedAt = now, UpdatedAt = now };
            var pBackend = new Project { Id = Guid.NewGuid(), WorkspaceId = wsDev.Id, Name = "Cloud API Core", Description = "High-throughput microservices", Color = "#008272", CreatedAt = now, UpdatedAt = now };
            var pBlog = new Project { Id = Guid.NewGuid(), WorkspaceId = wsPersonal.Id, Name = "Personal Tech Blog", Description = "Technical writing and tutorials", Color = "#FF8C00", CreatedAt = now, UpdatedAt = now };
            var pHome = new Project { Id = Guid.NewGuid(), WorkspaceId = wsPersonal.Id, Name = "Home Renovation", Description = "Smart workspace upgrade", Color = "#D13438", CreatedAt = now, UpdatedAt = now };
            
            var projectsList = new[] { pGallery, pMobile, pBackend, pBlog, pHome };
            _context.Projects.AddRange(projectsList);

            // 3. Tags
            var tags = new[]
            {
                new Tag { Id = Guid.NewGuid(), Name = "backend", Color = "#0078D4", CreatedAt = now },
                new Tag { Id = Guid.NewGuid(), Name = "frontend", Color = "#008272", CreatedAt = now },
                new Tag { Id = Guid.NewGuid(), Name = "architecture", Color = "#8E56FF", CreatedAt = now },
                new Tag { Id = Guid.NewGuid(), Name = "bug", Color = "#D13438", CreatedAt = now },
                new Tag { Id = Guid.NewGuid(), Name = "feature", Color = "#107C10", CreatedAt = now },
                new Tag { Id = Guid.NewGuid(), Name = "urgent", Color = "#C50F1F", CreatedAt = now },
                new Tag { Id = Guid.NewGuid(), Name = "documentation", Color = "#797775", CreatedAt = now },
                new Tag { Id = Guid.NewGuid(), Name = "devops", Color = "#FF8C00", CreatedAt = now },
                new Tag { Id = Guid.NewGuid(), Name = "personal", Color = "#00B7C3", CreatedAt = now },
                new Tag { Id = Guid.NewGuid(), Name = "health", Color = "#498205", CreatedAt = now }
            };
            _context.Tags.AddRange(tags);

            // 4. User Preferences
            var prefs = new[]
            {
                new UserPreference { Id = Guid.NewGuid(), Key = "Theme", Value = "System", UpdatedAt = now },
                new UserPreference { Id = Guid.NewGuid(), Key = "PomodoroDuration", Value = "25", UpdatedAt = now },
                new UserPreference { Id = Guid.NewGuid(), Key = "ShortBreakDuration", Value = "5", UpdatedAt = now },
                new UserPreference { Id = Guid.NewGuid(), Key = "LongBreakDuration", Value = "15", UpdatedAt = now },
                new UserPreference { Id = Guid.NewGuid(), Key = "AccentColor", Value = "#0078D4", UpdatedAt = now },
            };
            _context.UserPreferences.AddRange(prefs);

            // 5. Dashboard Widgets
            var widgets = new[]
            {
                new DashboardWidget { Id = Guid.NewGuid(), Title = "Today's Tasks", WidgetType = "TodaysTasks", SortOrder = 1, IsVisible = true, Width = 1, Height = 1 },
                new DashboardWidget { Id = Guid.NewGuid(), Title = "Completed Today", WidgetType = "Completed", SortOrder = 2, IsVisible = true, Width = 1, Height = 1 },
                new DashboardWidget { Id = Guid.NewGuid(), Title = "Focus Time", WidgetType = "FocusTime", SortOrder = 3, IsVisible = true, Width = 1, Height = 1 },
                new DashboardWidget { Id = Guid.NewGuid(), Title = "Weekly Progress", WidgetType = "Progress", SortOrder = 4, IsVisible = true, Width = 2, Height = 1 },
                new DashboardWidget { Id = Guid.NewGuid(), Title = "Productivity Trends", WidgetType = "ProductivityChart", SortOrder = 5, IsVisible = true, Width = 2, Height = 2 },
                new DashboardWidget { Id = Guid.NewGuid(), Title = "Workload Monitor", WidgetType = "Workload", SortOrder = 6, IsVisible = true, Width = 1, Height = 1 },
                new DashboardWidget { Id = Guid.NewGuid(), Title = "Recent Activity", WidgetType = "RecentActivity", SortOrder = 7, IsVisible = true, Width = 1, Height = 2 },
                new DashboardWidget { Id = Guid.NewGuid(), Title = "Upcoming Deadlines", WidgetType = "UpcomingDeadlines", SortOrder = 8, IsVisible = true, Width = 1, Height = 1 }
            };
            _context.DashboardWidgets.AddRange(widgets);

            // 6. Saved Filters
            var filters = new[]
            {
                new SavedFilter { Id = Guid.NewGuid(), Name = "Due Today", FilterJson = "{\"DueDate\": \"Today\"}", Icon = "📅", SortOrder = 1, CreatedAt = now },
                new SavedFilter { Id = Guid.NewGuid(), Name = "High Priority", FilterJson = "{\"Priority\": \"High\"}", Icon = "🔥", SortOrder = 2, CreatedAt = now },
                new SavedFilter { Id = Guid.NewGuid(), Name = "Overdue", FilterJson = "{\"IsOverdue\": true}", Icon = "⚠️", SortOrder = 3, CreatedAt = now },
                new SavedFilter { Id = Guid.NewGuid(), Name = "My Favorites", FilterJson = "{\"IsFavorite\": true}", Icon = "⭐", SortOrder = 4, CreatedAt = now },
                new SavedFilter { Id = Guid.NewGuid(), Name = "Recently Added", FilterJson = "{\"CreatedRecently\": true}", Icon = "🕒", SortOrder = 5, CreatedAt = now }
            };
            _context.SavedFilters.AddRange(filters);

            // 7. Tasks
            var tasks = new List<TaskItem>();
            var random = new Random(42);
            
            var taskNames = new[]
            {
                "Implement JWT authentication with refresh tokens",
                "Design dark mode Fluent UI surfaces",
                "Fix SQLite database connection pooling",
                "Write comprehensive API documentation with Swagger",
                "Set up GitHub Actions CI/CD build matrix",
                "Review pull request #342 for code quality",
                "Update dependencies to .NET 10 latest",
                "Configure distributed Redis caching layer",
                "Optimize memory allocation in Gantt rendering",
                "Configure Pomodoro focus timer audio cues",
                "Paint modern home office accent wall",
                "Research ergonomics for sit-stand motorized desk",
                "Plan hiking trip itinerary and gear checklist",
                "Write architectural overview for domain events",
                "Benchmark virtualization with 50,000 tasks"
            };

            for (int i = 0; i < 45; i++)
            {
                var proj = projectsList[random.Next(projectsList.Length)];
                var dueDate = now.AddDays(random.Next(-5, 14));
                var status = (TaskItemStatus)random.Next(0, 6);
                
                var t = new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = taskNames[i % taskNames.Length] + (i >= taskNames.Length ? $" (Phase {i / taskNames.Length + 1})" : ""),
                    Description = "High priority commercial desktop feature implementation with full unit test coverage and Fluent design system alignment.",
                    ProjectId = proj.Id,
                    WorkspaceId = proj.WorkspaceId,
                    Status = status,
                    Priority = (TaskPriority)random.Next(0, 5),
                    DueDate = dueDate,
                    StartDate = now.AddDays(-random.Next(1, 5)),
                    EstimatedDuration = TimeSpan.FromHours(random.Next(1, 8)),
                    ActualDuration = status == TaskItemStatus.Done ? TimeSpan.FromHours(random.Next(1, 6)) : null,
                    CreatedAt = now.AddDays(-random.Next(1, 30)),
                    UpdatedAt = now,
                    CompletedAt = status == TaskItemStatus.Done ? now.AddDays(-random.Next(0, 3)) : null,
                    IsFavorite = random.Next(0, 4) == 0,
                    SortOrder = i
                };
                tasks.Add(t);
            }
            _context.TaskItems.AddRange(tasks);
            await _context.SaveChangesAsync();
            
            // Task Tags
            var taskTags = new List<TaskTag>();
            foreach (var t in tasks)
            {
                int numTags = random.Next(1, 3);
                var shuffledTags = tags.OrderBy(_ => random.Next()).Take(numTags).ToList();
                foreach (var tag in shuffledTags)
                {
                    taskTags.Add(new TaskTag { TaskId = t.Id, TagId = tag.Id });
                }
            }
            _context.TaskTags.AddRange(taskTags);

            // Time Entries & Focus Sessions
            var timeEntries = new List<TimeEntry>();
            var focusSessions = new List<FocusSession>();
            var activities = new List<Activity>();
            
            for (int i = 0; i < 25; i++)
            {
                var t = tasks[random.Next(tasks.Count)];
                var start = now.AddDays(-random.Next(0, 14)).AddHours(random.Next(8, 16));
                var duration = TimeSpan.FromMinutes(random.Next(15, 95));
                timeEntries.Add(new TimeEntry
                {
                    Id = Guid.NewGuid(),
                    TaskId = t.Id,
                    StartTime = start,
                    EndTime = start + duration,
                    Duration = duration,
                    Notes = "Focused implementation session for " + t.Title,
                    CreatedAt = start
                });
            }
            
            for (int i = 0; i < 15; i++)
            {
                var t = tasks[random.Next(tasks.Count)];
                var start = now.AddDays(-random.Next(0, 7)).AddHours(random.Next(8, 17));
                focusSessions.Add(new FocusSession
                {
                    Id = Guid.NewGuid(),
                    TaskId = t.Id,
                    StartTime = start,
                    EndTime = start.AddMinutes(25),
                    PlannedDuration = TimeSpan.FromMinutes(25),
                    ActualDuration = TimeSpan.FromMinutes(25),
                    SessionNumber = (i % 4) + 1,
                    IsCompleted = true,
                    CreatedAt = start
                });
            }

            for (int i = 0; i < 50; i++)
            {
                var t = tasks[random.Next(tasks.Count)];
                activities.Add(new Activity
                {
                    Id = Guid.NewGuid(),
                    TaskId = t.Id,
                    ProjectId = t.ProjectId,
                    Type = (ActivityType)random.Next(0, 7),
                    Description = $"Updated task status: {t.Title}",
                    CreatedAt = now.AddHours(-i * 3)
                });
            }

            _context.TimeEntries.AddRange(timeEntries);
            _context.FocusSessions.AddRange(focusSessions);
            _context.Activities.AddRange(activities);

            // Notifications
            var notifications = new[]
            {
                new Notification { Id = Guid.NewGuid(), Title = "Upcoming Milestone Due", Message = "JWT Authentication milestone scheduled for today.", Type = NotificationType.Deadline, IsRead = false, CreatedAt = now },
                new Notification { Id = Guid.NewGuid(), Title = "Sprint Review Ready", Message = "45 tasks initialized across 5 active projects.", Type = NotificationType.Reminder, IsRead = true, ReadAt = now, CreatedAt = now.AddDays(-1) }
            };
            _context.Notifications.AddRange(notifications);

            await _context.SaveChangesAsync();
        }
    }
}
