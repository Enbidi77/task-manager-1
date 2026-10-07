using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Database;

namespace TaskManager.Infrastructure.Repositories
{
    public class UserPreferenceRepository : Repository<UserPreference>, IUserPreferenceRepository
    {
        public UserPreferenceRepository(FlowTaskDbContext context) : base(context)
        {
        }
    }
}
