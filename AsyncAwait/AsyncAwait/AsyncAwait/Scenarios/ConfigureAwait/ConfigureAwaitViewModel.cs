using MvvmHelpers.Commands;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AsyncAwait.ViewModels
{
    public class ConfigureAwaitViewModel : BaseViewModel
    {
        private AsyncCommand _noConfigureAwaitCommand;
        public AsyncCommand NoConfigureAwaitCommand => _noConfigureAwaitCommand ?? (_noConfigureAwaitCommand = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintThreadCheck();
            PrintStatus("Running 3 tasks without configure await");
            await TaskService.GetStringWithTaskRunAsync("Task1");
            PrintThreadCheck();
            await TaskService.GetStringWithTaskRunAsync("Task2");
            PrintThreadCheck();
            await TaskService.GetStringWithTaskRunAsync("Task3");
            PrintThreadCheck();
        }));

        private AsyncCommand _configureAwaitFalseCommand;
        public AsyncCommand ConfigureAwaitFalseCommand => _configureAwaitFalseCommand ?? (_configureAwaitFalseCommand = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintThreadCheck();
            PrintStatus("Running 3 tasks with ConfigureAwait(false)");
            await TaskService.GetStringWithTaskRunAsync("Task1").ConfigureAwait(false);
            PrintThreadCheck();
            await TaskService.GetStringWithTaskRunAsync("Task2").ConfigureAwait(false);
            PrintThreadCheck();
            await TaskService.GetStringWithTaskRunAsync("Task3").ConfigureAwait(false);
            PrintThreadCheck();
        }));
    }
}
