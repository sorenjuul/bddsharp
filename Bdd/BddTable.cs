#define GC
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace BddSharp.Kernel
{
    public struct TEntry
    {
        public readonly int low;
        public readonly int high;
        private uint varAndMark;        //saves both mark for garbage collection and var here.
        public readonly int u;

        public int GetVar()
        {
            return (int)(varAndMark >> 1);
        }

        public bool GetMark()
        {
            return varAndMark % 2 == 1;
        }

        public void SetMark(bool mark)
        {
            if (mark)
            {
                varAndMark = varAndMark | 1;
            }
            else
            {
                varAndMark = varAndMark & uint.MaxValue-1;      //0xfffffffe
            }
        }

        public TEntry(int var, int low, int high, int u)
        {
            this.varAndMark = (uint)var*2;
            this.low = low;
            this.high = high;
            this.u = u;
        }

        public static TEntry DirtyEntry()
        {
            return new TEntry(0, -1, -1, -1);
        }
    }

    internal class TCluster
    {
        internal TEntry[] Cluster;
        internal TCluster next = null;

        internal TCluster(int size)
        {
            Cluster = new TEntry[size];
        }

        internal bool IsLastCluster()
        {
            return next == null;
        }
    }

    internal class TPosition
    {
        internal TCluster cluster;
        internal int offset;
        internal TPosition(ref TCluster cluster, int offset)
        {
            this.cluster = cluster;
            this.offset = offset;
        }

        internal bool IsLastPosition()
        {
            return (this.cluster.IsLastCluster() && IsLastOffset());
        }

        internal bool IsLastOffset()
        {
            return this.offset == this.cluster.Cluster.Length;
        }

        public TEntry this[int offset]
        {
            get
            {
                return this.cluster.Cluster[offset];
            }
        }
    }
    
    public class BddTable : ICollection
    {
        private TCluster FirstCluster;
        private Dictionary<int, int> refCount = new Dictionary<int, int>();

        private int count = 0;
        public int Count
        {
            get { return count; }
        }

        private int size = 0;

        public int Size
        {
            get { return size; }
        }

        private readonly int factor = 2; //resizing factor

        private int startSize = 0x20000;
        public int StartSize
        {
            get { return startSize; }
        }

        public BddTable(int startSize)
        {
            if (startSize <= 0)
                throw new Exception("Start size to small. Must be more than zero");
            this.startSize = (int)Math.Pow(factor,Math.Ceiling(Math.Log(startSize, factor)));
            this.Clear();
        }

        public BddTable()
        {
            this.Clear();
        }

        private static TEntry GetTEntry(TPosition pos)
        {
            return pos.cluster.Cluster[pos.offset];
        }
        
        public BddNode this[int u]
        {
            get
            {
                TPosition pos = GenerateKey(u);
                while (!Null(pos.cluster.Cluster[pos.offset]))
                {
                    if (pos.cluster.Cluster[pos.offset].u == u)
                        return new BddNode(pos.cluster.Cluster[pos.offset].low, pos.cluster.Cluster[pos.offset].high, pos.cluster.Cluster[pos.offset].GetVar());

                    IncreaseKey(ref pos);
                }
                return new BddNode(0,0,0);
            }
        }

        internal void IncreaseKey(ref TPosition position)
        {
            position.offset++;
            if (position.cluster.Cluster.Length == position.offset) //treat array as a cycle
            {
                position.offset = 0;
                if (position.cluster.IsLastCluster())
                {
                    position.cluster = this.FirstCluster;
                }
                else
                {
                    position.cluster = position.cluster.next;
                }
            }
        }


        public bool NeedResize()
        {
            return count > ((this.size / 10) * 7);//fill degree = 70%
        }


        public void Add(int u, int low, int high, int var)
        {
            Add(new TEntry(var, low, high, u));
        }

        public void Add(TEntry entry)
        {
#if NoGC
            if (NeedResize())
                AddCluster();
#endif
            TPosition pos = GenerateKey(entry.u);
            while (Used(pos.cluster.Cluster[pos.offset]))
            {
                IncreaseKey(ref pos);
            }
            pos.cluster.Cluster[pos.offset] = entry;
            count++;
        }

        public void Remove(int u)
        {
            TPosition pos = GenerateKey(u);
            while (!Null(pos.cluster.Cluster[pos.offset]))
            {
                if (pos.cluster.Cluster[pos.offset].u == u)
                {
                    count--;
                    pos.cluster.Cluster[pos.offset] = TEntry.DirtyEntry();     //Only flags dirty
                    return;
                }
                IncreaseKey(ref pos);
            }
        }

        public bool ContainsKey(int u)
        {
            TPosition pos = GenerateKey(u);
            while (!Null(pos.cluster.Cluster[pos.offset]))
            {
                if (pos.cluster.Cluster[pos.offset].u == u)
                    return true;
                IncreaseKey(ref pos);
            }
            return false;
        }

        public void Clear()
        {
            count = 0;
            size = startSize;
            this.FirstCluster = new TCluster(size);
            refCount.Clear();
            refCount.Add(0, 1);
            refCount.Add(1, 1);
        }

        public void Clear(int size)
        {
            count = 0;
            this.size = size;
            startSize = size;
            this.FirstCluster = new TCluster(size);
            refCount.Clear();
            refCount.Add(0, 1);
            refCount.Add(1, 1);
        }

        public int FillDegree()
        {
            return (count * 100) / size;
        }

        private void AddCluster()
        {
            TCluster NewCluster = new TCluster(this.size);
            size = (int)(size * this.factor);

            NewCluster.next = this.FirstCluster;                //New cluster is added in the beginning
            this.FirstCluster = NewCluster;

            TPosition pos = new TPosition(ref FirstCluster.next, 0);

            while (!(pos.cluster == FirstCluster && pos.offset == 0))      //rehash
            {
                if (Used(pos.cluster.Cluster[pos.offset]))
                {
                    TEntry entry = pos.cluster.Cluster[pos.offset];
                    TPosition newPosition = GenerateKey(entry.u);

                    if (newPosition.offset != pos.offset || newPosition.cluster != pos.cluster)
                    {
                        pos.cluster.Cluster[pos.offset] = TEntry.DirtyEntry();
                        this.count--;
                        Add(entry);
                    }
                }

                IncreaseKey(ref pos);
            }
        }

        //splits a key into cluster + offset
        internal TPosition GenerateKey(int u)
        {
            int offset = u % size;
            TCluster probe = FirstCluster;

            while (offset >= probe.Cluster.Length)
            {
                offset -= probe.Cluster.Length;
                probe = probe.next;
            }

            return new TPosition(ref probe, offset);
        }



        private static bool IsTerminal(int u)
        {
            return u == 0 || u == 1;
        }

        private static bool Null(TEntry entry)
        {
            return  entry.u == 0 && entry.GetVar() == 0;
        }

        private static bool Used(TEntry entry)
        {
            return entry.GetVar() > 0 || entry.u > 0;
        }

        private static bool Dirty(TEntry entry)
        {
            return entry.low == -1;
        }


        //Garbage Collection methods
        static object padlock = new object();
        public void Ref(int u)
        {
            lock (padlock)
            {
                if (!refCount.ContainsKey(u))
                    refCount.Add(u, 1);
                else
                    refCount[u]++;
                if (NeedResize())
                    MoreSpace();
            }
        }

        public void UnRef(int u)
        {
            lock (padlock)
            {
                if (refCount.ContainsKey(u)) // for old uncollected objects
                {
                    if (refCount[u] == 1)
                        refCount.Remove(u);
                    else
                        refCount[u]--;
                }
            }
        }

        void GarbageCollect()
        {
            foreach (KeyValuePair<int, int> root in refCount)
            {
                Mark(root.Key);
            }

            TCluster cluster = this.FirstCluster;
            TEntry tempEntry;
            while (cluster != null)
            {
                for (int i = 0; i < cluster.Cluster.Length; i++)
                {
                    tempEntry = cluster.Cluster[i];
                    if (Used(tempEntry) && tempEntry.GetMark() == false)
                    {
                        Remove(tempEntry.u);
                        Kernel.H.Remove(tempEntry.GetVar(), tempEntry.low, tempEntry.high);
                    }
                    else if(Used(tempEntry) && tempEntry.GetMark() == true)
                        UnMark(tempEntry.u);
                }
                cluster = cluster.next;
            }
        }

        void Mark(int u)
        {
            TPosition pos = GenerateKey(u);
            while (!Null(pos.cluster.Cluster[pos.offset]))
            {
                if (pos.cluster.Cluster[pos.offset].u == u)
                {
                    break;
                }
                IncreaseKey(ref pos);
            }
            TEntry entry = GetTEntry(pos);
            if (entry.GetMark() != true)
            {
                if (!IsTerminal(entry.low))
                    Mark(entry.low);

                AddMark(ref pos);

                if (!IsTerminal(entry.high))
                    Mark(entry.high);
            }
        }

        void AddMark(ref TPosition pos)
        {
            pos.cluster.Cluster[pos.offset].SetMark(true);
        }

        void UnMark(int u)
        {
            TPosition pos = GenerateKey(u);
            while (!Null(pos.cluster.Cluster[pos.offset]))
            {
                if (pos.cluster.Cluster[pos.offset].u == u)
                {
                    pos.cluster.Cluster[pos.offset].SetMark(false);
                    break;
                }
                IncreaseKey(ref pos);
            }
        }

        public void MoreSpace()
        {
            GarbageCollect();
            AddCluster();
            Kernel.H.AddCluster();
            Kernel.G.Clear();
        }


        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            //index = tableArray.Length;
            //array = new BddTableEntry[index];
            //int i = 0;
            //foreach (BddTableEntry entry in this)
            //{
            //    array.SetValue(entry, i++);
            //}
        }

        int ICollection.Count
        {
            get { return this.count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            TPosition pos = new TPosition(ref FirstCluster, 0);

            while (!pos.IsLastPosition())
            {
                if (Used(pos.cluster.Cluster[pos.offset]))
                    yield return pos.cluster.Cluster[pos.offset];
                IncreaseKey(ref pos);
            }
        }

        #endregion
    }
}
