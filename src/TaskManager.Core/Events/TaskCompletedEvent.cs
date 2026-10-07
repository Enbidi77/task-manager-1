using System;

namespace TaskManager.Core.Events;

public record TaskCompletedEvent(Guid TaskId, DateTimeOffset CompletedAt);
