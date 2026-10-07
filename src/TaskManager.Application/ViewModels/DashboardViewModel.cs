using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.DTOs;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ITaskService _taskService;
        private readonly IDashboardService _dashboardService;
        private readonly IActivityService _activityService;

        [ObservableProperty]
        private DashboardStatsDto? _dashboardStats;

        [ObservableProperty]
        private double _todayProgress;

        [ObservableProperty]
        private string _greeting = "Good day, Duy";

        [ObservableProperty]
        private int _todayTasksCount;

        [ObservableProperty]
        private int _completedTodayCount;

        [ObservableProperty]
        private string _focusTimeStr = "0h 0m";

        [ObservableProperty]
        private int _progressPercentage;

        [ObservableProperty]
        private double _progressValue;

        public ObservableCollection<TaskItem> TodayTasks { get; } = new();
        public ObservableCollection<DashboardWidget> Widgets { get; } = new();
        public ObservableCollection<Activity> RecentActivities { get; } = new();

        public DashboardViewModel(
            IAnalyticsService analyticsService,
            ITaskService taskService,
            IDashboardService dashboardService,
            IActivityService activityService)
        {
            _analyticsService = analyticsService;
            _taskService = taskService;
            _dashboardService = dashboardService;
            _activityService = activityService;

            var hour = DateTime.Now.Hour;
            Greeting = hour switch
            {
                < 12 => "Good morning, Duy",
                < 18 => "Good afternoon, Duy",
                _ => "Good evening, Duy"
            };
        }

        [RelayCommand]
        public async System.Threading.Tasks.Task RefreshAsync()
        {
            DashboardStats = await _analyticsService.GetDashboardStatsAsync();
            if (DashboardStats != null)
            {
                TodayProgress = DashboardStats.CompletionPercentage;
                TodayTasksCount = DashboardStats.TodayTasksCount;
                CompletedTodayCount = DashboardStats.CompletedTodayCount;
                ProgressPercentage = (int)DashboardStats.CompletionPercentage;
                ProgressValue = DashboardStats.CompletionPercentage / 100.0;
                FocusTimeStr = $"{(int)DashboardStats.TotalFocusTimeToday.TotalHours}h {DashboardStats.TotalFocusTimeToday.Minutes}m";
            }

            var tasks = await _taskService.GetTodayTasksAsync();
            TodayTasks.Clear();
            foreach (var t in tasks)
            {
                TodayTasks.Add(t);
            }

            var widgets = await _dashboardService.GetWidgetsAsync();
            Widgets.Clear();
            foreach (var w in widgets)
            {
                Widgets.Add(w);
            }

            var activities = await _activityService.GetRecentAsync(10);
            RecentActivities.Clear();
            foreach (var a in activities)
            {
                RecentActivities.Add(a);
            }
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task CompleteTaskAsync(TaskItem task)
        {
            if (task != null)
            {
                await _taskService.CompleteAsync(task.Id);
                await RefreshAsync();
            }
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task NavigateToTaskAsync(TaskItem task)
        {
            await System.Threading.Tasks.Task.CompletedTask;
        }
    }
}
