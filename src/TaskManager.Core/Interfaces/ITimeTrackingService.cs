using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.DTOs;

namespace TaskManager.Core.Interfaces;

public interface ITimeTrackingService
{
    Task<TimeEntry> StartTimerAsync(Guid taskId);
    Task<TimeEntry> StopTimerAsync(Guid taskId, string? notes = null);
    Task<TimeEntry?> GetActiveTimerAsync();
    Task<IEnumerable<TimeEntry>> GetEntriesForTaskAsync(Guid taskId);
    Task<TimeSummaryDto> GetDailySummaryAsync(DateTimeOffset date);
    Task<TimeEntry> CreateManualEntryAsync(TimeEntry entry);
    Task<TimeEntry> UpdateEntryAsync(TimeEntry entry);
    Task DeleteEntryAsync(Guid id);
}
