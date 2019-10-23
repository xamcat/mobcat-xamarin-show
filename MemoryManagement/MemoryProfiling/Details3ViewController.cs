using System;
using Foundation;
using UIKit;

namespace testMemoryIssuesv3
{
    public partial class Details3ViewController : UIViewController
    {
        private UIBarButtonItem _item;

        public Details3ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            imgDetails.Image = UIImage.LoadFromData(NSData.FromUrl(NSUrl.FromString("https://picsum.photos/1024")));

            _item = new UIBarButtonItem(UIBarButtonSystemItem.Save, (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"{Environment.TickCount}: UIBarButtonItem.Save");
                this.NavigationController.PopViewController(true);
            });
            this.NavigationItem.SetRightBarButtonItem(_item, true);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            // In order to fix the memory leak - either use the UIBarButtonItem.Clicked even to subscrive and unsubscribe
            // order call the Dispose method to let the native world know that managed world has finished working with the object
            //_item?.Dispose();
        }
    }
}
