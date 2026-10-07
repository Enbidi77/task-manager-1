namespace TaskManager.Core.Constants;

public static class AppConstants
{
    public const string DatabaseName = "flowtask.db";
    public const string AppName = "FlowTask";
    public const int DefaultPomodoroMinutes = 25;
    public const int DefaultBreakMinutes = 5;
    public const int DefaultLongBreakMinutes = 15;
    public const int MaxSearchResults = 50;

    // Settings Keys
    public const string ThemeSettingKey = "Theme";
    public const string PomodoroDurationKey = "PomodoroDuration";
    public const string ShortBreakDurationKey = "ShortBreakDuration";
    public const string LongBreakDurationKey = "LongBreakDuration";
}
