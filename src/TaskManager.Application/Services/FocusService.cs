using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Events;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;

public class FocusService : IFocusService
{
    private readonly IFocusSessionRepository _focusSessionRepository;
    private readonly IEventAggregator _eventAggregator;

    public FocusService(IFocusSessionRepository focusSessionRepository, IEventAggregator eventAggregator)
    {
        _focusSessionRepository = focusSessionRepository;
        _eventAggregator = eventAggregator;
    }

    public async Task<FocusSession> StartSessionAsync(Guid? taskId, TimeSpan plannedDuration)
    {
        var todaySessions = (await GetTodaySessionsAsync()).ToList();
        int sessionNum = todaySessions.Count + 1;

        var session = new FocusSession
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            StartTime = DateTimeOffset.UtcNow,
            PlannedDuration = plannedDuration,
            SessionNumber = sessionNum,
            IsCompleted = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        return await _focusSessionRepository.AddAsync(session);
    }

    public async Task PauseSessionAsync(Guid sessionId)
    {
        // Session state tracking handled in memory/ViewModel
        await Task.CompletedTask;
    }

    public async Task ResumeSessionAsync(Guid sessionId)
    {
        await Task.CompletedTask;
    }

    public async Task<FocusSession> CompleteSessionAsync(Guid sessionId)
    {
        var session = await _focusSessionRepository.GetByIdAsync(sessionId);
        if (session != null)
        {
            session.EndTime = DateTimeOffset.UtcNow;
            session.ActualDuration = session.EndTime.Value - session.StartTime;
            session.IsCompleted = true;

            await _focusSessionRepository.UpdateAsync(session);
            _eventAggregator.Publish(new FocusSessionCompletedEvent(sessionId, session.TaskId, session.ActualDuration ?? session.PlannedDuration));
            return session;
        }

        return new FocusSession { Id = sessionId, IsCompleted = true };
    }

    public async Task SkipSessionAsync(Guid sessionId)
    {
        var session = await _focusSessionRepository.GetByIdAsync(sessionId);
        if (session != null)
        {
            session.EndTime = DateTimeOffset.UtcNow;
            session.ActualDuration = session.EndTime.Value - session.StartTime;
            session.IsCompleted = false;
            await _focusSessionRepository.UpdateAsync(session);
        }
    }

    public async Task<FocusSession?> GetActiveSessionAsync()
    {
        var sessions = await _focusSessionRepository.FindAsync(s => s.EndTime == null);
        return sessions.OrderByDescending(s => s.StartTime).FirstOrDefault();
    }

    public async Task<IEnumerable<FocusSession>> GetSessionHistoryAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var sessions = await _focusSessionRepository.FindAsync(s => s.StartTime >= startDate && s.StartTime <= endDate);
        return sessions.OrderByDescending(s => s.StartTime);
    }

    public async Task<IEnumerable<FocusSession>> GetTodaySessionsAsync()
    {
        var now = DateTimeOffset.UtcNow;
        var today = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero);
        var tomorrow = today.AddDays(1);
        var sessions = await _focusSessionRepository.FindAsync(s => s.StartTime >= today && s.StartTime < tomorrow);
        return sessions.OrderBy(s => s.StartTime);
    }
}
