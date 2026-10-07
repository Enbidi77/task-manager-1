using System;

namespace TaskManager.Core.Events;

public record TaskCreatedEvent(Guid TaskId);
