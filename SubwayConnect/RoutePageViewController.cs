using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;

namespace MetroMate
{
    public partial class RoutePageViewController : UIPageViewController
    {


		public List<List<Tuple<int, string>>> NList { set; get; }
		public List<List<Tuple<int, string>>> SList { set; get; }
		public MTAInfo src { set; get; }

		private string titleName;

		public RoutePageViewController (IntPtr handle) : base (handle)
        {
			pages = new List<RouteViewController>();
            DidFinishAnimating += RoutePageViewController_DidFinishAnimating;
		}

        private void RoutePageViewController_DidFinishAnimating(object sender, UIPageViewFinishedAnimationEventArgs e)
        {
			Title = ViewControllers[0].Title;
        }

        public void SetPages(List<List<Tuple<int, string>>> NList,
                            List<List<Tuple<int, string>>> SList,
                            MTAInfo src,
                            string titleName)
        {
			int MAXVALUE = -1;
            foreach (var listStop in NList)
				foreach (var x in listStop)
					MAXVALUE = Math.Max(MAXVALUE, x.Item1);

			int i = 0;
            foreach (var listStop in NList)
            {
				var a = Storyboard.InstantiateViewController("RVC") as RouteViewController;
				a.SetViewer(listStop, src, 0, i++, MAXVALUE, titleName[0]);
                pages.Add(a);
			}

			MAXVALUE = -1;
			foreach (var listStop in SList)
				foreach (var x in listStop)
					MAXVALUE = Math.Max(MAXVALUE, x.Item1);

			foreach (var listStop in SList)
			{
				var a = Storyboard.InstantiateViewController("RVC") as RouteViewController;
				a.SetViewer(listStop, src, 1, i++, MAXVALUE, titleName[0]);
				pages.Add(a);
			}
			this.titleName = titleName;
			this.src = src;
			nfloat H, B, S, A;
			UIColor color = src.GetLineColor(titleName[0]);
			// color.GetHSBA(out H, out S, out B, out A);
			// UIPageControl.Appearance.BackgroundColor = UIColor.FromHSBA(H, S, B * (nfloat)1.1, A);
			UIPageControl.Appearance.BackgroundColor = UIColor.SystemBackgroundColor;
			UIPageControl.Appearance.CurrentPageIndicatorTintColor = UIColor.SystemBlueColor;
			UIPageControl.Appearance.PageIndicatorTintColor = UIColor.SecondaryLabelColor;
		}
		private List<RouteViewController> pages;
        public RouteViewController ViewControllerAtIndex(int index)
        {
			// Title = titleName + (pages[index].Bound == 0 ? " N/B" : " S/B");
			return pages[index];
        }

		public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			DataSource = new PageViewControllerDataSource(this, pages.Count);
			var VC = new RouteViewController[1];
			VC[0] = pages[0];
			Title = titleName + (pages[0].Bound == 0 ? " N/B" : " S/B");
			SetViewControllers(VC, UIPageViewControllerNavigationDirection.Forward, false, null);
		}

		private class PageViewControllerDataSource : UIPageViewControllerDataSource
		{
			private RoutePageViewController _parentViewController;
			private int pagesNum;

			public PageViewControllerDataSource(UIViewController parentViewController, int pagesNum)
			{
				_parentViewController = parentViewController as RoutePageViewController;
				this.pagesNum = pagesNum;
			}

			public override UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
			{
				var vc = referenceViewController as RouteViewController;
				var index = vc.pageIndex;
				index--;
				if (index < 0)
				{
					index = 0;
					return null;
				}
				return _parentViewController.ViewControllerAtIndex(index);

			}

			public override UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
			{
				var vc = referenceViewController as RouteViewController;
				var index = vc.pageIndex;

				index++;
				if (index == pagesNum)
				{
					index = pagesNum - 1;
					return null;
				}
				return _parentViewController.ViewControllerAtIndex(index);

			}

			public override nint GetPresentationCount(UIPageViewController pageViewController)
			{
				return pagesNum;
			}

			public override nint GetPresentationIndex(UIPageViewController pageViewController)
			{
				return 0;
			}
		}
	}
}