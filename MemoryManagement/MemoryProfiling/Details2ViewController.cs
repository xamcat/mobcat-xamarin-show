using System;
using Foundation;
using UIKit;

namespace testMemoryIssuesv3
{
    public partial class Details2ViewController : UIViewController
    {
        //private NSObject _subscription;

        public Details2ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            imgDetails.Image = UIImage.LoadFromData(NSData.FromUrl(NSUrl.FromString("https://picsum.photos/1024")));
            NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, new Action<NSNotification>(this.OnOrientationChanged));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            // In order to fix the memory leak - move subscriptipn to WillAppear and don't forget to unsubscribe in DidDisappear
            //_subscription = NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, new Action<NSNotification>(this.OnOrientationChanged));
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            //if (_subscription != null) 
            //    NSNotificationCenter.DefaultCenter.RemoveObserver(_subscription);
        }

        private void OnOrientationChanged(NSNotification not)
        {
            System.Diagnostics.Debug.WriteLine($"{Environment.TickCount}: {nameof(OnOrientationChanged)}");
        }
    }
}
