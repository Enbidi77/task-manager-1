using System;
using Microsoft.Maui.Controls;
using TaskManager.Application.ViewModels;

namespace TaskManager.App.Views;

public partial class ResourcesView : ContentView
{
    private readonly ResourcesViewModel _viewModel;

    public ResourcesView(ResourcesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        await _viewModel.RefreshCommand.ExecuteAsync(null);
    }
}
