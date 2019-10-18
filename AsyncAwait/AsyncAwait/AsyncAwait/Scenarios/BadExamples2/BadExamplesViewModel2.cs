using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Command = MvvmHelpers.Commands.Command;

namespace AsyncAwait.ViewModels
{
    public class BadExamplesViewModel2 : BaseViewModel
    {
        #region Example: Exception Handling

        private Command _badExceptionHandling;
        public Command BadExceptionHandling => _badExceptionHandling ?? (_badExceptionHandling = new Command(() =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            PrintStatus("Try/Catch won't catch an exception for a fire and forget task!");
            PrintStatus("Note we aren't using async await");

            try
            {
                Action exceptionAction = () =>
                {
                    throw new Exception("My Exception");
                };

                TaskService.GetFireAndForgetTask(taskName: "TaskWithException",
                    delaySeconds: 2,
                    cancellationToken: default,
                    taskAction: exceptionAction);
            }
            catch (Exception ex)
            {
                PrintStatus($"Exception occurred: {ex.Message}");
            }

            PrintStatus("Command ending");
        }));

        private Command _goodExceptionHandling;
        public Command GoodExceptionHandling => _goodExceptionHandling ?? (_goodExceptionHandling = new Command(() =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            PrintStatus("Use Task.ContinueWith to catch exceptions properly on fire and forget.");
            PrintStatus("Note we aren't using async await");

            Action exceptionAction = () =>
            {
                throw new Exception("My Exception");
            };

            TaskService.GetFireAndForgetTask(taskName: "TaskWithException",
                delaySeconds: 2,
                cancellationToken: default,
                taskAction: exceptionAction)
            .ContinueWith(continuationAction: (task) =>
             {
                 PrintStatus($"Exception occurred: {task.Exception.Message}");
             }, continuationOptions: TaskContinuationOptions.OnlyOnFaulted);

            PrintStatus("Command ending");
        })); 
        
        private Command _goodExceptionHandling2;
        public Command GoodExceptionHandling2 => _goodExceptionHandling2 ?? (_goodExceptionHandling2 = new Command(async() =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            PrintStatus("Use Task.ContinueWith to catch exceptions properly on fire and forget.");

            Action exceptionAction = () =>
            {
                throw new Exception("My Exception");
            };

            try
            {
                await TaskService.GetFireAndForgetTask(taskName: "TaskWithException",
                    delaySeconds: 2,
                    cancellationToken: default,
                    taskAction: exceptionAction);
            }
            catch (Exception ex)
            {
                PrintStatus($"Exception occurred: {ex.Message}");
            }

            PrintStatus("Command ending");
        }));


        #endregion



        #region Example: Timing out     

        private Command _badTimeoutCommand;
        public Command BadTimeoutCommand => _badTimeoutCommand ?? (_badTimeoutCommand = new Command(() =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            PrintStatus("Running a 60s task with timeout on 3 seconds using Task.WaitAll(task, timeout)");

            var longRunningTask = TaskService.GetStringWithTaskRunAsync("60 second Task",
                   delaySeconds: 60,
                   taskResult: "TaskResult");
            var tasks = new Task[] { longRunningTask };

            var timeoutSeconds = 3;
            PrintStatus($"Task will timeout in {timeoutSeconds} seconds..");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Task.WaitAll(tasks, timeout: TimeSpan.FromSeconds(timeoutSeconds));

            stopwatch.Stop();
            PrintStatus($"Command ending after {stopwatch.Elapsed.TotalSeconds}s");
        }));


        private AsyncCommand _goodTimeoutCommand;
        public AsyncCommand GoodTimeoutCommand => _goodTimeoutCommand ?? (_goodTimeoutCommand = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            PrintStatus("Running a 60s task with timeout on 3 seconds using a cancellation token source");

            var longRunningTaskCompletionSource = new TaskCompletionSource<string>();
            var timeoutSeconds = 3;
            var cancellationTokenSource = new CancellationTokenSource(delay: TimeSpan.FromSeconds(timeoutSeconds));
            var stopwatch = new Stopwatch();

            try
            {
                PrintStatus($"Task will timeout in {timeoutSeconds} seconds..");

                stopwatch.Start();
                await TaskService.GetStringWithTaskRunAsync("60 second Task",
                    delaySeconds: 60,
                    taskResult: "TaskResult",
                    cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                PrintStatus($"An exception occurred: {ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                PrintStatus($"Command ending after {stopwatch.Elapsed.TotalSeconds}s");
            }
        }));

        #endregion



        #region Example: Not completing TaskCompletionSource Task

        private AsyncCommand _badTaskCompletionSource;
        public AsyncCommand BadTaskCompletionSource => _badTaskCompletionSource ?? (_badTaskCompletionSource = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            PrintStatus("Not setting exceptions on a TaskCompletionSource properly causes tasks to never complete. Always set the result, exception, or cancellation.");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var taskCount = 10;
            Task<string>[] tasks = new Task<string>[taskCount];
            for (int i = 0; i < taskCount; i++)
            {
                var currentException = $"Exception{i}";
                Action exceptionAction = () => throw new Exception(currentException);

                var task = TaskService.GetStringWithTaskCompletionSourceTheWrongWay($"Task {i}",
                    delaySeconds: i,
                    taskResult: $"Task{i}Result",
                    cancellationToken: default,
                    taskAction: exceptionAction);
                tasks[i] = task;
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                PrintStatus("Catch(Exception ex) only exposes the first exception.");
                PrintStatus($"An exception occurred: {ex.Message}");

                PrintStatus("Get all the exceptions from the tasks collection");
                var exceptions = tasks.Where(a => a.Exception != null)
                    .Select(a => a.Exception);
                foreach (var exception in exceptions)
                {
                    PrintStatus($"{exception.Message}");
                }
            }

            stopwatch.Stop();
            PrintStatus($"Command ending after: {stopwatch.Elapsed.TotalSeconds}s");
        }));


        private AsyncCommand _goodTaskCompletionSource;
        public AsyncCommand GoodTaskCompletionSource => _goodTaskCompletionSource ?? (_goodTaskCompletionSource = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            PrintStatus("Setting exceptions on TaskCompletionSources allows the exception to be handled.");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var taskCount = 10;
            Task<string>[] tasks = new Task<string>[taskCount];

            for (int i = 0; i < taskCount; i++)
            {
                var currentException = $"Exception{i}";
                Action exceptionAction = () => throw new Exception(currentException);

                var task = TaskService.GetStringWithTaskCompletionSource($"Task {i}",
                    delaySeconds: i,
                    taskResult: $"Task{i}Result",
                    cancellationToken: default,
                    taskAction: exceptionAction);
                tasks[i] = task;
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                PrintStatus("Catch(Exception ex) only exposes the first exception.");
                PrintStatus($"An exception occurred: {ex.Message}");

                PrintStatus("Get all the exceptions from the tasks collection");
                var exceptions = tasks.Where(a => a.Exception != null)
                    .Select(a => a.Exception);
                foreach (var exception in exceptions)
                {
                    PrintStatus($"{exception.Message}");
                }
            }

            stopwatch.Stop();
            PrintStatus($"Command ending after: {stopwatch.Elapsed.TotalSeconds}s");
        }));

        #endregion
    }
}