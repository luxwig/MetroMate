using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
namespace MetroMate
{
    public partial class ArrivalDetailViewController : UIViewController
    {
        
        public List<TripInfo> TripInfos;
        public RTInfos rtinfo;
        public MTAInfo src;
        public ArrivalDetailViewController (IntPtr handle) : base (handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ArrTimeTable.Source = new ArrivalDetailTVS(TripInfos,src);
            ArrTimeTable.RowHeight = 110;
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }


        public override void PrepareForSegue(UIStoryboardSegue segue,
            NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            var tripDetailViewController = segue.DestinationViewController
                                          as TripDetailsViewController;

            if (tripDetailViewController != null)
            {
                var TVS = (ArrivalDetailTVS)ArrTimeTable.Source;
                var rowPath = ArrTimeTable.IndexPathForSelectedRow;
                tripDetailViewController.Title = TVS.tripInfos[rowPath.Row].Id;
                tripDetailViewController.tripinfo = TVS.tripInfos[rowPath.Row];
                tripDetailViewController.src = src;
                tripDetailViewController.rtinfo = rtinfo;
            }
        }

    }
}