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
        public int Bound { set; get; }
        public int pageIndex { set; get; }
        private int MAXVALUE;
        public RouteViewController (IntPtr handle) : base (handle)
        {
            
        }

        public void SetViewer(List<Tuple<int, string>> ListStop,
                                    MTAInfo src,
                                    int Bound, int pageIndex, int MAXVALUE)
        {
            this.ListStop = ListStop;
            this.src = src;
            this.Bound = Bound;
            this.pageIndex = pageIndex;
            this.MAXVALUE = MAXVALUE;
            Title = Bound == 0 ? "N" : "S";
        }
        public RouteViewController(List<Tuple<int, string>> ListStop,
                                    MTAInfo src,
                                    int Bound, int pageIndex) : base()
        {
            this.ListStop = ListStop;
            this.src = src;
            this.Bound = Bound;
            this.pageIndex = pageIndex;

        }



        public override void ViewDidLoad()
        {
            base.ViewDidLoad();            
            tabLinex.Source = new RouteStopTVS(src, ListStop, MAXVALUE);
            tabLinex.RowHeight = 55;
        }


        public override void PrepareForSegue(UIStoryboardSegue segue,
            NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            List<string> URL = new List<string>();
            var rowPath = tabLinex.IndexPathForSelectedRow.Row;
            URL.Add(((RouteStopTVS)tabLinex.Source).Routes[rowPath].Item2);
            Console.WriteLine(((RouteStopTVS)tabLinex.Source).Routes[rowPath].Item2);



            if (string.Equals(segue.Identifier, "searchTrip"))
            {
                var arrivalDEtailViewController = segue.DestinationViewController
                                              as ArrivalDetailViewController;

                if (arrivalDEtailViewController != null)
                {
                    arrivalDEtailViewController.rtinfo = new RTInfos(src);
                    arrivalDEtailViewController.src = src;
                    arrivalDEtailViewController.TripInfos = arrivalDEtailViewController.rtinfo.QueryByStation(URL);
                }
            }

        }
    }
}