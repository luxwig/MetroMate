using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace MetroMate
{
    public abstract class UITableViewRefreshController : UITableViewController
    {
        private UIRefreshControl RefreshControl;
        private bool useRefreshControl = false;
        protected abstract void RefreshAsyncFunc();
        protected abstract void UpdateDataItem();
        protected UIRefreshControl GetRefreshControl() { return RefreshControl; }

        public UITableViewRefreshController(IntPtr handle) : base (handle)
        {
        }

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

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            await RefreshAsync();
            AddRefreshControl();
        }
    }
}
