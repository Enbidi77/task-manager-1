using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;

public class WorkspaceService : IWorkspaceService
{
    private readonly IWorkspaceRepository _workspaceRepository;

    public WorkspaceService(IWorkspaceRepository workspaceRepository)
    {
        _workspaceRepository = workspaceRepository;
    }

    public async Task<Workspace?> GetByIdAsync(Guid id)
    {
        return await _workspaceRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Workspace>> GetAllAsync()
    {
        var workspaces = await _workspaceRepository.GetAllAsync();
        return workspaces.OrderBy(w => w.SortOrder).ThenBy(w => w.Name);
    }

    public async Task<Workspace> CreateAsync(Workspace workspace)
    {
        workspace.CreatedAt = DateTimeOffset.UtcNow;
        workspace.UpdatedAt = DateTimeOffset.UtcNow;
        if (workspace.Id == Guid.Empty)
        {
            workspace.Id = Guid.NewGuid();
        }

        return await _workspaceRepository.AddAsync(workspace);
    }

    public async Task<Workspace> UpdateAsync(Workspace workspace)
    {
        workspace.UpdatedAt = DateTimeOffset.UtcNow;
        await _workspaceRepository.UpdateAsync(workspace);
        return workspace;
    }

    public async Task DeleteAsync(Guid id)
    {
        var workspace = await _workspaceRepository.GetByIdAsync(id);
        if (workspace != null)
        {
            await _workspaceRepository.DeleteAsync(workspace);
        }
    }
}
