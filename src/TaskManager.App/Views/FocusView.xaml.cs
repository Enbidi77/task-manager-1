using Microsoft.Maui.Controls;
using TaskManager.Application.ViewModels;

namespace TaskManager.App.Views;

public partial class FocusView : ContentView
{
    public FocusView(FocusViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
