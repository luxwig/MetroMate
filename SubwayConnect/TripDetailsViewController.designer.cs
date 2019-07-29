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
    [Register ("TripDetailsViewController")]
    partial class TripDetailsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView Xab { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Xab != null) {
                Xab.Dispose ();
                Xab = null;
            }
        }
    }
}