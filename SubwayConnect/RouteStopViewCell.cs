using Foundation;
using System;
using UIKit;
using System.Collections.Generic;

namespace MetroMate
{
    public partial class RouteStopViewCell : UITableViewCell
    {
        public RouteStopViewCell (IntPtr handle) : base (handle)
        {
        }
        internal void UpdateCell(MTAInfo src, Tuple<int, string> info)
        {
            lbl_stopName.Text = src.GetStationInfo(info.Item2).Name;
            lbl_stopid.Text = info.Item2;
        }

    }


    class RouteStopTVS : UITableViewSource
    {
        public MTAInfo src;
        List<List<Tuple<int, string>>> Routes;
        public RouteStopTVS(MTAInfo src, List<List<Tuple<int, string>>> Routes)
        {
            this.src = src;
            this.Routes = Routes;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Routes[0].Count;
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            RouteStopViewCell cell = (RouteStopViewCell)tableView.DequeueReusableCell("cell_routestop", indexPath);
            cell.UpdateCell(src, Routes[0][indexPath.Row]);
            return cell;
        }
    }
}