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
    public partial class ConfigureAwaitPage : ContentPage
    {
        public ConfigureAwaitPage()
        {
            InitializeComponent();
            BindingContext = new ConfigureAwaitViewModel();
        }
    }
}