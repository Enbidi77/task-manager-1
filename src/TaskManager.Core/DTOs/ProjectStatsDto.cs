using System;

namespace TaskManager.Core.DTOs;

public class ProjectStatsDto
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public double CompletionPercentage => TotalTasks > 0 ? (double)CompletedTasks / TotalTasks * 100 : 0;
}
