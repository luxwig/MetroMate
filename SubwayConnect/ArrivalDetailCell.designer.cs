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
    [Register ("ArrivalDetailCell")]
    partial class ArrivalDetailCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView img_Logo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txt_CC { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txt_detail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel txt_ID { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (img_Logo != null) {
                img_Logo.Dispose ();
                img_Logo = null;
            }

            if (txt_CC != null) {
                txt_CC.Dispose ();
                txt_CC = null;
            }

            if (txt_detail != null) {
                txt_detail.Dispose ();
                txt_detail = null;
            }

            if (txt_ID != null) {
                txt_ID.Dispose ();
                txt_ID = null;
            }
        }
    }
}