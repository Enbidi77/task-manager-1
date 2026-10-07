using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using System;
using System.Linq;

namespace TaskManager.Application.ViewModels
{
    public partial class TimeTrackingViewModel : ObservableObject
    {
        private readonly ITimeTrackingService _timeTrackingService;

        [ObservableProperty]
        private TimeEntry _activeTimer;

        [ObservableProperty]
        private TimeSpan _elapsedTime;

        [ObservableProperty]
        private TimeSpan _totalToday;

        public ObservableCollection<TimeEntry> TodayEntries { get; } = new();

        public TimeTrackingViewModel(ITimeTrackingService timeTrackingService)
        {
            _timeTrackingService = timeTrackingService;
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task StartAsync(Guid taskId)
        {
            await _timeTrackingService.StartTimerAsync(taskId);
            await LoadDataAsync();
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task StopAsync(Guid taskId)
        {
            await _timeTrackingService.StopTimerAsync(taskId, null);
            await LoadDataAsync();
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task AddManualEntryAsync(TimeEntry entry)
        {
            await _timeTrackingService.CreateManualEntryAsync(entry);
            await LoadDataAsync();
        }

        public async System.Threading.Tasks.Task LoadDataAsync()
        {
            ActiveTimer = await _timeTrackingService.GetActiveTimerAsync();
            
            var summary = await _timeTrackingService.GetDailySummaryAsync(DateTimeOffset.Now);
            TotalToday = summary.TotalTracked;

            TodayEntries.Clear();
            var entries = await _timeTrackingService.GetEntriesForTaskAsync(Guid.Empty);
            // or get recent
            if (ActiveTimer != null)
            {
                var taskEntries = await _timeTrackingService.GetEntriesForTaskAsync(ActiveTimer.TaskId);
                foreach (var e in taskEntries)
                {
                    TodayEntries.Add(e);
                }
            }
        }
    }
}
