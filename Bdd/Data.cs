#define GC
using System;
using System.Collections.Generic;
using System.Text;

namespace BddSharp.Kernel
{
    internal struct Entry
    {
        public readonly int low;
        public readonly int high;
        private uint varAndMark;        //saves both mark for garbage collection and var here.
        public readonly int u;

        public Entry(int var, int low, int high, int u)
        {
            this.varAndMark = (uint)var*2;
            this.low = low;
            this.high = high;
            this.u = u;
        }

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

        public static Entry DirtyEntry()
        {
            return new Entry(0,-1,-1,-1);
        }

        public static Entry NullEntry()
        {
            return new Entry(0, 0, 0, 0);
        }

    }

    internal class Cluster
    {
        internal Entry[] Clusters;
        internal Cluster next = null;

        internal Cluster(int size)
        {
            Clusters = new Entry[size];
        }

        internal bool IsLastCluster()
        {
            return next == null;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////

    class Data
    {

        //HT Vars used by the datastructures H + T
        //once clusters reach this size all new clusters will have this size
        private int maxClusterSize = 24000000;

        //resizing factor
        private readonly int factor = 2;

        //allocated records
        private int allocated;

        //size of first cluster
        private int startSize = 2000000;

        Cluster FirstHCluster;
        Cluster FirstTCluster;
        private Dictionary<int, int> refCount = new Dictionary<int, int>();
        ///////////////////////////// COMMON METHODS ////////////////////////////////////////////////
        private int count = 0;
        public int Count
        {
            get { return count; }
        }

        public Data()
        {
            Clear();
        }

        public Data(int startSize)
        {
            this.startSize = startSize;
            Clear();
        }

        public void Add(int var, int low, int high, int u)
        {
            Add(new Entry(var,low,high,u));
        }

        public void Add(Entry entry)
        {
#if NoGC
                if (NeedResize())
                    AddCluster();
#endif
            // T
            int offset = entry.u;
            Cluster cluster = this.FirstTCluster;
            GetPosition(ref cluster, ref offset);
            while (Used(ref cluster.Clusters[offset]))
            {
                IncreaseTKey(ref cluster, ref offset);
            }
            cluster.Clusters[offset] = entry;

            // H
            cluster = this.FirstHCluster;
            offset = 0;
            GetPosition(ref cluster, ref offset, entry.GetVar(), entry.low, entry.high);
            while (Used(ref cluster.Clusters[offset]))
            {
                IncreaseHKey(ref cluster, ref offset);
            }
            cluster.Clusters[offset] = entry;

            count++;
        }

        public void Remove(int u)
        {

            int offset = u;
            Cluster cluster = this.FirstTCluster;
            GetPosition(ref cluster, ref offset);
            int high;
            int low;
            int var;

            // T
            while (!Null(ref cluster.Clusters[offset]))
            {
                if (cluster.Clusters[offset].u == u)
                {
                    high = cluster.Clusters[offset].high;
                    low = cluster.Clusters[offset].low;
                    var = cluster.Clusters[offset].GetVar();
                    cluster.Clusters[offset] = Entry.DirtyEntry();     //Flags T dirty
                    // H
                    Cluster cluster2 = this.FirstHCluster;
                    int offset2 = 0;
                    GetPosition(ref cluster2, ref offset2, var, low, high);
                    while (!Null(ref cluster2.Clusters[offset2]))
                    {
                        if (cluster2.Clusters[offset2].GetVar() == var && cluster2.Clusters[offset2].low == low && cluster2.Clusters[offset2].high == high)
                        {
                            count--;
                            cluster2.Clusters[offset2] = Entry.DirtyEntry();     //Flag H dirty
                            return;
                        }
                        IncreaseHKey(ref cluster2,ref offset2);
                    }
                }
                IncreaseTKey(ref cluster, ref offset);
            }
        }

        public int this[int var, int low, int high]
        {
            get
            {
                Cluster cluster = this.FirstHCluster;
                int offset = 0;
                GetPosition(ref cluster, ref offset, var, low, high);

                while (!Null(ref cluster.Clusters[offset]))
                {
                    if (cluster.Clusters[offset].low == low && cluster.Clusters[offset].high == high && cluster.Clusters[offset].GetVar() == var)
                        return cluster.Clusters[offset].u;

                    IncreaseHKey(ref cluster, ref offset);
                }

                throw new KeyNotFoundException("Key not present in collection.");
            }
        }

        public BddNode this[int u]
        {
            get
            {
                Cluster cluster = this.FirstTCluster;
                int offset = u;
                GetPosition(ref cluster,ref offset);
                while (!Null(ref cluster.Clusters[offset]))
                {
                    if (cluster.Clusters[offset].u == u)
                        return new BddNode(cluster.Clusters[offset].low, cluster.Clusters[offset].high, cluster.Clusters[offset].GetVar());

                    IncreaseTKey(ref cluster,ref offset);
                }
                throw new KeyNotFoundException("Key not present in collection.");
            }
        }

        public void GetPosition(ref Cluster cluster, ref int offset)
        {
            offset = offset % allocated;

            while (offset >= cluster.Clusters.Length)
            {
                offset -= cluster.Clusters.Length;
                cluster = cluster.next;
            }
        }

        public void GetPosition(ref Cluster cluster, ref int offset, int var, int low, int high)
        {
            offset = (int)(Triple(var, low, high) % (ulong)allocated);

            while (offset >= cluster.Clusters.Length)
            {
                offset -= cluster.Clusters.Length;
                cluster = cluster.next;
            }
        }

        public bool NeedResize()
        {
            return count > ((allocated / 10) * 5);               //fill degree = 70%
        }

        public bool ContainsKey(int u)
        {
            Cluster cluster = this.FirstTCluster;
            int offset = u;
            GetPosition(ref cluster,ref offset);
            while (!Null(ref cluster.Clusters[offset]))
            {
                if (cluster.Clusters[offset].u == u)
                    return true;
                IncreaseTKey(ref cluster, ref offset);
            }
            return false;
        }

        public bool ContainsKey(int var, int low, int high)
        {
            Cluster cluster = this.FirstHCluster;
            int offset = 0;
            GetPosition(ref cluster, ref offset, var, low, high);
            while (!Null(ref cluster.Clusters[offset]))
            {
                if (cluster.Clusters[offset].low == low && cluster.Clusters[offset].high == high && cluster.Clusters[offset].GetVar() == var)
                    return true;
                IncreaseHKey(ref cluster, ref offset);
            }
            return false;
        }

        public void Clear()
        {
            this.FirstTCluster = new Cluster(this.startSize);
            this.FirstHCluster = new Cluster(this.startSize);
            this.count = 0;
            this.allocated = this.startSize;
        }

        public int FillDegree()
        {
            return (count * 100) / allocated;
        }

        private bool IsTerminal(int u)
        {
            return u == 0 || u == 1;
        }

        private bool Null(ref Entry entry)
        {
            return entry.u == 0 && entry.GetVar() == 0;
        }

        private bool Used(ref Entry entry)
        {
            return entry.GetVar() > 0 || entry.u > 0;
        }

        private bool Dirty(ref Entry entry)
        {
            return entry.low == -1;
        }

        internal void IncreaseTKey(ref Cluster cluster, ref int offset)
        {
            offset++;
            if (cluster.Clusters.Length == offset) //treat array as a circle
            {
                offset = 0;
                if (cluster.next == null)       //Last cluster
                {
                    cluster = this.FirstTCluster;
                }
                else
                {
                    cluster = cluster.next;
                }
            }
        }

        internal void IncreaseHKey(ref Cluster cluster, ref int offset)
        {
            offset++;
            if (cluster.Clusters.Length == offset) //treat array as a circle
            {
                offset = 0;
                if (cluster.next == null)       //Last cluster
                {
                    cluster = this.FirstHCluster;
                }
                else
                {
                    cluster = cluster.next;
                }
            }
        }

        public ulong Triple(int var, int low, int high)
        {
            return (ulong)Pair((ulong)var, Pair((ulong)low, (ulong)high));
        }

        public ulong Pair(ulong i, ulong j)
        {
            return ((i + j) * (i + j + 1)) / 2 + i;
        }

        private void AddCluster()
        {
            Cluster NewTCluster;
            Cluster NewHCluster;

            //T+H
            if ((FirstTCluster.Clusters.Length * factor) >= this.maxClusterSize)
            {
                NewHCluster = new Cluster(this.maxClusterSize);
                NewTCluster = new Cluster(this.maxClusterSize);
            }
            else
            {
                NewTCluster = new Cluster(FirstTCluster.Clusters.Length * factor);
                NewHCluster = new Cluster(FirstHCluster.Clusters.Length * factor);
            }
            this.allocated += NewHCluster.Clusters.Length;

            // T
            NewTCluster.next = this.FirstTCluster;                //New cluster is added in the beginning
            this.FirstTCluster = NewTCluster;

            // H
            NewHCluster.next = this.FirstHCluster;
            this.FirstHCluster = NewHCluster;

            // Rehash T
            Cluster cluster = FirstTCluster.next;
            int offset = 0;

            do
            {
                if (!Null(ref cluster.Clusters[offset]))
                {
                    if(Dirty(ref cluster.Clusters[offset]))
                    {
                        cluster.Clusters[offset] = Entry.NullEntry();      //turn dirty into null
                    }
                    else
                    {
                        Entry entry = new Entry(cluster.Clusters[offset].GetVar(), cluster.Clusters[offset].low, cluster.Clusters[offset].high, cluster.Clusters[offset].u);
                        cluster.Clusters[offset] = Entry.DirtyEntry();
                        RehashT(ref entry);
                    }
                }
                IncreaseTKey(ref cluster,ref offset);
            } while (!(cluster == FirstTCluster && offset == 0));


            // Rehash H
            cluster = FirstHCluster.next;
            offset = 0;

            do
            {
                if (!Null(ref cluster.Clusters[offset]))
                {
                    if (Dirty(ref cluster.Clusters[offset]))
                    {
                        cluster.Clusters[offset] = Entry.NullEntry();      //turn dirty into null
                    }
                    else
                    {
                        Entry entry = new Entry(cluster.Clusters[offset].GetVar(), cluster.Clusters[offset].low, cluster.Clusters[offset].high, cluster.Clusters[offset].u);
                        cluster.Clusters[offset] = Entry.DirtyEntry();
                        RehashH(ref entry);
                    }
                }
                IncreaseHKey(ref cluster, ref offset);
            } while (!(cluster == FirstHCluster && offset == 0));

        }

        private void RehashT(ref Entry entry)
        {
            Cluster cluster = this.FirstTCluster;
            int offset = entry.u;
            GetPosition(ref cluster, ref offset);
            while (Used(ref cluster.Clusters[offset]))
            {
                IncreaseTKey(ref cluster,ref offset);
            }
            cluster.Clusters[offset] = entry;
        }

        private void RehashH(ref Entry entry)
        {
            Cluster cluster = this.FirstHCluster;
            int offset = 0;
            GetPosition(ref cluster, ref offset, entry.GetVar(), entry.low, entry.high);
            while (Used(ref cluster.Clusters[offset]))
            {
                IncreaseHKey(ref cluster, ref offset);
            }
            cluster.Clusters[offset] = entry;
        }

        // Garbage Collection Methods

        void GarbageCollect()
        {
            lock (Bdd.padlock)
            {
                foreach (KeyValuePair<int, int> root in Bdd.bddCollection)
                {
                    Mark(root.Key);
                }
            }

            Cluster cluster = this.FirstTCluster;
            Entry tempEntry;
            while (cluster != null)
            {
                for (int i = 0; i < cluster.Clusters.Length; i++)
                {
                    tempEntry = cluster.Clusters[i];
                    if (Used(ref tempEntry) && tempEntry.GetMark() == false)
                    {
                        Remove(tempEntry.u);
                        removed++;
                    }
                    else if (Used(ref tempEntry) && tempEntry.GetMark() == true)
                        UnMark(tempEntry.u);
                }
                cluster = cluster.next;
            }
        }
        int removed = 0;
        void Mark(int u)
        {
            Cluster cluster = this.FirstTCluster;
            int offset = u;
            GetPosition(ref cluster,ref offset);
            while (!Null(ref cluster.Clusters[offset]))
            {
                if (cluster.Clusters[offset].u == u)
                {
                    break;
                }
                IncreaseTKey(ref cluster,ref offset);
            }

            if (cluster.Clusters[offset].GetMark() != true)
            {
                if (!IsTerminal(cluster.Clusters[offset].low))
                    Mark(cluster.Clusters[offset].low);

                cluster.Clusters[offset].SetMark(true);

                if (!IsTerminal(cluster.Clusters[offset].high))
                    Mark(cluster.Clusters[offset].high);
            }
        }

        void UnMark(int u)
        {
            Cluster cluster = this.FirstTCluster;
            int offset = u;
            GetPosition(ref cluster,ref offset);
            while (!Null(ref cluster.Clusters[offset]))
            {
                if (cluster.Clusters[offset].u == u)
                {
                    cluster.Clusters[offset].SetMark(false);
                    break;
                }
                IncreaseTKey(ref cluster,ref offset);
            }
        }

        public void MoreSpace()
        {
            GC.Collect(1);
            GC.WaitForPendingFinalizers();
            GarbageCollect();
            AddCluster();
            Kernel.G.Clear();
            Kernel.R.Clear();
            Kernel.GAppEx.Clear();
            Kernel.composeHash.Clear();
            Kernel.quantiHash.Clear();
            Kernel.tw.WriteLine(removed);
            Kernel.tw.Flush();
            removed = 0;
        }
    }
}
