using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwait.Services
{
    public class TaskStatusChangedEventArgs : EventArgs
    {
        public Task Task { get; set; }
        public string Status { get; set; }

        public TaskStatusChangedEventArgs(Task task, string status)
        {
            Task = task;
            Status = status;
        }
    }
}
