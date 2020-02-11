#pragma warning disable CS0649

using CsvHelper;
using System;
using TransitRealtime;
using ProtoBuf;
using StopTimeUpdate = TransitRealtime.TripUpdate.StopTimeUpdate;
using Foundation;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using CsvHelper.Configuration.Attributes;
using System.Linq;
using System.Net;
using UIKit;

using Xamarin.Essentials;

namespace MetroMate
{

    /// <summary>
    /// Station infomation
    /// </summary>
    public class StationInfo:IComparable, IComparable<StationInfo>
    {
        [Name("stop_id")]
        public string ID { get; set; }

        [Name("stop_name")]
        public string Name { get; set; }

        [Name("parent_station")]
        public string Parents { get; set; }

        [Name("serv_line")]
        public string ServLine { get; set; }

        // Constructor 
        public StationInfo()
        {
            ID = ""; Name = ""; Parents = ""; ServLine = "";
        }
        public StationInfo(string id, string name, string parents, string serv_line)
        {
            ID = id;
            Name = name;
            Parents = parents;
            ServLine = serv_line;
        }


        // Compare
        public int CompareTo(object obj)
        {
            return
                string.Compare(Name, (obj as StationInfo).Name) == 0 ?
                string.Compare(ID, (obj as StationInfo).ID) :
                string.Compare(Name, (obj as StationInfo).Name);
        }

        public int CompareTo(StationInfo other)
        {
            return string.Compare(Name, other.Name) == 0 ?
                string.Compare(ID, other.ID) :
                string.Compare(Name, other.Name);
        }
    }


    /// <summary>
    /// Transfer information
    /// </summary>
    public class TransferInfo
    {
        [Name("from_stop_id")]
        public string FromStop { get; set; }

        [Name("to_stop_id")]
        public string ToStop { get; set; }

        // Constructor
        public TransferInfo()
        {
            FromStop = ToStop = "";
        }
        public TransferInfo(string fromStop, string toStop)
        {
            FromStop = fromStop;
            ToStop = toStop;
        }
    }

    /// <summary>
    /// Trip information
    /// </summary>
    public class TripInfo : IComparable, IComparable<TripInfo>
    {

        // Constructor
        public TripInfo(int index, StopTimeUpdate[] stopTime, string refStop, string id)
        {
            Index = index;
            StopTime = stopTime;
            RefStop = refStop;
            Id = id;
            if (stopTime[Index].Arrival != null)
                est = stopTime[Index].Arrival.Time;
            else
                est = stopTime[Index].Departure.Time;
        }
        protected long est;

        public int Count { get { return StopTime.Length; } }
        public long GetTargetLongTime { get { return est; } }
        public DateTime GetTargetTime { get {
                DateTimeOffset dtf = DateTimeOffset.FromUnixTimeSeconds(est).ToLocalTime();
                return dtf.DateTime;
            } }

        public string Id { get; set; }
        public int Index { get; set; }
        public string RefStop { get; set; }
        public StopTimeUpdate[] StopTime { get; set; }
        

        //Compare
        public int CompareTo(object obj)
        {
            if (est < (obj as TripInfo).est)
                return -1;
            if (est == (obj as TripInfo).est)
                return 0;
            return 1;
        }

        public int CompareTo(TripInfo other)
        {
            if (est < other.est)
                return -1;
             if (est == other.est)
                return 0;
            return 1;
        }
    }

    public class SingleTripInfoDataSource
    {
        private TripInfoDataSource tiDS;
        private TripInfo trip;
        private readonly string uid;
        private TripInfo TripHelper(out Exception e)
        {
            var ts = tiDS.GetData(out e);
            foreach (var t in ts)
                if (t.Id == uid)
                    return t;
            return null;
        }

        public TripInfo Trip { get { return trip; } }
        public SingleTripInfoDataSource(TripInfoDataSource tiDS, string uid)
        {
            this.uid = uid;
            this.tiDS = tiDS;
            Exception e = null;
            trip = TripHelper(out e);
            if (trip == null)
                throw new Exception("Trip does not exsit");
            if (e != null)
                throw (FeedFetchException)e;
        }

