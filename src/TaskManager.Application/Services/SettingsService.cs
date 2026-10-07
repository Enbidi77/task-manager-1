using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services
{
    public class SettingsService : ISettingsService
    {
        public async Task<T?> GetAsync<T>(string key, T? defaultValue = default)
        {
            return await Task.FromResult(defaultValue);
        }

        public async Task SetAsync<T>(string key, T value)
        {
            await Task.CompletedTask;
        }

        public async Task<ThemeMode> GetThemeAsync()
        {
            return await Task.FromResult(ThemeMode.System);
        }

        public async Task SetThemeAsync(ThemeMode theme)
        {
            await Task.CompletedTask;
        }

        public async Task<IDictionary<string, string>> GetAllPreferencesAsync()
        {
            return new Dictionary<string, string>();
        }
    }
}
