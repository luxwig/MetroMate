using System;
using Xunit;
using Xunit.Abstractions;
using MetroMate;
using System.Collections.Generic;
using System.Linq;

namespace NTreeUnitTest
{
    public class NTreeTest
    {

        private readonly ITestOutputHelper output;

        public NTreeTest(ITestOutputHelper testOutputHelper)
        {
            output = testOutputHelper;
        }

        [Fact]
        public void CreateTree()
        {
            NTree<int> tree = new NTree<int>(0);
            Assert.True(tree.CurrentData == 0);
        }

        [Fact]
        public void AddNode()
        {
            NTree<int> tree = new NTree<int>(0);
            var node1 = tree.AddNode(1);
            var node2 = tree.AddNode(2);
            Assert.True(node1.data == 1);
            Assert.True(node1.parent.ToArray()[0].data == 0);
            Assert.True(node2.data == 2);
            Assert.True(node2.parent.ToArray()[0].data == 0);
            Assert.True(tree.GetChild.Count == 2);
        }

        [Fact]
        public void GetAllPath()
        {
            NTree<int> tree = new NTree<int>(0);
            tree.AddNode(1);
            tree.AddNode(2);
            var p = tree.GetAllPath();
        }

        [Fact]
        public void GetAllPathData()
        {
            NTree<int> tree = new NTree<int>(0);
            tree.AddNode(1);
            tree.AddNode(2);
            tree.NextNode(1);
            tree.AddNode(3);
            tree.AddNode(4);
            var p = tree.GetAllPathData();
            foreach (List<int> l in p)
            {
                foreach(var i in l)
                    output.WriteLine("{0}",i);
                output.WriteLine("");
               // Console.WriteLine();
            }
        }

        [Fact]
        public void PrevNode()
        {
            NTree<int> tree = new NTree<int>(0);
            tree.AddNode(1);
            tree.AddNode(2);
            tree.NextNode(1);
            tree.AddNode(3);
            tree.AddNode(4, true);
            Assert.True(tree.PrevNode(1).data == 1);
            Assert.True(tree.PrevNode(0).data == 0);
            Assert.True(tree.PrevNode(0) == null);
        }



        [Fact]
        public void GetAllPathDataWithAddHead()
        {
            NTree<int> tree = new NTree<int>();
            tree.AddHead(0, true);
            tree.AddNode(1);
            tree.AddNode(2);
            tree.NextNode(1);
            tree.AddNode(3);
            tree.AddNode(4);
            var p = tree.GetAllPathData();
            foreach (List<int> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }
        }

        [Fact]
        public void GetAllPathDataWithMultipleHead()
        {
            NTree<string> tree = new NTree<string>();
            tree.AddHead("a0");
            tree.AddHead("a1");
            tree.FindNode("a0");
            tree.AddNode("b0");
            var b1 = tree.AddNode("b1");
            tree.FindNode("a1");
            tree.AddNode("b2");
            tree.AddNode(b1);
            tree.AddNode(b1);
            tree.AddNode(b1);
            tree.AddNode(b1);
            tree.FindNode("b0");
            tree.AddNode("c0");
            tree.FindNode("b1");
            tree.AddNode("c1");
            tree.AddNode("c2");
            tree.FindNode("b2");
            tree.AddNode("c3");
            var p = tree.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }
        }


