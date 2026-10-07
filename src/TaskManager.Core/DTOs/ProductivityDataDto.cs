using System;

namespace TaskManager.Core.DTOs;

public class ProductivityDataDto
{
    public DateTimeOffset Date { get; set; }
    public int TasksCompleted { get; set; }
    public double FocusHours { get; set; }
}
