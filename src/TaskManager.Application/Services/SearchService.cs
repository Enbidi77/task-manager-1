using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.DTOs;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.Services;

public class SearchService : ISearchService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ITagRepository _tagRepository;

    public SearchService(
        ITaskRepository taskRepository,
        IProjectRepository projectRepository,
        ITagRepository tagRepository)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _tagRepository = tagRepository;
    }

    public async Task<IEnumerable<SearchResultDto>> SearchAsync(SearchQuery query)
    {
        var results = new List<SearchResultDto>();
        var text = query.Text?.Trim().ToLower() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(text)) return results;

        if (query.IncludeTasks)
        {
            var tasks = await _taskRepository.SearchAsync(text);
            foreach (var t in tasks)
            {
                if (query.Status.HasValue && t.Status != query.Status.Value) continue;
                if (query.Priority.HasValue && t.Priority != query.Priority.Value) continue;

                results.Add(new SearchResultDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Type = "Task",
                    MatchSnippet = t.Description?.Length > 60 ? t.Description[..60] + "..." : t.Description
                });
            }
        }

        if (query.IncludeProjects)
        {
            var projects = await _projectRepository.FindAsync(p => p.Name.ToLower().Contains(text) || (p.Description != null && p.Description.ToLower().Contains(text)));
            foreach (var p in projects)
            {
                results.Add(new SearchResultDto
                {
                    Id = p.Id,
                    Title = p.Name,
                    Type = "Project",
                    MatchSnippet = p.Description
                });
            }
        }

        if (query.IncludeTags)
        {
            var tags = await _tagRepository.FindAsync(t => t.Name.ToLower().Contains(text));
            foreach (var tag in tags)
            {
                results.Add(new SearchResultDto
                {
                    Id = tag.Id,
                    Title = "#" + tag.Name,
                    Type = "Tag",
                    MatchSnippet = "Tag"
                });
            }
        }

        return results;
    }
}
