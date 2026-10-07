using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetAllAsync();
    Task<IEnumerable<Notification>> GetUnreadAsync();
    Task<int> GetUnreadCountAsync();
    Task MarkAsReadAsync(Guid id);
    Task MarkAllAsReadAsync();
    Task<Notification> CreateAsync(Notification notification);
    Task DeleteAsync(Guid id);
}
