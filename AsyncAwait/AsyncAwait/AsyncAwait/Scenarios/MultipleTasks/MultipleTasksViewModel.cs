using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Command = Xamarin.Forms.Command;

namespace AsyncAwait.ViewModels
{
    public class MultipleTasksViewModel : BaseViewModel
    {

        private AsyncCommand _whenAnyCommand;
        public AsyncCommand WhenAnyCommand => _whenAnyCommand ?? (_whenAnyCommand = new AsyncCommand(async () =>
        {
            //Good example. Doesn't block
            ClearStatus();
            PrintStatus("Running 3 tasks using Task.WhenAny with 3, 4, and 5s completion times.");
            var threeSecondTask = TaskService.GetStringWithTaskRunAsync("3s Task", delaySeconds: 3, taskResult: "3s Task Result!");
            var fourSecondTask = TaskService.GetStringWithTaskRunAsync("4s Task", delaySeconds: 4);
            var fiveSecondTask = TaskService.GetStringWithTaskRunAsync("5s Task", delaySeconds: 5);

            var whenAnyTask = await Task.WhenAny(threeSecondTask, fourSecondTask, fiveSecondTask);
            if (whenAnyTask.Status == TaskStatus.RanToCompletion)
            {
                PrintStatus($"WhenAnyTask completed with result: {whenAnyTask.Result}"); //since the task is completed, we can get the completed result
            }
            else
            {
                PrintStatus($"WhenAnyTask has an unexpected status: {whenAnyTask.Status}");
            }
        }));

        private AsyncCommand _whenAllCommand;
        public AsyncCommand WhenAllCommand => _whenAllCommand ?? (_whenAllCommand = new AsyncCommand(async () =>
        {
            //Good example. Doesn't block.
            ClearStatus();
            PrintStatus("Running 3 tasks using Task.WhenAll with 3, 4, and 5s completion times.");
            var threeSecondTask = TaskService.GetStringWithTaskRunAsync("3s Task", delaySeconds: 3, taskResult: "3s Task Result!");
            var fourSecondTask = TaskService.GetStringWithTaskRunAsync("4s Task", delaySeconds: 4, taskResult: "4s Task Result!");
            var fiveSecondTask = TaskService.GetStringWithTaskRunAsync("5s Task", delaySeconds: 5, taskResult: "5s Task Result!");

            var whenAllTaskResults = await Task.WhenAll(threeSecondTask, fourSecondTask, fiveSecondTask);
            PrintStatus("Task.WhenAll Completed! Results:");
            for (int i = 0; i < whenAllTaskResults.Length; i++)
            {
                PrintStatus($"Result {i + 1}: {whenAllTaskResults[i]}");
            }

            PrintStatus("\nSeparately, we can get the results from the completed tasks themselves");
            PrintStatus($"{nameof(threeSecondTask)} result: {threeSecondTask.Result}");
            PrintStatus($"{nameof(fourSecondTask)} result: {fourSecondTask.Result}");
            PrintStatus($"{nameof(fiveSecondTask)} result: {fiveSecondTask.Result}");
        }));

        private Command _waitAnyCommand;
        public Command WaitAnyCommand => _waitAnyCommand ?? (_waitAnyCommand = new Command(() =>
        {
            //Bad example. Blocks UI Thread
            ClearStatus();
            PrintStatus("Running 3 tasks using Task.WhenAny with 3, 4, and 5s completion times.");
            var threeSecondTask = TaskService.GetStringWithTaskRunAsync("3s Task", delaySeconds: 3, taskResult: "3s Task Result!");
            var fourSecondTask = TaskService.GetStringWithTaskRunAsync("4s Task", delaySeconds: 4);
            var fiveSecondTask = TaskService.GetStringWithTaskRunAsync("5s Task", delaySeconds: 5);

            var tasks = new Task<string>[] { threeSecondTask, fourSecondTask, fiveSecondTask };
            var waitAnyResult = Task.WaitAny(tasks); //Returns index of completed task
            PrintStatus($"WaitAny result: {waitAnyResult}");
            PrintStatus($"Completed Task result: {tasks[waitAnyResult].Result}");
        }));

        private Command _waitAllCommand;
        public Command WaitAllCommand => _waitAllCommand ?? (_waitAllCommand = new Command(() =>
        {
            //Bad example. Blocks UI Thread
            //Notice how the UI Thread is blocked while this executes
            //Device.BeginInvokeOnMainThread(ClearStatus); //Supposedly this will help clear the status before executing? Try it out.
            ClearStatus();
            PrintStatus("Running 3 tasks using Task.WhenAll with 3, 4, and 5s completion times.");
            var threeSecondTask = TaskService.GetStringWithTaskRunAsync("3s Task", delaySeconds: 3, taskResult: "3s Task Result!");
            var fourSecondTask = TaskService.GetStringWithTaskRunAsync("4s Task", delaySeconds: 4, taskResult: "4s Task Result!");
            var fiveSecondTask = TaskService.GetStringWithTaskRunAsync("5s Task", delaySeconds: 5, taskResult: "5s Task Result!");

            var tasks = new Task<string>[] { threeSecondTask, fourSecondTask, fiveSecondTask };
            Task.WaitAll(tasks); //Returns void
            PrintStatus("Task.WhenAll Completed! Results:");
            for (int i = 0; i < tasks.Length; i++)
            {
                PrintStatus($"Result {i + 1}: {tasks[i].Result}");
            }

            PrintStatus("\nSeparately, we can get the results from the completed tasks themselves");
            PrintStatus($"{nameof(threeSecondTask)} result: {threeSecondTask.Result}");
            PrintStatus($"{nameof(fourSecondTask)} result: {fourSecondTask.Result}");
            PrintStatus($"{nameof(fiveSecondTask)} result: {fiveSecondTask.Result}");
        }));

    }
}
