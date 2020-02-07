using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;

namespace MetroMate
{
    public partial class RouteLineViewController : UITableViewRefreshController
    {


        private void RefreshLineRoutes()
        {
            var Keys = LineRoutes.Keys.ToList();
            foreach (string key in Keys)
            {
                if (LineRoutes.ContainsKey(key))
                {
                    Tuple<HashSet<string>, HashSet<string>> tmp;
                    LineRoutes[key] = routes.GetRoutesCountUnique(key,out tmp);
                    if (tmp != null)
                        ht[key] = tmp.Item2.ToList();
                    else
                        ht[key] = null;
                    Console.WriteLine(key);
                }
            }
        }

        public RouteLineViewController(IntPtr handle) : base(handle)
        {
            try
            {
                mTAInfo = new MTAInfo("ResSummary.json");
                rtinfos = new RTInfos(mTAInfo);
                ht = new Dictionary<string, List<string>>();
                LineRoutes = new Dictionary<string, List<List<Tuple<int, string>>>>();
                foreach (char line in mTAInfo.Lines)
                {
                    LineRoutes[line + "N"] = new List<List<Tuple<int, string>>>();
                    LineRoutes[line + "S"] = new List<List<Tuple<int, string>>>();
                }
                routes = new RouteInfo(mTAInfo, rtinfos);
                routes.Refresh();
            }
            catch (FeedFetchException e)
            {
                e.ShowAlert(this);
            }
        }

        private bool init = true;
        public int pageIndex { get; set; }
        RouteInfo routes;
        MTAInfo mTAInfo;
        RTInfos rtinfos;
        Dictionary<string, List<List<Tuple<int, string>>>> LineRoutes;
        Dictionary<string, List<string>> ht;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            tabLine.RowHeight = 100;
            RefreshLineRoutes();
            tabLine.Source = new RouteLineTVS(mTAInfo, LineRoutes, ht);
            tabLine.Add(GetRefreshControl());
        }

        public override bool ShouldPerformSegue(string segueIdentifier, NSObject sender)
        {
            var index = tabLine.IndexPathForSelectedRow.Row;
            if (segueIdentifier == "showLine")
            {
                if (LineRoutes.ContainsKey(mTAInfo.Lines[index].ToString() + "N") &&
                    LineRoutes[mTAInfo.Lines[index].ToString() + "N"] != null &&
                    LineRoutes[mTAInfo.Lines[index].ToString() + "N"].Count > 0)
                    return true;
                if (LineRoutes.ContainsKey(mTAInfo.Lines[index].ToString() + "S") &&
                    LineRoutes[mTAInfo.Lines[index].ToString() + "S"] != null &&
                    LineRoutes[mTAInfo.Lines[index].ToString() + "S"].Count > 0)
                    return true;
                return false;
                //if (!LineRoutes.ContainsKey(mTAInfo.Lines[index].ToString() + "S") &&
                //    !LineRoutes.ContainsKey(mTAInfo.Lines[index].ToString() + "N"))
                //    return false;
                //if (LineRoutes.ContainsKey(mTAInfo.Lines[index].ToString() + "S") &&
                //    LineRoutes.ContainsKey(mTAInfo.Lines[index].ToString() + "N") &&
                //    (LineRoutes[mTAInfo.Lines[index].ToString() + "N"].Count +
                //    LineRoutes[mTAInfo.Lines[index].ToString() + "S"].Count == 0))
                //    return false;
            }
            return base.ShouldPerformSegue(segueIdentifier, sender);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue,
            NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (string.Equals(segue.Identifier, "showLine"))
            {
                var lineDetailViewController = segue.DestinationViewController as RoutePageViewController;
                var index = tabLine.IndexPathForSelectedRow.Row;
                if (lineDetailViewController != null)
                {
                    lineDetailViewController.SetPages(LineRoutes[mTAInfo.Lines[index].ToString() + "N"],
                        LineRoutes[mTAInfo.Lines[index].ToString() + "S"],
                        mTAInfo,
                        mTAInfo.Lines[index].ToString());
                    //lineDetailViewController.NList = LineRoutes[mTAInfo.Lines[index].ToString() + "N"];
                    //lineDetailViewController.SList = LineRoutes[mTAInfo.Lines[index].ToString() + "S"];
                    //lineDetailViewController.Title = (mTAInfo.Lines[index].ToString());
                    //lineDetailViewController.src = mTAInfo;
                }
            }

        }

        protected override void RefreshAsyncFunc()
        {
            tabLine.ReloadData();
        }

        protected override void UpdateDataItem()
        {
            try
            {
                rtinfos.Refresh();
                routes.Refresh(rtinfos);
                RefreshLineRoutes();
            }
            catch (FeedFetchException e)
            {
                e.ShowAlert(this);
            }

        }
    }
}