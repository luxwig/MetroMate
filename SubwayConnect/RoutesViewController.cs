using Foundation;
using System;
using UIKit;
using System.Collections.Generic;

namespace MetroMate
{
    public partial class RoutesViewController : UIViewController
    {
        public RoutesViewController (IntPtr handle) : base (handle)
        {
        }

        public List<Tuple<int, string>> ListStop { set; get; }
        public MTAInfo src { set; get; }
        public int Bound { set; get; }
        public int pageIndex { set; get; }

        public RoutesViewController(List<Tuple<int, string>> ListStop,
                                   MTAInfo src,
                                   int Bound, int pageIndex) : base()
        {
            this.ListStop = ListStop;
            this.src = src;
            this.Bound = Bound;
            this.pageIndex = pageIndex;
            tabL = new RouteView();
            //View.Add(tabL);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            tabL.Source = new RouteStopTVS(src, ListStop, -1) ;
            tabL.RegisterClassForCellReuse(typeof(RouteStopViewCell), "cell_routestop");
            //Add(tabLinex);
        }
    }
}