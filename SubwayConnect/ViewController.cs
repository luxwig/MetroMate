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
                new StationInfo("","","")
            };
            ArrTimeTable.Source = new StopSelTVS(stationInfos, mTAInfo);
            ArrTimeTable.RowHeight = 85;

            
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

        public override void PrepareForSegue(UIStoryboardSegue segue,
            NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            var arrivalDEtailViewController = segue.DestinationViewController
                                          as ArrivalDetailViewController;

            if (arrivalDEtailViewController != null)
            {
                stationInfos = ((StopSelTVS)ArrTimeTable.Source).GetStations(ArrTimeTable);
                List<string> URL = new List<string>();
                
                foreach (var stationinfo in stationInfos)
                {
                    if (!string.Equals(stationinfo.Name,""))
                    {
                        URL.Add(stationinfo.ID);
                    }
                }
                
                arrivalDEtailViewController.TripInfos = rtinfo.QueryByStation(URL);
                arrivalDEtailViewController.rtinfo = rtinfo;
                arrivalDEtailViewController.src = mTAInfo;
            }
        }

    }
}