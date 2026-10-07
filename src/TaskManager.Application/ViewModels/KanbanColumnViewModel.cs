using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;

namespace TaskManager.Application.ViewModels
{
    public partial class KanbanColumnViewModel : ObservableObject
    {
        [ObservableProperty]
        private TaskItemStatus _status;

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private int _taskCount;

        [ObservableProperty]
        private int _wipLimit;

        [ObservableProperty]
        private bool _isCollapsed;

        public ObservableCollection<TaskItem> Tasks { get; } = new();

        public KanbanColumnViewModel(TaskItemStatus status, string title)
        {
            Status = status;
            Title = title;
        }

        public void UpdateCount()
        {
            TaskCount = Tasks.Count;
        }
    }
}
