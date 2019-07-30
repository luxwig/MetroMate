using Foundation;
using System;
using UIKit;
using System.Collections.Generic;

namespace MetroMate
{
    public partial class ArrivalDetailCell : UITableViewCell
    {
        public ArrivalDetailCell (IntPtr handle) : base (handle)
        {
        }
        internal void UpdateCell(TripInfo tripInfo, MTAInfo src)
        {
            txt_ID.Text = tripInfo.Id;
            txt_detail.Text = string.Format("At {0}\n{1} stop{2} from {3}",
                src.GetStationInfo(tripInfo.StopTime[0].StopId).Name,
                tripInfo.Index,
                (tripInfo.Index <= 1 ? "" : "s"),
                string.Equals(src.GetStationInfo(tripInfo.RefStop).Name,"")?
                "<Waypoint "+ tripInfo.RefStop +">": src.GetStationInfo(tripInfo.RefStop).Name
                );
            txt_CC.Text = (DateTime.Now - tripInfo.GetTargetTime).ToString("mm'm 'ss's\n'");
            txt_CC.Text += tripInfo.GetTargetTime.ToString("HH:mm:ss");
            int index = tripInfo.Id.IndexOf('_');
            if (index != -1)
            {
                img_Logo.Image = UIImage.FromBundle("logo/"+ tripInfo.Id[index + 1].ToString() +".png");
            }
        }
    }


    class ArrivalDetailTVS : UITableViewSource
    {
        public List<TripInfo> tripInfos;
        public MTAInfo src;
        
        public ArrivalDetailTVS(List<TripInfo> tripInfos, MTAInfo src)
        {
            this.tripInfos = tripInfos;
            this.src = src;
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (ArrivalDetailCell)tableView.DequeueReusableCell("cell_arrdetail", indexPath);
            var tripInfo = tripInfos[indexPath.Row];
            cell.UpdateCell(tripInfo,src);
            return cell;
        }
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return tripInfos.Count;
        }
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
        }

    }




}