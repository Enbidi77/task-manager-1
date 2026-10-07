using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Core.DTOs;

namespace TaskManager.Core.Interfaces;

public interface IAnalyticsService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<double> GetCompletionRateAsync(DateTimeOffset startDate, DateTimeOffset endDate);
    Task<IEnumerable<ProductivityDataDto>> GetProductivityByDayAsync(int days);
    Task<IEnumerable<ProductivityDataDto>> GetProductivityByProjectAsync(Guid projectId);
    Task<IDictionary<string, TimeSpan>> GetTimeDistributionAsync(DateTimeOffset startDate, DateTimeOffset endDate);
    Task<IEnumerable<WorkloadDataDto>> GetWorkloadHeatmapAsync();
    Task<IDictionary<DateTimeOffset, int>> GetTasksCompletedPerDayAsync(int days);
}
