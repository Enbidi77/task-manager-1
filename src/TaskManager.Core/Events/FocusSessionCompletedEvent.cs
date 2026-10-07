using System;

namespace TaskManager.Core.Events;

public record FocusSessionCompletedEvent(Guid SessionId, Guid? TaskId, TimeSpan Duration);
