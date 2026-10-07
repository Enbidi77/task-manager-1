using System;
using System.Collections.Generic;

namespace TaskManager.Core.Entities;

public class Tag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation
    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
