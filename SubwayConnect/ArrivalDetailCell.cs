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
        internal void UpdateCell(TripInfo tripInfo, MTAInfo src, DateTime RefTime)
        {
            txt_ID.Text = tripInfo.Id;
            txt_detail.Text = string.Format("At {0}\n{1} stop{2} from {3}",
                src.GetStationInfo(tripInfo.StopTime[0].StopId).Name,
                tripInfo.Index,
                (tripInfo.Index <= 1 ? "" : "s"),
                string.Equals(src.GetStationInfo(tripInfo.RefStop).Name,"")?
                "<Waypoint "+ tripInfo.RefStop +">": src.GetStationInfo(tripInfo.RefStop).Name
                );
            txt_CC.Text = "";
            if (Math.Abs((DateTime.Now - tripInfo.GetTargetTime).Hours) > 0)
                txt_CC.Text = (DateTime.Now - tripInfo.GetTargetTime).ToString("h'h'");
            txt_CC.Text += (DateTime.Now - tripInfo.GetTargetTime).ToString("mm'm'ss's\n'");
            txt_CC.Text += tripInfo.GetTargetTime.ToString("HH:mm:ss");
            if (src.GetLineCode(tripInfo.Id).Length > 0)
            {
                img_Logo.Image = UIImage.FromBundle("logo/"+ src.GetLineCode(tripInfo.Id) +".png");
            }

            if (DateTime.Now > tripInfo.GetTargetTime)
                txt_CC.TextColor = UIColor.FromRGB(142, 142, 147);
            else if (RefTime > tripInfo.GetTargetTime)
                txt_CC.TextColor = UIColor.FromRGB(255,45,85);
            else
                txt_CC.TextColor = TintColor;
        }
    }


    class ArrivalDetailTVS : UITableViewSource
    {
        public TripInfoDataSource tiDS;
        public List<TripInfo> tripInfos;
        public List<TripInfo> filteredInfos;
        public MTAInfo src;
        public ArrivalDetailViewController owner;
        public DateTime RefTime;
        public ArrivalDetailTVS(TripInfoDataSource tiDS, MTAInfo src, ArrivalDetailViewController owner, DateTime RefTime)
        {
            Exception e = null;
            this.tiDS = tiDS;
            tripInfos = tiDS.GetData(out e);
            filteredInfos = tripInfos;
            this.src = src;
            this.owner = owner;
            this.RefTime = RefTime;
            if (e != null)
                ((FeedFetchException)e).ShowAlert(owner);
        }
        public void Refresh()
        {
            Exception e = null;
            tripInfos = tiDS.GetData(out e, 1);
            if (e != null)
                ((FeedFetchException)e).ShowAlert(owner);
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (ArrivalDetailCell)tableView.DequeueReusableCell("cell_arrdetail", indexPath);
            var tripInfo = filteredInfos[indexPath.Row];
            cell.UpdateCell(tripInfo, src, RefTime);
            return cell;
        }
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return filteredInfos.Count;
        }
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
        }
        private char GetDirectionFromId(string str)
        {
            int index = str.LastIndexOf(".");
            if (index != -1)
                return str[index + 1];
            return ' ';
        }
        public void filterDirection(int dirc)
        {
            if (dirc == 0)
                filteredInfos = tripInfos;
            if (dirc == 1)
                filteredInfos = tripInfos.FindAll(x => GetDirectionFromId(x.Id) == 'N');
            if (dirc == 2)
                filteredInfos = tripInfos.FindAll(x => GetDirectionFromId(x.Id) == 'S');
        }

        public int GetRefTimeIndex()
        {
            for (int i = 0; i < filteredInfos.Count; i++)
            {
                if (filteredInfos[i].GetTargetTime >= RefTime)
                    return i;
            }
            return filteredInfos.Count - 1;
        }
    }




}