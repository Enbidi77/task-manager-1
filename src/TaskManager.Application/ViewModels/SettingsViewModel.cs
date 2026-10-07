using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskManager.Core.Constants;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly IDataService _dataService;

    [ObservableProperty]
    private ThemeMode _themeMode = ThemeMode.System;

    [ObservableProperty]
    private string _accentColor = "#0078D4";

    [ObservableProperty]
    private TaskPriority _defaultPriority = TaskPriority.Medium;

    [ObservableProperty]
    private TaskItemStatus _defaultStatus = TaskItemStatus.Todo;

    [ObservableProperty]
    private int _pomodoroDuration = 25;

    [ObservableProperty]
    private int _breakDuration = 5;

    [ObservableProperty]
    private bool _notificationsEnabled = true;

    [ObservableProperty]
    private string _language = "English";

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public SettingsViewModel(ISettingsService settingsService, IDataService dataService)
    {
        _settingsService = settingsService;
        _dataService = dataService;
    }

    public async Task LoadSettingsAsync()
    {
        ThemeMode = await _settingsService.GetThemeAsync();
        PomodoroDuration = await _settingsService.GetAsync<int>("PomodoroDuration", 25);
        BreakDuration = await _settingsService.GetAsync<int>("ShortBreakDuration", 5);
        AccentColor = await _settingsService.GetAsync<string>("AccentColor", "#0078D4") ?? "#0078D4";
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await _settingsService.SetThemeAsync(ThemeMode);
        await _settingsService.SetAsync("PomodoroDuration", PomodoroDuration);
        await _settingsService.SetAsync("ShortBreakDuration", BreakDuration);
        await _settingsService.SetAsync("AccentColor", AccentColor);
        StatusMessage = "Settings successfully saved!";
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        var json = await _dataService.ExportTasksAsync(ExportFormat.Json);
        StatusMessage = $"Exported {json.Length} characters of data to JSON.";
    }

    [RelayCommand]
    private async Task BackupAsync()
    {
        var path = await _dataService.BackupDatabaseAsync();
        StatusMessage = $"Backup saved to: {path}";
    }
}
