using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
namespace MetroMate
{
    public partial class RouteViewController : UITableViewController
    {
        public List<Tuple<int, string>> ListStop { set; get; }
        public MTAInfo src { set; get;}
        public char Line { get; set; }
        public int Bound { set; get; }
        public int pageIndex { set; get; }
        private int MAXVALUE;
        public RouteViewController (IntPtr handle) : base (handle)
        {
        }

        public void SetViewer(List<Tuple<int, string>> ListStop,
                                    MTAInfo src,
                                    int Bound, int pageIndex, int MAXVALUE,
                                    char Line)
        {
            this.ListStop = ListStop;
            this.src = src;
            this.Bound = Bound;
            this.pageIndex = pageIndex;
            this.MAXVALUE = MAXVALUE;
            this.Line = Line;
            Title = Line + " " + (Bound == 0 ? "N/B" : "S/B");
        }

        public RouteViewController(List<Tuple<int, string>> ListStop,
                                    MTAInfo src,
                                    int Bound, int pageIndex):base()
        {
            this.ListStop = ListStop;
            this.src = src;
            this.Bound = Bound;
            this.pageIndex = pageIndex;

        }


        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();            
            tabLinex.Source = new RouteStopTVS(src, ListStop, MAXVALUE);
            tabLinex.RowHeight = 55;
            View.BackgroundColor = src.GetLineColor(Line);
            var blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Prominent);
            var blurView = new UIVisualEffectView(blur);
            //var vibrancyEffect = UIVibrancyEffect.FromBlurEffect(
            //    UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark));
            //var vibrancyView = new UIVisualEffectView(vibrancyEffect);
            ((UITableView)View).BackgroundView = blurView;
        }


        public override void PrepareForSegue(UIStoryboardSegue segue,
            NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            List<string> URL = new List<string>();
            var rowPath = tabLinex.IndexPathForSelectedRow.Row;
            string stopID = ((RouteStopTVS)tabLinex.Source).Routes[rowPath].Item2;
            stopID = stopID.Substring(0, stopID.Length - 1);
            URL.Add(stopID);
            URL.AddRange(src.TransferComplex.GetTransferStations(stopID));
            URL = MTAInfo.AddBothDirc(URL);
            Console.WriteLine(((RouteStopTVS)tabLinex.Source).Routes[rowPath].Item2);



            if (string.Equals(segue.Identifier, "searchTrip"))
            {
                var arrivalDEtailViewController = segue.DestinationViewController
                                              as ArrivalDetailViewController;

                if (arrivalDEtailViewController != null)
                {
                    try
                    {
                        arrivalDEtailViewController.rtinfo = new RTInfos(src);
                        arrivalDEtailViewController.src = src;
                        arrivalDEtailViewController.rtinfo.QueryByStation(URL, result: out arrivalDEtailViewController.TripInfos);
                        arrivalDEtailViewController.SegDircValue = (Bound + 1);
                    }
                    catch (FeedFetchException e)
                    {
                        arrivalDEtailViewController.SagueE = e;
                    }
                }
            }

        }

    }
}