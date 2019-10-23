// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace testMemoryIssuesv3
{
	[Register ("Details3ViewController")]
	partial class Details3ViewController
	{
		[Outlet]
		UIKit.UIImageView imgDetails { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (imgDetails != null) {
				imgDetails.Dispose ();
				imgDetails = null;
			}
		}
	}
}
