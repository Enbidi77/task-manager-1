using System;
using Microsoft.Maui.Controls;
using TaskManager.Application.ViewModels;

namespace TaskManager.App.Views;

public partial class AnalyticsView : ContentView
{
    private readonly AnalyticsViewModel _viewModel;

    public AnalyticsView(AnalyticsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        await _viewModel.LoadDataAsync();
    }

    private async void OnRefreshClicked(object? sender, EventArgs e)
    {
        await _viewModel.LoadDataAsync();
    }
}
