using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.ViewModels
{
    public partial class ProjectListViewModel : ObservableObject
    {
        private readonly IProjectService _projectService;

        public ObservableCollection<Project> Projects { get; } = new();

        [ObservableProperty]
        private Project _selectedProject;

        public ProjectListViewModel(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public async System.Threading.Tasks.Task LoadProjectsAsync()
        {
            var projects = await _projectService.GetAllAsync();
            Projects.Clear();
            foreach (var p in projects)
            {
                Projects.Add(p);
            }
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task CreateProjectAsync()
        {
            var p = new Project { Name = "New Project" };
            var created = await _projectService.CreateAsync(p);
            Projects.Add(created);
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task DeleteProjectAsync(Project project)
        {
            if (project != null)
            {
                await _projectService.DeleteAsync(project.Id);
                Projects.Remove(project);
            }
        }
    }
}
