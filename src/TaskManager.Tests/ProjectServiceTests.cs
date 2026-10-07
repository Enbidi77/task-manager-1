using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TaskManager.Application.Services;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;
using Xunit;

namespace TaskManager.Tests;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepoMock;
    private readonly Mock<ITaskRepository> _taskRepoMock;
    private readonly ProjectService _projectService;

    public ProjectServiceTests()
    {
        _projectRepoMock = new Mock<IProjectRepository>();
        _taskRepoMock = new Mock<ITaskRepository>();
        _projectService = new ProjectService(_projectRepoMock.Object, _taskRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetTimestampsAndId()
    {
        var project = new Project
        {
            Name = "New Project",
            Description = "Test description"
        };

        _projectRepoMock.Setup(r => r.AddAsync(It.IsAny<Project>(), default))
            .ReturnsAsync((Project p, System.Threading.CancellationToken ct) => p);

        var created = await _projectService.CreateAsync(project);

        created.Should().NotBeNull();
        created.Id.Should().NotBeEmpty();
        created.Name.Should().Be("New Project");
        created.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetProjectStatsAsync_ShouldCalculateCorrectMetrics()
    {
        var projectId = Guid.NewGuid();
        var tasks = new List<TaskItem>
        {
            new TaskItem { Id = Guid.NewGuid(), ProjectId = projectId, Status = TaskItemStatus.Done, ActualDuration = TimeSpan.FromHours(2) },
            new TaskItem { Id = Guid.NewGuid(), ProjectId = projectId, Status = TaskItemStatus.InProgress, ActualDuration = TimeSpan.FromHours(1) },
            new TaskItem { Id = Guid.NewGuid(), ProjectId = projectId, Status = TaskItemStatus.Todo, DueDate = DateTimeOffset.UtcNow.AddDays(-2) } // overdue
        };

        _taskRepoMock.Setup(r => r.GetByProjectAsync(projectId, default))
            .ReturnsAsync(tasks);

        var stats = await _projectService.GetProjectStatsAsync(projectId);

        stats.Should().NotBeNull();
        stats.ProjectId.Should().Be(projectId);
        stats.TotalTasks.Should().Be(3);
        stats.CompletedTasks.Should().Be(1);
        stats.OverdueTasks.Should().Be(1);
        stats.CompletionPercentage.Should().BeApproximately(33.3, 0.1);
    }
}
