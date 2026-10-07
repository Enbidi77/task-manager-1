using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardWidgetRepository _widgetRepository;

    public DashboardService(IDashboardWidgetRepository widgetRepository)
    {
        _widgetRepository = widgetRepository;
    }

    public async Task<IEnumerable<DashboardWidget>> GetWidgetsAsync()
    {
        var widgets = await _widgetRepository.GetAllAsync();
        return widgets.OrderBy(w => w.SortOrder);
    }

    public async Task UpdateWidgetLayoutAsync(IEnumerable<DashboardWidget> widgets)
    {
        foreach (var w in widgets)
        {
            await _widgetRepository.UpdateAsync(w);
        }
    }

    public async Task ToggleWidgetVisibilityAsync(Guid widgetId, bool isVisible)
    {
        var widget = await _widgetRepository.GetByIdAsync(widgetId);
        if (widget != null)
        {
            widget.IsVisible = isVisible;
            await _widgetRepository.UpdateAsync(widget);
        }
    }

    public async Task ResetLayoutAsync()
    {
        var all = (await _widgetRepository.GetAllAsync()).ToList();
        int order = 1;
        foreach (var w in all)
        {
            w.IsVisible = true;
            w.SortOrder = order++;
            await _widgetRepository.UpdateAsync(w);
        }
    }
}
