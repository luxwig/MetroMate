using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
namespace MetroMate
{
    public partial class RouteView : UITableView
    {
        public RouteView (IntPtr handle) : base (handle)
        {
        }

        public RouteView() : base()
        {

        }

        public List<List<Tuple<int, string>>> NList { set; get; }
        public List<List<Tuple<int, string>>> SList { set; get; }
        public MTAInfo src { set; get; }
        private string titleName;

        public void SetView()
        {
            this.Source = new RouteStopTVS(src, null, -1);
            this.RowHeight = 55;
        }
    }
}