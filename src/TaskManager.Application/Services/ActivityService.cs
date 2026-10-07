using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;

public class ActivityService : IActivityService
{
    private readonly IActivityRepository _activityRepository;

    public ActivityService(IActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<IEnumerable<Activity>> GetRecentAsync(int count)
    {
        var all = await _activityRepository.GetAllAsync();
        return all.OrderByDescending(a => a.CreatedAt).Take(count);
    }

    public async Task<IEnumerable<Activity>> GetByTaskAsync(Guid taskId)
    {
        var all = await _activityRepository.FindAsync(a => a.TaskId == taskId);
        return all.OrderByDescending(a => a.CreatedAt);
    }

    public async Task<IEnumerable<Activity>> GetByProjectAsync(Guid projectId)
    {
        var all = await _activityRepository.FindAsync(a => a.ProjectId == projectId);
        return all.OrderByDescending(a => a.CreatedAt);
    }

    public async Task<Activity> LogActivityAsync(
        ActivityType type,
        string description,
        Guid? taskId = null,
        Guid? projectId = null,
        string? oldValue = null,
        string? newValue = null)
    {
        var activity = new Activity
        {
            Id = Guid.NewGuid(),
            Type = type,
            Description = description,
            TaskId = taskId,
            ProjectId = projectId,
            OldValue = oldValue,
            NewValue = newValue,
            CreatedAt = DateTimeOffset.UtcNow
        };

        return await _activityRepository.AddAsync(activity);
    }
}
