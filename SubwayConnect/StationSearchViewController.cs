using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;

namespace MetroMate
{

    public partial class StationSearchViewController : UITableViewController
    {
        public List<StationInfo> stationInfos;
        public List<string> url;

        public StationSearchTVS TVS;
        public StationSearchViewController(IntPtr handle) : base(handle)
        {
            
        }
        private void searchTable()
        {
            //perform the search, and refresh the table with the results
            TVS.PerformSearch(searchBar.Text, (int)searchBar.SelectedScopeButtonIndex) ;
            tab_search.ReloadData();
        }

        public void SetSearchBarText(string text)
        {
            searchBar.Text = text;
        }

        public int GetSearchType()
        {
            return (int)searchBar.SelectedScopeButtonIndex;
        }
        public string searchBar_Text;
        public string SearchBarText { get { return searchBar.Text; } set { searchBar.Text = value; } }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            searchBar.Text = searchBar_Text;
            searchBar.SizeToFit();
            searchBar.AutocorrectionType = UITextAutocorrectionType.No;
            searchBar.AutocapitalizationType = UITextAutocapitalizationType.None;
            searchBar.TextChanged += (sender, e) =>
            {
                //this is the method that is called when the user searches
                searchTable();

                tab_search.ReloadData();
            };
            
            //tab_search.TableHeaderView = searchBar;
            TVS = new StationSearchTVS(stationInfos, url, this);
            tab_search.Source = TVS;
            tab_search.ReloadData();
            searchBar.SelectedScopeButtonIndexChanged += (sender, e) => {
                //this is the method that is called when the user searches
                searchTable();

                tab_search.ReloadData();
            };

            searchBar.SearchButtonClicked += (sender, e) =>
            {
                searchBar.ResignFirstResponder();
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public override void PrepareForSegue(UIStoryboardSegue segue,
            NSObject sender)
        {
            Console.WriteLine("Unwind");

        }


    }
    public class StationSearchTVS : UITableViewSource
    {
        string cellIdentifier = "cell_searchstation";

        Dictionary<string, List<StationInfo>> stationMap = new Dictionary<string, List<StationInfo>>();
        string[] keys;
        List<StationInfo> stationInfos;
        public List<string> checkedStation;
        public StationSearchTVS(List<StationInfo> stationInfos, List<string> url, StationSearchViewController owner)
        {
            this.stationInfos = stationInfos;
            PerformSearch(owner.SearchBarText, owner.GetSearchType());
            checkedStation = url;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return keys.Length;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {

            return stationMap[keys[section]].Count;
        }

        public override string[] SectionIndexTitles(UITableView tableView)
        {
            return keys;
        }


        public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.CellAt(indexPath);
            if (cell.Accessory == UITableViewCellAccessory.None)
            {
                cell.Accessory = UITableViewCellAccessory.Checkmark;
                checkedStation.Add(cell.DetailTextLabel.Text);
            }
            else
            {
                cell.Accessory = UITableViewCellAccessory.None;
                checkedStation.Remove(cell.DetailTextLabel.Text);
            }
            tableView.DeselectRow(indexPath, true);
        }
        public override string TitleForHeader (UITableView tableView, nint section)
        {
            return keys[section];
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            // request a recycled cell to save memory
            UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);
            // if there are no cells to reuse, create a new one
            if (cell == null)
                cell = new UITableViewCell(UITableViewCellStyle.Subtitle, cellIdentifier);
            cell.Accessory = UITableViewCellAccessory.None;
            cell.TextLabel.Text = stationMap[keys[indexPath.Section]][indexPath.Row].Name;
            cell.DetailTextLabel.Text = stationMap[keys[indexPath.Section]][indexPath.Row].ID;
            if (checkedStation.Contains(cell.DetailTextLabel.Text))
                cell.Accessory = UITableViewCellAccessory.Checkmark;
            else
                cell.Accessory = UITableViewCellAccessory.None;
            return cell;
            
        }

        internal void PerformSearch(string text, int NS)
        {
            stationMap.Clear();
            var filtered = stationInfos.FindAll(x =>
                (x.Name.IndexOf(text, StringComparison.OrdinalIgnoreCase) >=0 || x.ID.IndexOf(text, StringComparison.OrdinalIgnoreCase)>=0) &&
                 char.IsLetter(x.ID.Last()));
            if (NS == 1)
                filtered = filtered.FindAll(x => x.ID[x.ID.Length - 1] == 'N');
            if (NS == 2)
                filtered = filtered.FindAll(x => x.ID[x.ID.Length - 1] == 'S');
            foreach (StationInfo station in filtered)
            {
                if (stationMap.ContainsKey(station.Name[0].ToString()))
                    stationMap[station.Name[0].ToString()].Add(station);
                else
                    stationMap.Add(station.Name[0].ToString(), new List<StationInfo>() { station });
            }
            keys = stationMap.Keys.ToArray();
        }
    }
}
