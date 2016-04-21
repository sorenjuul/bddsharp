#define GC

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace BddSharp.Kernel
{

    public struct HEntry
    {
        public readonly int low;
        public readonly int high;
        public readonly int var;
        public readonly int u;

        public HEntry(int var, int low, int high, int u)
        {
            this.var = var;
            this.low = low;
            this.high = high;
            this.u = u;
        }

        public static HEntry DirtyEntry()
        {
            return new HEntry(-1, -1, -1, -1);
        }

        //public bool Null()
        //{
        //    return var == 0 && u == 0;
        //}

        //public bool Used()
        //{
        //    return var > 0 || u > 0;
        //}

        //public bool Dirty()
        //{
        //    return low == -1;
        //}

        //public bool Available()         //for insert
        //{
        //    return IsNull() || Dirty();
        //}

        //public static bool operator ==(BddHashEntry left, BddHashEntry right)
        //{
        //    return (left.high == right.high && left.low == right.low && left.var == right.var);
        //}

        //public static bool operator !=(BddHashEntry left, BddHashEntry right)
        //{
        //    return (left.high != right.high || left.low != right.low || left.var != right.var);
        //}

        //public override bool Equals(object obj)
        //{
        //    return this == (BddHashEntry)obj;
        //}

        //public override int GetHashCode()
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}
    }

    internal class HCluster
    {
        internal HEntry[] Cluster;
        internal HCluster next = null;

        internal HCluster(int size)
        {
            Cluster = new HEntry[size];
        }

        internal bool IsLastCluster()
        {
            return next == null;
        }
    }

    internal class HPosition
    {
        internal HCluster cluster;
        internal int offset;
        internal HPosition(ref HCluster cluster, int offset)
        {
            this.cluster = cluster;
            this.offset = offset;
        }

        internal bool IsLastPosition()
        {
            return (this.cluster.IsLastCluster() && IsLastOffset() );
        }

        internal bool IsLastOffset()
        {
            return this.offset == this.cluster.Cluster.Length;
        }

        public HEntry this[int offset]
        {
            get
            {
                return this.cluster.Cluster[offset];
            }
        }
    }

    public class BDDHash
    {
        private HCluster FirstCluster;
        private int startSize = 0x20000;

        private int size;
        private double factor = 2;
        private int count = 0;

        public int Count
        {
            get { return count; }
        }

        public int this[int var, int low, int high]
        {
            get
            {
                HPosition pos = GenerateKey(var, low, high);

                while (!Null(pos.cluster.Cluster[pos.offset]))
                {
                    if (pos.cluster.Cluster[pos.offset].low == low && pos.cluster.Cluster[pos.offset].high == high && pos.cluster.Cluster[pos.offset].var == var)
                        return pos.cluster.Cluster[pos.offset].u;

                    IncreaseKey(ref pos);
                }

                return 0;
            }
        }

        public bool NeedResize()
        {
            return (count > (size / 10) * 6) ? true : false; //fill degree = 60%
        }

        public BDDHash()
        {
            this.Clear();
        }

        public BDDHash(int startSize)
        {
            if (startSize <= 0)
                throw new Exception("Start size to small. Must be more than zero");
            this.startSize = (int)Math.Pow(factor, Math.Ceiling(Math.Log(startSize, factor)));
            this.Clear();
        }

        //splits a key into cluster + offset
        internal HPosition GenerateKey(int var, int low, int high)
        {
            int offset = (int)(Triple(var, low, high) % (ulong)size);
            HCluster probe = FirstCluster;

            while (offset >= probe.Cluster.Length)
            {
                offset -= probe.Cluster.Length;
                probe = probe.next;
            }

            return new HPosition(ref probe,offset);
        }

        public ulong Triple(int var, int low, int high)
        {
            return (ulong)Pair((ulong)var, Pair((ulong)low, (ulong)high));
        }

        public ulong Pair(ulong i, ulong j)
        {
            return ((i + j) * (i + j + 1)) / 2 + i;
        }

        internal void IncreaseKey(ref HPosition position)
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


        public void Add(HEntry entry)
        {
#if NoGC
            if (NeedResize())
                AddCluster(); 
#endif
            HPosition pos = GenerateKey(entry.var, entry.low, entry.high);
            while (Used(pos.cluster.Cluster[pos.offset]))
            {
                IncreaseKey(ref pos);
            }
            pos.cluster.Cluster[pos.offset] = entry;
            count++;
        }

        public void Add(int var, int low, int high, int u)
        {
            Add(new HEntry(var, low, high, u));
        }

        public void AddCluster()
        {
            HCluster NewCluster = new HCluster(this.size);
            size = (int)(size * factor);

            NewCluster.next = this.FirstCluster;                //New cluster is added in the beginning
            this.FirstCluster = NewCluster;

            HPosition pos = new HPosition(ref FirstCluster.next, 0);

            while (!(pos.cluster==FirstCluster && pos.offset==0))      //rehash
            {
                if (Used(pos.cluster.Cluster[pos.offset]))
                {
                    HEntry entry = pos.cluster.Cluster[pos.offset];
                    HPosition newPosition = GenerateKey(entry.var, entry.low, entry.high);

                    if (newPosition.offset != pos.offset || newPosition.cluster != pos.cluster)
                    {
                        pos.cluster.Cluster[pos.offset] = HEntry.DirtyEntry();
                        this.count--;
                        Add(entry);
                    }
                }

                IncreaseKey(ref pos);
            }
        }

        public void Remove(int var, int low, int high)
        {
            HPosition pos = GenerateKey(var, low, high);
            while (!Null(pos.cluster.Cluster[pos.offset]))
            {
                if (pos.cluster.Cluster[pos.offset].var == var && pos.cluster.Cluster[pos.offset].low == low && pos.cluster.Cluster[pos.offset].high == high)
                {
                    count--;
                    pos.cluster.Cluster[pos.offset] = HEntry.DirtyEntry();     //Only flags dirty
                    return;
                }
                IncreaseKey(ref pos);
            }
        }

        public bool ContainsKey(int var, int low, int high)
        {
            HPosition pos = GenerateKey(var, low, high);
            while (!Null(pos.cluster.Cluster[pos.offset]))
            {
                if (pos.cluster.Cluster[pos.offset].low == low && pos.cluster.Cluster[pos.offset].high == high && pos.cluster.Cluster[pos.offset].var == var)
                    return true;
                IncreaseKey(ref pos);
            }
            return false;
        }

        public bool Null(HEntry entry)
        {
            return entry.var == 0 && entry.u == 0;
        }

        public bool Used(HEntry entry)
        {
            return entry.var > 0 || entry.u > 0;
        }

        public bool Dirty(HEntry entry)
        {
            return entry.low == -1;
        }

        public void Clear()
        {
            this.FirstCluster = new HCluster(startSize);
            this.size = this.startSize;
            this.count = 0;
        }

        public void Clear(int size)
        {
            this.FirstCluster = new HCluster(size);
            this.startSize = size;
            this.size = size;
            this.count = 0;
        }

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            HPosition pos = new HPosition(ref FirstCluster, 0);

            while(!pos.IsLastPosition())
            {
                if(Used(pos.cluster.Cluster[pos.offset]))
                    yield return pos.cluster.Cluster[pos.offset];
                IncreaseKey(ref pos);
            }
        }

        #endregion
    }
}
