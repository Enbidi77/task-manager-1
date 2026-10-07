using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.DTOs;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;

    public ProjectService(IProjectRepository projectRepository, ITaskRepository taskRepository)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _projectRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        return projects.OrderBy(p => p.SortOrder).ThenBy(p => p.Name);
    }

    public async Task<Project> CreateAsync(Project project)
    {
        project.CreatedAt = DateTimeOffset.UtcNow;
        project.UpdatedAt = DateTimeOffset.UtcNow;
        if (project.Id == Guid.Empty)
        {
            project.Id = Guid.NewGuid();
        }

        return await _projectRepository.AddAsync(project);
    }

    public async Task<Project> UpdateAsync(Project project)
    {
        project.UpdatedAt = DateTimeOffset.UtcNow;
        await _projectRepository.UpdateAsync(project);
        return project;
    }

    public async Task DeleteAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project != null)
        {
            await _projectRepository.DeleteAsync(project);
        }
    }

    public async Task<ProjectStatsDto> GetProjectStatsAsync(Guid id)
    {
        var tasks = (await _taskRepository.GetByProjectAsync(id)).ToList();
        int total = tasks.Count;
        int completed = tasks.Count(t => t.Status == TaskItemStatus.Done);
        var now = DateTimeOffset.UtcNow;
        int overdue = tasks.Count(t => t.Status != TaskItemStatus.Done && t.DueDate.HasValue && t.DueDate.Value < now);

        double completionPercentage = total == 0 ? 0 : Math.Round((double)completed / total * 100, 1);
        long totalSeconds = tasks.Where(t => t.ActualDuration.HasValue).Sum(t => (long)t.ActualDuration!.Value.TotalSeconds);

        var project = await _projectRepository.GetByIdAsync(id);
        return new ProjectStatsDto
        {
            ProjectId = id,
            Name = project?.Name ?? string.Empty,
            TotalTasks = total,
            CompletedTasks = completed,
            OverdueTasks = overdue
        };
    }
}
