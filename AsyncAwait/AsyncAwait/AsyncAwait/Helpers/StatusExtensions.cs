using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace AsyncAwait.Helpers
{
    public static class StatusExtensions
    {
        public static string AppendMainThreadAlert(this string status)
        {
            if (MainThread.IsMainThread)
            {
                status += " *on MainThread!*\n";
            }
            return status;
        }
    }
}
