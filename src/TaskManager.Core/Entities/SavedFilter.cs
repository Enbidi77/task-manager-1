using System;

namespace TaskManager.Core.Entities;

public class SavedFilter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string FilterJson { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public int SortOrder { get; set; }
}
