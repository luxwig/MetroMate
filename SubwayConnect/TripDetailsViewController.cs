using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetroMate
{
    public partial class TripDetailsViewController : UITableViewRefreshController
    {
        public TripInfo tripinfo;
        public MTAInfo src;
        public RTInfos rtinfo;

        public TripDetailsViewController (IntPtr handle) : base (handle)
        {
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            Xab.Source = new TripTVS(tripinfo, src);
            foreach (var id in tripinfo.StopTime)
            {
                Console.WriteLine(id.StopId);
            }
            Xab.RowHeight = 70;
            // Perform any additional setup after loading the view, typically from a nib.
            Xab.Add(GetRefreshControl());
        }

        
        public override void PrepareForSegue(UIStoryboardSegue segue,
            NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            var arrivalDEtailViewController = segue.DestinationViewController
                                         as ArrivalDetailViewController;


            if (arrivalDEtailViewController != null)
            {
                var TVS = (TripTVS)Xab.Source;
                var rowPath = Xab.IndexPathForSelectedRow;
                arrivalDEtailViewController.Title = src.GetStationInfo(TVS.tripinfo.StopTime[rowPath.Row].StopId).Name;
                arrivalDEtailViewController.src = src;
                arrivalDEtailViewController.rtinfo = rtinfo;
                List<string> stationList = new List<string>() { TVS.tripinfo.StopTime[rowPath.Row].StopId };
                stationList.AddRange(src.TransferComplex.GetTransferStations(TVS.tripinfo.StopTime[rowPath.Row].StopId));
                arrivalDEtailViewController.segDircValue = 
                    TVS.tripinfo.StopTime[rowPath.Row].StopId[TVS.tripinfo.StopTime[rowPath.Row].StopId.Length - 1] == 'N' ? 1 : 2;
                arrivalDEtailViewController.tripDS = new TripInfoDataSource(MTAInfo.AddBothDirc(stationList), rtinfo);
                arrivalDEtailViewController.refTime = RTInfos.StopLongTimeToDateTime(TVS.tripinfo.StopTime[rowPath.Row]);
            }
        }

        protected override void RefreshAsyncFunc()
        {
            Xab.ReloadData();
        }

        protected override void UpdateDataItem()
        {
            try
            { rtinfo.Refresh(); }
            catch (FeedFetchException e)
            { e.ShowAlert(this); }
        }
    }
}