using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MetroMate
{
    public class NTreeNode<T>
    {
        public T data;
        public HashSet<NTreeNode<T>> child { get; set; }
        public HashSet<NTreeNode<T>> parent { get; set; }
        public int Count;
        public NTreeNode(T data)
        {
            Count = 1;
            this.data = data;
            this.parent = new HashSet<NTreeNode<T>>();
            this.child = new HashSet<NTreeNode<T>>();
        }
        public NTreeNode(T data, NTreeNode<T> parent, HashSet<NTreeNode<T>> child = null)
        {
            Count = 1;
            this.data = data;
            this.parent = new HashSet<NTreeNode<T>>();
            this.parent.Add(parent);
            this.child = child;
            if (this.child == null)
                this.child = new HashSet<NTreeNode<T>>();
        }
        public NTreeNode(T data, HashSet<NTreeNode<T>> parent, HashSet<NTreeNode<T>> child = null)
        {
            Count = 1;
            this.data = data;
            this.parent = parent;
            this.child = child;
            if (this.child == null)
                this.child = new HashSet<NTreeNode<T>>();
        }
    };

    public class NTree<T>
    {
        private List<NTreeNode<T>> Head;
        private NTreeNode<T> Current;
        private Dictionary<T, NTreeNode<T>> Nodes;

        public T CurrentData { get => Current.data; set => Current.data = value; }
        public NTreeNode<T> GetCurrentNode { get { return Current; } }
        public HashSet<NTreeNode<T>> GetChild { get => Current.child; }
        public NTreeNode<T> GetNode(T data) {
            if (Nodes.ContainsKey(data)) return Nodes[data];
            return null;
        }

        // Constructor
        // Default
        // With Only Data
        // With Existing Node
        public NTree()
        {
            Head = new List<NTreeNode<T>>();
            Current = null;
            Nodes = new Dictionary<T, NTreeNode<T>>();
        }
        public NTree(T head)
        {
            Head = new List<NTreeNode<T>>();
            Nodes = new Dictionary<T, NTreeNode<T>>();
            AddHead(head, true);        
        }
        public NTree(NTreeNode<T> node)
        {
            Head = new List<NTreeNode<T>>();
            Nodes = new Dictionary<T, NTreeNode<T>>();
            // Head.Add(new NTreeNode<T>(node.data, new HashSet<NTreeNode<T>>(), node.child));
            // Current = Head[0];
            AddHead(node, true);
        }
        public NTree(List<T> Nodes)
        {
            Head = new List<NTreeNode<T>>();
            this.Nodes = new Dictionary<T, NTreeNode<T>>();
            AddHead(Nodes[0], true);
            for (int i = 1; i < Nodes.Count; i++)
                AddNode(Nodes[i], true);
            Reset();
        }

        public void Reset() { Current = Head[0]; }

        // Add Head Node
        public NTreeNode<T> AddHead(T data, bool move = false)
        {
            if (Nodes.ContainsKey(data))
                throw new System.Exception("Element is already exist.");

            NTreeNode<T> newNode = new NTreeNode<T>(data, new HashSet<NTreeNode<T>>());
            Head.Add(newNode);
            Nodes[data] = newNode;
            if (move) Current = newNode;
            return newNode;
        }
        public NTreeNode<T> AddHead(NTreeNode<T> n, bool move = false)
        {
            if (Nodes.ContainsKey(n.data))
                throw new System.Exception("Element is already exist.");

            NTreeNode<T> newNode = new NTreeNode<T>(n.data, new HashSet<NTreeNode<T>>(), n.child);
            Head.Add(newNode);
            Nodes[n.data] = newNode;
            if (move) Current = newNode;
            return newNode;
        }


        // Add Leaf Node
        public NTreeNode<T> AddNode(T child, bool move = false)
        {
            if (Nodes.ContainsKey(child))
                throw new System.Exception("Element is already exist.");

            NTreeNode<T> newNode = new NTreeNode<T>(child, Current);
            
            Current.child.Add(newNode);
            if (move) Current = newNode;
            Nodes[child] = newNode;
            return newNode;
        }
        public NTreeNode<T> AddNode(NTreeNode<T> child, bool move = false)
        {
            NTreeNode<T> newNode;
            if (Nodes.ContainsKey(child.data))
                if (!ReferenceEquals(Nodes[child.data], child))
                    throw new System.Exception("Element is already exist.");
                else
                {
                    if (!Current.child.Contains(child))
                        Current.child.Add(child);
                    if (!child.parent.Contains(Current))
                        child.parent.Add(Current);
                    if (Head.Contains(child))
                        Head.Remove(child);
                    newNode = child;
                    newNode.Count++;
                }
            else
            {
                newNode  = new NTreeNode<T>(child.data, Current, child.child);
                newNode.Count = child.Count;
                Current.child.Add(newNode);
            }

            if (move) Current = newNode;
            Nodes[child.data] = newNode;
            return newNode;
        }

        // Move prev and next
        // will return null if not exsist
        public NTreeNode<T> NextNode(T node)
        {
            foreach (NTreeNode<T> childnode in Current.child)
                if (childnode.data.Equals(node))
                {
                    Current = childnode;
                    return Current;
                }
            
            return null;
        }
        public NTreeNode<T> PrevNode(T node)
        {
            foreach (NTreeNode<T> parentnode in Current.parent)
                if (parentnode.data.Equals(node))
                {
                    Current = parentnode;
                    return Current;
                }
            return null;
        }
        public NTreeNode<T> FindNode(T node)
        {
            Current = GetNode(node);
            return Current;
        }

        // Get all possible path
        // return type list of Node traces
        public List<List<NTreeNode<T>> > GetAllPath()
        {
            List<List<NTreeNode<T>>> r = new List<List<NTreeNode<T>>>();
            foreach (var h in Head)
                r.AddRange(GetAllPathHelper(h, new List<NTreeNode<T>>()));
            return r;
        }
        public List<List<T>> GetAllPathData()
        {

            List<List<NTreeNode<T>>> r = GetAllPath();
            List<List<T>> result = new List<List<T>>();
            foreach (var l in r)
            {
                result.Add(new List<T>());
                foreach (var t in l)
                    result[result.Count - 1].Add(t.data);
            }
            return result;
        }
        public List<List<Tuple<int,T>>> GetAllPathDataCount()
        {
            List<List<NTreeNode<T>>> r = GetAllPath();
            List<List<Tuple<int,T>>> result = new List<List<Tuple<int, T>>>();
            foreach (var l in r)
            {
                result.Add(new List<Tuple<int, T>>());
                foreach (var t in l)
                    result[result.Count - 1].Add(Tuple.Create(t.Count,t.data));
            }
            return result;
        }
        public List<List<Tuple<int, T>>> GetAllPathDataCountUnique()
        {

            List<List<Tuple<int, T>>> r = new List<List<Tuple<int, T>>>();
            List<List<NTreeNode<T>>> rN = new List<List<NTreeNode<T>>>();
            foreach (var h in Head)
            {
                // create a new final list 
                List<List<NTreeNode<T>>> finalLists = new List<List<NTreeNode<T>>>();
                var rawLists = GetAllPathHelper(h, new List<NTreeNode<T>>());
                foreach(var rawList in rawLists)
                {
                    //create raw set
                    HashSet<T> rawSet = new HashSet<T>();
                    foreach (NTreeNode<T> node in rawList)
                        rawSet.Add(node.data);

                    // for each final list candidate
                    bool flagHasCombined = false;
                    bool flagHasNotBeenAdded = true;
                    for (int i = 0; i<finalLists.Count;  i ++)
                    {
                        var finalList = finalLists[i];
                        //create final set
                        HashSet<T> finalSet = new HashSet<T>();
                        foreach (NTreeNode<T> node in finalList)
                            finalSet.Add(node.data);
                        if (!finalSet.IsSupersetOf(rawSet))
                        {
                            // raw set is bigger
                            if (rawSet.IsSupersetOf(finalSet))
                            {
                                finalLists[i] = rawList;
                                flagHasCombined = true;
                                flagHasNotBeenAdded = false;
                            }
                        }
                        // raw set is subset
                        else
                        {
                            flagHasNotBeenAdded = false;
                            break;
                        }
                    }

                    if (flagHasNotBeenAdded)
                        finalLists.Add(rawList);
                    while (flagHasCombined)
                    {
                        flagHasCombined = false;
                        for (int i = 0; i < finalLists.Count; i++)
                        {
                            HashSet<T> outerSet = new HashSet<T>();
                            foreach (NTreeNode<T> node in finalLists[i])
                                outerSet.Add(node.data);
                            for (int j = i+1; j < finalLists.Count; j++)
                            {
                                HashSet<T> innerSet = new HashSet<T>();
                                foreach (NTreeNode<T> node in finalLists[j])
                                    innerSet.Add(node.data);
                                if (outerSet.IsSupersetOf(innerSet))
                                {
                                    finalLists.RemoveAt(j);
                                    j--;
                                    continue;
                                }
                                if (outerSet.IsSubsetOf(innerSet))
                                {
                                    finalLists.RemoveAt(i);
                                    flagHasCombined = true;
                                }
                            }
                            if (flagHasCombined) break;
                        }
                    }
                    
                }
                rN.AddRange(finalLists);
            }
            foreach(var chain in rN)
            {
                r.Add(new List<Tuple<int, T>>());
                foreach(var node in chain)
                    r[r.Count - 1].Add(Tuple.Create(node.Count, node.data));
            }
            return r;
        }

        private List<List<NTreeNode<T>>> GetAllPathHelper(NTreeNode<T> CurrentNode,
                                                          List<NTreeNode<T>> trace)
        {
            List<List<NTreeNode<T>>> r = new List<List<NTreeNode<T>>>();
            if (CurrentNode.child.Count == 0)
            {
                List<NTreeNode<T>> newtrace;
                NTreeNode<T>[] tracearray = trace.ToArray();
                newtrace = tracearray.OfType<NTreeNode<T>>().ToList();
                newtrace.Add(CurrentNode);
                r.Add(newtrace);
                return r;
            }
            trace.Add(CurrentNode);
            foreach (NTreeNode<T> node in CurrentNode.child)
                r.AddRange(GetAllPathHelper(node, trace));
            trace.RemoveAt(trace.Count - 1);
            return r;
        }

        public bool Combine(NTree<T> chain)
        {
            Reset();
            var t = chain.GetAllPath();
            if (t.Count != 1) return false;
            List<NTreeNode<T>> lchain = t[0];
            int i = 0;
            // Find the first common nodes
            for (i = 0; i < lchain.Count; i++)
            {
                if (!Nodes.ContainsKey(lchain[i].data))
                    if (i == 0)
                        AddHead(lchain[i].data, true);
                    else
                        AddNode(lchain[i].data, true);
                else if (i == 0)
                    FindNode(lchain[i].data);
                else
                {
                    NTreeNode<T> n = GetNode(lchain[i].data);
                    AddNode(n, true);
                }
            }
            Reset();
            return true;
        }
    };
}
