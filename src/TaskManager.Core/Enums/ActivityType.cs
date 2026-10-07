namespace TaskManager.Core.Enums;

public enum ActivityType
{
    TaskCreated = 0,
    TaskCompleted = 1,
    TaskUpdated = 2,
    TaskDeleted = 3,
    TaskMoved = 4,
    StatusChanged = 5,
    PriorityChanged = 6,
    TagAdded = 7,
    TagRemoved = 8,
    CommentAdded = 9,
    TimeEntryAdded = 10,
    FocusSessionStarted = 11,
    FocusSessionCompleted = 12,
    ProjectCreated = 13,
    ProjectUpdated = 14
}
