using AsyncAwait.Services;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.ViewModels
{
    public class BadExamplesViewModel3 : BaseViewModel
    {
        #region Example: Not using longrunning tasks

        private AsyncCommand _badLongRunningTaskCommand;
        public AsyncCommand BadLongRunningTaskCommand => _badLongRunningTaskCommand ?? (_badLongRunningTaskCommand = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command starting");

            var taskCount = 100;
            PrintStatus($"Running {taskCount} long running tasks that print a \".\" every second using Task.Run, then running {taskCount} tasks which complete in 2 seconds.");

            var stopwatch = new Stopwatch();
            stopwatch.Start();


            Task[] longRunningTasks = new Task[taskCount];

            for (int i = 0; i < taskCount; i++)
            {
                //Fire and forget long running task using Task.Run 
                //For example purposes only. Real fire and forget tasks should always have a cancel mechanism and exception handling!!
                Task.Run(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(millisecondsTimeout: (int)TimeSpan.FromSeconds(1).TotalMilliseconds);
                        PrintDot();
                    }
                });
            }

            //Then run normal Task.Runs
            Task<string>[] tasks = new Task<string>[taskCount];
            for (int i = 0; i < taskCount; i++)
            {
                var task = TaskService.GetStringWithTaskRunAsync($"Task {i}");
                tasks[i] = task;
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            PrintStatus($"Command ending after: {stopwatch.Elapsed.TotalSeconds}s");

            PrintStatus("Command ending");
        }));


        private AsyncCommand _goodLongRunningTaskCommand;
        public AsyncCommand GoodLongRunningTaskCommand => _goodLongRunningTaskCommand ?? (_goodLongRunningTaskCommand = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            var taskCount = 100;
            PrintStatus($"Running {taskCount} long running tasks that print a \".\" every second using Task.Factory.StartNew(action, TaskCreationOptions.LongRunning), then running {taskCount} tasks which complete in 2 seconds.");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Task[] longRunningTasks = new Task[taskCount];

            for (int i = 0; i < taskCount; i++)
            {
                //Fire and forget long running task using Task.Factory.StartNew(()=>{}, TaskCreationOptions.LongRunning)
                //For example purposes only. Real fire and forget tasks should always have a cancel mechanism and exception handling!!
                //Reference: https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcreationoptions?view=netframework-4.8
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(millisecondsTimeout: (int)TimeSpan.FromSeconds(1).TotalMilliseconds);
                        PrintDot();
                    }
                }, creationOptions: TaskCreationOptions.LongRunning);
            }

            //Then run normal Task.Runs
            Task<string>[] tasks = new Task<string>[taskCount];
            for (int i = 0; i < taskCount; i++)
            {
                var task = TaskService.GetStringWithTaskRunAsync($"Task {i}");
                tasks[i] = task;
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            PrintStatus($"Command ending after: {stopwatch.Elapsed.TotalSeconds}s");
        }));

        #endregion
    }
}
