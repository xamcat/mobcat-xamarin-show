using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.ViewModels
{
    public class BadExamplesViewModel : BaseViewModel
    {
        #region Example: Not awaiting Task - The intent is to use the result after the task without knowing the consequences.

        private Command _badTaskInvokeCommand;
        public Command BadTaskInvokeCommand => _badTaskInvokeCommand ?? (_badTaskInvokeCommand = new Command(() =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            PrintStatus("This locks up the UI. Never use .Result");

            var taskResult = TaskService.GetStringWithTaskRunAsync("Using Task.Result").Result;
            PrintStatus("Task Completed with result: {taskResult}");

            PrintStatus("Command ending");
        }));

        private AsyncCommand _goodTaskInvokeCommand;
        public AsyncCommand GoodTaskInvokeCommand => _goodTaskInvokeCommand ?? (_goodTaskInvokeCommand = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            PrintStatus("This allows the task to run on a background thread without locking the UI.");

            var taskResult = await TaskService.GetStringWithTaskRunAsync("Using await");

            PrintStatus("Command ending");
        }));

        #endregion



        #region Example: Multiple task execution - The intent is to await multiple tasks. 

        private AsyncCommand _badMultipleTaskExecutionCommand;
        public AsyncCommand BadMultipleTaskExecutionCommand => _badMultipleTaskExecutionCommand ?? (_badMultipleTaskExecutionCommand = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            PrintStatus("Awaiting tasks in a for loop causes the tasks to be awaited synchronously (one after another).");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var taskCount = 5;
            for (int i = 0; i < taskCount; i++)
            {
                await TaskService.GetStringWithTaskRunAsync($"Task {i}");
            }

            stopwatch.Stop();

            PrintStatus($"Command ending after: {stopwatch.Elapsed.TotalSeconds}s");
        }));

        private AsyncCommand _goodMultipleTaskExecutionCommand;
        public AsyncCommand GoodMultipleTaskExecutionCommand => _goodMultipleTaskExecutionCommand ?? (_goodMultipleTaskExecutionCommand = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            PrintStatus("Awaiting Task.WhenAll lets all the tasks run asynchronously.");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var taskCount = 5;
            var tasks = new Task<string>[taskCount];
            for (int i = 0; i < taskCount; i++)
            {
                tasks[i] = TaskService.GetStringWithTaskRunAsync($"Task {i}");
            }

            await Task.WhenAll(tasks);

            stopwatch.Stop();

            PrintStatus($"Command ending after: {stopwatch.Elapsed.TotalSeconds}s");
        }));

        #endregion



        #region Example: Not returning Task

        private AsyncCommand _badTaskReturnCommand;
        public AsyncCommand BadTaskReturnCommand => _badTaskReturnCommand ?? (_badTaskReturnCommand = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            PrintStatus("Return await causes additional overhead in context switching.");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var taskCount = 100;
            Task<string>[] tasks = new Task<string>[taskCount];
            for (int i = 0; i < taskCount; i++)
            {
                var task = TaskService.AwaitStringWithTaskRunAsync($"Task {i}");
                tasks[i] = task;
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            var completedStatus = $"Command ending after: {stopwatch.Elapsed.TotalSeconds}s";
            PrintStatus(completedStatus);
            Console.WriteLine(completedStatus);
        }));

        private AsyncCommand _goodTaskReturnCommand;
        public AsyncCommand GoodTaskReturnCommand => _goodTaskReturnCommand ?? (_goodTaskReturnCommand = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            PrintStatus("Returning the task directly is faster.");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var taskCount = 100;
            Task<string>[] tasks = new Task<string>[taskCount];
            for (int i = 0; i < taskCount; i++)
            {
                var task = TaskService.GetStringWithTaskRunAsync($"Task {i}");
                tasks[i] = task;
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            var completedStatus = $"Command ending after: {stopwatch.Elapsed.TotalSeconds}s";
            PrintStatus(completedStatus);
            Console.WriteLine(completedStatus);
        }));

        #endregion



        #region Example: Not using configure await false

        private AsyncCommand _badConsecutiveOperations;
        public AsyncCommand BadConsecutiveOperations => _badConsecutiveOperations ?? (_badConsecutiveOperations = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            PrintStatus("Not using ConfigureAwait(false) causes execution to continue on the UI thread.");

            PrintThreadCheck();
            await TaskService.GetStringWithTaskRunAsync();
            PrintThreadCheck();

            //Simulate CPU intensive operation
            Thread.Sleep((int)TimeSpan.FromSeconds(5).TotalMilliseconds);
          
            PrintStatus("Command ending");
        }));

        private AsyncCommand _goodConsecutiveOperations;
        public AsyncCommand GoodConsecutiveOperations => _goodConsecutiveOperations ?? (_goodConsecutiveOperations = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            PrintStatus("Not using ConfigureAwait(false) allows execution to continue on a background thread.");

            PrintThreadCheck();
            await TaskService.GetStringWithTaskRunAsync().ConfigureAwait(false);
            PrintThreadCheck();

            //Simulate CPU intensive operation. Eg: Deserializing a lot of JSON
            Thread.Sleep((int)TimeSpan.FromSeconds(5).TotalMilliseconds);

            PrintStatus("Command ending");
        }));

        #endregion
    }
}
