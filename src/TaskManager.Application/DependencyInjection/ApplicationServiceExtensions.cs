using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Services;
using TaskManager.Application.Validators;
using TaskManager.Application.ViewModels;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Events;

namespace TaskManager.Application.DependencyInjection
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Events
            services.AddSingleton<IEventAggregator, EventAggregator>();

            // Services
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IWorkspaceService, WorkspaceService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ITimeTrackingService, TimeTrackingService>();
            services.AddScoped<IFocusService, FocusService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IDashboardService, DashboardService>();

            // Validators
            services.AddTransient<TaskValidator>();
            services.AddTransient<ProjectValidator>();
            services.AddTransient<TimeEntryValidator>();

            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<TaskListViewModel>();
            services.AddTransient<TaskDetailViewModel>();
            services.AddTransient<KanbanViewModel>();
            services.AddTransient<CalendarViewModel>();
            services.AddTransient<TimelineViewModel>();
            services.AddTransient<ProjectListViewModel>();
            services.AddTransient<ProjectDetailViewModel>();
            services.AddTransient<ResourcesViewModel>();
            services.AddTransient<AnalyticsViewModel>();
            services.AddTransient<FocusViewModel>();
            services.AddTransient<TimeTrackingViewModel>();
            services.AddTransient<NotificationViewModel>();
            services.AddTransient<ActivityViewModel>();
            services.AddTransient<SearchViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<InboxViewModel>();

            return services;
        }
    }
}
