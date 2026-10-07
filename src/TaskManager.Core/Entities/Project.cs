using System;
using System.Collections.Generic;

namespace TaskManager.Core.Entities;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    
    public Guid? WorkspaceId { get; set; }
    public Workspace? Workspace { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public bool IsArchived { get; set; }
    public int SortOrder { get; set; }
    
    // Navigation
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
