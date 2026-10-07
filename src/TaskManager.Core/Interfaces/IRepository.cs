using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;

namespace TaskManager.Core.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetPagedAsync(
        int skip,
        int take,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeString = "",
        CancellationToken cancellationToken = default);
}

public interface ITaskRepository : IRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskItemStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetTodayTasksAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetUpcomingTasksAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetFavoritesAsync(CancellationToken cancellationToken = default);
}

public interface IProjectRepository : IRepository<Project> { }
public interface IWorkspaceRepository : IRepository<Workspace> { }
public interface ITagRepository : IRepository<Tag> { }
public interface ITimeEntryRepository : IRepository<TimeEntry> { }
public interface IFocusSessionRepository : IRepository<FocusSession> { }
public interface INotificationRepository : IRepository<Notification> { }
public interface IActivityRepository : IRepository<Activity> { }
public interface ISavedFilterRepository : IRepository<SavedFilter> { }
public interface IDashboardWidgetRepository : IRepository<DashboardWidget> { }
public interface IUserPreferenceRepository : IRepository<UserPreference> { }

public interface IDatabaseInitializer
{
    Task InitializeAsync();
}
