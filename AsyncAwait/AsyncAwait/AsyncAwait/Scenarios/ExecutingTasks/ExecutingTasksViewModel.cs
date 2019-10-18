using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AsyncAwait.ViewModels
{
    public class ExecutingTasksViewModel : BaseViewModel
    {
        private Command _invokeTaskCommand;
        public Command InvokeTaskCommand =>
            _invokeTaskCommand ?? (_invokeTaskCommand = new Command(() =>
            {
                ClearStatus();
                TaskService.GetStringWithNewTaskAsync("GetStringAsync()");
            }));

        private Command _taskRunCommand;
        public Command TaskRunCommand =>
            _taskRunCommand ?? (_taskRunCommand = new Command(() =>
            {
                ClearStatus();
                TaskService.GetStringWithTaskRunAsync("Task.Run()");
            }));

        private Command _runSynchronouslyCommand;
        public Command RunSynchronouslyCommand =>
            _runSynchronouslyCommand ?? (_runSynchronouslyCommand = new Command(() =>
            {
                ClearStatus();
                TaskService.GetStringWithNewTaskAsync("RunSynchronously()").RunSynchronously();
            }));


        private Command _continueWithCommand;
        public Command ContinueWithCommand =>
            _continueWithCommand ?? (_continueWithCommand = new Command(() =>
            {
                ClearStatus();
                TaskService.GetStringWithNewTaskAsync().ContinueWith(task => PrintStatus("ContinueWith()"));
            }));


        private Command _startCommand;
        public Command StartCommand => _startCommand ?? (_startCommand = new Command(() =>
        {
            ClearStatus();
            TaskService.GetStringWithNewTaskAsync(".Start()").Start();
        }));

        private Command _startAndWaitCommand;
        public Command StartAndWaitCommand => _startAndWaitCommand ?? (_startAndWaitCommand = new Command(() =>
        {
            ClearStatus();
            var task = TaskService.GetStringWithNewTaskAsync(".Start()");
            task.Start();
            PrintStatus($"Current task status: {task.Status}");
            task.Wait();
            PrintStatus($"Current task status: {task.Status}");
        }));

        private Command _waitCommand;
        public Command WaitCommand => _waitCommand ?? (_waitCommand = new Command(() =>
        {
            ClearStatus();
            var task = TaskService.GetStringWithNewTaskAsync(".Wait()");
            task.Wait();
        }));


        private Command _resultCommand;
        public Command ResultCommand => _resultCommand ?? (_resultCommand = new Command(() =>
        {
            ClearStatus();
            Device.OpenUri(new Uri("https://montemagno.com/c-sharp-developers-stop-calling-dot-result/"));
            var result = TaskService.GetStringWithNewTaskAsync(".Result").Result;
        }));

        private Command _resultInTaskRunCommand;
        public Command ResultInTaskRunCommand => _resultInTaskRunCommand ?? (_resultInTaskRunCommand = new Command(() =>
        {
            ClearStatus();
            var result = Task.Run(() => TaskService.GetStringWithNewTaskAsync(".Result").Result);
        }));

        private Command _awaiterResultCommand;
        public Command AwaiterResultCommand => _awaiterResultCommand ?? (_awaiterResultCommand = new Command(() =>
        {
            ClearStatus();
            var awaiterResult = TaskService.GetStringWithNewTaskAsync(".GetAwaiter().GetResult()").GetAwaiter().GetResult();
            //More on GetAwaiter.GetResult vs .Result: https://devblogs.microsoft.com/pfxteam/task-exception-handling-in-net-4-5/
        }));
    }
}
