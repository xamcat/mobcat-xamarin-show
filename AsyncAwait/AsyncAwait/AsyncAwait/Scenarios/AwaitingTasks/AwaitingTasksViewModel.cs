using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwait.ViewModels
{
    public class AwaitingTasksViewModel : BaseViewModel
    {
        private AsyncCommand _awaitNewTaskCommand;
        public AsyncCommand AwaitNewTaskCommand => _awaitNewTaskCommand ?? (_awaitNewTaskCommand = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command invoked");
            var result = await TaskService.GetStringWithNewTaskAsync("New Task");
            PrintStatus("Command returning");
        }));

        private AsyncCommand _awaitTaskRunCommand;
        public AsyncCommand AwaitTaskRunCommand => _awaitTaskRunCommand ?? (_awaitTaskRunCommand = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command invoked");
            var result = await TaskService.GetStringWithTaskRunAsync("Task.Run");
            PrintStatus("Command returning");
        }));

        private Command _noAwaitTaskRunCommand;
        public Command NoAwaitTaskRunCommand => _noAwaitTaskRunCommand ?? (_noAwaitTaskRunCommand = new Command(() =>
        {
            ClearStatus();
            PrintStatus("Command invoked");
            var result = TaskService.AwaitStringWithTaskRunAsync("No await on Task.Run"); //AwaitStringWithTaskRunAsync awaits internally
            PrintStatus($"Command completed with result: {result}");
        }));

        private Command _taskRunResultCommand;
        public Command TaskRunResultCommand => _taskRunResultCommand ?? (_taskRunResultCommand = new Command(() =>
        {
            ClearStatus();
            PrintStatus("Command invoked");
            var result = TaskService.AwaitStringWithTaskRunAsync("Task.Run.Result").Result;//AwaitStringWithTaskRunAsync awaits internally
            PrintStatus($"Command completed with result: {result}");
        }));
    }
}