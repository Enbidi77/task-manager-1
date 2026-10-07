using System;

namespace TaskManager.Core.DTOs;

public class SearchResultDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Task, Project, Tag
    public string? MatchSnippet { get; set; }
}
