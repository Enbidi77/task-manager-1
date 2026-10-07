using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.DTOs;
using TaskManager.Core.Entities;
using TaskManager.Core.Events;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;

public class TimeTrackingService : ITimeTrackingService
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IEventAggregator _eventAggregator;

    public TimeTrackingService(
        ITimeEntryRepository timeEntryRepository,
        ITaskRepository taskRepository,
        IEventAggregator eventAggregator)
    {
        _timeEntryRepository = timeEntryRepository;
        _taskRepository = taskRepository;
        _eventAggregator = eventAggregator;
    }

    public async Task<TimeEntry> StartTimerAsync(Guid taskId)
    {
        // Stop any running timer first
        var active = await GetActiveTimerAsync();
        if (active != null)
        {
            await StopTimerAsync(active.TaskId);
        }

        var entry = new TimeEntry
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            StartTime = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var created = await _timeEntryRepository.AddAsync(entry);
        _eventAggregator.Publish(new TimerStartedEvent(taskId, entry.StartTime));
        return created;
    }

    public async Task<TimeEntry> StopTimerAsync(Guid taskId, string? notes = null)
    {
        var entries = await _timeEntryRepository.FindAsync(e => e.TaskId == taskId && e.EndTime == null);
        var active = entries.OrderByDescending(e => e.StartTime).FirstOrDefault();

        if (active == null)
        {
            var fallback = new TimeEntry
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                StartTime = DateTimeOffset.UtcNow.AddMinutes(-1),
                EndTime = DateTimeOffset.UtcNow,
                Duration = TimeSpan.FromMinutes(1),
                Notes = notes,
                CreatedAt = DateTimeOffset.UtcNow
            };
            await _timeEntryRepository.AddAsync(fallback);
            active = fallback;
        }
        else
        {
            active.EndTime = DateTimeOffset.UtcNow;
            active.Duration = active.EndTime.Value - active.StartTime;
            if (!string.IsNullOrWhiteSpace(notes))
            {
                active.Notes = notes;
            }
            await _timeEntryRepository.UpdateAsync(active);
        }

        // Update task's total actual duration
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task != null)
        {
            var allEntries = await _timeEntryRepository.FindAsync(e => e.TaskId == taskId && e.Duration.HasValue);
            var totalSeconds = allEntries.Sum(e => e.Duration!.Value.TotalSeconds);
            task.ActualDuration = TimeSpan.FromSeconds(totalSeconds);
            await _taskRepository.UpdateAsync(task);
        }

        _eventAggregator.Publish(new TimerStoppedEvent(taskId, active.EndTime!.Value, active.Duration ?? TimeSpan.Zero));
        return active;
    }

    public async Task<TimeEntry?> GetActiveTimerAsync()
    {
        var entries = await _timeEntryRepository.FindAsync(e => e.EndTime == null);
        return entries.OrderByDescending(e => e.StartTime).FirstOrDefault();
    }

    public async Task<IEnumerable<TimeEntry>> GetEntriesForTaskAsync(Guid taskId)
    {
        var entries = await _timeEntryRepository.FindAsync(e => e.TaskId == taskId);
        return entries.OrderByDescending(e => e.StartTime);
    }

    public async Task<TimeSummaryDto> GetDailySummaryAsync(DateTimeOffset date)
    {
        var targetDate = new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, date.Offset);
        var nextDate = targetDate.AddDays(1);

        var entries = (await _timeEntryRepository.FindAsync(e => e.StartTime >= targetDate && e.StartTime < nextDate && e.Duration.HasValue)).ToList();
        var total = TimeSpan.FromSeconds(entries.Sum(e => e.Duration!.Value.TotalSeconds));

        return new TimeSummaryDto
        {
            TotalTracked = total,
            TotalFocusTime = total,
            FocusSessionsCompleted = entries.Count
        };
    }

    public async Task<TimeEntry> CreateManualEntryAsync(TimeEntry entry)
    {
        entry.CreatedAt = DateTimeOffset.UtcNow;
        if (entry.Id == Guid.Empty)
        {
            entry.Id = Guid.NewGuid();
        }

        if (entry.EndTime.HasValue && entry.StartTime < entry.EndTime.Value)
        {
            entry.Duration = entry.EndTime.Value - entry.StartTime;
        }

        var created = await _timeEntryRepository.AddAsync(entry);

        var task = await _taskRepository.GetByIdAsync(entry.TaskId);
        if (task != null)
        {
            var allEntries = await _timeEntryRepository.FindAsync(e => e.TaskId == entry.TaskId && e.Duration.HasValue);
            var totalSeconds = allEntries.Sum(e => e.Duration!.Value.TotalSeconds);
            task.ActualDuration = TimeSpan.FromSeconds(totalSeconds);
            await _taskRepository.UpdateAsync(task);
        }

        return created;
    }

    public async Task<TimeEntry> UpdateEntryAsync(TimeEntry entry)
    {
        if (entry.EndTime.HasValue && entry.StartTime < entry.EndTime.Value)
        {
            entry.Duration = entry.EndTime.Value - entry.StartTime;
        }

        await _timeEntryRepository.UpdateAsync(entry);

        var task = await _taskRepository.GetByIdAsync(entry.TaskId);
        if (task != null)
        {
            var allEntries = await _timeEntryRepository.FindAsync(e => e.TaskId == entry.TaskId && e.Duration.HasValue);
            var totalSeconds = allEntries.Sum(e => e.Duration!.Value.TotalSeconds);
            task.ActualDuration = TimeSpan.FromSeconds(totalSeconds);
            await _taskRepository.UpdateAsync(task);
        }

        return entry;
    }

    public async Task DeleteEntryAsync(Guid id)
    {
        var entry = await _timeEntryRepository.GetByIdAsync(id);
        if (entry != null)
        {
            var taskId = entry.TaskId;
            await _timeEntryRepository.DeleteAsync(entry);

            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task != null)
            {
                var allEntries = await _timeEntryRepository.FindAsync(e => e.TaskId == taskId && e.Duration.HasValue);
                var totalSeconds = allEntries.Sum(e => e.Duration!.Value.TotalSeconds);
                task.ActualDuration = TimeSpan.FromSeconds(totalSeconds);
                await _taskRepository.UpdateAsync(task);
            }
        }
    }
}
