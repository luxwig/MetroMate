
using System;
using NUnit.Framework;
using MetroMate;
using System.Collections.Generic;

namespace SubwayConnectUnitTest
{
    [TestFixture]
    public class RouteInfoUnitTest
    {
        [Test]
        public void MTAInfoInitUnitTest()
        {
            MTAInfo src = new MTAInfo("ResSummary.json");
            RTInfos rtinfo = new RTInfos(src);
            RouteInfo route = new RouteInfo(src, rtinfo);
            route.Refresh();

            var lines = src.Lines;
            foreach (char _LINE in lines)
            {
                List<char> _DIRS = new List<char>();
                _DIRS.Add('N'); _DIRS.Add('S');
                foreach (char _DIR in _DIRS)
                {
                    string str_id = _LINE.ToString() + _DIR.ToString();
                    List<List<string>> p = route.GetRoutes(str_id);

                    foreach (List<string> r in p)
                    {
                        Console.WriteLine("***********" + str_id);
                        foreach (string str in r)
                        {
                            Console.WriteLine("{0}\t{1}", str, src.GetStationInfo(str).Name);
                        }
                    }
                }
            }
        }   
    }
}

