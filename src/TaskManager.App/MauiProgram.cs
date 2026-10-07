using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskManager.Application.DependencyInjection;
using TaskManager.Infrastructure.DependencyInjection;
using TaskManager.App.Views;
using TaskManager.Application.ViewModels;
using TaskManager.Core.Interfaces;

namespace TaskManager.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Layer Dependency Injection
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructure();

        // Register Views
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DashboardView>();
        builder.Services.AddTransient<TaskListView>();
        builder.Services.AddTransient<KanbanView>();
        builder.Services.AddTransient<CalendarView>();
        builder.Services.AddTransient<TimelineView>();
        builder.Services.AddTransient<ProjectListView>();
        builder.Services.AddTransient<ResourcesView>();
        builder.Services.AddTransient<AnalyticsView>();
        builder.Services.AddTransient<ActivityView>();
        builder.Services.AddTransient<FocusView>();
        builder.Services.AddTransient<TimeTrackingView>();
        builder.Services.AddTransient<NotificationView>();
        builder.Services.AddTransient<SettingsView>();
        builder.Services.AddTransient<TaskDetailPanel>();
        builder.Services.AddTransient<SearchOverlay>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Initialize SQLite Database and seed realistic demo data
        try
        {
            using var scope = app.Services.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
            Task.Run(async () => await initializer.InitializeAsync()).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
        }

        return app;
    }
}
