using System;
using Foundation;
using UIKit;
using Xamarin.Essentials;

namespace testMemoryIssuesv3
{
	public partial class DetailsViewController : UIViewController
	{
        public DetailsViewController (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            imgDetails.Image = UIImage.LoadFromData(NSData.FromUrl(NSUrl.FromString("https://picsum.photos/1024")));
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        public override void ViewWillAppear(bool animated)
        {
            // In order to fix the memory leak - move subscriptipn to WillAppear and don't forget to unsubscribe in DidDisappear
            //Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            base.ViewWillAppear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            //Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"{Environment.TickCount}: {nameof(Connectivity_ConnectivityChanged)}");
        }
    }
}
