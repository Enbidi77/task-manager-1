using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Constants;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Infrastructure.Database;
using TaskManager.Infrastructure.Repositories;
using Xunit;

namespace TaskManager.Tests;

public class DatabaseIntegrationTests : IDisposable
{
    private readonly string _testDbPath;
    private readonly FlowTaskDbContext _context;

    public DatabaseIntegrationTests()
    {
        _testDbPath = Path.Combine(Path.GetTempPath(), $"flowtask_test_{Guid.NewGuid():N}.db");
        var options = new DbContextOptionsBuilder<FlowTaskDbContext>()
            .UseSqlite($"Data Source={_testDbPath}")
            .Options;

        _context = new FlowTaskDbContext(options);
    }

    [Fact]
    public async Task DatabaseInitializer_ShouldSeedAllEntitiesSuccessfully()
    {
        var initializer = new DatabaseInitializer(_context);
        await initializer.InitializeAsync();

        // Verify Workspaces seeded
        var workspaces = await _context.Workspaces.ToListAsync();
        workspaces.Should().HaveCountGreaterThanOrEqualTo(2);
        workspaces.Should().Contain(w => w.Name == "Development");
        workspaces.Should().Contain(w => w.Name == "Personal");

        // Verify Projects seeded
        var projects = await _context.Projects.ToListAsync();
        projects.Should().HaveCountGreaterThanOrEqualTo(5);
        projects.Should().Contain(p => p.Name == "Gallery Web App");
        projects.Should().Contain(p => p.Name == "FlowTask Mobile App");

        // Verify Tags seeded
        var tags = await _context.Tags.ToListAsync();
        tags.Should().HaveCountGreaterThanOrEqualTo(10);
        tags.Should().Contain(t => t.Name == "backend");
        tags.Should().Contain(t => t.Name == "urgent");

        // Verify Tasks seeded
        var tasks = await _context.TaskItems.ToListAsync();
        tasks.Should().HaveCountGreaterThanOrEqualTo(40);
        tasks.Should().Contain(t => t.Status == TaskItemStatus.Done);
        tasks.Should().Contain(t => t.Status == TaskItemStatus.InProgress);
        tasks.Should().Contain(t => t.Status == TaskItemStatus.Todo);

        // Verify Dashboard Widgets seeded
        var widgets = await _context.DashboardWidgets.ToListAsync();
        widgets.Should().HaveCountGreaterThanOrEqualTo(4);

        // Verify Time Entries & Focus Sessions seeded
        var timeEntries = await _context.TimeEntries.ToListAsync();
        timeEntries.Should().NotBeEmpty();

        var focusSessions = await _context.FocusSessions.ToListAsync();
        focusSessions.Should().NotBeEmpty();
    }

    [Fact]
    public async Task TaskItem_CRUD_Operations_ShouldSucceedWithTimestamps()
    {
        await _context.Database.EnsureCreatedAsync();

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Integration Test Task",
            Description = "Verifying EF Core SQLite persistence",
            Status = TaskItemStatus.Todo,
            Priority = TaskPriority.High,
            DueDate = DateTimeOffset.UtcNow.AddDays(1)
        };

        await _context.TaskItems.AddAsync(task);
        await _context.SaveChangesAsync();

        var fetched = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == task.Id);
        fetched.Should().NotBeNull();
        fetched!.Title.Should().Be("Integration Test Task");
        fetched.Status.Should().Be(TaskItemStatus.Todo);
        fetched.IsCompleted.Should().BeFalse();

        // Complete the task
        fetched.IsCompleted = true;
        await _context.SaveChangesAsync();

        var completed = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == task.Id);
        completed!.Status.Should().Be(TaskItemStatus.Done);
        completed.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DateFilteringQueries_ShouldExecuteWithoutTranslationError()
    {
        var initializer = new DatabaseInitializer(_context);
        await initializer.InitializeAsync();

        var repo = new TaskRepository(_context);
        var todayTasks = await repo.GetTodayTasksAsync();
        todayTasks.Should().NotBeNull();

        var upcomingTasks = await repo.GetUpcomingTasksAsync();
        upcomingTasks.Should().NotBeNull();

        var overdueTasks = await repo.GetOverdueTasksAsync();
        overdueTasks.Should().NotBeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
        try
        {
            if (File.Exists(_testDbPath))
            {
                File.Delete(_testDbPath);
            }
        }
        catch
        {
            // Ignore cleanup lock issues on temp db
        }
    }
}
