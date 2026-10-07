using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using System.Linq;

namespace TaskManager.Application.ViewModels
{
    public partial class InboxViewModel : ObservableObject
    {
        private readonly ITaskService _taskService;

        public ObservableCollection<TaskItem> InboxTasks { get; } = new();

        public InboxViewModel(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async System.Threading.Tasks.Task LoadTasksAsync()
        {
            var tasks = await _taskService.GetAllAsync();
            InboxTasks.Clear();
            foreach (var t in tasks.Where(x => x.ProjectId == null))
            {
                InboxTasks.Add(t);
            }
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task MoveToTodoAsync(TaskItem task)
        {
            if (task != null)
            {
                await _taskService.MoveToStatusAsync(task.Id, Core.Enums.TaskItemStatus.Todo);
                await LoadTasksAsync();
            }
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task QuickProcessAsync()
        {
            await System.Threading.Tasks.Task.CompletedTask;
        }
    }
}
