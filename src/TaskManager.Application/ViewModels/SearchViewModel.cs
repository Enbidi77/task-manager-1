using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TaskManager.Core.DTOs;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.ViewModels
{
    public partial class SearchViewModel : ObservableObject
    {
        private readonly ISearchService _searchService;

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private bool _isOpen;

        public ObservableCollection<SearchResultDto> Results { get; } = new();

        public SearchViewModel(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Results.Clear();
                return;
            }

            var results = await _searchService.SearchAsync(new SearchQuery { Text = SearchText });
            Results.Clear();
            foreach (var r in results)
            {
                Results.Add(r);
            }
        }

        [RelayCommand]
        private void SelectResult(SearchResultDto result)
        {
            // Navigation logic based on result type
            IsOpen = false;
        }

        [RelayCommand]
        private void Clear()
        {
            SearchText = string.Empty;
            Results.Clear();
        }
    }
}
