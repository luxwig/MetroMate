using Foundation;
using System;
using UIKit;
using System.Collections.Generic;

namespace MetroMate
{
    public partial class TripDetailsViewController : UITableViewController
    {
        public TripInfo tripinfo;
        public MTAInfo src;
        public RTInfos rtinfo;
        public TripDetailsViewController (IntPtr handle) : base (handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Xab.Source = new TripTVS(tripinfo, src);
            foreach (var id in tripinfo.StopTime)
            {
                Console.WriteLine(id.StopId);
            }
            Xab.RowHeight = 70;
            // Perform any additional setup after loading the view, typically from a nib.
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
                arrivalDEtailViewController.TripInfos = rtinfo.QueryByStation(new List<string>() { TVS.tripinfo.StopTime[rowPath.Row].StopId });
            }
        }
    }
}