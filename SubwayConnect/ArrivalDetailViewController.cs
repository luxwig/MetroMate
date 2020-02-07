using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MetroMate
{
    public partial class ArrivalDetailViewController : UIViewController
    {
        public FeedFetchException SagueE = null;
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
        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (SagueE != null)
                SagueE.ShowAlert(this);
            ATVS =  new ArrivalDetailTVS(TripInfos,src, this, RefTime);
            ArrTimeTable.Source = ATVS;
            ArrTimeTable.RowHeight = 110;
            SetSegDirc(SegDircValue);
            ATVS.filterDirection((int)seg_dirc.SelectedSegment);

            await RefreshAsync();
            AddRefreshControl();
            ArrTimeTable.Add(GetRefreshControl());

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


        // The following codes are designing to implemented pull-to-reload functionality
        // Due to poor designinig decision, the class UITableViewRefreshController cannot be used here

        private UIRefreshControl RefreshControl;
        private bool useRefreshControl = false;
        protected void RefreshAsyncFunc() { ArrTimeTable.ReloadData(); }
        protected void UpdateDataItem()
        {
            try { rtinfo.Refresh(); } catch (FeedFetchException e) { e.ShowAlert(this); }
        } 
        protected UIRefreshControl GetRefreshControl() { return RefreshControl; }

        private async Task RefreshAsync()
        {
            // only activate the refresh control if the feature is available  
            if (useRefreshControl)
                RefreshControl.BeginRefreshing();

            if (useRefreshControl)
                RefreshControl.EndRefreshing();

            RefreshAsyncFunc();
        }

        private void AddRefreshControl()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(6, 0))
            {
                RefreshControl = new UIRefreshControl();
                RefreshControl.ValueChanged += async (sender, e) =>
                {
                    // the refresh control is available, let's add it  
                    UpdateDataItem();
                    await RefreshAsync();
                };
                useRefreshControl = true;
            }
        }

    }
}
