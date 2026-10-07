using System;
using FluentAssertions;
using TaskManager.Application.Validators;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using Xunit;

namespace TaskManager.Tests;

public class ValidationTests
{
    private readonly TaskValidator _taskValidator = new();
    private readonly ProjectValidator _projectValidator = new();
    private readonly TimeEntryValidator _timeEntryValidator = new();

    [Fact]
    public void TaskValidator_ShouldFail_WhenTitleIsEmpty()
    {
        var task = new TaskItem { Title = "" };
        bool isValid = _taskValidator.Validate(task, out string error);

        isValid.Should().BeFalse();
        error.Should().Contain("Title cannot be empty");
    }

    [Fact]
    public void TaskValidator_ShouldPass_WhenValid()
    {
        var task = new TaskItem
        {
            Title = "Valid task",
            DueDate = DateTimeOffset.UtcNow.AddDays(1),
            EstimatedDuration = TimeSpan.FromHours(2)
        };
        bool isValid = _taskValidator.Validate(task, out string error);

        isValid.Should().BeTrue();
        error.Should().BeNull();
    }

    [Fact]
    public void ProjectValidator_ShouldFail_WhenNameIsEmpty()
    {
        var project = new Project { Name = "" };
        bool isValid = _projectValidator.Validate(project, out string error);

        isValid.Should().BeFalse();
        error.Should().Contain("Project Name cannot be empty");
    }

    [Fact]
    public void ProjectValidator_ShouldFail_WhenColorInvalid()
    {
        var project = new Project { Name = "Valid Name", Color = "invalid-color" };
        bool isValid = _projectValidator.Validate(project, out string error);

        isValid.Should().BeFalse();
        error.Should().Contain("Color must be a valid hex");
    }

    [Fact]
    public void ProjectValidator_ShouldPass_WithValidHexColor()
    {
        var project = new Project { Name = "Valid Name", Color = "#0078D4" };
        bool isValid = _projectValidator.Validate(project, out string error);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void TimeEntryValidator_ShouldFail_WhenEndTimeBeforeStartTime()
    {
        var entry = new TimeEntry
        {
            StartTime = DateTimeOffset.UtcNow,
            EndTime = DateTimeOffset.UtcNow.AddHours(-1)
        };
        bool isValid = _timeEntryValidator.Validate(entry, out string error);

        isValid.Should().BeFalse();
        error.Should().ContainEquivalentOf("EndTime must be after StartTime");
    }
}
