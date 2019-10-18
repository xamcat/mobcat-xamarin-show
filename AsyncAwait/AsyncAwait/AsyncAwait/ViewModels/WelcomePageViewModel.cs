using System;
using System.Collections.Generic;
using System.Text;

namespace AsyncAwait.ViewModels
{
    public class WelcomePageViewModel : BaseViewModel
    {
        private string _text =
            @"Welcome to Async Await Scenarios in Xamarin!
Use the main menu to get started.
---------------------------------

Scenarios to explore:
=================================
Bad examples and how to fix them
=================================
Bad Examples 1:
- Invoking tasks
- Awaiting multiple tasks
- Returning tasks
- Threading

Bad Examples 2:
- Updating UI properties
- Handling exceptions
- Timing out tasks
- Using TaskCompletionSource

Bad Examples 3:
- Using LongRunningTask

=================================
Other samples/experiments
=================================
- Executing tasks:
    - The various ways to execute a task without awaiting
- Awaiting tasks:
    - How to await tasks properly
- Configure await:
    - Using configure await
- Awaiting multiple tasks

Resources:

Channel9 Async Await: https://channel9.msdn.com/Tags/async

Stephen Toub's blog posts: https://devblogs.microsoft.com/dotnet/author/toub/

Stephen Cleary's blog posts: https://blog.stephencleary.com/

Brandon Minnick's Async/Await talk: https://www.youtube.com/watch?v=J0mcYVxJEl0

TPL Dataflow: https://channel9.msdn.com/Shows/Going+Deep/Stephen-Toub-Inside-TPL-Dataflow





";
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }
    }
}
