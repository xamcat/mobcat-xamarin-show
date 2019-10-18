using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwait.ViewModels
{
    public class AsyncLazyViewModel : BaseViewModel
    {
        private Task<string> _lazyStringTask;

        private AsyncCommand _asyncLazyInitializeCommand;
        public AsyncCommand AsyncLazyInitializeCommand => _asyncLazyInitializeCommand ?? (_asyncLazyInitializeCommand = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            if (_lazyStringTask == default)
            {
                var initializeSeconds = 10;
                PrintStatus($"Initializing Lazy Task for {initializeSeconds}seconds.");
                _lazyStringTask = TaskService.GetStringWithTaskRunAsync(taskName: "Lazy Task",
                    delaySeconds: initializeSeconds);
            }
            else
            {
                PrintStatus("Lazy task initialization already executed");
            }
            PrintStatus("Command ending");
        }));


        private AsyncCommand _asyncLazyAwaitCommand;
        public AsyncCommand AsyncLazyAwaitCommand => _asyncLazyAwaitCommand ?? (_asyncLazyAwaitCommand = new AsyncCommand(async () =>
        {
            ClearStatus();
            PrintStatus("Command starting");
            if (_lazyStringTask == default)
            {
                PrintStatus($"{nameof(_lazyStringTask)} hasn't been initialized! Please initialize it first.");
            }
            else
            {
                PrintStatus("Awaiting lazy string task");
                var result = await _lazyStringTask;
                PrintStatus($"Lazy string result: {result}");
            }
            PrintStatus("Command ending");
        }));
    }
}
