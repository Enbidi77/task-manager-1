using System;
using Microsoft.Maui.Controls;
using TaskManager.Application.ViewModels;

namespace TaskManager.App.Views;

public partial class SearchOverlay : ContentView
{
    private SearchViewModel? _viewModel;
    public event EventHandler? CloseRequested;

    public SearchOverlay()
    {
        InitializeComponent();
    }

    public SearchOverlay(SearchViewModel viewModel) : this()
    {
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    private async void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        var vm = _viewModel ?? BindingContext as SearchViewModel;
        if (vm != null)
        {
            await vm.SearchCommand.ExecuteAsync(null);
        }
    }

    private void OnCloseClicked(object? sender, EventArgs e)
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}
