using System;
using TaskManager.Core.Enums;

namespace TaskManager.Core.Events;

public record StatusChangedEvent(Guid TaskId, TaskItemStatus OldStatus, TaskItemStatus NewStatus);
