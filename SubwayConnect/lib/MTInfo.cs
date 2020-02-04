#pragma warning disable CS0649

using CsvHelper;
using System;
using TransitRealtime;
using ProtoBuf;
using StopTimeUpdate = TransitRealtime.TripUpdate.StopTimeUpdate;
using Foundation;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using CsvHelper.Configuration.Attributes;
using System.Linq;
using System.Net;
using UIKit;

namespace MetroMate
{
    
    public class StationInfo:IComparable, IComparable<StationInfo>
    {
        [Name("stop_id")]
        public string ID { get; set; }

        [Name("stop_name")]
        public string Name { get; set; }

        [Name("parent_station")]
        public string Parents { get; set; }
        public StationInfo()
        {
            ID = ""; Name = ""; Parents = "";
        }
        public StationInfo(string id, string name, string parents)
        {
            ID = id;
            Name = name;
            Parents = parents;
        }

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


    public class TransferInfo
    {
        [Name("from_stop_id")]
        public string From_stop { get; set; }

        [Name("to_stop_id")]
        public string To_stop { get; set; }
        public TransferInfo()
        {
            From_stop = To_stop = "";
        }
        public TransferInfo(string From_stop, string To_stop)
        {
            this.From_stop = From_stop;
            this.To_stop = To_stop;
        }
    }


    public class TripInfo : IComparable, IComparable<TripInfo>
    {
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
        public long GetTargetLongTime { get { return est; } }
        public DateTime GetTargetTime { get {
                DateTimeOffset dtf = DateTimeOffset.FromUnixTimeSeconds(est).ToLocalTime();
                return dtf.DateTime;
            } }
        public int Count { get { return StopTime.Length; } }
        public int Index { get; set; }
        public StopTimeUpdate[] StopTime { get; set; }
        public string RefStop { get; set; }
        public string Id { get; set; }
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

    public class FeedIDInfo
    {
        public readonly List<string> FeedID;
        public readonly string Abb;
        public FeedIDInfo(char[] feedid, String abb)
        {
            FeedID = new List<string>();
            foreach (int i in feedid)
                FeedID.Add(i.ToString());
            Abb = abb;
        }
    }


    public class RTInfos
    {
        private struct FeedMessageCashe
        {
            public FeedMessage Feed;
            public DateTime Timestamp;
            public FeedMessageCashe(FeedMessage Feed, DateTime Timestamp) { this.Feed = Feed; this.Timestamp = Timestamp; }
        }
        private MTAInfo src;
        private HashSet<string> LastQuery;
        public RTInfos(MTAInfo src)
        {
            this.src = src;
            CacheFeed = new Dictionary<string, FeedMessageCashe>();
        }
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
            finally
            {
                if (dataStream != null)
                    dataStream.Close();
                if (response != null)
                    response.Close();
            }
            return feed;
        }
        private Dictionary<string, FeedMessageCashe> CacheFeed;


        public static DateTime StopLongTimeToDateTime(StopTimeUpdate stop)
        {
            long est;
            if (stop.Arrival != null)
                est = stop.Arrival.Time;
            else
                est = stop.Departure.Time;
            return DateTimeOffset.FromUnixTimeSeconds(est).ToLocalTime().DateTime;

        }

        // SEEKING_STOP = 255 means all TripInfo

        private static List<TripInfo> GetTripInfos(
            FeedMessage feed, string SEEKING_STOP)
        {
            List<TripInfo> tripInfos = new List<TripInfo>();
            foreach (FeedEntity ent in feed.Entities)
            {
                if (ent.TripUpdate != null &&
                    ent.TripUpdate.StopTimeUpdates.Count != 0)
                {
                    int i = 0;
                    foreach (StopTimeUpdate stopTimeUpdate in ent.TripUpdate.StopTimeUpdates)
                    {
                        
                        if (string.Equals(stopTimeUpdate.StopId, SEEKING_STOP) || string.Equals((char)255+"", SEEKING_STOP))
                        {
                            tripInfos.Add(new TripInfo(i, ent.TripUpdate.StopTimeUpdates.ToArray(), SEEKING_STOP, ent.TripUpdate.Trip.TripId));
                            break;
                        }
                        i += 1;
                    }

                }
            }
            return tripInfos;
        }



        public void Refresh()
        {
            foreach (string url in LastQuery){
                Console.WriteLine("Refreshing {0}", url);
                CacheFeed[url] = new FeedMessageCashe(GetFeed(url), DateTime.Now);
            }
        }

