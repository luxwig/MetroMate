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
            return string.Compare(Name, (obj as StationInfo).Name);
        }

        public int CompareTo(StationInfo other)
        {
            return string.Compare(Name, other.Name);
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
                dataStream.Close();
                response.Close();
            }
            return feed;
        }
        private Dictionary<string, FeedMessageCashe> CacheFeed;



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
                        
                        if (string.Equals(stopTimeUpdate.StopId, SEEKING_STOP))
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


        // Refreshflag: 0 Auto, 1 Force Refresh, 2 Force not Refresh

        public List<TripInfo> QueryByStation(List<string> Stations, int RefreshFlag = 0)
        {
            List<TripInfo> r = new List<TripInfo>();
            foreach (string station in Stations)
            {
                foreach (string url in src.GetFeedURL(station)) {
                    if (CacheFeed.ContainsKey(url))
                        Console.WriteLine("Cashe Time {0}", (DateTime.Now - CacheFeed[url].Timestamp).TotalSeconds);
                    if (!CacheFeed.ContainsKey(url) || (DateTime.Now - CacheFeed[url].Timestamp).TotalSeconds > 30)
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
/*
    public class MTInfo
    {
        public MTInfo()
        {
        }
    }
    */
}
