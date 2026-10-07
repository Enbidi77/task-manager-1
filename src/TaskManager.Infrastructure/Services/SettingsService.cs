using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Constants;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Database;

namespace TaskManager.Infrastructure.Services;

public class SettingsService : ISettingsService
{
    private readonly FlowTaskDbContext _context;
    private readonly ConcurrentDictionary<string, string> _cache = new();
    private bool _isCacheInitialized;
    private readonly object _lock = new();

    public SettingsService(FlowTaskDbContext context)
    {
        _context = context;
    }

    private async Task EnsureCacheInitializedAsync()
    {
        if (_isCacheInitialized) return;

        var preferences = await _context.UserPreferences.ToListAsync();
        lock (_lock)
        {
            if (!_isCacheInitialized)
            {
                foreach (var pref in preferences)
                {
                    _cache[pref.Key] = pref.Value;
                }
                _isCacheInitialized = true;
            }
        }
    }

    public async Task<T?> GetAsync<T>(string key, T? defaultValue = default)
    {
        await EnsureCacheInitializedAsync();

        if (_cache.TryGetValue(key, out var value))
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)value;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        return defaultValue;
    }

    public async Task SetAsync<T>(string key, T value)
    {
        await EnsureCacheInitializedAsync();

        string stringValue = typeof(T) == typeof(string)
            ? (string)(object)value!
            : JsonSerializer.Serialize(value);

        _cache[key] = stringValue;

        var pref = await _context.UserPreferences.FirstOrDefaultAsync(p => p.Key == key);
        if (pref == null)
        {
            pref = new UserPreference
            {
                Id = Guid.NewGuid(),
                Key = key,
                Value = stringValue,
                UpdatedAt = DateTimeOffset.UtcNow
            };
            await _context.UserPreferences.AddAsync(pref);
        }
        else
        {
            pref.Value = stringValue;
            pref.UpdatedAt = DateTimeOffset.UtcNow;
            _context.UserPreferences.Update(pref);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<ThemeMode> GetThemeAsync()
    {
        var themeStr = await GetAsync<string>(AppConstants.ThemeSettingKey, "System");
        if (Enum.TryParse<ThemeMode>(themeStr, true, out var theme))
        {
            return theme;
        }
        return ThemeMode.System;
    }

    public async Task SetThemeAsync(ThemeMode theme)
    {
        await SetAsync(AppConstants.ThemeSettingKey, theme.ToString());
    }

    public async Task<IDictionary<string, string>> GetAllPreferencesAsync()
    {
        await EnsureCacheInitializedAsync();
        return new Dictionary<string, string>(_cache);
    }
}
