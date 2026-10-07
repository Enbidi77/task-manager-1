using System;

namespace TaskManager.Core.Events;

public record TimerStartedEvent(Guid TaskId, DateTimeOffset StartedAt);
