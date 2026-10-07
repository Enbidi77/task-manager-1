using System;
using System.Collections.Generic;
using TaskManager.Core.Enums;

namespace TaskManager.Core.Entities;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    
    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }
    
    public Guid? WorkspaceId { get; set; }
    public Workspace? Workspace { get; set; }
    
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public TimeSpan? EstimatedDuration { get; set; }
    public TimeSpan? ActualDuration { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
    
    public Guid? ParentTaskId { get; set; }
    public TaskItem? ParentTask { get; set; }
    
    public string? Assignee { get; set; }
    public RecurrenceType RecurrenceType { get; set; }
    public int? RecurrenceInterval { get; set; }
    public DateTimeOffset? RecurrenceEndDate { get; set; }
    
    public bool IsFavorite { get; set; }
    public bool IsArchived { get; set; }
    public int SortOrder { get; set; }
    public bool IsCompleted
    {
        get => Status == TaskItemStatus.Done;
        set
        {
            if (value)
            {
                Status = TaskItemStatus.Done;
                CompletedAt = DateTimeOffset.UtcNow;
            }
            else if (Status == TaskItemStatus.Done)
            {
                Status = TaskItemStatus.Todo;
                CompletedAt = null;
            }
        }
    }

    // Navigation
    public ICollection<TaskItem> SubTasks { get; set; } = new List<TaskItem>();
    public ICollection<TaskTag> Tags { get; set; } = new List<TaskTag>();
    public ICollection<TaskDependency> Dependencies { get; set; } = new List<TaskDependency>();
    public ICollection<TaskDependency> DependentOn { get; set; } = new List<TaskDependency>();
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
