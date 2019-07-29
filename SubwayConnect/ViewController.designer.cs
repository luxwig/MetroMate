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
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView ArrTimeTable { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btn_add { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btn_search { get; set; }

        [Action ("Btn_add_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void Btn_add_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ArrTimeTable != null) {
                ArrTimeTable.Dispose ();
                ArrTimeTable = null;
            }

            if (btn_add != null) {
                btn_add.Dispose ();
                btn_add = null;
            }

            if (btn_search != null) {
                btn_search.Dispose ();
                btn_search = null;
            }
        }
    }
}