using System;

namespace TaskManager.Core.Events;

public record TimerStoppedEvent(Guid TaskId, DateTimeOffset StoppedAt, TimeSpan Duration);