        public void Refresh()
        {
            Exception e = null;
            TripInfo temp = TripHelper(out e);
            if (temp == null)
                return;
            int i = 0;
            for (i = 0; i < trip.StopTime.Length; i++)
                if (trip.StopTime[i].StopId == temp.StopTime[0].StopId)
                    break;
            
            foreach (var stop in temp.StopTime)
            {
                if (i >= trip.StopTime.Length) {
#if DEBUG
                    Console.WriteLine("Stop cannot be match {0}", stop.StopId);
#endif
                    return;
                }
                    if (stop.StopId == trip.StopTime[i].StopId)
                    trip.StopTime[i] = stop;
                else
                    i++;
            }
        }
          
    }
    public class TripInfoDataSource
    {
        private readonly List<string> stations;
        private readonly RTInfos RTInfosSrc;

        public TripInfoDataSource(List<string> stations, RTInfos RTInfosSrc)
        {
            this.stations = stations;
            this.RTInfosSrc = RTInfosSrc;
        }


        /// <summary>
        /// Get trip infomation
        /// </summary>
        /// <param name="e">FeedFetchException if exsist</param>
        /// <param name="refreshFlag">0 Auto, 1 Force Refresh, 2 Force not Refresh</param>
        /// <returns></returns>
        public List<TripInfo> GetData(out Exception e, int refreshFlag = 0)
        {
            List<TripInfo> result = null;
            e = (FeedFetchException)RTInfosSrc.QueryByStation(stations, out result, refreshFlag);
            return result;
        }


        /// <summary>
        /// Refresh RTInfo
        /// </summary>
        /// <exception cref="FeedFetchException">It will be propogated from RTInfosSrc.Refresh()</exception>
        public void Refresh()
        {
            RTInfosSrc.Refresh();
        }
    }
    /// <summary>
    /// Feed information - Store ID with the name in the JSON
    /// </summary>
    public class FeedIDInfo
    {
        /// <summary>ID in JSON</summary>
        public readonly List<string> FeedID;
        /// <summary>Name in JSON</summary>
        public readonly string Abb;

        // Constructor
        public FeedIDInfo(char[] feedid, string abb)
        {
            FeedID = new List<string>();
            foreach (int i in feedid)
                FeedID.Add(i.ToString());
            Abb = abb;
        }
    }

    /// <summary>
    /// Real time information
    /// </summary>
    public class RTInfos
    {

        /// <summary>
        /// Cashe for feed message
        /// Refresh() has to be called before any other actions
        /// </summary>
        private struct FeedMessageCashe
        {
            public FeedMessage Feed;
            public DateTime Timestamp;
            public FeedMessageCashe(FeedMessage Feed, DateTime Timestamp) { this.Feed = Feed; this.Timestamp = Timestamp; }
        }

        private HashSet<string> lastQuery;
        private Dictionary<string, FeedMessageCashe> cacheFeed;
        public  MTAInfo src;

        // Constructor
        public RTInfos(MTAInfo src)
        {
            this.src = src;
            cacheFeed = new Dictionary<string, FeedMessageCashe>();
        }

        /// <summary>
        /// Get the feed from url path
        /// </summary>
        /// <exception cref="FeedMessageException">
        /// Rethrow any exception when getting url failed
        /// </exception>
        private static FeedMessage GetFeed(string path)
        {
            var req = WebRequest.Create(path);
            HttpWebResponse response = null;
            Stream dataStream = null;
            FeedMessage feed = null;
            try
            {
                response = (HttpWebResponse)req.GetResponse();
                dataStream = response.GetResponseStream();
                feed = Serializer.Deserialize<FeedMessage>(dataStream);
            }
            catch (Exception e)
            {
                throw new FeedMessageException(e);
            }
            finally
            {
                if (dataStream != null)
                    dataStream.Close();
                if (response != null)
                    response.Close();
            }
            return feed;
        }
        private static List<TripInfo> GetTripInfos(
            FeedMessage feed, string seekStop)
        {
            // seekingStop = 255 all stops
            List<TripInfo> tripInfos = new List<TripInfo>();
            foreach (FeedEntity ent in feed.Entities)
            {
                if (ent.TripUpdate != null &&
                    ent.TripUpdate.StopTimeUpdates.Count != 0)
                {
                    int i = 0;
                    foreach (StopTimeUpdate stopTimeUpdate in ent.TripUpdate.StopTimeUpdates)
                    {

                        if (string.Equals(stopTimeUpdate.StopId, seekStop) || string.Equals((char)255 + "", seekStop))
                        {
                            tripInfos.Add(new TripInfo(i, ent.TripUpdate.StopTimeUpdates.ToArray(), seekStop, ent.TripUpdate.Trip.TripId));
                            break;
                        }
                        i += 1;
                    }

                }
            }
            return tripInfos;
        }

