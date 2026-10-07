using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Enums;
using System;

namespace TaskManager.Application.ViewModels
{
    public partial class KanbanViewModel : ObservableObject
    {
        private readonly ITaskService _taskService;

        public ObservableCollection<KanbanColumnViewModel> Columns { get; } = new();

        [ObservableProperty]
        private string _filterText;

        public KanbanViewModel(ITaskService taskService)
        {
            _taskService = taskService;
            InitializeColumns();
        }

        private void InitializeColumns()
        {
            Columns.Add(new KanbanColumnViewModel(TaskItemStatus.Todo, "To Do"));
            Columns.Add(new KanbanColumnViewModel(TaskItemStatus.InProgress, "In Progress"));
            Columns.Add(new KanbanColumnViewModel(TaskItemStatus.Done, "Done"));
        }

        public async System.Threading.Tasks.Task LoadTasksAsync()
        {
            var tasks = await _taskService.GetAllAsync();
            foreach (var column in Columns)
            {
                column.Tasks.Clear();
                var colTasks = tasks.Where(t => t.Status == column.Status);
                foreach (var task in colTasks)
                {
                    column.Tasks.Add(task);
                }
                column.UpdateCount();
            }
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task MoveTaskAsync(Tuple<Guid, TaskItemStatus> moveData)
        {
            if (moveData != null)
            {
                await _taskService.MoveToStatusAsync(moveData.Item1, moveData.Item2);
                await LoadTasksAsync();
            }
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task QuickAddAsync(TaskItemStatus status)
        {
            var task = new TaskItem { Title = "New Task", Status = status };
            await _taskService.CreateAsync(task);
            await LoadTasksAsync();
        }
    }
}
