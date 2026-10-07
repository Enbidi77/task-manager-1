using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Database;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskRepository : Repository<TaskItem>, ITaskRepository
    {
        public TaskRepository(FlowTaskDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskItemStatus status, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(t => t.Status == status).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TaskItem>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(t => t.ProjectId == projectId).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TaskItem>> GetTodayTasksAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var today = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero);
            var tomorrow = today.AddDays(1);
            
            return await _dbSet
                .Where(t => (t.DueDate >= today && t.DueDate < tomorrow) || 
                            (t.StartDate >= today && t.StartDate < tomorrow) ||
                            (t.Status == TaskItemStatus.InProgress))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TaskItem>> GetUpcomingTasksAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var today = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero);
            var nextWeek = today.AddDays(7);
            
            return await _dbSet
                .Where(t => t.DueDate >= today && t.DueDate <= nextWeek && t.Status != TaskItemStatus.Done)
                .OrderBy(t => t.DueDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var today = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero);
            
            return await _dbSet
                .Where(t => t.DueDate < today && t.Status != TaskItemStatus.Done)
                .OrderBy(t => t.DueDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TaskItem>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return new List<TaskItem>();
            
            var lowerTerm = searchTerm.ToLower();
            
            return await _dbSet
                .Where(t => (t.Title != null && t.Title.ToLower().Contains(lowerTerm)) || 
                            (t.Description != null && t.Description.ToLower().Contains(lowerTerm)))
                .ToListAsync(cancellationToken);
        }

        public async Task<TaskItem?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(t => t.Project)
                .Include(t => t.Tags)
                    .ThenInclude(tt => tt.Tag)
                .Include(t => t.SubTasks)
                .Include(t => t.TimeEntries)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<TaskItem>> GetFavoritesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(t => t.IsFavorite)
                .ToListAsync(cancellationToken);
        }
    }
}
