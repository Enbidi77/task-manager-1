using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.ViewModels
{
    public partial class ActivityViewModel : ObservableObject
    {
        private readonly IActivityService _activityService;

        public ObservableCollection<Activity> Activities { get; } = new();

        [ObservableProperty]
        private string _filterType = "All";

        public ActivityViewModel(IActivityService activityService)
        {
            _activityService = activityService;
        }

        public async System.Threading.Tasks.Task LoadInitialAsync()
        {
            Activities.Clear();
            await LoadMoreAsync();
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task LoadMoreAsync()
        {
            var more = await _activityService.GetRecentAsync(50);
            foreach (var a in more)
            {
                Activities.Add(a);
            }
        }
    }
}
