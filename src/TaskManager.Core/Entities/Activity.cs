using System;
using TaskManager.Core.Enums;

namespace TaskManager.Core.Entities;

public class Activity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ActivityType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    
    public Guid? TaskId { get; set; }
    public TaskItem? Task { get; set; }
    
    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }
    
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
