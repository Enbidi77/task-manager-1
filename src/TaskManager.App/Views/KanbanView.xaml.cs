using System;
using Microsoft.Maui.Controls;
using TaskManager.Application.ViewModels;

namespace TaskManager.App.Views;

public partial class KanbanView : ContentView
{
    private readonly KanbanViewModel _viewModel;

    public KanbanView(KanbanViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        await _viewModel.LoadTasksAsync();
    }

    private async void OnRefreshClicked(object? sender, EventArgs e)
    {
        await _viewModel.LoadTasksAsync();
    }
}
