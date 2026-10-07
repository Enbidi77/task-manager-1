using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TaskManager.Application.Services;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;
using Xunit;

namespace TaskManager.Tests;

public class AnalyticsServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock;
    private readonly Mock<ITimeEntryRepository> _timeEntryRepoMock;
    private readonly Mock<IFocusSessionRepository> _focusSessionRepoMock;
    private readonly Mock<IProjectRepository> _projectRepoMock;
    private readonly AnalyticsService _analyticsService;

    public AnalyticsServiceTests()
    {
        _taskRepoMock = new Mock<ITaskRepository>();
        _timeEntryRepoMock = new Mock<ITimeEntryRepository>();
        _focusSessionRepoMock = new Mock<IFocusSessionRepository>();
        _projectRepoMock = new Mock<IProjectRepository>();
        _analyticsService = new AnalyticsService(
            _taskRepoMock.Object,
            _timeEntryRepoMock.Object,
            _focusSessionRepoMock.Object,
            _projectRepoMock.Object);
    }

    [Fact]
    public async Task GetDashboardStatsAsync_ShouldReturnValidCalculations()
    {
        var tasks = new List<TaskItem>
        {
            new TaskItem { Id = Guid.NewGuid(), Status = TaskItemStatus.Done },
            new TaskItem { Id = Guid.NewGuid(), Status = TaskItemStatus.InProgress },
            new TaskItem { Id = Guid.NewGuid(), Status = TaskItemStatus.Todo },
            new TaskItem { Id = Guid.NewGuid(), Status = TaskItemStatus.Done }
        };

        _taskRepoMock.Setup(r => r.GetTodayTasksAsync(default))
            .ReturnsAsync(tasks);

        var sessions = new List<FocusSession>
        {
            new FocusSession
            {
                StartTime = DateTimeOffset.UtcNow,
                PlannedDuration = TimeSpan.FromMinutes(25),
                ActualDuration = TimeSpan.FromMinutes(25),
                IsCompleted = true
            }
        };

        _focusSessionRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<FocusSession, bool>>>(), default))
            .ReturnsAsync(sessions);

        var stats = await _analyticsService.GetDashboardStatsAsync();

        stats.Should().NotBeNull();
        stats.TodayTasksCount.Should().Be(4);
        stats.CompletedTodayCount.Should().Be(2);
        stats.CompletionPercentage.Should().Be(50.0);
        stats.TotalFocusTimeToday.Should().Be(TimeSpan.FromMinutes(25));
    }
}
