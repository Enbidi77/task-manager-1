using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Events;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IEventAggregator _eventAggregator;
    private readonly IActivityService _activityService;

    public TaskService(
        ITaskRepository taskRepository,
        IEventAggregator eventAggregator,
        IActivityService activityService)
    {
        _taskRepository = taskRepository;
        _eventAggregator = eventAggregator;
        _activityService = activityService;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _taskRepository.GetWithDetailsAsync(id) ?? await _taskRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        var tasks = await _taskRepository.GetAllAsync();
        return tasks.OrderBy(t => t.SortOrder).ThenByDescending(t => t.CreatedAt);
    }

    public async Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskItemStatus status)
    {
        return await _taskRepository.GetByStatusAsync(status);
    }

    public async Task<IEnumerable<TaskItem>> GetByProjectAsync(Guid projectId)
    {
        return await _taskRepository.GetByProjectAsync(projectId);
    }

    public async Task<IEnumerable<TaskItem>> GetTodayTasksAsync()
    {
        return await _taskRepository.GetTodayTasksAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetUpcomingTasksAsync()
    {
        return await _taskRepository.GetUpcomingTasksAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync()
    {
        return await _taskRepository.GetOverdueTasksAsync();
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        task.CreatedAt = DateTimeOffset.UtcNow;
        task.UpdatedAt = DateTimeOffset.UtcNow;
        if (task.Id == Guid.Empty)
        {
            task.Id = Guid.NewGuid();
        }

        var created = await _taskRepository.AddAsync(task);

        await _activityService.LogActivityAsync(ActivityType.TaskCreated, $"Created task: {task.Title}", task.Id, task.ProjectId);
        _eventAggregator.Publish(new TaskCreatedEvent(task.Id));

        return created;
    }

    public async Task<TaskItem> UpdateAsync(TaskItem task)
    {
        task.UpdatedAt = DateTimeOffset.UtcNow;
        await _taskRepository.UpdateAsync(task);
        await _activityService.LogActivityAsync(ActivityType.TaskUpdated, $"Updated task: {task.Title}", task.Id, task.ProjectId);
        return task;
    }

    public async Task DeleteAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task != null)
        {
            await _taskRepository.DeleteAsync(task);
            await _activityService.LogActivityAsync(ActivityType.TaskDeleted, $"Deleted task: {task.Title}", id, task.ProjectId);
        }
    }

    public async Task CompleteAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task != null)
        {
            var oldStatus = task.Status;
            task.Status = TaskItemStatus.Done;
            task.CompletedAt = DateTimeOffset.UtcNow;
            task.UpdatedAt = DateTimeOffset.UtcNow;

            await _taskRepository.UpdateAsync(task);

            await _activityService.LogActivityAsync(ActivityType.TaskCompleted, $"Completed task: {task.Title}", id, task.ProjectId);
            _eventAggregator.Publish(new TaskCompletedEvent(id, task.CompletedAt.Value));
            _eventAggregator.Publish(new StatusChangedEvent(id, oldStatus, TaskItemStatus.Done));

            // Handle recurring task generation
            if (task.RecurrenceType != RecurrenceType.None && (!task.RecurrenceEndDate.HasValue || task.RecurrenceEndDate > DateTimeOffset.UtcNow))
            {
                var nextDueDate = CalculateNextDueDate(task.DueDate ?? DateTimeOffset.UtcNow, task.RecurrenceType, task.RecurrenceInterval ?? 1);
                var recurringTask = new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = task.Title,
                    Description = task.Description,
                    Status = TaskItemStatus.Todo,
                    Priority = task.Priority,
                    ProjectId = task.ProjectId,
                    WorkspaceId = task.WorkspaceId,
                    DueDate = nextDueDate,
                    EstimatedDuration = task.EstimatedDuration,
                    RecurrenceType = task.RecurrenceType,
                    RecurrenceInterval = task.RecurrenceInterval,
                    RecurrenceEndDate = task.RecurrenceEndDate,
                    IsFavorite = task.IsFavorite
                };
                await CreateAsync(recurringTask);
            }
        }
    }

    public async Task MoveToStatusAsync(Guid id, TaskItemStatus newStatus)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task != null && task.Status != newStatus)
        {
            var oldStatus = task.Status;
            task.Status = newStatus;
            task.UpdatedAt = DateTimeOffset.UtcNow;
            if (newStatus == TaskItemStatus.Done)
            {
                task.CompletedAt = DateTimeOffset.UtcNow;
            }
            else
            {
                task.CompletedAt = null;
            }

            await _taskRepository.UpdateAsync(task);

            await _activityService.LogActivityAsync(ActivityType.StatusChanged, $"Moved task to {newStatus}: {task.Title}", id, task.ProjectId);
            _eventAggregator.Publish(new StatusChangedEvent(id, oldStatus, newStatus));
        }
    }

    public async Task<IEnumerable<TaskItem>> SearchAsync(string query)
    {
        return await _taskRepository.SearchAsync(query);
    }

    public async Task ReorderAsync(Guid id, int newSortOrder)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task != null)
        {
            task.SortOrder = newSortOrder;
            task.UpdatedAt = DateTimeOffset.UtcNow;
            await _taskRepository.UpdateAsync(task);
        }
    }

    public async Task<IEnumerable<TaskItem>> GetSubtasksAsync(Guid parentId)
    {
        var tasks = await _taskRepository.FindAsync(t => t.ParentTaskId == parentId);
        return tasks.OrderBy(t => t.SortOrder);
    }

    public async Task<IEnumerable<TaskItem>> GetFavoritesAsync()
    {
        return await _taskRepository.GetFavoritesAsync();
    }

    private static DateTimeOffset CalculateNextDueDate(DateTimeOffset current, RecurrenceType type, int interval)
    {
        return type switch
        {
            RecurrenceType.Daily => current.AddDays(interval),
            RecurrenceType.Weekly => current.AddDays(7 * interval),
            RecurrenceType.Monthly => current.AddMonths(interval),
            RecurrenceType.Yearly => current.AddYears(interval),
            RecurrenceType.Weekdays => current.DayOfWeek switch
            {
                DayOfWeek.Friday => current.AddDays(3),
                DayOfWeek.Saturday => current.AddDays(2),
                _ => current.AddDays(1)
            },
            _ => current.AddDays(interval)
        };
    }
}
