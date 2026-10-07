using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Core.Enums;

namespace TaskManager.Core.Interfaces;

public interface ISettingsService
{
    Task<T?> GetAsync<T>(string key, T? defaultValue = default);
    Task SetAsync<T>(string key, T value);
    Task<ThemeMode> GetThemeAsync();
    Task SetThemeAsync(ThemeMode theme);
    Task<IDictionary<string, string>> GetAllPreferencesAsync();
}
