using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;

namespace TaskManager.Core.Interfaces;

public interface IActivityService
{
    Task<IEnumerable<Activity>> GetRecentAsync(int count);
    Task<IEnumerable<Activity>> GetByTaskAsync(Guid taskId);
    Task<IEnumerable<Activity>> GetByProjectAsync(Guid projectId);
    Task<Activity> LogActivityAsync(ActivityType type, string description, Guid? taskId = null, Guid? projectId = null, string? oldValue = null, string? newValue = null);
}
