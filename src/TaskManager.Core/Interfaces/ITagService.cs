using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces;

public interface ITagService
{
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<Tag?> GetByIdAsync(Guid id);
    Task<Tag> CreateAsync(Tag tag);
    Task<Tag> UpdateAsync(Tag tag);
    Task DeleteAsync(Guid id);
    Task AddTagToTaskAsync(Guid taskId, Guid tagId);
    Task RemoveTagFromTaskAsync(Guid taskId, Guid tagId);
}
