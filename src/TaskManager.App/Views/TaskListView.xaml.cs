using System;
using Microsoft.Maui.Controls;
using TaskManager.Application.ViewModels;

namespace TaskManager.App.Views;

public partial class TaskListView : ContentView
{
    private readonly TaskListViewModel _viewModel;

    public TaskListView(TaskListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        await _viewModel.LoadTasksCommand.ExecuteAsync(null);
    }

    private async void OnRefreshClicked(object? sender, EventArgs e)
    {
        await _viewModel.LoadTasksCommand.ExecuteAsync(null);
    }

    private async void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        await _viewModel.LoadTasksCommand.ExecuteAsync(null);
    }

    private void OnTaskSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // SelectedTask is bound via two-way binding
    }

    private void OnCloseDetailsClicked(object? sender, EventArgs e)
    {
        _viewModel.SelectedTask = null!;
    }
}
