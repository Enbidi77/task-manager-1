using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces;

public interface IFocusService
{
    Task<FocusSession> StartSessionAsync(Guid? taskId, TimeSpan plannedDuration);
    Task PauseSessionAsync(Guid sessionId);
    Task ResumeSessionAsync(Guid sessionId);
    Task<FocusSession> CompleteSessionAsync(Guid sessionId);
    Task SkipSessionAsync(Guid sessionId);
    Task<FocusSession?> GetActiveSessionAsync();
    Task<IEnumerable<FocusSession>> GetSessionHistoryAsync(DateTimeOffset startDate, DateTimeOffset endDate);
    Task<IEnumerable<FocusSession>> GetTodaySessionsAsync();
}
