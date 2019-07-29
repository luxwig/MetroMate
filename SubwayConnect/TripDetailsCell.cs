using Foundation;
using System;
using UIKit;

namespace MetroMate
{
    public partial class TripDetailsCell : UITableViewCell
    {
        public TripDetailsCell (IntPtr handle) : base (handle)
        {
        }
        internal void UpdateCell(TripInfo tripInfo, MTAInfo src, int pos)
        {
            long est;
            txt_stop.Text = src.GetStationInfo(tripInfo.StopTime[pos].StopId).Name;
            if (tripInfo.StopTime[pos].Arrival != null)
                est = tripInfo.StopTime[pos].Arrival.Time;
            else
                est = tripInfo.StopTime[pos].Departure.Time;
            DateTime dtf = DateTimeOffset.FromUnixTimeSeconds(est).ToLocalTime().DateTime;
            txt_time.Text = string.Format("{0}\n{1}", (dtf - DateTime.Now).ToString("mm'm 'ss's'"),
                dtf.ToString("HH:mm:ss"));
            if (dtf < DateTime.Now)
            {
                txt_time.TextColor = UIColor.FromRGB(142, 142, 147);
                txt_stop.TextColor = UIColor.FromRGB(99, 99, 102);
                Console.WriteLine(dtf.ToLongTimeString());
                Console.WriteLine(DateTime.Now.ToLongTimeString());
            }
            else
            {
                txt_time.TextColor = TintColor;
                txt_stop.TextColor = UIColor.Black;
            }
        }

    }

    class TripTVS : UITableViewSource
    {
        public TripInfo tripinfo;
        public MTAInfo src;
        public TripTVS(TripInfo tripinfo, MTAInfo src)
        {
            this.tripinfo = tripinfo;
            this.src = src;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return tripinfo.Count;
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            TripDetailsCell cell = (TripDetailsCell)tableView.DequeueReusableCell("cell_trip", indexPath);
            cell.UpdateCell(tripinfo, src, indexPath.Row);
            return cell;
        }
    }
}