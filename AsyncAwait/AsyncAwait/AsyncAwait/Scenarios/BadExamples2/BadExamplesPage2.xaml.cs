using AsyncAwait.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AsyncAwait.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BadExamplesPage2 : ContentPage
    {
        public BadExamplesViewModel2 VM => BindingContext as BadExamplesViewModel2;

        public BadExamplesPage2()
        {
            InitializeComponent();
            BindingContext = new BadExamplesViewModel2();
        }

        /// <summary>
        /// This is for example purposes only. Use button commands for Xamarin Forms. Example for updating UI from background thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void HandleBadUIUpdateClicked(object sender, EventArgs e)
        {
            VM.ClearStatus();
            VM.PrintStatus("Button Clicked EventHandler Starting");
            VM.PrintThreadCheck();
            await VM.TaskService.GetStringWithTaskRunAsync().ConfigureAwait(false);

            VM.PrintThreadCheck();
            StatusLabel.Text += "Updated StatusLabel from non-UI (background) thread!";
            VM.PrintStatus("Button Clicked EventHandler Ending");
        }

        /// <summary>
        /// This is for example purposes only. Use button commands for Xamarin Forms. Example for updating UI from UI thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void HandleGoodUIUpdateClicked(object sender, EventArgs e)
        {
            VM.ClearStatus();
            VM.PrintStatus("Button Clicked EventHandler Starting");
            VM.PrintThreadCheck();
            await VM.TaskService.GetStringWithTaskRunAsync().ConfigureAwait(false); //If there was no configure await, execution would continue on the UI thread
            Device.BeginInvokeOnMainThread(() =>
            {
                VM.PrintThreadCheck();
                StatusLabel.Text += "Updated StatusLabel from UI thread!";
            });
            VM.PrintStatus("Button Clicked EventHandler Ending");
        }
    }
}