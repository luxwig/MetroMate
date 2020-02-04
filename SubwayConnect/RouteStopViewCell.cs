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
        internal void UpdateCell(MTAInfo src, Tuple<int, string> info, int MAX)
        {
            lbl_stopName.Text = src.GetStationInfo(info.Item2).Name;
            lbl_stopid.Text = info.Item2;
            float v = (float)info.Item1 / MAX;
            bar_count.SetProgress(v, true);
        }

    }


    class RouteStopTVS : UITableViewSource
    {
        public MTAInfo src;
        private int MaxValue = -1;
        public List<Tuple<int, string>> Routes;
        public RouteStopTVS(MTAInfo src, List<Tuple<int, string>> Routes, int MaxValue)
        {
            this.src = src;
            this.Routes = Routes;
            this.MaxValue = MaxValue;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Routes.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            RouteStopViewCell cell = (RouteStopViewCell)tableView.DequeueReusableCell("cell_routestop", indexPath);
            cell.UpdateCell(src, Routes[indexPath.Row], MaxValue);
            return cell;
        }
    }
}