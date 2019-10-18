using System;
using System.Threading.Tasks;
using Command = MvvmHelpers.Commands.Command;

namespace AsyncAwait.ViewModels
{
    public class AsyncVoidViewModel : BaseViewModel
    {
        private Command _asyncVoidNoAwaitCommand;
        public Command AsyncVoidNoAwaitCommand => _asyncVoidNoAwaitCommand ?? (_asyncVoidNoAwaitCommand = new Command(() =>
        {
            ClearStatus();
            PrintStatus("Command starting");

            AsyncVoidMethod();

            PrintStatus("Command ending");
        }));

        private async void AsyncVoidMethod()
        {
            PrintStatus($"{nameof(AsyncVoidMethod)} starting");

            Action exceptionAction = () => throw new Exception("Async Void Exception");

            await TaskService.GetStringWithTaskRunAsync("AsyncVoidTask",
                delaySeconds: 2,
                taskResult: "Task Result",
                cancellationToken: default,
                taskAction: exceptionAction);

            PrintStatus($"{nameof(AsyncVoidMethod)} ending");
        }


        private Command _asyncVoidTryCatchCommand;
        public Command AsyncVoidTryCatchCommand => _asyncVoidTryCatchCommand ?? (_asyncVoidTryCatchCommand = new Command(async() =>
        {
            ClearStatus();
            PrintStatus("Command starting");

            try
            {
                PrintStatus("Command try block starting");
                await AsyncVoidMethodWithTryCatch();
                PrintStatus("Command try block ending");
            }
            catch (Exception ex)
            {
                PrintStatus($"Command exception caught: {ex.Message}");
            }

            PrintStatus("Command ending");
        }));

        private async Task AsyncVoidMethodWithTryCatch()
        {
            PrintStatus($"{nameof(AsyncVoidMethod)} starting");
            try
            {
                PrintStatus($"{nameof(AsyncVoidMethod)} try block starting");

                Action exceptionAction = () => throw new Exception("Async Void Exception");

                await TaskService.GetStringWithTaskRunAsync("AsyncVoidTask",
                    delaySeconds: 2,
                    taskResult: "Task Result",
                    cancellationToken: default,
                    taskAction: exceptionAction);

                PrintStatus($"{nameof(AsyncVoidMethod)} try block ending");
            }
            catch (Exception ex)
            {
                PrintStatus($"Exception caught!: {ex.Message}");
                throw;
            }
            PrintStatus($"{nameof(AsyncVoidMethodWithTryCatch)} ending");
        }
    }
}
