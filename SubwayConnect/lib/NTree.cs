using System;
using System.Collections;
using System.Collections.Generic;

namespace MetroMate
{
    public class NTreeNode<T>
    {
        public T data;
        public List<NTreeNode<T>> child { get; set; }
        public NTreeNode<T> parent { get; set; }
        public NTreeNode(T data, NTreeNode<T> parent, List<NTreeNode<T>> child = null)
        {
            this.data = data;
            this.parent = parent;
            this.child = child;
            if (this.child == null)
                this.child = new List<NTreeNode<T>>();
        }

    };

    public class NTree<T>
    {
        private NTreeNode<T> Head;
        private NTreeNode<T> Current;

        public T CurrentData { get => Current.data; set => Current.data = value; }
        public NTreeNode<T> GetCurrentNode { get { return Current; } }
        public List<NTreeNode<T>> GetChild { get => Current.child; }

        public NTree()
        {
            Head = null;
            Current = null;
        }

        public NTree(T head)
        {
            Head = new NTreeNode<T>(head, null);
            Current = Head;
        }

        public NTree(NTreeNode<T> node)
        {
            Head = node;
            Head.parent = null;
            Current = Head;
        }

        public void Reset() { Head = Current; }

        public NTreeNode<T> AddNode(T child)
        {
            NTreeNode<T> newNode = new NTreeNode<T>(child, Current);
            Current.child.Add(newNode);
            return newNode;
        }

        public NTreeNode<T> AddNode(NTreeNode<T> child)
        {
            NTreeNode<T> newNode = child;
            newNode.parent = Current;
            Current.child.Add(newNode);
            return newNode;
        }

        // will return null if not exsist
        public NTreeNode<T> NextNode(T node)
        {
            foreach (NTreeNode<T> childnode in Current.child)
            {
                if (childnode.data.Equals(node))
                {
                    Current = childnode;
                    return Current;
                }
            }
            return null;
        }


        private List<List<NTreeNode<T>>> GetAllPathHelper(NTreeNode<T> CurrentNode,
                                                          List<NTreeNode<T>> trace)
        {
            List<List<NTreeNode<T>>> r = new List<List<NTreeNode<T>>>();
            if (CurrentNode.child.Count == 0)
            {
                List<NTreeNode<T>> newtrace = trace;
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

        // return type list of Node traces
        public List< List<NTreeNode<T>> > GetAllPath()
        {
            return GetAllPathHelper(Head, new List<NTreeNode<T>>());
        }
    };
}