        public static DateTime StopLongTimeToDateTime(StopTimeUpdate stop)
        {
            long est;
            if (stop.Arrival != null)
                est = stop.Arrival.Time;
            else
                est = stop.Departure.Time;
            return DateTimeOffset.FromUnixTimeSeconds(est).ToLocalTime().DateTime;

        }

        /// <summary>Refresh data in last query</summary>
        /// <exception cref="FeedFetchException">
        /// When one of the feed requests is failed,
        /// the rest of the feed requests would still be processed.
        /// After all requests are processed,
        /// FeedFetchException is retrhown
        /// </exception>   
        public void Refresh()
        {
            FeedFetchException feedE = null;
            foreach (string url in lastQuery){
                try
                {
                    Console.WriteLine("Refreshing {0}", url);
                    cacheFeed[url] = new FeedMessageCashe(GetFeed(url), DateTime.Now);
                }
                catch (FeedMessageException e)
                {
                    if (feedE == null) feedE = new FeedFetchException();
                    feedE.AddException(e, src.GetLineNameFromURL(url));
                }
            }
            if (feedE != null) throw feedE;
        }

        /// <summary>
        /// Get trip information based on stations iD
        /// </summary>
        /// <param name="stations">List of station ID. If id is (char)255, this means all data</param>
        /// <param name="result">
        /// List of TripInfo conatins trip informations requested.
        /// Same as return value. Used when the exceptions are raised
        /// </param>
        /// <param name="refreshFlag">0 Auto, 1 Force Refresh, 2 Force not Refresh</param>
        /// <returns>Exception if raised. Otherwise null</returns>
        public Exception QueryByStation(List<string> stations,  out List<TripInfo> result, int refreshFlag = 0)
        {
            List<TripInfo> r = new List<TripInfo>();
            lastQuery = new HashSet<string>();
            FeedFetchException feedE = null;
            foreach (string station in stations)
            {
                foreach (string url in src.GetFeedURL(station))
                {
                    lastQuery.Add(url);
                    if (cacheFeed.ContainsKey(url))
                        Console.WriteLine("Cashe Time {0}", (DateTime.Now - cacheFeed[url].Timestamp).TotalSeconds);
                    if (!cacheFeed.ContainsKey(url) ||
                        ((DateTime.Now - cacheFeed[url].Timestamp).TotalSeconds > 30 && refreshFlag != 2) ||
                        refreshFlag == 1)
                    {
                        try
                        {
                            Console.WriteLine("Refreshing {0}", url);
                            FeedMessage feed = GetFeed(url);
                            cacheFeed[url] = new FeedMessageCashe(feed, DateTime.Now);
                        }
                        catch (FeedMessageException e)
                        {
                            if (feedE == null) feedE = new FeedFetchException();
                            feedE.AddException(e, src.GetLineNameFromURL(url));
                        }
                    }
                    if (cacheFeed.ContainsKey(url))
                        r.AddRange(GetTripInfos(cacheFeed[url].Feed, station));
                }
            }
            r.Sort();
            result = r;
            return feedE;
        }
    }

