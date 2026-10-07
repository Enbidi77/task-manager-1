using System;

namespace TaskManager.Core.Entities;

public class TimeEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid TaskId { get; set; }
    public TaskItem? Task { get; set; }
    
    public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? Notes { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
