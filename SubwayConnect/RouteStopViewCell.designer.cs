// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MetroMate
{
    [Register ("RouteStopViewCell")]
    partial class RouteStopViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView bar_count { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_stopid { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_stopName { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (bar_count != null) {
                bar_count.Dispose ();
                bar_count = null;
            }

            if (lbl_stopid != null) {
                lbl_stopid.Dispose ();
                lbl_stopid = null;
            }

            if (lbl_stopName != null) {
                lbl_stopName.Dispose ();
                lbl_stopName = null;
            }
        }
    }
}