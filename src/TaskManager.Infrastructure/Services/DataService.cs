using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Database;

namespace TaskManager.Infrastructure.Services;

public class DataService : IDataService
{
    private readonly FlowTaskDbContext _context;

    public DataService(FlowTaskDbContext context)
    {
        _context = context;
    }

    public async Task<string> ExportTasksAsync(ExportFormat format)
    {
        var tasks = await _context.TaskItems
            .Include(t => t.Project)
            .Include(t => t.Tags)
                .ThenInclude(tt => tt.Tag)
            .ToListAsync();

        if (format == ExportFormat.Json)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(tasks, options);
        }
        else if (format == ExportFormat.Csv)
        {
            var sw = new StringWriter();
            sw.WriteLine("Id,Title,Description,Status,Priority,DueDate,ProjectName");
            foreach (var t in tasks)
            {
                var projName = t.Project?.Name?.Replace(",", " ") ?? "";
                var title = t.Title?.Replace(",", " ") ?? "";
                var desc = t.Description?.Replace(",", " ")?.Replace("\n", " ") ?? "";
                sw.WriteLine($"{t.Id},{title},{desc},{t.Status},{t.Priority},{t.DueDate},{projName}");
            }
            return sw.ToString();
        }

        throw new NotSupportedException($"Export format '{format}' is not supported.");
    }

    public async Task ImportTasksAsync(string data, ExportFormat format)
    {
        if (format == ExportFormat.Json)
        {
            var tasks = JsonSerializer.Deserialize<List<TaskItem>>(data);
            if (tasks != null)
            {
                foreach (var task in tasks)
                {
                    var existing = await _context.TaskItems.FindAsync(task.Id);
                    if (existing == null)
                    {
                        task.Project = null;
                        task.Workspace = null;
                        task.ParentTask = null;
                        task.Tags = new List<TaskTag>();
                        
                        await _context.TaskItems.AddAsync(task);
                    }
                }
                await _context.SaveChangesAsync();
            }
        }
        else
        {
            throw new NotSupportedException($"Import format '{format}' is not supported.");
        }
    }

    public async Task<string> BackupDatabaseAsync()
    {
        var dbConnection = _context.Database.GetDbConnection();
        var currentDbPath = dbConnection.DataSource;

        if (string.IsNullOrEmpty(currentDbPath) || !File.Exists(currentDbPath))
        {
            throw new InvalidOperationException("Source database file not found.");
        }

        var backupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FlowTask", "Backups");
        Directory.CreateDirectory(backupDir);
        var backupPath = Path.Combine(backupDir, $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");

        await Task.Run(() => File.Copy(currentDbPath, backupPath, true));
        return backupPath;
    }

    public async Task RestoreDatabaseAsync(string backupPath)
    {
        if (!File.Exists(backupPath))
        {
            throw new FileNotFoundException("Backup file not found.", backupPath);
        }

        var dbConnection = _context.Database.GetDbConnection();
        var currentDbPath = dbConnection.DataSource;

        if (string.IsNullOrEmpty(currentDbPath))
        {
            throw new InvalidOperationException("Current database path not found.");
        }

        await dbConnection.CloseAsync();
        await Task.Run(() => File.Copy(backupPath, currentDbPath, true));
    }
}
