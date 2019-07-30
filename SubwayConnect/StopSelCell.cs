using System;
using UIKit;

using System.Collections.Generic;
using Foundation;

namespace MetroMate
{
    public partial class StopSelCell : UITableViewCell
    {

        public StopSelCell(IntPtr handle) : base(handle)
        {

        }
        private MTAInfo mTAInfo;
        private StopSelTVS TVS;
        private int ID;
        internal void UpdateCell(
            StationInfo StationInfo,
            MTAInfo mTAInfox, StopSelTVS TVs, int ID)
        {
            this.mTAInfo = mTAInfox;
            txt_stopid.Text = StationInfo.ID;
            lbl_stopname.Text = mTAInfo.GetStationInfo(txt_stopid.Text).Name;
            TVS = TVs;
            this.ID = ID;
            txt_stopid.ShouldReturn = (sender) => {
                sender.ResignFirstResponder();
                return false;
            };
        }
        
        public void setTxt(string a)
        {
            this.lbl_stopname.Text = a;
        }

        partial void txt_stopidChanged(UITextField sender)
        {
            txt_stopid.Text = txt_stopid.Text.ToUpper();
            lbl_stopname.Text = mTAInfo.GetStationInfo(txt_stopid.Text).Name;
            TVS.stationInfos[ID] = new StationInfo(txt_stopid.Text, lbl_stopname.Text, "");
        }

        

    }

    class StopSelTVS : UITableViewSource
    {
        public List<StationInfo> stationInfos;
        MTAInfo mTAInfo;
        public StopSelTVS(List<StationInfo> stationInfos, MTAInfo mTAInfo)
        {
            this.mTAInfo = mTAInfo;
            this.stationInfos = stationInfos;
        }
        
        public List<StationInfo> GetStations(UITableView tableView)
        {   
            return stationInfos;
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (StopSelCell)tableView.DequeueReusableCell("cell_stopsel", indexPath);
            var stationInfo = stationInfos[indexPath.Row];
            cell.UpdateCell(stationInfo, mTAInfo, this, indexPath.Row);
            return cell;
        }
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return stationInfos.Count;
        }


        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
        {
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    // remove the item from the underlying data source
                    stationInfos.RemoveAt(indexPath.Row);
                    // delete the row from the table
                    tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    break;
                case UITableViewCellEditingStyle.None:
                    Console.WriteLine("CommitEditingStyle:None called");
                    break;
            }
        }
        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true; // return false if you wish to disable editing for a specific indexPath or for all rows
        }
        public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
        {   // Optional - default text is 'Delete'
            return "Trash (" + stationInfos[indexPath.Row].Name + ")";
        }

    }
}