using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Enums;
using System;
using System.Collections.Generic;

namespace TaskManager.Application.ViewModels
{
    public partial class TaskListViewModel : ObservableObject
    {
        private readonly ITaskService _taskService;

        public ObservableCollection<TaskItem> Tasks { get; } = new();

        [ObservableProperty]
        private TaskItem _selectedTask;

        [ObservableProperty]
        private string _filterMode = "All";

        [ObservableProperty]
        private string _sortMode = "Date";

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private bool _isDetailsPanelOpen;

        public TaskListViewModel(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task LoadTasksAsync()
        {
            IEnumerable<TaskItem> tasks;
            
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                tasks = await _taskService.SearchAsync(SearchText);
            }
            else
            {
                tasks = await _taskService.GetAllAsync();
            }

            Tasks.Clear();
            foreach (var t in tasks)
            {
                Tasks.Add(t);
            }
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task CreateTaskAsync()
        {
            var task = new TaskItem { Title = "New Task" };
            var created = await _taskService.CreateAsync(task);
            Tasks.Add(created);
            SelectedTask = created;
            IsDetailsPanelOpen = true;
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task CompleteTaskAsync(TaskItem task)
        {
            if (task != null)
            {
                await _taskService.CompleteAsync(task.Id);
                await LoadTasksAsync();
            }
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task DeleteTaskAsync(TaskItem task)
        {
            if (task != null)
            {
                await _taskService.DeleteAsync(task.Id);
                Tasks.Remove(task);
                if (SelectedTask == task)
                {
                    SelectedTask = null;
                    IsDetailsPanelOpen = false;
                }
            }
        }

        [RelayCommand]
        private void OpenDetails(TaskItem task)
        {
            SelectedTask = task;
            IsDetailsPanelOpen = true;
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task ChangeStatusAsync(TaskItemStatus status)
        {
            if (SelectedTask != null)
            {
                await _taskService.MoveToStatusAsync(SelectedTask.Id, status);
                await LoadTasksAsync();
            }
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task ChangePriorityAsync(TaskPriority priority)
        {
            if (SelectedTask != null)
            {
                SelectedTask.Priority = priority;
                await _taskService.UpdateAsync(SelectedTask);
                await LoadTasksAsync();
            }
        }
    }
}
