using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.DTOs;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IFocusSessionRepository _focusSessionRepository;
    private readonly IProjectRepository _projectRepository;

    public AnalyticsService(
        ITaskRepository taskRepository,
        ITimeEntryRepository timeEntryRepository,
        IFocusSessionRepository focusSessionRepository,
        IProjectRepository projectRepository)
    {
        _taskRepository = taskRepository;
        _timeEntryRepository = timeEntryRepository;
        _focusSessionRepository = focusSessionRepository;
        _projectRepository = projectRepository;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var todayTasks = (await _taskRepository.GetTodayTasksAsync()).ToList();
        int todayCount = todayTasks.Count;
        int completedToday = todayTasks.Count(t => t.Status == TaskItemStatus.Done);

        var now = DateTimeOffset.UtcNow;
        var today = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero);
        var tomorrow = today.AddDays(1);
        var sessions = await _focusSessionRepository.FindAsync(s => s.StartTime >= today && s.StartTime < tomorrow && s.IsCompleted);
        var totalFocusSeconds = sessions.Sum(s => (s.ActualDuration ?? s.PlannedDuration).TotalSeconds);

        double percentage = todayCount == 0 ? 0 : Math.Round((double)completedToday / todayCount * 100, 1);

        return new DashboardStatsDto
        {
            TodayTasksCount = todayCount,
            CompletedTodayCount = completedToday,
            TotalFocusTimeToday = TimeSpan.FromSeconds(totalFocusSeconds),
            CompletionPercentage = percentage
        };
    }

    public async Task<double> GetCompletionRateAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var tasks = (await _taskRepository.FindAsync(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)).ToList();
        if (tasks.Count == 0) return 0.0;

        int completed = tasks.Count(t => t.Status == TaskItemStatus.Done);
        return Math.Round((double)completed / tasks.Count * 100, 1);
    }

    public async Task<IEnumerable<ProductivityDataDto>> GetProductivityByDayAsync(int days)
    {
        var result = new List<ProductivityDataDto>();
        var now = DateTimeOffset.UtcNow;
        var endDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero);
        var startDate = endDate.AddDays(-days);

        var tasks = (await _taskRepository.FindAsync(t => t.CompletedAt.HasValue && t.CompletedAt.Value >= startDate)).ToList();
        var sessions = (await _focusSessionRepository.FindAsync(s => s.StartTime >= startDate && s.IsCompleted)).ToList();

        for (int i = 0; i < days; i++)
        {
            var date = startDate.AddDays(i);
            var nextDate = date.AddDays(1);

            int completed = tasks.Count(t => t.CompletedAt >= date && t.CompletedAt < nextDate);
            var focusHours = sessions.Where(s => s.StartTime >= date && s.StartTime < nextDate)
                .Sum(s => (s.ActualDuration ?? s.PlannedDuration).TotalHours);

            result.Add(new ProductivityDataDto
            {
                Date = date,
                TasksCompleted = completed,
                FocusHours = Math.Round(focusHours, 1)
            });
        }

        return result;
    }

    public async Task<IEnumerable<ProductivityDataDto>> GetProductivityByProjectAsync(Guid projectId)
    {
        var tasks = (await _taskRepository.GetByProjectAsync(projectId)).ToList();
        return tasks.GroupBy(t => (t.CompletedAt ?? t.CreatedAt).Date)
            .OrderBy(g => g.Key)
            .Select(g => new ProductivityDataDto
            {
                Date = g.Key,
                TasksCompleted = g.Count(t => t.Status == TaskItemStatus.Done),
                FocusHours = Math.Round(g.Sum(t => t.ActualDuration?.TotalHours ?? 0), 1)
            });
    }

    public async Task<IDictionary<string, TimeSpan>> GetTimeDistributionAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var entries = (await _timeEntryRepository.FindAsync(e => e.StartTime >= startDate && e.StartTime <= endDate && e.Duration.HasValue)).ToList();
        var projects = (await _projectRepository.GetAllAsync()).ToDictionary(p => p.Id, p => p.Name);
        var tasks = (await _taskRepository.GetAllAsync()).ToDictionary(t => t.Id, t => t.ProjectId);

        var distribution = new Dictionary<string, TimeSpan>();

        foreach (var entry in entries)
        {
            string category = "General";
            if (tasks.TryGetValue(entry.TaskId, out var projId) && projId.HasValue && projects.TryGetValue(projId.Value, out var projName))
            {
                category = projName;
            }

            if (!distribution.ContainsKey(category))
            {
                distribution[category] = TimeSpan.Zero;
            }

            distribution[category] += entry.Duration!.Value;
        }

        return distribution;
    }

    public async Task<IEnumerable<WorkloadDataDto>> GetWorkloadHeatmapAsync()
    {
        var projects = (await _projectRepository.GetAllAsync()).ToList();
        var tasks = (await _taskRepository.GetAllAsync()).ToList();

        return projects.Select(p =>
        {
            var projTasks = tasks.Where(t => t.ProjectId == p.Id && t.Status != TaskItemStatus.Done).ToList();
            var hours = projTasks.Sum(t => t.EstimatedDuration?.TotalHours ?? 2.0);
            return new WorkloadDataDto
            {
                AssigneeId = p.Id,
                AssigneeName = p.Name,
                ActiveTasks = projTasks.Count,
                EstimatedHoursRemaining = Math.Round(hours, 1)
            };
        });
    }

    public async Task<IDictionary<DateTimeOffset, int>> GetTasksCompletedPerDayAsync(int days)
    {
        var now = DateTimeOffset.UtcNow;
        var startDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero).AddDays(-days);
        var tasks = (await _taskRepository.FindAsync(t => t.CompletedAt.HasValue && t.CompletedAt.Value >= startDate)).ToList();

        var dict = new Dictionary<DateTimeOffset, int>();
        for (int i = 0; i < days; i++)
        {
            var date = startDate.AddDays(i);
            var next = date.AddDays(1);
            dict[date] = tasks.Count(t => t.CompletedAt >= date && t.CompletedAt < next);
        }

        return dict;
    }
}
