using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Application.ViewModels
{
    public partial class FocusViewModel : ObservableObject
    {
        private readonly IFocusService _focusService;
        private PeriodicTimer _timer;
        private CancellationTokenSource _cts;

        [ObservableProperty]
        private TaskItem _currentTask;
        
        private FocusSession _currentSession;

        [ObservableProperty]
        private TimeSpan _remainingTime;

        [ObservableProperty]
        private TimeSpan _elapsedTime;

        [ObservableProperty]
        private double _progress;

        [ObservableProperty]
        private int _sessionNumber;

        [ObservableProperty]
        private bool _isRunning;

        [ObservableProperty]
        private bool _isPaused;

        public FocusViewModel(IFocusService focusService)
        {
            _focusService = focusService;
            RemainingTime = TimeSpan.FromMinutes(25);
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task StartAsync()
        {
            if (CurrentTask == null) return;

            _currentSession = await _focusService.StartSessionAsync(CurrentTask.Id, TimeSpan.FromMinutes(25));
            IsRunning = true;
            IsPaused = false;
            
            _cts = new CancellationTokenSource();
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            
            _ = RunTimerAsync();
        }

        private async System.Threading.Tasks.Task RunTimerAsync()
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(_cts.Token))
                {
                    if (IsPaused) continue;

                    ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(1));
                    RemainingTime = RemainingTime.Subtract(TimeSpan.FromSeconds(1));
                    Progress = 1 - (RemainingTime.TotalSeconds / (25 * 60));

                    if (RemainingTime.TotalSeconds <= 0)
                    {
                        await FinishAsync();
                        break;
                    }
                }
            }
            catch (OperationCanceledException) { }
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task PauseAsync()
        {
            IsPaused = true;
            if (_currentSession != null)
                await _focusService.PauseSessionAsync(_currentSession.Id);
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task ResumeAsync()
        {
            IsPaused = false;
            if (_currentSession != null)
                await _focusService.ResumeSessionAsync(_currentSession.Id);
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task SkipAsync()
        {
            StopInternalTimer();
            if (_currentSession != null)
                await _focusService.SkipSessionAsync(_currentSession.Id);
            ResetTimer();
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task FinishAsync()
        {
            StopInternalTimer();
            if (_currentSession != null)
                await _focusService.CompleteSessionAsync(_currentSession.Id);
            ResetTimer();
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task SelectTaskAsync(TaskItem task)
        {
            CurrentTask = task;
            await System.Threading.Tasks.Task.CompletedTask;
        }

        private void StopInternalTimer()
        {
            _cts?.Cancel();
            _timer?.Dispose();
            IsRunning = false;
            IsPaused = false;
        }

        private void ResetTimer()
        {
            RemainingTime = TimeSpan.FromMinutes(25);
            ElapsedTime = TimeSpan.Zero;
            Progress = 0;
        }
    }
}
