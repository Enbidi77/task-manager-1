using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TaskManager.Application.Services;
using TaskManager.Core.Entities;
using TaskManager.Core.Events;
using TaskManager.Core.Interfaces;
using Xunit;

namespace TaskManager.Tests;

public class FocusServiceTests
{
    private readonly Mock<IFocusSessionRepository> _focusRepoMock;
    private readonly Mock<IEventAggregator> _eventAggregatorMock;
    private readonly FocusService _focusService;

    public FocusServiceTests()
    {
        _focusRepoMock = new Mock<IFocusSessionRepository>();
        _eventAggregatorMock = new Mock<IEventAggregator>();
        _focusService = new FocusService(_focusRepoMock.Object, _eventAggregatorMock.Object);
    }

    [Fact]
    public async Task StartSessionAsync_ShouldCreateSessionWithPlannedDuration()
    {
        var taskId = Guid.NewGuid();
        _focusRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<FocusSession, bool>>>(), default))
            .ReturnsAsync(new List<FocusSession>());
        _focusRepoMock.Setup(r => r.AddAsync(It.IsAny<FocusSession>(), default))
            .ReturnsAsync((FocusSession s, System.Threading.CancellationToken ct) => s);

        var session = await _focusService.StartSessionAsync(taskId, TimeSpan.FromMinutes(25));

        session.Should().NotBeNull();
        session.PlannedDuration.Should().Be(TimeSpan.FromMinutes(25));
        session.TaskId.Should().Be(taskId);
    }

    [Fact]
    public async Task CompleteSessionAsync_ShouldMarkCompletedAndPublishEvent()
    {
        var taskId = Guid.NewGuid();
        var session = new FocusSession
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            StartTime = DateTimeOffset.UtcNow.AddMinutes(-25),
            PlannedDuration = TimeSpan.FromMinutes(25),
            IsCompleted = false
        };

        _focusRepoMock.Setup(r => r.GetByIdAsync(session.Id, default))
            .ReturnsAsync(session);

        await _focusService.CompleteSessionAsync(session.Id);

        session.IsCompleted.Should().BeTrue();
        session.ActualDuration.Should().NotBeNull();
        _eventAggregatorMock.Verify(e => e.Publish(It.Is<FocusSessionCompletedEvent>(ev => ev.SessionId == session.Id)), Times.Once);
    }
}
