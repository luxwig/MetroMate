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
    [Register ("StopSelCell")]
    partial class StopSelCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_stopname { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txt_stopid { get; set; }

        [Action ("txt_stopidChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void txt_stopidChanged (UIKit.UITextField sender);

        void ReleaseDesignerOutlets ()
        {
            if (lbl_stopname != null) {
                lbl_stopname.Dispose ();
                lbl_stopname = null;
            }

            if (txt_stopid != null) {
                txt_stopid.Dispose ();
                txt_stopid = null;
            }
        }
    }
}