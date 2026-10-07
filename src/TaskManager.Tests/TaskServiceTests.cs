using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TaskManager.Application.Services;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Events;
using TaskManager.Core.Interfaces;
using Xunit;

namespace TaskManager.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock;
    private readonly Mock<IEventAggregator> _eventAggregatorMock;
    private readonly Mock<IActivityService> _activityServiceMock;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _taskRepoMock = new Mock<ITaskRepository>();
        _eventAggregatorMock = new Mock<IEventAggregator>();
        _activityServiceMock = new Mock<IActivityService>();
        _taskService = new TaskService(_taskRepoMock.Object, _eventAggregatorMock.Object, _activityServiceMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedAtAndPublishEvent()
    {
        // Arrange
        var task = new TaskItem
        {
            Title = "Test FlowTask implementation",
            Status = TaskItemStatus.Todo,
            Priority = TaskPriority.High
        };

        _taskRepoMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>(), default))
            .ReturnsAsync((TaskItem t, System.Threading.CancellationToken ct) => t);

        // Act
        var result = await _taskService.CreateAsync(task);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Test FlowTask implementation");
        _eventAggregatorMock.Verify(e => e.Publish(It.IsAny<TaskCreatedEvent>()), Times.Once);
        _activityServiceMock.Verify(a => a.LogActivityAsync(
            ActivityType.TaskCreated,
            It.IsAny<string>(),
            It.IsAny<Guid?>(),
            It.IsAny<Guid?>(),
            null,
            null), Times.Once);
    }

    [Fact]
    public async Task CompleteAsync_ShouldSetCompletedAtAndStatusDone()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Finish milestone",
            Status = TaskItemStatus.InProgress
        };

        _taskRepoMock.Setup(r => r.GetByIdAsync(taskId, default))
            .ReturnsAsync(task);
        _taskRepoMock.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>(), default))
            .Returns(Task.CompletedTask);

        // Act
        await _taskService.CompleteAsync(taskId);

        // Assert
        task.Status.Should().Be(TaskItemStatus.Done);
        task.CompletedAt.Should().NotBeNull();
        _eventAggregatorMock.Verify(e => e.Publish(It.Is<TaskCompletedEvent>(ev => ev.TaskId == taskId)), Times.Once);
    }

    [Fact]
    public async Task MoveToStatusAsync_ShouldUpdateStatusAndPublishEvent()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Move to Review",
            Status = TaskItemStatus.InProgress
        };

        _taskRepoMock.Setup(r => r.GetByIdAsync(taskId, default))
            .ReturnsAsync(task);

        // Act
        await _taskService.MoveToStatusAsync(taskId, TaskItemStatus.Review);

        // Assert
        task.Status.Should().Be(TaskItemStatus.Review);
        _eventAggregatorMock.Verify(e => e.Publish(It.Is<StatusChangedEvent>(ev => ev.TaskId == taskId && ev.NewStatus == TaskItemStatus.Review)), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingTasks()
    {
        // Arrange
        var matching = new List<TaskItem>
        {
            new TaskItem { Title = "Implement Kanban Board", Status = TaskItemStatus.InProgress }
        };

        _taskRepoMock.Setup(r => r.SearchAsync("Kanban", default))
            .ReturnsAsync(matching);

        // Act
        var results = await _taskService.SearchAsync("Kanban");

        // Assert
        results.Should().HaveCount(1);
        results.First().Title.Should().Contain("Kanban");
    }
}
