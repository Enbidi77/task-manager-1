using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces;

public interface IDashboardService
{
    Task<IEnumerable<DashboardWidget>> GetWidgetsAsync();
    Task UpdateWidgetLayoutAsync(IEnumerable<DashboardWidget> widgets);
    Task ToggleWidgetVisibilityAsync(Guid widgetId, bool isVisible);
    Task ResetLayoutAsync();
}
