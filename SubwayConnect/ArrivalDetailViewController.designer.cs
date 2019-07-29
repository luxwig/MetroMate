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
    [Register ("ArrivalDetailViewController")]
    partial class ArrivalDetailViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView ArrTimeTable { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ArrTimeTable != null) {
                ArrTimeTable.Dispose ();
                ArrTimeTable = null;
            }
        }
    }
}