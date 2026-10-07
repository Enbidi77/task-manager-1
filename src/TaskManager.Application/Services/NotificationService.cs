using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<Notification>> GetAllAsync()
    {
        var all = await _notificationRepository.GetAllAsync();
        return all.OrderByDescending(n => n.CreatedAt);
    }

    public async Task<IEnumerable<Notification>> GetUnreadAsync()
    {
        var unread = await _notificationRepository.FindAsync(n => !n.IsRead);
        return unread.OrderByDescending(n => n.CreatedAt);
    }

    public async Task<int> GetUnreadCountAsync()
    {
        var unread = await _notificationRepository.FindAsync(n => !n.IsRead);
        return unread.Count();
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        var notif = await _notificationRepository.GetByIdAsync(id);
        if (notif != null && !notif.IsRead)
        {
            notif.IsRead = true;
            notif.ReadAt = DateTimeOffset.UtcNow;
            await _notificationRepository.UpdateAsync(notif);
        }
    }

    public async Task MarkAllAsReadAsync()
    {
        var unread = await _notificationRepository.FindAsync(n => !n.IsRead);
        var now = DateTimeOffset.UtcNow;
        foreach (var n in unread)
        {
            n.IsRead = true;
            n.ReadAt = now;
            await _notificationRepository.UpdateAsync(n);
        }
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        notification.CreatedAt = DateTimeOffset.UtcNow;
        if (notification.Id == Guid.Empty)
        {
            notification.Id = Guid.NewGuid();
        }

        return await _notificationRepository.AddAsync(notification);
    }

    public async Task DeleteAsync(Guid id)
    {
        var notif = await _notificationRepository.GetByIdAsync(id);
        if (notif != null)
        {
            await _notificationRepository.DeleteAsync(notif);
        }
    }
}
