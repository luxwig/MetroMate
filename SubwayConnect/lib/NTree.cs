﻿using System;
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
        public NTreeNode(T data)
        {
            this.data = data;
            this.parent = new HashSet<NTreeNode<T>>();
            this.child = new HashSet<NTreeNode<T>>();
        }
        public NTreeNode(T data, NTreeNode<T> parent, HashSet<NTreeNode<T>> child = null)
        {
            this.data = data;
            this.parent = new HashSet<NTreeNode<T>>();
            this.parent.Add(parent);
            this.child = child;
            if (this.child == null)
                this.child = new HashSet<NTreeNode<T>>();
        }
        public NTreeNode(T data, HashSet<NTreeNode<T>> parent, HashSet<NTreeNode<T>> child = null)
        {
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
            Head.Add(new NTreeNode<T>(head));
            Current = Head[0];
            Nodes = new Dictionary<T, NTreeNode<T>>();
        }
        public NTree(NTreeNode<T> node)
        {
            Head = new List<NTreeNode<T>>();
            Head.Add(new NTreeNode<T>(node.data, new HashSet<NTreeNode<T>>(), node.child));
            Current = Head[0];
            Nodes = new Dictionary<T, NTreeNode<T>>();
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
                    newNode = child;
                }
            else
            {
                newNode  = new NTreeNode<T>(child.data, Current, child.child);
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
        public List< List<NTreeNode<T>> > GetAllPath()
        {
            List<List<NTreeNode<T>>> r = new List<List<NTreeNode<T>>>();
            foreach (var h in Head)
                r.AddRange(GetAllPathHelper(h, new List<NTreeNode<T>>()));
            return r;
        }
        public List<List<T>> GetAllPathData()
        {

            List<List<NTreeNode<T>>> r = new List<List<NTreeNode<T>>>();
            foreach (var h in Head)
                r.AddRange(GetAllPathHelper(h, new List<NTreeNode<T>>()));
            List<List<T>> result = new List<List<T>>();
            foreach (var l in r)
            {
                result.Add(new List<T>());
                foreach (var t in l)
                    result[result.Count - 1].Add(t.data);
            }
            return result;
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