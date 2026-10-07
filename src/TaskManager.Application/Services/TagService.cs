using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly ITaskRepository _taskRepository;

    public TagService(ITagRepository tagRepository, ITaskRepository taskRepository)
    {
        _tagRepository = tagRepository;
        _taskRepository = taskRepository;
    }

    public async Task<Tag?> GetByIdAsync(Guid id)
    {
        return await _tagRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        var tags = await _tagRepository.GetAllAsync();
        return tags.OrderBy(t => t.Name);
    }

    public async Task<Tag> CreateAsync(Tag tag)
    {
        tag.CreatedAt = DateTimeOffset.UtcNow;
        if (tag.Id == Guid.Empty)
        {
            tag.Id = Guid.NewGuid();
        }

        return await _tagRepository.AddAsync(tag);
    }

    public async Task<Tag> UpdateAsync(Tag tag)
    {
        await _tagRepository.UpdateAsync(tag);
        return tag;
    }

    public async Task DeleteAsync(Guid id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        if (tag != null)
        {
            await _tagRepository.DeleteAsync(tag);
        }
    }

    public async Task AddTagToTaskAsync(Guid taskId, Guid tagId)
    {
        var task = await _taskRepository.GetWithDetailsAsync(taskId);
        if (task != null && !task.Tags.Any(t => t.TagId == tagId))
        {
            task.Tags.Add(new TaskTag { TaskId = taskId, TagId = tagId });
            task.UpdatedAt = DateTimeOffset.UtcNow;
            await _taskRepository.UpdateAsync(task);
        }
    }

    public async Task RemoveTagFromTaskAsync(Guid taskId, Guid tagId)
    {
        var task = await _taskRepository.GetWithDetailsAsync(taskId);
        if (task != null)
        {
            var item = task.Tags.FirstOrDefault(t => t.TagId == tagId);
            if (item != null)
            {
                task.Tags.Remove(item);
                task.UpdatedAt = DateTimeOffset.UtcNow;
                await _taskRepository.UpdateAsync(task);
            }
        }
    }
}
