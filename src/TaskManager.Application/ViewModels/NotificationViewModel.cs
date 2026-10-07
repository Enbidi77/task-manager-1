using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using System.Linq;

namespace TaskManager.Application.ViewModels
{
    public partial class NotificationViewModel : ObservableObject
    {
        private readonly INotificationService _notificationService;

        public ObservableCollection<Notification> Notifications { get; } = new();

        [ObservableProperty]
        private int _unreadCount;

        public NotificationViewModel(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async System.Threading.Tasks.Task LoadNotificationsAsync()
        {
            var notifs = await _notificationService.GetAllAsync();
            Notifications.Clear();
            int unread = 0;
            foreach (var n in notifs.OrderByDescending(x => x.CreatedAt))
            {
                Notifications.Add(n);
                if (!n.IsRead) unread++;
            }
            UnreadCount = unread;
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task MarkAsReadAsync(Notification notification)
        {
            if (notification != null && !notification.IsRead)
            {
                await _notificationService.MarkAsReadAsync(notification.Id);
                await LoadNotificationsAsync();
            }
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task MarkAllReadAsync()
        {
            await _notificationService.MarkAllAsReadAsync();
            await LoadNotificationsAsync();
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task DeleteAsync(Notification notification)
        {
            if (notification != null)
            {
                await _notificationService.DeleteAsync(notification.Id);
                await LoadNotificationsAsync();
            }
        }
    }
}
