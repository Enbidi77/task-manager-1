using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.ViewModels;

public partial class TimelineViewModel : ObservableObject
{
    private readonly ITaskService _taskService;

    public ObservableCollection<TaskItem> Tasks { get; } = new();

    [ObservableProperty]
    private TimelineZoom _zoomLevel;

    [ObservableProperty]
    private double _todayMarkerPosition;

    public TimelineViewModel(ITaskService taskService)
    {
        _taskService = taskService;
        ZoomLevel = TimelineZoom.Week;
    }

    [RelayCommand]
    private void ZoomIn()
    {
        if (ZoomLevel > TimelineZoom.Day)
        {
            ZoomLevel--;
        }
    }

    [RelayCommand]
    private void ZoomOut()
    {
        if (ZoomLevel < TimelineZoom.Quarter)
        {
            ZoomLevel++;
        }
    }

    [RelayCommand]
    private void ScrollToToday()
    {
        TodayMarkerPosition = 250;
    }

    public async Task LoadDataAsync()
    {
        var allTasks = await _taskService.GetAllAsync();
        Tasks.Clear();
        foreach (var task in allTasks.Where(t => t.DueDate.HasValue))
        {
            Tasks.Add(task);
        }
    }
}
