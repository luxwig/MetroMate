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
    [Register ("RouteViewController")]
    partial class RouteViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationItem nav_Name { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MetroMate.RouteView tabLinex { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (nav_Name != null) {
                nav_Name.Dispose ();
                nav_Name = null;
            }

            if (tabLinex != null) {
                tabLinex.Dispose ();
                tabLinex = null;
            }
        }
    }
}