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
        private ArrivalDetailTVS ATVS;
        public int SegDircValue=0;
        public DateTime RefTime;
        public void SetSegDirc(int value)
        {
            seg_dirc.SelectedSegment = value;
        }
        public ArrivalDetailViewController (IntPtr handle) : base (handle)
        {
            RefTime = DateTime.MinValue;
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ATVS =  new ArrivalDetailTVS(TripInfos,src, this, RefTime);
            ArrTimeTable.Source = ATVS;
            ArrTimeTable.RowHeight = 110;
            SetSegDirc(SegDircValue);
            ATVS.filterDirection((int)seg_dirc.SelectedSegment);
            ArrTimeTable.ReloadData();
            //ArrTimeTable.SelectRow(NSIndexPath.FromRowSection(ATVS.GetRefTimeIndex(), 0), true, UITableViewScrollPosition.None);
            //ArrTimeTable.ScrollToNearestSelected(UITableViewScrollPosition.Middle, true);

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
                tripDetailViewController.Title = TVS.filteredInfos[rowPath.Row].Id;
                tripDetailViewController.tripinfo = TVS.filteredInfos[rowPath.Row];
                tripDetailViewController.src = src;
                tripDetailViewController.rtinfo = rtinfo;
                
            }
        }

        partial void DirctionValueChanged(UISegmentedControl sender)
        {
            ATVS.filterDirection((int)seg_dirc.SelectedSegment);
            ArrTimeTable.ReloadData();
        }
    }
}
