using System;
using TaskManager.Core.Enums;

namespace TaskManager.Core.Entities;

public class TaskDependency
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid TaskId { get; set; }
    public TaskItem? Task { get; set; }
    
    public Guid DependsOnTaskId { get; set; }
    public TaskItem? DependsOnTask { get; set; }
    
    public DependencyType Type { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
