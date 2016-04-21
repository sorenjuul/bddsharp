using System;
using System.Collections.Generic;
using System.Text;

namespace BddSharp.Kernel
{
    internal struct BddGCacheEntry
    {
        public int u1;
        public int u2;
        public Op op;
        public int result;

        public BddGCacheEntry(int u1, int u2, Op op, int result)
        {
            this.u1=u1;
            this.u2=u2;
            this.op = op;
            this.result = result;
        }
    }

    internal class BddGCache
    {
        private BddGCacheEntry[] cacheArray;
        private uint size = 100000;
        
        //################## Just DEBUG #################
        //public int collisions = 0;
        //public int inserts = 0;
        //###############################################

        public int this[int u1, int u2, Op op]
        {
            get
            {
                uint key = GenerateKey(op, u1, u2);

                if (cacheArray[key].u1 == u1 &&
                    cacheArray[key].u2 == u2 &&
                    cacheArray[key].op == op
                    )
                    return cacheArray[key].result;
                return 0;
            }
        }

        public BddGCache(int startCacheAllaction)
        {
            this.size = (uint) startCacheAllaction;
            Clear();
        }

        public BddGCache()
        {
            Clear();
        }

        private uint GenerateKey(Op op, int u1, int u2)
        {
            return (uint)Triple((byte)op,u1,u2) % size;
        }

        public ulong Triple(byte op, int low, int high)
        {
            return (ulong)Pair((ulong)op, Pair((ulong)low, (ulong)high));
        }

        public ulong Pair(ulong i, ulong j)
        {
            return ((i + j) * (i + j + 1)) / 2 + i;
        }

        public void Add(int u1, int u2, Op op, int u)
        {
            BddGCacheEntry entry = new BddGCacheEntry(u1, u2, op, u);
            uint key = GenerateKey(op,u1, u2);

            //################## Just DEBUG #################
            //if (cacheArray[key].u1 != 0 && cacheArray[key].u2 != 0) collisions++;
            //inserts++;
            //###############################################

            cacheArray[key] = entry;
        }

        public bool ContainsKey(int u1, int u2, Op op)
        {
            uint key = GenerateKey(op,u1, u2);

            if (cacheArray[key].u1 == u1 && cacheArray[key].u2 == u2 && cacheArray[key].op == op)
                return true;

            return false;
        }

        public void Clear()
        {
            this.cacheArray = new BddGCacheEntry[this.size];
            cacheArray[0] = new BddGCacheEntry(-1, -1, Op.BIMP, -1);

        }
    }
}
