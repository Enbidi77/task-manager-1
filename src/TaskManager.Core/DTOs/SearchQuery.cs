using TaskManager.Core.Enums;

namespace TaskManager.Core.DTOs;

public class SearchQuery
{
    public string Text { get; set; } = string.Empty;
    public TaskItemStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public bool IncludeTasks { get; set; } = true;
    public bool IncludeProjects { get; set; } = true;
    public bool IncludeTags { get; set; } = true;
}
