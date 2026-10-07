namespace TaskManager.Core.DTOs;

public class DashboardStatsDto
{
    public int TodayTasksCount { get; set; }
    public int CompletedTodayCount { get; set; }
    public TimeSpan TotalFocusTimeToday { get; set; }
    public double CompletionPercentage { get; set; }
}
