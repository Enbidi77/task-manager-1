using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Database;

namespace TaskManager.Infrastructure.Repositories
{
    public class TimeEntryRepository : Repository<TimeEntry>, ITimeEntryRepository
    {
        public TimeEntryRepository(FlowTaskDbContext context) : base(context)
        {
        }
    }
}