    public class MTAInfo
    {
        private struct JSONFeed
        {
            public char[] Idef;
            public string Name;
            public char[] ID;
        }
        private struct JSONColor
        {
            public char[] Idef;
            public string Name;
            public string Color;
        }
        private struct JSONFeedInv
        {
            public char[] Idef;
            public string Name;
            public int ID;
        }
        public class TransferComplexInfo
        {
#if DEBUG
            public Dictionary<string, List<string>> transferMap;
#else
            private Dictionary<string, List<string>> transferMap;
#endif
            // Constructor
            public TransferComplexInfo(List<TransferInfo> infos)
            {
                transferMap = new Dictionary<string, List<string>>();
                foreach (var Info in infos)
                {
                    if (!string.Equals(Info.FromStop, Info.ToStop))
                    {
                        if (transferMap.ContainsKey(Info.FromStop))
                            transferMap[Info.FromStop].Add(Info.ToStop);
                        else
                            transferMap.Add(Info.FromStop, new List<string>() { Info.ToStop });
                        if (transferMap.ContainsKey(Info.ToStop))
                            transferMap[Info.ToStop].Add(Info.FromStop);
                        else
                            transferMap.Add(Info.ToStop, new List<string>() { Info.FromStop });
                    }
                }
            }

            public List<string> GetTransferStations(string station)
            {
                string s = station;
                if (char.IsLetter(station[station.Length - 1]))
                    s = station.Substring(0, station.Length - 1);
                if (transferMap.ContainsKey(s))
                    return transferMap[s];
                else
                    return new List<string>();
            }
        }

        private readonly string m_key;
        private readonly List<string> m_URL;
        private readonly List<StationInfo> m_station;
        private readonly Dictionary<string, StationInfo> m_station_map;
        private readonly Dictionary<char, UIColor> m_color_map;
        private readonly Dictionary<char, FeedIDInfo> m_feedid;
        private readonly Dictionary<string, string> m_feedid_inv;
        private readonly Dictionary<string, Dictionary<string, List<string>>>
                                                    m_servline;
        private readonly Dictionary<string, string> m_code;
        // Constructor 
        public MTAInfo(string filename) : this(ToDict(filename)) { }
        public MTAInfo(Dictionary<string, List<string>> dict)
        {
            if (dict.ContainsKey("FeedID"))
            {
                m_feedid = new Dictionary<char, FeedIDInfo>();
                string path = NSBundle.MainBundle.PathForResource(dict["FeedID"][0], "");
                string text = System.IO.File.ReadAllText(path);
                JSONFeed[] jsonfeed = JsonConvert.DeserializeObject<JSONFeed[]>(text);
                foreach (JSONFeed j in jsonfeed)
                    foreach (char c in j.Idef)
                        m_feedid.Add(c, new FeedIDInfo(j.ID, j.Name));
            }

            if (dict.ContainsKey("Key")) m_key = dict["Key"][0];

            if (dict.ContainsKey("URL")) m_URL = dict["URL"];

            if (dict.ContainsKey("Station"))
            {
                string path = NSBundle.MainBundle.PathForResource(dict["Station"][0], "");
                StreamReader reader = new StreamReader(File.OpenRead(path));
                CsvReader csvReader = new CsvReader(reader);
                m_station = csvReader.GetRecords<StationInfo>().ToList();
                reader.Close();
                m_station_map = m_station.ToDictionary(x => x.ID, x => x);
            }

            if (dict.ContainsKey("Transfer"))
            {
                string path = NSBundle.MainBundle.PathForResource(dict["Transfer"][0], "");
                StreamReader reader = new StreamReader(File.OpenRead(path));
                CsvReader csvReader = new CsvReader(reader);
                var x = csvReader.GetRecords<TransferInfo>().ToList();
                TransferComplex = new  TransferComplexInfo(x);
                reader.Close();
            }

            if (dict.ContainsKey("Color"))
            {
                m_color_map = new Dictionary<char, UIColor>();
                string path = NSBundle.MainBundle.PathForResource(dict["Color"][0], "");
                string text = System.IO.File.ReadAllText(path);
                JSONColor[] jsonfeed = JsonConvert.DeserializeObject<JSONColor[]>(text);
                foreach (JSONColor j in jsonfeed)
                    foreach (char c in j.Idef)
                    {
                        var color = ColorConverters.FromHex("#" + j.Color);
                        UIColor uIColor = UIColor.FromRGBA(color.R, color.G, color.B, color.A);
                        m_color_map.Add(c, uIColor);
                    }
            }

            if (dict.ContainsKey("FeedIDInd"))
            {
                m_feedid_inv = new Dictionary<string, string>();
                string path = NSBundle.MainBundle.PathForResource(dict["FeedIDInd"][0], "");
                string text = System.IO.File.ReadAllText(path);
                JSONFeedInv[] jsonfeed = JsonConvert.DeserializeObject<JSONFeedInv[]>(text);
                foreach (JSONFeedInv j in jsonfeed)
                    m_feedid_inv[string.Format(m_URL[0] + m_URL[1], m_key, j.ID.ToString())] = j.Name;
            }

            if (dict.ContainsKey("ServLine"))
            {
                string path = NSBundle.MainBundle.PathForResource(dict["ServLine"][0], "");
                string text = System.IO.File.ReadAllText(path);
                m_servline = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<string>>>>(text);
            }

            if (dict.ContainsKey("SpecialCode"))
            {
                string path = NSBundle.MainBundle.PathForResource(dict["SpecialCode"][0], "");
                string text = System.IO.File.ReadAllText(path);
                m_code = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
                /*
                var k = m_code.Keys.ToList();
                foreach (var i in k)
                    m_code[m_code[i]] = i;
                    */
            }
        }

