using AsyncAwait.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AsyncAwait.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private TaskService _taskService;
        public TaskService TaskService => _taskService ?? (_taskService = InitializeTaskService());

        private string _status = "Status: \n";
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        private TaskService InitializeTaskService()
        {
            var taskService = new TaskService();
            taskService.TaskCreated += HandleTaskStatusUpdated;
            taskService.TaskStarting += HandleTaskStatusUpdated;
            taskService.TaskCompleted += HandleTaskStatusUpdated;
            taskService.TaskFaulted += HandleTaskStatusUpdated;
            return taskService;
        }

        private void HandleTaskStatusUpdated(object sender, TaskStatusChangedEventArgs status)
        {
            Console.WriteLine(status.Status);
            PrintStatus($"{status.Status}");
        }

        public void ClearStatus() => Status = string.Empty;

        public void PrintStatus(string input) => Device.BeginInvokeOnMainThread(() => Status += $"{input}\n");

        public void PrintDot() => Status += ".";

        public void PrintThreadCheck(bool addSpacing = true) => Status += $"{(addSpacing ? "\n" : "")}" +
            $"==={(MainThread.IsMainThread ? "Is" : "Not")} on MainThread===\n" +
            $"{(addSpacing ? "\n" : "")}";

        #region Forms BaseViewModel
        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #endregion
    }
}
