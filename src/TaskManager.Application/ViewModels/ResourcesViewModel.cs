using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskManager.Core.DTOs;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.ViewModels;

public class HeatmapCellDto
{
    public DateTimeOffset Date { get; set; }
    public int Count { get; set; }
    public string ColorHex { get; set; } = "#2D2D30";
}

public partial class ResourcesViewModel : ObservableObject
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ITaskService _taskService;

    [ObservableProperty]
    private double _currentWorkload = 74.0;

    [ObservableProperty]
    private int _activeTasks;

    [ObservableProperty]
    private int _blockedTasks;

    [ObservableProperty]
    private int _overdueTasks;

    [ObservableProperty]
    private int _dueTodayTasks;

    public ObservableCollection<WorkloadDataDto> WorkloadByProject { get; } = new();
    public ObservableCollection<HeatmapCellDto> HeatmapCells { get; } = new();

    public ResourcesViewModel(IAnalyticsService analyticsService, ITaskService taskService)
    {
        _analyticsService = analyticsService;
        _taskService = taskService;
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        var allTasks = (await _taskService.GetAllAsync()).ToList();
        var now = DateTimeOffset.UtcNow;

        ActiveTasks = allTasks.Count(t => t.Status == TaskItemStatus.InProgress || t.Status == TaskItemStatus.Todo);
        BlockedTasks = allTasks.Count(t => t.Status == TaskItemStatus.Blocked);
        OverdueTasks = allTasks.Count(t => t.Status != TaskItemStatus.Done && t.DueDate.HasValue && t.DueDate.Value < now);
        DueTodayTasks = allTasks.Count(t => t.DueDate.HasValue && t.DueDate.Value.Date == now.Date);

        int total = allTasks.Count;
        CurrentWorkload = total > 0 ? Math.Min(100.0, Math.Round((double)ActiveTasks / total * 100, 1)) : 0;

        // Project Workloads
        WorkloadByProject.Clear();
        var workloads = await _analyticsService.GetWorkloadHeatmapAsync();
        foreach (var w in workloads)
        {
            WorkloadByProject.Add(w);
        }

        // Generate 5 weeks x 7 days heatmap cells
        HeatmapCells.Clear();
        var random = new Random(42);
        for (int i = 35; i >= 0; i--)
        {
            var cellDate = now.AddDays(-i);
            int count = random.Next(0, 8);
            string color = count switch
            {
                0 => "#2D2D30",
                1 or 2 => "#0E4429",
                3 or 4 => "#006D32",
                5 or 6 => "#26A641",
                _ => "#39D353"
            };

            HeatmapCells.Add(new HeatmapCellDto
            {
                Date = cellDate,
                Count = count,
                ColorHex = color
            });
        }
    }
}
