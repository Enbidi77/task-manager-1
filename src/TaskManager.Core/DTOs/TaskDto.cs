using System;
using TaskManager.Core.Enums;

namespace TaskManager.Core.DTOs;

public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public bool IsFavorite { get; set; }
    public bool IsCompleted => Status == TaskItemStatus.Done;
}
