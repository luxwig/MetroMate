using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetroMate
{
    public partial class TripDetailsViewController : UITableViewController
    {
        public TripInfo tripinfo;
        public MTAInfo src;
        public RTInfos rtinfo;
        private bool useRefreshControl = false;
        public UIRefreshControl RefreshControl;

        private async Task RefreshAsync()
        {
            // only activate the refresh control if the feature is available  
            if (useRefreshControl)
                RefreshControl.BeginRefreshing();

            if (useRefreshControl)
                RefreshControl.EndRefreshing();

            Xab.ReloadData();
        }


        private void AddRefreshControl()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(6, 0))
            {
                RefreshControl = new UIRefreshControl();
                RefreshControl.ValueChanged += async (sender, e) =>
                {
                    // the refresh control is available, let's add it  
                    rtinfo.Refresh();
                    await RefreshAsync();
                };
                useRefreshControl = true;
            }
        }

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

            await RefreshAsync();
            AddRefreshControl();
            Xab.Add(RefreshControl);
        }

        private List<string> AddBothDirc(List<string> list)
        {
            HashSet<string> r = new HashSet<string>();
            foreach (var item in list)
            {
                if (char.IsLetter(item[item.Length - 1]))
                {
                    r.Add(item.Substring(0, item.Length - 1) + "S");
                    r.Add(item.Substring(0, item.Length - 1) + "N");
                }
                if (char.IsDigit(item[item.Length - 1]))
                {
                    r.Add(item + "S");
                    r.Add(item + "N");
                }
            }
            return r.ToList();
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
                arrivalDEtailViewController.SegDircValue = 
                    TVS.tripinfo.StopTime[rowPath.Row].StopId[TVS.tripinfo.StopTime[rowPath.Row].StopId.Length - 1] == 'N' ? 1 : 2;
                arrivalDEtailViewController.TripInfos = rtinfo.QueryByStation(AddBothDirc(stationList));
                arrivalDEtailViewController.RefTime = RTInfos.StopLongTimeToDateTime(TVS.tripinfo.StopTime[rowPath.Row]);
            }
        }
    }
}