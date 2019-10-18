using AsyncAwait.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AsyncAwait.Services
{
    public class TaskService
    {
        public event EventHandler<TaskStatusChangedEventArgs> TaskCreated;
        public event EventHandler<TaskStatusChangedEventArgs> TaskStarting;
        public event EventHandler<TaskStatusChangedEventArgs> TaskCompleted;
        public event EventHandler<TaskStatusChangedEventArgs> TaskFaulted;
        public event EventHandler<TaskStatusChangedEventArgs> TaskCancelled;

        public void RaiseTaskCreated(Task task, [CallerMemberName] string taskName = default)
        {
            var status = $"[Created] Task: {taskName}";

            TaskCreated?.Invoke(this, new TaskStatusChangedEventArgs(task, status));
        }

        public void RaiseTaskStarting(Task task, [CallerMemberName] string taskName = default)
        {
            var status = $"[Started] Task: {taskName} ".AppendMainThreadAlert();
            TaskStarting?.Invoke(this, new TaskStatusChangedEventArgs(task, status));
        }

        public void RaiseTaskCompleted(Task task, string taskName = default)
        {
            var status = $"[Completed] Task: {taskName}. status: {task.Status}".AppendMainThreadAlert();
            TaskCompleted?.Invoke(this, new TaskStatusChangedEventArgs(task, status));
        }

        public void RaiseTaskFaulted(Task task, [CallerMemberName] string taskName = default)
        {
            var status = $"[Faulted] Task: {taskName}. status: {task.Status}".AppendMainThreadAlert();
            TaskFaulted?.Invoke(this, new TaskStatusChangedEventArgs(task, status));
        }

        public void RaiseTaskCancelled(Task task, [CallerMemberName] string taskName = default)
        {
            var status = $"[Cancelled] Task: {taskName}. status: {task.Status}".AppendMainThreadAlert();
            TaskCancelled?.Invoke(this, new TaskStatusChangedEventArgs(task, status));
        }

        public Task<string> GetStringWithNewTaskAsync([CallerMemberName] string taskName = default, int delaySeconds = 1, string taskResult = "Task Result")
        {
            Task<string> getStringTask = default;
            getStringTask = new Task<string>(() =>
            {
                RaiseTaskStarting(getStringTask, taskName);
                Thread.Sleep((int)TimeSpan.FromSeconds(delaySeconds).TotalMilliseconds); //Use Thread.Sleep to block the current thread. https://stackoverflow.com/questions/20082221/when-to-use-task-delay-when-to-use-thread-sleep
                return taskResult;
            });

            RaiseTaskCreated(getStringTask, taskName);

            getStringTask.ContinueWith(completedGetStringTask =>
            {
                if (completedGetStringTask.IsCanceled)
                {
                    RaiseTaskCancelled(completedGetStringTask, taskName);
                }
                else if (completedGetStringTask.IsFaulted)
                {
                    RaiseTaskFaulted(completedGetStringTask, taskName);
                }
                else
                {
                    RaiseTaskCompleted(completedGetStringTask, taskName);
                }
            });

            return getStringTask;
        }

        public Task<string> GetStringWithTaskRunAsync([CallerMemberName] string taskName = default,
            int delaySeconds = 1,
            string taskResult = "Task Result",
            CancellationToken cancellationToken = default,
            Action taskAction = default)
        {
            Task<string> getStringTask = default;
            getStringTask = Task.Run(() =>
            {
                RaiseTaskStarting(getStringTask, taskName);
                var elapsedSeconds = 0;
                while (elapsedSeconds < delaySeconds)
                {
                    //Simulate work
                    Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds); //Use Thread.Sleep to block the current thread. https://stackoverflow.com/questions/20082221/when-to-use-task-delay-when-to-use-thread-sleep
                    elapsedSeconds++;

                    cancellationToken.ThrowIfCancellationRequested();
                }

                taskAction?.Invoke();

                return taskResult;
            }, cancellationToken: cancellationToken);

            RaiseTaskCreated(getStringTask, taskName);

            getStringTask.ContinueWith(completedGetStringTask =>
            {
                if (completedGetStringTask.IsCanceled)
                {
                    RaiseTaskCancelled(completedGetStringTask, taskName);
                }
                else if (completedGetStringTask.IsFaulted)
                {
                    RaiseTaskFaulted(completedGetStringTask, taskName);
                }
                else
                {
                    RaiseTaskCompleted(completedGetStringTask, taskName);
                }
            });

            return getStringTask;
        }

        public async Task<string> AwaitStringWithTaskRunAsync([CallerMemberName] string taskName = default,
            int delaySeconds = 1,
            string taskResult = "Task Result")
        {
            return await GetStringWithTaskRunAsync(taskName);
        }

        public Task GetFireAndForgetTask([CallerMemberName] string taskName = default,
            int delaySeconds = 1,
            CancellationToken cancellationToken = default,
            Action taskAction = default)
        {
            Task fireAndForgetTask = default;
            fireAndForgetTask = Task.Run(() =>
            {
                RaiseTaskStarting(fireAndForgetTask, taskName);
                var elapsedSeconds = 0;
                while (elapsedSeconds < delaySeconds)
                {
                    //Simulate work
                    Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds); //Use Thread.Sleep to block the current thread. https://stackoverflow.com/questions/20082221/when-to-use-task-delay-when-to-use-thread-sleep
                    elapsedSeconds++;

                    cancellationToken.ThrowIfCancellationRequested();
                }
                taskAction?.Invoke();
            }, cancellationToken: cancellationToken);

            RaiseTaskCreated(fireAndForgetTask, taskName);

            fireAndForgetTask.ContinueWith(completedGetStringTask =>
            {
                if (completedGetStringTask.IsCanceled)
                {
                    RaiseTaskCancelled(completedGetStringTask, taskName);
                }
                else if (completedGetStringTask.IsFaulted)
                {
                    RaiseTaskFaulted(completedGetStringTask, taskName);
                }
                else
                {
                    RaiseTaskCompleted(completedGetStringTask, taskName);
                }
            });

            return fireAndForgetTask;
        }

        public Task<string> GetStringWithTaskCompletionSource([CallerMemberName] string taskName = default,
            int delaySeconds = 1,
            string taskResult = "Task Result",
            CancellationToken cancellationToken = default,
            Action taskAction = default)
        {
            var taskCompletionSource = new TaskCompletionSource<string>();
            var tcsTask = taskCompletionSource.Task;
            RaiseTaskCreated(tcsTask, taskName);

            tcsTask.ContinueWith(completedTcsTask =>
            {
                if (completedTcsTask.IsCanceled)
                {
                    RaiseTaskCancelled(completedTcsTask, taskName);
                }
                else if (completedTcsTask.IsFaulted)
                {
                    RaiseTaskFaulted(completedTcsTask, taskName);
                }
                else
                {
                    RaiseTaskCompleted(completedTcsTask, taskName);
                }
            });

            var internalTask = Task.Run(() =>
            {
                try
                {
                    Thread.Sleep((int)TimeSpan.FromSeconds(delaySeconds).TotalMilliseconds);

                    taskAction?.Invoke();

                    if (cancellationToken.IsCancellationRequested)
                    {
                        taskCompletionSource.TrySetCanceled(cancellationToken);
                    }
                    else
                    {
                        taskCompletionSource.TrySetResult(taskResult);
                    }
                }
                catch (Exception ex)
                {
                    taskCompletionSource.TrySetException(ex);
                    throw;
                }
            });

            var internalTaskName = $"Internal - {taskName}";

            RaiseTaskCreated(internalTask, taskName: internalTaskName);
            internalTask.ContinueWith(completedInternalTask =>
            {
                if (completedInternalTask.IsCanceled)
                {
                    RaiseTaskCancelled(completedInternalTask, taskName: internalTaskName);
                }
                else if (completedInternalTask.IsFaulted)
                {
                    RaiseTaskFaulted(completedInternalTask, taskName: internalTaskName);
                }
                else
                {
                    RaiseTaskCompleted(completedInternalTask, taskName: internalTaskName);
                }
            });

            return tcsTask;
        }
        public Task<string> GetStringWithTaskCompletionSourceTheWrongWay(string taskName = default,
            int delaySeconds = 1,
            string taskResult = "Task Result",
            CancellationToken cancellationToken = default,
            Action taskAction = default)
        {
            var taskCompletionSource = new TaskCompletionSource<string>();
            var tcsTask = taskCompletionSource.Task;
            RaiseTaskCreated(tcsTask, taskName);

            tcsTask.ContinueWith(completedGetStringTask =>
            {
                if (completedGetStringTask.IsCanceled)
                {
                    RaiseTaskCancelled(completedGetStringTask, taskName);
                }
                else if (completedGetStringTask.IsFaulted)
                {
                    RaiseTaskFaulted(completedGetStringTask, taskName);
                }
                else
                {
                    RaiseTaskCompleted(completedGetStringTask, taskName);
                }
            });

            var internalTask = Task.Run(() =>
            {
                Thread.Sleep(millisecondsTimeout: (int)TimeSpan.FromSeconds(delaySeconds).TotalMilliseconds);
                taskAction?.Invoke(); //This doesn't take into account taskAction can throw an exception so it appears to be "swallowed"
                taskCompletionSource.TrySetResult(taskResult);
            });

            var internalTaskName = $"Internal - {taskName}";

            RaiseTaskCreated(internalTask, taskName: internalTaskName);
            internalTask.ContinueWith(completedGetStringTask =>
            {
                if (completedGetStringTask.IsCanceled)
                {
                    RaiseTaskCancelled(completedGetStringTask, taskName: internalTaskName);
                }
                else if (completedGetStringTask.IsFaulted)
                {
                    RaiseTaskFaulted(completedGetStringTask, taskName: internalTaskName);
                }
                else
                {
                    RaiseTaskCompleted(completedGetStringTask, taskName: internalTaskName);
                }
            });

            return taskCompletionSource.Task;
        }


        private string _valueTaskResult;

        public ValueTask<string> GetStringWithValueTask([CallerMemberName] string taskName = default,
            string taskResult = "Task Result",
            int delaySeconds = 1)
        {
            if (string.IsNullOrEmpty(_valueTaskResult))
            {
                var getStringTask = GetStringWithTaskRunAsync(taskName, delaySeconds: delaySeconds, taskResult: taskResult);
                getStringTask.ContinueWith((completedTask) =>
                {
                    _valueTaskResult = completedTask.Result;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
                return new ValueTask<string>(getStringTask);
            }

            return new ValueTask<string>(_valueTaskResult);
        }

        private string _taskResult;

        public Task<string> GetStringWithoutValueTask([CallerMemberName] string taskName = default,
            string taskResult = "Task Result",
            int delaySeconds = 1)
        {
            if (string.IsNullOrEmpty(_taskResult))
            {
                var getStringTask = GetStringWithTaskRunAsync(taskName, delaySeconds: delaySeconds, taskResult: taskResult);
                getStringTask.ContinueWith((completedTask) =>
                {
                    _taskResult = completedTask.Result;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
                return getStringTask;
            }

            return Task.FromResult(_taskResult);
        }
    }
}