        [Fact]
        public void CombineFalse()
        {
            NTree<string> tree = new NTree<string>();
            tree.AddHead("A", true);
            tree.AddNode("B1");
            tree.AddNode("B2");
            tree.FindNode("B1");
            tree.AddNode("C1", true);
            var D1 = tree.AddNode("D1");
            tree.FindNode("B2");
            tree.AddNode("C2", true);
            tree.AddNode(D1, true);
            tree.AddNode("E1", true);
            tree.AddNode("F1", true);
            tree.AddNode("G1");
            tree.FindNode("E1");
            tree.AddNode("F2");

            NTree<string> chain = new NTree<string>();
            chain.AddHead("X1", true);
            chain.AddNode("X2");
            chain.AddNode("X3");

            Assert.False(tree.Combine(chain));
            var p = tree.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }
        }

        [Fact]
        public void CombineTrueShareHead()
        {
            NTree<string> tree = new NTree<string>();
            tree.AddHead("A", true);
            tree.AddNode("B1");
            tree.AddNode("B2");
            tree.FindNode("B1");
            tree.AddNode("C1", true);
            var D1 = tree.AddNode("D1");
            tree.FindNode("B2");
            tree.AddNode("C2", true);
            tree.AddNode(D1, true);
            tree.AddNode("E1", true);
            tree.AddNode("F1", true);
            tree.AddNode("G1");
            tree.FindNode("E1");
            tree.AddNode("F2");

            output.WriteLine("********Init********");

            var p = tree.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }

            output.WriteLine("********Chain********");
            NTree<string> chain = new NTree<string>();
            chain.AddHead("A", true);
            chain.AddNode("B1", true);
            chain.AddNode("C1", true);
            chain.AddNode("X1", true);
            chain.AddNode("X2", true);
            chain.AddNode("X3", true);
            chain.AddNode("F1", true);
            chain.AddNode("X4", true);

            p = chain.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }
            Assert.True(tree.Combine(chain));

            output.WriteLine("********Result********");

            p = tree.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }
        }

        [Fact]
        public void CombineTrueNotSharingHead()
        {
            NTree<string> tree = new NTree<string>();
            tree.AddHead("A", true);
            tree.AddNode("B1");
            tree.AddNode("B2");
            tree.FindNode("B1");
            tree.AddNode("C1", true);
            var D1 = tree.AddNode("D1");
            tree.FindNode("B2");
            tree.AddNode("C2", true);
            tree.AddNode(D1, true);
            tree.AddNode("E1", true);
            tree.AddNode("F1", true);
            tree.AddNode("G1");
            tree.FindNode("E1");
            tree.AddNode("F2");

            output.WriteLine("********Init********");

            var p = tree.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }

            output.WriteLine("********Chain********");
            NTree<string> chain = new NTree<string>();
            //chain.AddHead("A", true);
            //chain.AddNode("B1", true);
            //chain.AddNode("C1", true);
            chain.AddHead("X1", true);
            chain.AddNode("X2", true);
            chain.AddNode("X3", true);
            chain.AddNode("F1", true);
            chain.AddNode("X4", true);

            p = chain.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }
            Assert.True(tree.Combine(chain));

            output.WriteLine("********Result********");

            p = tree.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }
        }

        [Fact]
        public void CombineTrueNoCommon()
        {
            NTree<string> tree = new NTree<string>();
            tree.AddHead("A", true);
            tree.AddNode("B1");
            tree.AddNode("B2");
            tree.FindNode("B1");
            tree.AddNode("C1", true);
            var D1 = tree.AddNode("D1");
            tree.FindNode("B2");
            tree.AddNode("C2", true);
            tree.AddNode(D1, true);
            tree.AddNode("E1", true);
            tree.AddNode("F1", true);
            tree.AddNode("G1");
            tree.FindNode("E1");
            tree.AddNode("F2");

            output.WriteLine("********Init********");

            var p = tree.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }

            output.WriteLine("********Chain********");
            NTree<string> chain = new NTree<string>();
            //chain.AddHead("A", true);
            //chain.AddNode("B1", true);
            //chain.AddNode("C1", true);
            chain.AddHead("X1", true);
            chain.AddNode("X2", true);
            chain.AddNode("X3", true);
            chain.AddNode("X4", true);

            p = chain.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }
            Assert.True(tree.Combine(chain));

            output.WriteLine("********Result********");

            p = tree.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }
        }

        [Fact]
        public void CombineTrueNoCommonAndAddOne()
        {
            NTree<string> tree = new NTree<string>();
            tree.AddHead("A", true);
            tree.AddNode("B1");
            tree.AddNode("B2");
            tree.FindNode("B1");
            tree.AddNode("C1", true);
            var D1 = tree.AddNode("D1");
            tree.FindNode("B2");
            tree.AddNode("C2", true);
            tree.AddNode(D1, true);
            tree.AddNode("E1", true);
            tree.AddNode("F1", true);
            tree.AddNode("G1");
            tree.FindNode("E1");
            tree.AddNode("F2");

            output.WriteLine("********Init********");

            var p = tree.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }

            output.WriteLine("********Chain1********");
            NTree<string> chain = new NTree<string>();
            //chain.AddHead("A", true);
            //chain.AddNode("B1", true);
            //chain.AddNode("C1", true);
            chain.AddHead("X1", true);
            chain.AddNode("X2", true);
            chain.AddNode("X3", true);
            chain.AddNode("X4", true);

            p = chain.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }
            Assert.True(tree.Combine(chain));

            output.WriteLine("********Chain2********");
            chain = new NTree<string>();
            //chain.AddHead("A", true);
            //chain.AddNode("B1", true);
            //chain.AddNode("C1", true);
            chain.AddHead("X3", true);
            chain.AddNode("F1", true);
            chain.AddNode("X4", true);

            p = chain.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }
            Assert.True(tree.Combine(chain));


            output.WriteLine("********Result********");

            p = tree.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }
        }


        [Fact]
        public void CombineTrueRemoveHead()
        {
            NTree<string> tree = new NTree<string>();
            tree.AddHead("A", true);
            tree.AddNode("B1");
            tree.AddNode("B2");
            tree.FindNode("B1");
            tree.AddNode("C1", true);
            var D1 = tree.AddNode("D1");
            tree.FindNode("B2");
            tree.AddNode("C2", true);
            tree.AddNode(D1, true);
            tree.AddNode("E1", true);
            tree.AddNode("F1", true);
            tree.AddNode("G1");
            tree.FindNode("E1");
            tree.AddNode("F2");

            output.WriteLine("********Init********");

            var p = tree.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }

            output.WriteLine("********Chain1********");
            NTree<string> chain = new NTree<string>();
            //chain.AddHead("A", true);
            //chain.AddNode("B1", true);
            //chain.AddNode("C1", true);
            chain.AddHead("X1", true);
            chain.AddNode("X2", true);
            chain.AddNode("A", true);

            p = chain.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }
            Assert.True(tree.Combine(chain));

            output.WriteLine("********Result********");

            p = tree.GetAllPathData();
            foreach (List<string> l in p)
            {
                foreach (var i in l)
                    output.WriteLine("{0}", i);
                output.WriteLine("");
                // Console.WriteLine();
            }

        }
    }
}
