using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using TaskManager.App.Views;
using TaskManager.Application.ViewModels;

namespace TaskManager.App;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;
    private readonly IServiceProvider _serviceProvider;
    private bool _isSidebarCollapsed;

    public MainPage(MainViewModel viewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _serviceProvider = serviceProvider;
        BindingContext = viewModel;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        SearchOverlayControl.BindingContext = _serviceProvider.GetRequiredService<SearchViewModel>();
        SearchOverlayControl.CloseRequested += (s, e) => SearchOverlayControl.IsVisible = false;

        Loaded += OnMainPageLoaded;
    }

    private void OnMainPageLoaded(object? sender, EventArgs e)
    {
        NavigateToPage("Dashboard");
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.CurrentPage))
        {
            NavigateToPage(_viewModel.CurrentPage);
        }
    }

    public void NavigateToPage(string pageName)
    {
        View? targetView = pageName switch
        {
            "Dashboard" => _serviceProvider.GetRequiredService<DashboardView>(),
            "Tasks" or "Inbox" or "Today" or "Upcoming" => _serviceProvider.GetRequiredService<TaskListView>(),
            "Calendar" => _serviceProvider.GetRequiredService<CalendarView>(),
            "Kanban" => _serviceProvider.GetRequiredService<KanbanView>(),
            "Timeline" => _serviceProvider.GetRequiredService<TimelineView>(),
            "Projects" => _serviceProvider.GetRequiredService<ProjectListView>(),
            "Resources" => _serviceProvider.GetRequiredService<ResourcesView>(),
            "Analytics" => _serviceProvider.GetRequiredService<AnalyticsView>(),
            "Activity" => _serviceProvider.GetRequiredService<ActivityView>(),
            "Focus" => _serviceProvider.GetRequiredService<FocusView>(),
            "TimeTracking" => _serviceProvider.GetRequiredService<TimeTrackingView>(),
            "Notifications" => _serviceProvider.GetRequiredService<NotificationView>(),
            "Settings" => _serviceProvider.GetRequiredService<SettingsView>(),
            _ => _serviceProvider.GetRequiredService<DashboardView>()
        };

        if (targetView != null)
        {
            MainContentView.Content = targetView;
        }
    }

    private void OnToggleSidebarClicked(object? sender, EventArgs e)
    {
        _isSidebarCollapsed = !_isSidebarCollapsed;
        SidebarContainer.WidthRequest = _isSidebarCollapsed ? 48 : 240;
    }

    private void OnSearchBoxTapped(object? sender, TappedEventArgs e)
    {
        SearchOverlayControl.IsVisible = !SearchOverlayControl.IsVisible;
    }

    private void OnNotificationsClicked(object? sender, EventArgs e)
    {
        NavigateToPage("Notifications");
    }

    private void OnSettingsClicked(object? sender, EventArgs e)
    {
        NavigateToPage("Settings");
    }
}
