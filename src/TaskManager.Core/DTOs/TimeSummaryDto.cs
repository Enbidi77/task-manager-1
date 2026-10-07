using System;

namespace TaskManager.Core.DTOs;

public class TimeSummaryDto
{
    public TimeSpan TotalTracked { get; set; }
    public TimeSpan TotalFocusTime { get; set; }
    public int FocusSessionsCompleted { get; set; }
}
