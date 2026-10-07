using System;

namespace TaskManager.Core.Entities;

public class FocusSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid? TaskId { get; set; }
    public TaskItem? Task { get; set; }
    
    public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? EndTime { get; set; }
    public TimeSpan PlannedDuration { get; set; }
    public TimeSpan? ActualDuration { get; set; }
    public int SessionNumber { get; set; }
    public bool IsCompleted { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
