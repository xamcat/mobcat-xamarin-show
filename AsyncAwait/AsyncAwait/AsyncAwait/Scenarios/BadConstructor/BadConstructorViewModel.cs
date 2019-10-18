using System;
using System.Collections.Generic;
using System.Text;

namespace AsyncAwait.ViewModels
{
    public class BadConstructorViewModel : BaseViewModel
    {
        public BadConstructorViewModel()
        {
            PrintStatus($"{nameof(BadConstructorViewModel)} Constructor starting");
            TaskService.AwaitStringWithTaskRunAsync().GetAwaiter().GetResult();
            PrintStatus($"{nameof(BadConstructorViewModel)} Constructor ending");
        }
    }
}