        // Refreshflag: 0 Auto, 1 Force Refresh, 2 Force not Refresh
        // If Station str is (char)255, this means all data
        public List<TripInfo> QueryByStation(List<string> Stations, int RefreshFlag = 0)
        {
            List<TripInfo> r = new List<TripInfo>();
            LastQuery = new HashSet<string>();
            foreach (string station in Stations)
            {
                foreach (string url in src.GetFeedURL(station)) {
                    LastQuery.Add(url);
                    if (CacheFeed.ContainsKey(url))
                        Console.WriteLine("Cashe Time {0}", (DateTime.Now - CacheFeed[url].Timestamp).TotalSeconds);
                    if (!CacheFeed.ContainsKey(url) ||
                        ((DateTime.Now - CacheFeed[url].Timestamp).TotalSeconds > 30 && RefreshFlag != 2) ||
                        RefreshFlag == 1)
                    {
                        Console.WriteLine("Refreshing {0}", url);
                        FeedMessage feed = GetFeed(url);
                        CacheFeed[url] = new FeedMessageCashe(feed, DateTime.Now);
                    }
                    r.AddRange(GetTripInfos(CacheFeed[url].Feed, station));
                }
            }
            r.Sort();
            return r;
        }
    }

    public class MTAInfo
    {

        public MTAInfo(string Filename) : this(MTAInfo.ToDict(Filename)) { }
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
        }

        

        public class TransferComplexInfo
        {
#if DEBUG
            public Dictionary<string, List<string>> transfer_map;
#else
            private Dictionary<string, List<string>> transfer_map;
#endif
            public TransferComplexInfo(List<TransferInfo> infos)
            {
                transfer_map = new Dictionary<string, List<string>>();
                foreach (var Info in infos)
                {
                    if (!string.Equals(Info.From_stop, Info.To_stop))
                    {
                        if (transfer_map.ContainsKey(Info.From_stop))
                            transfer_map[Info.From_stop].Add(Info.To_stop);
                        else
                            transfer_map.Add(Info.From_stop, new List<string>() { Info.To_stop });
                        if (transfer_map.ContainsKey(Info.To_stop))
                            transfer_map[Info.To_stop].Add(Info.From_stop);
                        else
                            transfer_map.Add(Info.To_stop, new List<string>() { Info.From_stop });
                    }
                }
            }

            public List<string> GetTransferStations(string station)
            {
                string s = station;
                if (char.IsLetter(station[station.Length - 1]))
                    s = station.Substring(0, station.Length - 1);
                if (transfer_map.ContainsKey(s))
                    return transfer_map[s];
                else
                    return new List<string>() ;
            }
        }

        private struct JSONFeed
        {
            public char[] Idef;
            public string Name;
            public char[] ID;
        }
        private Dictionary<char, FeedIDInfo> m_feedid;
        private static Dictionary<string, List<string>> ToDict(string Filename)
        {
            string path = NSBundle.MainBundle.PathForResource(Filename, "");
            string text = System.IO.File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(text);
        }
        private readonly string  m_key;
        private readonly List<string> m_URL;
        private readonly List<StationInfo> m_station;
        private readonly Dictionary<string, StationInfo> m_station_map;

        public List<char> Lines {get {
                List<char> r = m_feedid.Keys.ToList();
                r.Remove((char)255);
                return r;
            } }
        public TransferComplexInfo TransferComplex;
        public List<StationInfo> GetStations() { return m_station; }
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
                return new StationInfo("", "", "");
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

        public List<List<Tuple<int, string>>> GetRoutesCountUnique(string id)
        {
            if (map.ContainsKey(id))
                if (map[id] != null)
                    return map[id].GetAllPathDataCountUnique();
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
            this.rtinfo = rtinfo;
            map = new Dictionary<string, NTree<string>>();
            Refresh();
        }

        public void Refresh()
        {
            List<string> a = new List<string>();
            a.Add((char)255 + "");
            List<TripInfo> tripInfos = rtinfo.QueryByStation(a, 1);

            foreach (char _LINE in src.Lines)
            {
                List<char> _DIRS = new List<char>();
                _DIRS.Add('N'); _DIRS.Add('S');
                foreach (char _DIR in _DIRS)
                {
                    string str_id = _LINE.ToString() + _DIR.ToString();
                    map[str_id] = null;
                    foreach (TripInfo i in tripInfos)
                        if (i.Id.IndexOf('_') != -1 &&   // Find the line number
                            i.Id[i.Id.IndexOf('_') + 1] == _LINE &&
                            i.Id.IndexOf("..") != -1 && // Find the direction
                            i.Id[i.Id.IndexOf("..") + 2] == _DIR &&
                            i.StopTime.Length > 0)
                            {
                            NTree<string> t = null;
                            for (int j = 0; j < i.StopTime.Length; j++) {
                                if (string.Equals(src.GetStationInfo(i.StopTime[j].StopId).Name, ""))
                                    continue;
                                if(t == null)
                                    t = new NTree<string>(i.StopTime[j].StopId);
                                else
                                    t.AddNode(i.StopTime[j].StopId, true);
                            }
                            if (map[str_id] == null)
                                map[str_id] = t;
                            else
                                map[str_id].Combine(t);
                        }
                }
            }
        }
    }
/*
    public class MTInfo
    {
        public MTInfo()
        {
        }
    }
    */
}
