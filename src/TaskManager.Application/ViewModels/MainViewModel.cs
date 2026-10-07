using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly INotificationService _notificationService;
        private readonly IWorkspaceService _workspaceService;

        [ObservableProperty]
        private string _selectedNavigationItem;

        [ObservableProperty]
        private bool _isNavigationExpanded;

        [ObservableProperty]
        private string _currentPage;

        [ObservableProperty]
        private int _unreadNotificationCount;

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private Workspace _currentWorkspace;

        public MainViewModel(INotificationService notificationService, IWorkspaceService workspaceService)
        {
            _notificationService = notificationService;
            _workspaceService = workspaceService;
            IsNavigationExpanded = true;
        }

        [RelayCommand]
        private async Task NavigateAsync(string pageName)
        {
            CurrentPage = pageName;
            SelectedNavigationItem = pageName;
            await Task.CompletedTask; // In MAUI, use Shell navigation usually
        }

        public async Task LoadInitialDataAsync()
        {
            // Load Unread count, Workspace etc.
        }
    }
}
