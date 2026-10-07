using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.DTOs;

namespace TaskManager.Core.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskItemStatus status);
    Task<IEnumerable<TaskItem>> GetByProjectAsync(Guid projectId);
    Task<IEnumerable<TaskItem>> GetTodayTasksAsync();
    Task<IEnumerable<TaskItem>> GetUpcomingTasksAsync();
    Task<IEnumerable<TaskItem>> GetOverdueTasksAsync();
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem> UpdateAsync(TaskItem task);
    Task DeleteAsync(Guid id);
    Task CompleteAsync(Guid id);
    Task MoveToStatusAsync(Guid id, TaskItemStatus newStatus);
    Task ReorderAsync(Guid id, int newSortOrder);
    Task<IEnumerable<TaskItem>> SearchAsync(string query);
    Task<IEnumerable<TaskItem>> GetSubtasksAsync(Guid parentId);
    Task<IEnumerable<TaskItem>> GetFavoritesAsync();
}
