using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using System;

namespace TaskManager.Application.ViewModels
{
    public partial class TaskDetailViewModel : ObservableObject
    {
        private readonly ITaskService _taskService;
        private readonly ITimeTrackingService _timeTrackingService;
        private readonly IActivityService _activityService;

        [ObservableProperty]
        private TaskItem _task;

        public ObservableCollection<TaskItem> Subtasks { get; } = new();
        public ObservableCollection<Tag> Tags { get; } = new();
        public ObservableCollection<TimeEntry> TimeEntries { get; } = new();
        public ObservableCollection<Activity> Activities { get; } = new();

        public TaskDetailViewModel(ITaskService taskService, ITimeTrackingService timeTrackingService, IActivityService activityService)
        {
            _taskService = taskService;
            _timeTrackingService = timeTrackingService;
            _activityService = activityService;
        }

        public async System.Threading.Tasks.Task LoadTaskAsync(Guid taskId)
        {
            Task = await _taskService.GetByIdAsync(taskId);
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task SaveAsync()
        {
            if (Task != null)
            {
                await _taskService.UpdateAsync(Task);
            }
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task AddSubtaskAsync()
        {
            await System.Threading.Tasks.Task.CompletedTask;
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task ToggleSubtaskAsync(TaskItem subtask)
        {
            await System.Threading.Tasks.Task.CompletedTask;
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task AddTagAsync(Tag tag)
        {
            await System.Threading.Tasks.Task.CompletedTask;
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task RemoveTagAsync(Tag tag)
        {
            await System.Threading.Tasks.Task.CompletedTask;
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task StartTimerAsync()
        {
            if (Task != null)
            {
                await _timeTrackingService.StartTimerAsync(Task.Id);
            }
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task StopTimerAsync()
        {
            if (Task != null)
            {
                await _timeTrackingService.StopTimerAsync(Task.Id);
            }
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task DeleteAsync()
        {
            if (Task != null)
            {
                await _taskService.DeleteAsync(Task.Id);
            }
        }
    }
}
