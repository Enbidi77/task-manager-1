using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.ViewModels
{
    public partial class CalendarViewModel : ObservableObject
    {
        private readonly ITaskService _taskService;

        [ObservableProperty]
        private DateTime _currentDate;

        [ObservableProperty]
        private CalendarViewMode _viewMode;

        [ObservableProperty]
        private DateTime _selectedDate;

        public ObservableCollection<TaskItem> CalendarItems { get; } = new();

        public CalendarViewModel(ITaskService taskService)
        {
            _taskService = taskService;
            CurrentDate = DateTime.Today;
            SelectedDate = DateTime.Today;
            ViewMode = CalendarViewMode.Month;
        }

        [RelayCommand]
        private void NavigateForward()
        {
            CurrentDate = ViewMode switch
            {
                CalendarViewMode.Day => CurrentDate.AddDays(1),
                CalendarViewMode.Week => CurrentDate.AddDays(7),
                CalendarViewMode.Month => CurrentDate.AddMonths(1),
                _ => CurrentDate.AddMonths(1)
            };
        }

        [RelayCommand]
        private void NavigateBackward()
        {
            CurrentDate = ViewMode switch
            {
                CalendarViewMode.Day => CurrentDate.AddDays(-1),
                CalendarViewMode.Week => CurrentDate.AddDays(-7),
                CalendarViewMode.Month => CurrentDate.AddMonths(-1),
                _ => CurrentDate.AddMonths(-1)
            };
        }

        [RelayCommand]
        private void Today()
        {
            CurrentDate = DateTime.Today;
            SelectedDate = DateTime.Today;
        }

        [RelayCommand]
        private void ChangeViewMode(CalendarViewMode mode)
        {
            ViewMode = mode;
        }

        public async System.Threading.Tasks.Task LoadTasksAsync()
        {
            var tasks = await _taskService.GetAllAsync();
            CalendarItems.Clear();
            foreach (var t in tasks)
            {
                if (t.DueDate.HasValue)
                {
                    CalendarItems.Add(t);
                }
            }
        }
    }
}
