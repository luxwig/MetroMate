using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;

namespace MetroMate
{
    public partial class RouteLineViewController : UITableViewController
    {
        public RouteLineViewController (IntPtr handle) : base (handle)
        {
            mTAInfo = new MTAInfo("ResSummary.json");
            routes = new RouteInfo(mTAInfo, new RTInfos(mTAInfo));
            LineRoutes = new Dictionary<string, List<List<Tuple<int, string>>>>();
            foreach (char line in mTAInfo.Lines)
            {
                LineRoutes[line + "N"] = new List<List<Tuple<int, string>>>();
                LineRoutes[line + "S"] = new List<List<Tuple<int, string>>>();
            }
        }

        public int pageIndex { get; set; }
        RouteInfo routes;
        MTAInfo mTAInfo;
        Dictionary<string, List<List<Tuple<int, string>>>> LineRoutes;
        public override  void ViewDidLoad()
        {
            var Keys = LineRoutes.Keys.ToList();
            foreach (string key in Keys)
                LineRoutes[key] = routes.GetRoutesCountUnique(key);
            base.ViewDidLoad();
            tabLine.Source = new RouteLineTVS(mTAInfo, LineRoutes);
            tabLine.RowHeight = 100;
        }

        public override bool ShouldPerformSegue(string segueIdentifier, NSObject sender)
        {
            var index = tabLine.IndexPathForSelectedRow.Row;
            if (segueIdentifier == "showLine")
            {
                if (LineRoutes.ContainsKey(mTAInfo.Lines[index].ToString() + "N") &&
                    LineRoutes[mTAInfo.Lines[index].ToString() + "N"].Count > 0)
                    return true;
                if (LineRoutes.ContainsKey(mTAInfo.Lines[index].ToString() + "S") &&
                    LineRoutes[mTAInfo.Lines[index].ToString() + "S"].Count > 0)
                    return true;
                if (!LineRoutes.ContainsKey(mTAInfo.Lines[index].ToString() + "S") &&
                    !LineRoutes.ContainsKey(mTAInfo.Lines[index].ToString() + "N"))
                    return false;
                if (LineRoutes.ContainsKey(mTAInfo.Lines[index].ToString() + "S") &&
                    LineRoutes.ContainsKey(mTAInfo.Lines[index].ToString() + "N") &&
                    (LineRoutes[mTAInfo.Lines[index].ToString() + "N"].Count +
                    LineRoutes[mTAInfo.Lines[index].ToString() + "S"].Count == 0))
                    return false;
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
    }
}