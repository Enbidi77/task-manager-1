using System.Threading.Tasks;

namespace TaskManager.Core.Interfaces;

public interface IDataService
{
    Task<string> ExportTasksAsync(TaskManager.Core.Enums.ExportFormat format);
    Task ImportTasksAsync(string data, TaskManager.Core.Enums.ExportFormat format);
    Task<string> BackupDatabaseAsync();
    Task RestoreDatabaseAsync(string backupPath);
}
