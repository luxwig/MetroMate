using Foundation;
using System;
using UIKit;
using System.Collections.Generic;

namespace MetroMate
{
    public partial class RouteLineViewCell : UITableViewCell
    {
        public RouteLineViewCell (IntPtr handle) : base (handle)
        {

        }
        internal void UpdateCell(MTAInfo src, int index, Dictionary<string, List<List<Tuple<int, string>>>> LineInfo)
        {
            string line = src.Lines[index].ToString();
            img_line.Image = UIImage.FromBundle("logo/" + line + ".png");
            int nah = 0;
            if (LineInfo.ContainsKey(line + "N") && LineInfo[line + "N"] != null && LineInfo[line + "N"].Count > 0)
            {
                lbl_NB.Text = src.GetStationInfo(LineInfo[line + "N"][0][LineInfo[line + "N"][0].Count - 1].Item2).Name;
                switch (LineInfo[line + "N"].Count)
                {
                    case 0:
                    case 1:
                        lbl_NBcount.Text = " ";
                        break;
                    case 2:
                        lbl_NBcount.Text =
                            src.GetStationInfo(LineInfo[line + "N"][1][LineInfo[line + "N"][1].Count - 1].Item2).Name;
                        break;
                    default:
                        lbl_NBcount.Text =
                                src.GetStationInfo(LineInfo[line + "N"][1][LineInfo[line + "N"][1].Count - 1].Item2).Name +
                                " +" + (LineInfo[line + "N"].Count - 2).ToString();
                        break;
                }
                lbl_NB.Alpha = (nfloat)1;
                lbl_NBcount.Alpha = (nfloat)1;
            }
            else
            {
                lbl_NB.Text = "NO SERVICE";
                lbl_NBcount.Text = "";

                lbl_NB.Alpha = (nfloat)0.3;
                lbl_NBcount.Alpha = (nfloat)0.3;
                nah++;
            }
            if (LineInfo.ContainsKey(line + "S") && LineInfo[line + "S"] != null && LineInfo[line + "S"].Count > 0)
            {
                lbl_SB.Text = src.GetStationInfo(LineInfo[line + "S"][0][LineInfo[line + "S"][0].Count - 1].Item2).Name;
                switch (LineInfo[line + "S"].Count)
                {
                    case 0:
                    case 1:
                        lbl_SBcount.Text = " ";
                        break;
                    case 2:
                        lbl_SBcount.Text =
                            src.GetStationInfo(LineInfo[line + "S"][1][LineInfo[line + "S"][1].Count - 1].Item2).Name;
                        break;
                    default:
                        lbl_SBcount.Text =
                                src.GetStationInfo(LineInfo[line + "S"][1][LineInfo[line + "S"][1].Count - 1].Item2).Name +
                                " +" + (LineInfo[line + "S"].Count - 2).ToString();
                        break;
                }
                lbl_SB.Alpha = (nfloat)1;
                lbl_SBcount.Alpha = (nfloat)1;
            }
            else
            {
                lbl_SB.Text = "NO SERVICE";
                lbl_SBcount.Text = "";

                lbl_SB.Alpha = (nfloat)0.3;
                lbl_SBcount.Alpha = (nfloat)0.3;
                nah++;
            }

            if (nah == 2)
            {
                img_line.Alpha = (nfloat)0.3;
                this.SelectionStyle = UITableViewCellSelectionStyle.None;
            }
            else
            {
                img_line.Alpha = (nfloat)1;
                this.SelectionStyle = UITableViewCellSelectionStyle.Default;
            }
        }
    }
    class RouteLineTVS : UITableViewSource
    {
        public MTAInfo src;
        Dictionary<string, List<List<Tuple<int, string>>>> LineRoutes;
        public RouteLineTVS(MTAInfo src, Dictionary<string, List<List<Tuple<int, string>>>> LineRoutes)
        {
            this.src = src;
            this.LineRoutes = LineRoutes;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return src.Lines.Count;
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            RouteLineViewCell cell = (RouteLineViewCell)tableView.DequeueReusableCell("cell_routeline", indexPath);
            cell.UpdateCell(src, indexPath.Row, LineRoutes);
            return cell;
        }
    }
}