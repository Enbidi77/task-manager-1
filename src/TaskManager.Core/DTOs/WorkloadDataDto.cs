using System;

namespace TaskManager.Core.DTOs;

public class WorkloadDataDto
{
    public Guid AssigneeId { get; set; }
    public string AssigneeName { get; set; } = string.Empty;
    public int ActiveTasks { get; set; }
    public double EstimatedHoursRemaining { get; set; }
}