        public string GetLineCode(string tripid)
        {
            int index1 = tripid.IndexOf("_"),
                index2 = tripid.IndexOf(".");
            if (index1 == -1 || index2 == -1 || index1 + 1 >= index2) return "";
            string code = tripid.Substring(index1 + 1, index2 - (index1 + 1));
            if (m_code.ContainsKey(code)) return m_code[code];
            return code;
        }

        private static Dictionary<string, List<string>> ToDict(string Filename)
        {
            string path = NSBundle.MainBundle.PathForResource(Filename, "");
            string text = System.IO.File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(text);
        }

        public static List<string> AddBothDirc(List<string> list)
        {
            HashSet<string> r = new HashSet<string>();
            foreach (var item in list)
            {
                if (char.IsLetter(item[item.Length - 1]))
                {
                    r.Add(item.Substring(0, item.Length - 1) + "S");
                    r.Add(item.Substring(0, item.Length - 1) + "N");
                }
                if (char.IsDigit(item[item.Length - 1]))
                {
                    r.Add(item + "S");
                    r.Add(item + "N");
                }
            }
            return r.ToList();
        }
        public List<char> Lines
        {
            get
            {
                List<char> r = m_feedid.Keys.ToList();
                r.Remove((char)255);
                return r;
            }
        }
        public TransferComplexInfo TransferComplex { get; set; }
        public List<StationInfo> GetStations() { return m_station; }
        public UIColor GetLineColor(char c)
        {
            if (m_color_map.ContainsKey(c))
                return m_color_map[c];
            return UIColor.SystemBlueColor;
        }
        public FeedIDInfo GetFeedIDInfo(string str)
        {
            if (m_feedid == null) return null;
            if (!m_feedid.ContainsKey(str[0])) return null;
            return m_feedid[str[0]];
        }
        public List<string> GetFeedURL(string str)
        {
            if (m_key == null || m_URL ==  null)
                return null;
            if (GetFeedIDInfo(str) != null)
            {
                List<string> r = new List<string>();
                foreach (string url in GetFeedIDInfo(str).FeedID)
                    r.Add(string.Format(m_URL[0] + m_URL[1], m_key, url));
                return r;
            }
            return new List<string>() { string.Format(m_URL[0], m_key) };

        }
        public StationInfo GetStationInfo(string ID)
        {
            if (m_station_map.ContainsKey(ID))
                return m_station_map[ID];
            else
                return new StationInfo("", "", "", "");
        }
        public string GetLineNameFromURL(string url)
        {
            if (m_feedid_inv.ContainsKey(url))
                return m_feedid_inv[url];
            return "UNKNOWN";
        }
        public string GetServLine(string stopID)
        {
            return GetStationInfo(stopID).ServLine;
        }
        public Tuple<string, UIColor> GetServLineColor(string line)
        {
            string lineG = GetStationInfo(line).ServLine;
            if (m_servline.ContainsKey(lineG))
                if (m_servline[lineG]["Ext"][0].Length!=0)
                    return Tuple.Create(m_servline[lineG]["Ext"][0], UIColor.White);
                else
                    return Tuple.Create("", GetLineColor(m_servline[lineG]["Line"][0][0]));
            return Tuple.Create("", UIColor.White);
        }
    }

