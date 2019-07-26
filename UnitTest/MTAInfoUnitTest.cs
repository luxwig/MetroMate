
using System;
using NUnit.Framework;
using SubwayConnect;
using System.Collections.Generic;

namespace SubwayConnectUnitTest
{
    [TestFixture]
    public class MTAInfoUnitTest
    {
        [Test]
        public void MTAInfoInitUnitTest()
        {
            try
            {
                Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                List<string> T = new List<string>() { "FeedID.json", "json" };
                dict.Add("FeedID", T);
                MTAInfo mTAInfo = new MTAInfo(dict);
            }
            catch (Exception)
            {
                Assert.False(true);
            }
            Assert.True(true);
        }

        [Test]
        public void GetFeedIDUnitTest()
        {
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            List<string> T = new List<string>() { "FeedID.json", "json"};
            dict.Add("FeedID", T);
            MTAInfo mTAInfo = new MTAInfo(dict);

            Assert.True(string.Equals(mTAInfo.GetFeedIDInfo("A123").Abb,"ACE"));
            Assert.True(string.Equals(mTAInfo.GetFeedIDInfo("B123").Abb, "BDFM"));
            Assert.True(string.Equals(mTAInfo.GetFeedIDInfo("Q123").Abb, "NRQW"));
        }

        [Test]
        public void MTAInfoInitUnitTestViaResUnitTest()
            {
                MTAInfo mTAInfo = new MTAInfo("ResSummary.json");
                Assert.True(string.Equals(mTAInfo.GetFeedIDInfo("A123").Abb, "ACE"));
                Assert.True(string.Equals(mTAInfo.GetFeedIDInfo("B123").Abb, "BDFM"));
                Assert.True(string.Equals(mTAInfo.GetFeedIDInfo("Q123").Abb, "NRQW"));
            }
        [Test]
        public void GetFeedURLUnitTest()
        {
            MTAInfo mTAInfo = new MTAInfo("ResSummary.json");
            Assert.True(string.Equals(mTAInfo.GetFeedURL("A123"),
                "http://datamine.mta.info/mta_esi.php?key=b12af9a5fd7d6665a230010a00d63ca4&feed_id=26"));
        }

        [Test]
        public void GetStationUnitTest()
        {
            try
            {
                MTAInfo mTAInfo = new MTAInfo("ResSummary.json");
                Assert.True(string.Equals(mTAInfo.GetStationInfo("101").Name, "Van Cortlandt Park - 242 St"));
            }
            catch (Exception e)
            {
                Assert.True(false);
            }

            
        }
    }
}
