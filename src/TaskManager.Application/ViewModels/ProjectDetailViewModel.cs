using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.DTOs;
using TaskManager.Core.Interfaces;
using System;
using System.Linq;

namespace TaskManager.Application.ViewModels
{
    public partial class ProjectDetailViewModel : ObservableObject
    {
        private readonly IProjectService _projectService;
        private readonly ITaskService _taskService;

        [ObservableProperty]
        private Project _project;

        [ObservableProperty]
        private ProjectStatsDto _stats;

        [ObservableProperty]
        private double _progressPercentage;

        public ObservableCollection<TaskItem> Tasks { get; } = new();

        public ProjectDetailViewModel(IProjectService projectService, ITaskService taskService)
        {
            _projectService = projectService;
            _taskService = taskService;
        }

        public async System.Threading.Tasks.Task LoadProjectAsync(Guid id)
        {
            Project = await _projectService.GetByIdAsync(id);
            Stats = await _projectService.GetProjectStatsAsync(id);
            if (Stats != null)
                ProgressPercentage = Stats.CompletionPercentage;

            var allTasks = await _taskService.GetAllAsync();
            Tasks.Clear();
            foreach (var t in allTasks.Where(x => x.ProjectId == id))
            {
                Tasks.Add(t);
            }
        }
    }
}