    public class RouteInfo
    {
        private RTInfos rtinfo;
        private MTAInfo src;
        private Dictionary<string,NTree<string>> map;

        public List<List<string>> GetRoutes(string id)
        {
            if (map.ContainsKey(id))
                if (map[id] != null)
                    return map[id].GetAllPathData();
                else
                    return new List<List<string>>();
            else
                return null;
        }

        public List<List<Tuple<int, string>>> GetRoutesCountUnique(string id, out Tuple<HashSet<string>,HashSet<string>> ht)
        {
            ht = null;
            if (map.ContainsKey(id))
                if (map[id] != null)
                    return map[id].GetAllPathDataCountUnique(out ht);
                else
                    return new List<List<Tuple<int, string>>>();
            else
                return null;
        }

        public List<List<Tuple<int, string>>> GetRoutesCount(string id)
        {
            if (map.ContainsKey(id))
                if (map[id] != null)
                    return map[id].GetAllPathDataCount();
                else
                    return new List<List<Tuple<int, string>>>();
            else
                return null;
        }

        public RouteInfo(MTAInfo src, RTInfos rtinfo)
        {
            this.src = src;
            map = new Dictionary<string, NTree<string>>();
            this.rtinfo = rtinfo;
        }

        public void Refresh()
        {
            map.Clear();
            List<string> a = new List<string>();
            a.Add((char)255 + "");
            List<TripInfo> tripInfos = new List<TripInfo>();
            Exception e = rtinfo.QueryByStation(a, out tripInfos, 1);
            foreach (char _LINE in src.Lines)
            {
                List<char> _DIRS = new List<char>();
                _DIRS.Add('N'); _DIRS.Add('S');
                foreach (char _DIR in _DIRS)
                {
                    string str_id = _LINE.ToString() + _DIR.ToString();
                    map[str_id] = null;
                    if (tripInfos != null)
                        foreach (TripInfo i in tripInfos)
                            if (string.Equals(src.GetLineCode(i.Id),_LINE.ToString()) &&
                                i.Id.LastIndexOf(".") != -1 && // Find the direction
                                i.Id[i.Id.LastIndexOf(".") + 1] == _DIR &&
                                i.StopTime.Length > 0)
                            {
                                bool exceptionFalg = false;
                                NTree<string> t = null;
                                for (int j = 0; j < i.StopTime.Length; j++)
                                {
                                    if (string.Equals(src.GetStationInfo(i.StopTime[j].StopId).Name, ""))
                                        continue;
                                    if (t == null)
                                        t = new NTree<string>(i.StopTime[j].StopId);
                                    else
                                    {
                                        try
                                        {
                                            t.AddNode(i.StopTime[j].StopId, true);
                                        }
                                        catch (ExceptionTreeNodeDuplication eTree)
                                        {
#if DEBUG
                                            Console.WriteLine(eTree.Message);
#endif
                                            exceptionFalg = true;
                                            break;
                                        }
                                    }
                                }
                                if (exceptionFalg) continue;
                                if (map[str_id] == null)
                                    map[str_id] = t;
                                else
                                    map[str_id].Combine(t);
                            }
                }
            }

            if (e != null)
                throw (FeedFetchException)e;
        }
        public void Refresh(RTInfos rtinfo)
        {
            this.rtinfo = rtinfo;
            Refresh();
        }
    }

    
}
