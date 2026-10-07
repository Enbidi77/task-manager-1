using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Core.DTOs;

namespace TaskManager.Core.Interfaces;

public interface ISearchService
{
    Task<IEnumerable<SearchResultDto>> SearchAsync(SearchQuery query);
}
