using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.DTOs;

namespace TaskManager.Core.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(Guid id);
    Task<Project> CreateAsync(Project project);
    Task<Project> UpdateAsync(Project project);
    Task DeleteAsync(Guid id);
    Task<ProjectStatsDto> GetProjectStatsAsync(Guid id);
}
