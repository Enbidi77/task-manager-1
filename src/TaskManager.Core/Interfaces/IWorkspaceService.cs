using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces;

public interface IWorkspaceService
{
    Task<IEnumerable<Workspace>> GetAllAsync();
    Task<Workspace?> GetByIdAsync(Guid id);
    Task<Workspace> CreateAsync(Workspace workspace);
    Task<Workspace> UpdateAsync(Workspace workspace);
    Task DeleteAsync(Guid id);
}
