using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;

namespace MetroMate
{

    enum StationType
    {
        TerminalA, TerminalAX, TransferA, TransferB, TransferX, Local, Express, TerminalB, TerminalBX
    }
    public partial class RouteStopViewCell : UITableViewCell
    {
        public UIImage RotateImage(UIImage imagex, float degree)
        {
            UIImage image = new UIImage(imagex.CGImage, 1, UIImageOrientation.DownMirrored);
            float Radians = degree * (float)Math.PI / 180;

            UIView view = new UIView(frame: new CGRect(0, 0, image.Size.Width, image.Size.Height));
            CGAffineTransform t = CGAffineTransform.MakeRotation(Radians);
            view.Transform = t;
            CGSize size = view.Frame.Size;

            UIGraphics.BeginImageContext(size);
            CGContext context = UIGraphics.GetCurrentContext();

            context.TranslateCTM(size.Width / 2, size.Height / 2);
            context.RotateCTM(Radians);
            context.ScaleCTM(1, -1);

            context.DrawImage(new CGRect(-image.Size.Width / 2, -image.Size.Height / 2, image.Size.Width, image.Size.Height), image.CGImage);

            UIImage imageCopy = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return imageCopy;
        }

        public RouteStopViewCell (IntPtr handle) : base (handle)
        {
        }
        internal void UpdateCell(MTAInfo src, int count, string stopid, StationType st, int MAX)
        {
            lbl_stopName.Text = src.GetStationInfo(stopid).Name;
            lbl_stopid.Text = stopid;
            float v = (float)count / MAX;
            bar_count.SetProgress(v, true);
            string path = "";
            var re = src.GetServLineColor(stopid);
            if (re.Item1.Length != 0)
                path = re.Item1 + "/";
            path = "lineindicator/" + path;

            switch (st)
            {
                case StationType.TerminalA:
                    imgViewer.Image = UIImage.FromBundle(path + "terminal.png");
                    break;
                case StationType.Local:
                    imgViewer.Image = UIImage.FromBundle(path + "local.png");
                    break;
                case StationType.Express:
                    imgViewer.Image = UIImage.FromBundle(path + "express.png");
                    break;
                case StationType.TransferA:
                    imgViewer.Image = 
                         new UIImage(UIImage.FromBundle(path + "transfer.png").CGImage,
                         1, UIImageOrientation.DownMirrored);
                    break;
                case StationType.TransferB:
                    imgViewer.Image = UIImage.FromBundle(path + "transfer.png");
                    break;
                case StationType.TransferX:
                    imgViewer.Image = UIImage.FromBundle(path + "transferX.png");
                    break;
                case StationType.TerminalB:
                    imgViewer.Image =
                        new UIImage(UIImage.FromBundle(path + "terminal.png").CGImage,
                        1, UIImageOrientation.DownMirrored);
                    break;
            }
            if (re.Item1.Length == 0)
            {
                imgViewer.TintColor = re.Item2;
                imgViewer.Image = imgViewer.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            }
            else
            {
                imgViewer.Image = imgViewer.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
            }

        }

    }

 

    class RouteStopTVS : UITableViewSource
    {
        public MTAInfo src;
        private int MaxValue = -1;
        private List<int> stationCount;
        public List<string> stationName;
        private List<StationType> stationType;

        public List<StationType> Promote(List<StationType> stations)
        {
            if (stations[stations.Count - 1] == StationType.Local)
            {
                stations[stations.Count - 1] = StationType.TransferA;
            }
            else
            {
                if (stations[stations.Count - 1] == StationType.TransferB)
                { stations[stations.Count - 1] = StationType.TransferX; }
            }
            stations.Add(StationType.TransferB);
            return stations;
        }
        public RouteStopTVS(MTAInfo src, List<Tuple<int, string>> Routes, int MaxValue)
        {
            this.src = src;
            this.MaxValue = MaxValue;
            stationCount = new List<int>();
            stationName = new List<string>();
            stationType = new List<StationType>();
            int i = 0;
            for (i = 0; i < Routes.Count; i++)
            {
                var r = Routes[i];
                if (i == 0)
                    stationType.Add(StationType.TerminalA);
                else
                    if (string.Equals(src.GetServLine(stationName[i - 1]),
                                     src.GetServLine(r.Item2)))
                    stationType.Add(StationType.Local);
                else
                {
                    stationType = Promote(stationType);
                }
                stationName.Add(r.Item2);
                stationCount.Add(r.Item1);
            }
            stationType[i - 1] = StationType.TerminalB;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return stationCount.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            RouteStopViewCell cell = (RouteStopViewCell)tableView.DequeueReusableCell("cell_routestop", indexPath);
            cell.UpdateCell(src, stationCount[indexPath.Row],
                                stationName[indexPath.Row],
                                stationType[indexPath.Row], MaxValue);
            return cell;
        }
    }
}