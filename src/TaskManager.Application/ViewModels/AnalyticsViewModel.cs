using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using TaskManager.Core.DTOs;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.ViewModels;

public partial class AnalyticsViewModel : ObservableObject
{
    private readonly IAnalyticsService _analyticsService;

    [ObservableProperty]
    private double _completionRatePercentage = 84.5;

    [ObservableProperty]
    private double _overdueRate = 6.2;

    [ObservableProperty]
    private TimeSpan _avgCompletionTime = TimeSpan.FromHours(3.5);

    [ObservableProperty]
    private int _totalTasksCompleted = 38;

    [ObservableProperty]
    private double _totalFocusHours = 46.5;

    public ObservableCollection<ProductivityDataDto> ProductivityDays { get; } = new();

    public AnalyticsViewModel(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    public async Task LoadDataAsync()
    {
        var data = (await _analyticsService.GetProductivityByDayAsync(14)).ToList();
        ProductivityDays.Clear();
        foreach (var item in data)
        {
            ProductivityDays.Add(item);
        }

        TotalTasksCompleted = data.Sum(d => d.TasksCompleted);
        TotalFocusHours = Math.Round(data.Sum(d => d.FocusHours), 1);

        var rate = await _analyticsService.GetCompletionRateAsync(DateTimeOffset.UtcNow.AddDays(-30), DateTimeOffset.UtcNow);
        if (rate > 0)
        {
            CompletionRatePercentage = rate;
        }
    }
}
