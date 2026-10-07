using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TaskManager.Application.Services;
using TaskManager.Core.Entities;
using TaskManager.Core.Events;
using TaskManager.Core.Interfaces;
using Xunit;

namespace TaskManager.Tests;

public class TimeTrackingServiceTests
{
    private readonly Mock<ITimeEntryRepository> _timeEntryRepoMock;
    private readonly Mock<ITaskRepository> _taskRepoMock;
    private readonly Mock<IEventAggregator> _eventAggregatorMock;
    private readonly TimeTrackingService _timeTrackingService;

    public TimeTrackingServiceTests()
    {
        _timeEntryRepoMock = new Mock<ITimeEntryRepository>();
        _taskRepoMock = new Mock<ITaskRepository>();
        _eventAggregatorMock = new Mock<IEventAggregator>();
        _timeTrackingService = new TimeTrackingService(
            _timeEntryRepoMock.Object,
            _taskRepoMock.Object,
            _eventAggregatorMock.Object);
    }

    [Fact]
    public async Task StartTimerAsync_ShouldCreateActiveEntryAndPublishEvent()
    {
        var taskId = Guid.NewGuid();
        _timeEntryRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<TimeEntry, bool>>>(), default))
            .ReturnsAsync(new List<TimeEntry>());

        _timeEntryRepoMock.Setup(r => r.AddAsync(It.IsAny<TimeEntry>(), default))
            .ReturnsAsync((TimeEntry e, System.Threading.CancellationToken ct) => e);

        var entry = await _timeTrackingService.StartTimerAsync(taskId);

        entry.Should().NotBeNull();
        entry.TaskId.Should().Be(taskId);
        entry.EndTime.Should().BeNull();
        _eventAggregatorMock.Verify(e => e.Publish(It.Is<TimerStartedEvent>(ev => ev.TaskId == taskId)), Times.Once);
    }

    [Fact]
    public async Task StopTimerAsync_ShouldCalculateDurationAndPublishEvent()
    {
        var taskId = Guid.NewGuid();
        var active = new TimeEntry
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            StartTime = DateTimeOffset.UtcNow.AddMinutes(-30)
        };

        _timeEntryRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<TimeEntry, bool>>>(), default))
            .ReturnsAsync(new List<TimeEntry> { active });

        _taskRepoMock.Setup(r => r.GetByIdAsync(taskId, default))
            .ReturnsAsync(new TaskItem { Id = taskId });

        var stopped = await _timeTrackingService.StopTimerAsync(taskId);

        stopped.Should().NotBeNull();
        stopped!.EndTime.Should().NotBeNull();
        stopped.Duration.Should().BeCloseTo(TimeSpan.FromMinutes(30), TimeSpan.FromSeconds(5));
        _eventAggregatorMock.Verify(e => e.Publish(It.Is<TimerStoppedEvent>(ev => ev.TaskId == taskId)), Times.Once);
    }
}
