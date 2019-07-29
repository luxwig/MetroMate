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
    [Register ("TripDetailsCell")]
    partial class TripDetailsCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txt_stop { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txt_time { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (txt_stop != null) {
                txt_stop.Dispose ();
                txt_stop = null;
            }

            if (txt_time != null) {
                txt_time.Dispose ();
                txt_time = null;
            }
        }
    }
}