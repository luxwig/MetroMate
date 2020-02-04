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
    [Register ("RouteLineViewCell")]
    partial class RouteLineViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView img_line { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_NB { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_NBcount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_SB { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lbl_SBcount { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (img_line != null) {
                img_line.Dispose ();
                img_line = null;
            }

            if (lbl_NB != null) {
                lbl_NB.Dispose ();
                lbl_NB = null;
            }

            if (lbl_NBcount != null) {
                lbl_NBcount.Dispose ();
                lbl_NBcount = null;
            }

            if (lbl_SB != null) {
                lbl_SB.Dispose ();
                lbl_SB = null;
            }

            if (lbl_SBcount != null) {
                lbl_SBcount.Dispose ();
                lbl_SBcount = null;
            }
        }
    }
}