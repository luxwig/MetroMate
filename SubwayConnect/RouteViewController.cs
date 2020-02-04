using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
namespace MetroMate
{
    public partial class RouteViewController : UITableViewController
    {
        public List<List<Tuple<int, string>>> NList { set; get; }
        public List<List<Tuple<int, string>>> SList { set; get; }
        public MTAInfo src { set; get;}
        private string titleName;
        public RouteViewController (IntPtr handle) : base (handle)
        {
            
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            tabLine.Source = new RouteStopTVS(src, NList);
            tabLine.RowHeight = 55;
        }
    }
}