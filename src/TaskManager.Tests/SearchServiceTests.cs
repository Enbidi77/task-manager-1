using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TaskManager.Application.Services;
using TaskManager.Core.DTOs;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;
using Xunit;

namespace TaskManager.Tests;

public class SearchServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock;
    private readonly Mock<IProjectRepository> _projectRepoMock;
    private readonly Mock<ITagRepository> _tagRepoMock;
    private readonly SearchService _searchService;

    public SearchServiceTests()
    {
        _taskRepoMock = new Mock<ITaskRepository>();
        _projectRepoMock = new Mock<IProjectRepository>();
        _tagRepoMock = new Mock<ITagRepository>();
        _searchService = new SearchService(
            _taskRepoMock.Object,
            _projectRepoMock.Object,
            _tagRepoMock.Object);
    }

    [Fact]
    public async Task SearchAsync_WithQuery_ShouldReturnMatchingTaskResults()
    {
        var taskList = new List<TaskItem>
        {
            new TaskItem { Id = Guid.NewGuid(), Title = "Fix database bug", Description = "SQLite issue", Status = TaskItemStatus.Todo },
            new TaskItem { Id = Guid.NewGuid(), Title = "Implement search", Description = "Search service", Status = TaskItemStatus.Done }
        };

        _taskRepoMock.Setup(r => r.SearchAsync("bug", default))
            .ReturnsAsync(taskList.Where(t => t.Title.Contains("bug")).ToList());

        var query = new SearchQuery { Text = "bug", IncludeTasks = true };
        var results = await _searchService.SearchAsync(query);

        results.Should().NotBeNull();
        results.Should().HaveCount(1);
        results.First().Title.Should().Be("Fix database bug");
        results.First().Type.Should().Be("Task");
    }

    [Fact]
    public async Task SearchAsync_WithEmptyQuery_ShouldReturnEmptyList()
    {
        var query = new SearchQuery { Text = "   " };
        var results = await _searchService.SearchAsync(query);

        results.Should().BeEmpty();
    }
}
