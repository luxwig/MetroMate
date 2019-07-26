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

namespace SubwayConnect
{
    public class TripInfo : IComparable, IComparable<TripInfo>
    {
        public TripInfo()
        {

        }
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
        public readonly string FeedID;
        public readonly string Abb;
        public FeedIDInfo(string feedid, String abb)
        {
            FeedID = feedid;
            Abb = abb;
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
            public string ID;
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
        private readonly Hashtable m_station_name;


        public struct StationInfo
        {
            [Name("stop_id")]
            public string ID { get; set; }

            [Name("stop_name")]
            public  string Name { get; set; }

            [Name("parent_station")]
            public  string Parents { get; set; }
            public StationInfo(string id, string name, string parents)
            {
                ID = id;
                Name = name;
                Parents = parents;
            }
        }
        public FeedIDInfo GetFeedIDInfo(string str)
        {
            if (m_feedid == null) return null;
            if (!m_feedid.ContainsKey(str[0])) return null;
            return m_feedid[str[0]];
        }
        public string GetFeedURL(string str)
        {
            if (m_key == null || m_URL ==  null)
                return null;
            if (GetFeedIDInfo(str) != null)
                return string.Format(m_URL[0]+m_URL[1], m_key, GetFeedIDInfo(str).FeedID);
            return string.Format(m_URL[0], m_key);

        }
        public StationInfo GetStationInfo(string ID)
        {
            return m_station_map[ID];
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
