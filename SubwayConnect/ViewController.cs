using Foundation;
using System;
using UIKit;
using System.Linq;
using System.Collections.Generic;

namespace MetroMate
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
            rtinfo = new RTInfos(mTAInfo);
        }
        List<StationInfo> stationInfos;
        MTAInfo mTAInfo = new MTAInfo("ResSummary.json");
        RTInfos rtinfo;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            stationInfos = new List<StationInfo>
            {
                new StationInfo()
            };
            ArrTimeTable.Source = new StopSelTVS(stationInfos, mTAInfo);
            ArrTimeTable.RowHeight = 75;
            ArrTimeTable.ReloadData();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void Btn_add_TouchUpInside(UIButton sender)
        {
            stationInfos = ((StopSelTVS)ArrTimeTable.Source).GetStations(ArrTimeTable);
            stationInfos.Add(new StationInfo());
            ArrTimeTable.ReloadData();
        }

        [Action("UnwindToArrView:")]
        public void UnwindToArrView(UIStoryboardSegue segue)
        {
            StationSearchViewController stationSearchViewController = segue.SourceViewController as StationSearchViewController;
            if (stationSearchViewController != null)
            {
                StopSelTVS TVS = ArrTimeTable.Source as StopSelTVS;
                TVS.stationInfos = new List<StationInfo>();
                foreach (string station in stationSearchViewController.TVS.checkedStation)
                {
                    TVS.stationInfos.Add(mTAInfo.GetStationInfo(station));
                }
                ArrTimeTable.ReloadData();
            }
        }


        public override void PrepareForSegue(UIStoryboardSegue segue,
            NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            stationInfos = ((StopSelTVS)ArrTimeTable.Source).GetStations(ArrTimeTable);
            List<string> URL = new List<string>();

            foreach (var stationinfo in stationInfos)
            {
                if (!string.Equals(stationinfo.Name, ""))
                {
                    if (char.IsLetter(stationinfo.ID[stationinfo.ID.Length-1])) URL.Add(stationinfo.ID);
                    else
                    {
                        URL.Add(stationinfo.ID + "S");
                        URL.Add(stationinfo.ID + "N");
                    }
                }
            }


            if (string.Equals(segue.Identifier, "searchTrip"))
            {
                var arrivalDEtailViewController = segue.DestinationViewController
                                              as ArrivalDetailViewController;

                if (arrivalDEtailViewController != null)
                {
                    arrivalDEtailViewController.tripDS = new TripInfoDataSource(URL, rtinfo);
                    arrivalDEtailViewController.rtinfo = rtinfo;
                    arrivalDEtailViewController.src = mTAInfo;
                }
            }

            if (string.Equals(segue.Identifier, "searchStation"))
            {
                StationSearchViewController stationSearchViewController = segue.DestinationViewController as StationSearchViewController;
                if (stationSearchViewController != null)
                {
                    var cell = ArrTimeTable.CellAt(ArrTimeTable.IndexPathForSelectedRow) as StopSelCell;

                    stationSearchViewController.searchBar_Text = cell.Getstopid();
                    stationSearchViewController.stationInfos = mTAInfo.GetStations();
                    stationSearchViewController.stationInfos.Sort();
                    stationSearchViewController.url = URL;
                }
            }

        }
    }
}