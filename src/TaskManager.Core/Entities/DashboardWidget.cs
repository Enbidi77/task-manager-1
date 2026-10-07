using System;

namespace TaskManager.Core.Entities;

public class DashboardWidget
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string WidgetType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Column { get; set; }
    public int Row { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string? ConfigJson { get; set; }
    public bool IsVisible { get; set; }
    public int SortOrder { get; set; }
}
