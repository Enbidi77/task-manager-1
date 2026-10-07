using System;

namespace TaskManager.Core.Entities;

public class TaskTag
{
    public Guid TaskId { get; set; }
    public TaskItem? Task { get; set; }
    
    public Guid TagId { get; set; }
    public Tag? Tag { get; set; }
}
