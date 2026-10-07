using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Database;

namespace TaskManager.Infrastructure.Repositories
{
    public class DashboardWidgetRepository : Repository<DashboardWidget>, IDashboardWidgetRepository
    {
        public DashboardWidgetRepository(FlowTaskDbContext context) : base(context)
        {
        }
    }
}
