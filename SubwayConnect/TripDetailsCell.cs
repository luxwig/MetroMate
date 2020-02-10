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
            txt_stop.Text = string.Equals(src.GetStationInfo(tripInfo.StopTime[pos].StopId).Name, "")?
                 "<Waypoint "+tripInfo.StopTime[pos].StopId+">" : src.GetStationInfo(tripInfo.StopTime[pos].StopId).Name;

            if (tripInfo.StopTime[pos].Arrival != null)
                est = tripInfo.StopTime[pos].Arrival.Time;
            else
                est = tripInfo.StopTime[pos].Departure.Time;
            DateTime dtf = DateTimeOffset.FromUnixTimeSeconds(est).ToLocalTime().DateTime;
            if (Math.Abs((dtf - DateTime.Now).Hours) == 0)
                txt_time.Text = string.Format("{0}\n{1}", (dtf - DateTime.Now).ToString("mm'm'ss's'"),
                dtf.ToString("HH:mm:ss"));
            else
                txt_time.Text = string.Format("{0}\n{1}", (dtf - DateTime.Now).ToString("h'h'mm'm'ss's'"),
                dtf.ToString("HH:mm:ss"));
            if (dtf < DateTime.Now)
            {
                txt_time.TextColor = UIColor.FromRGB(142, 142, 147);
                txt_stop.TextColor = UIColor.FromRGB(99, 99, 102);
#if DEBUG
                Console.WriteLine(dtf.ToLongTimeString());
                Console.WriteLine(DateTime.Now.ToLongTimeString());
#endif
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
        public MTAInfo src;
        SingleTripInfoDataSource stiDS;

        public TripTVS(SingleTripInfoDataSource stiDS, MTAInfo src)
        {
            this.stiDS = stiDS;
            this.src = src;
        }
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return stiDS.Trip.Count;
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            TripDetailsCell cell = (TripDetailsCell)tableView.DequeueReusableCell("cell_trip", indexPath);
            cell.UpdateCell(stiDS.Trip, src, indexPath.Row);
            return cell;
        }
    }
}