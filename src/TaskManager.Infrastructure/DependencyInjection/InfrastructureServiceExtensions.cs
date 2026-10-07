using Microsoft.Extensions.DependencyInjection;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Database;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Infrastructure.Services;

namespace TaskManager.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Database and Initializer
            services.AddDbContext<FlowTaskDbContext>(options =>
            {
                var folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                var appDir = System.IO.Path.Combine(folder, "FlowTask");
                System.IO.Directory.CreateDirectory(appDir);
                var dbPath = System.IO.Path.Combine(appDir, TaskManager.Core.Constants.AppConstants.DatabaseName);
                Microsoft.EntityFrameworkCore.SqliteDbContextOptionsBuilderExtensions.UseSqlite(options, $"Data Source={dbPath}");
            });
            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();
            services.AddScoped<IFocusSessionRepository, FocusSessionRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<ISavedFilterRepository, SavedFilterRepository>();
            services.AddScoped<IDashboardWidgetRepository, DashboardWidgetRepository>();
            services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();

            // Services
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IDataService, DataService>();

            return services;
        }
    }
}
