using System;
using System.Collections.Generic;
using System.Text;

namespace BddSharp.Kernel
{
    internal struct BddRCacheEntry
    {
        public int u;
        public int var;
        public bool value;
        public int result;

        public BddRCacheEntry(int u, int var, bool value, int result)
        {
            this.u = u;
            this.var = var;
            this.value = value;
            this.result = result;
        }
    }

    internal class BddRCache
    {
        private BddRCacheEntry[] cacheArray;
        //private uint size = 100000;
        private uint size = 100000;

        //################## Just DEBUG #################
        public int collisions = 0;
        public int inserts = 0;
        //###############################################

        public int this[int u, int var, bool value]
        {
            get
            {
                uint key = GenerateKey(value, u, var);

                if (cacheArray[key].u == u &&
                    cacheArray[key].var == var &&
                    cacheArray[key].value == value
                    )
                    return cacheArray[key].result;
                return 0;
            }
        }

        public BddRCache(int startCacheAllocation)
        {
            this.size = (uint) startCacheAllocation;
            Clear();
        }

        public BddRCache()
        {
            Clear();
        }

        private uint GenerateKey(bool value, int u, int var)
        {
            return (uint)Triple(value?1:0, u, var) % size;
        }

        public ulong Triple(int value, int u, int var)
        {
            return (ulong)Pair((ulong)value, Pair((ulong)u, (ulong)var));
        }

        public ulong Pair(ulong i, ulong j)
        {
            return ((i + j) * (i + j + 1)) / 2 + i;
        }

        public void Add(int u, int var, bool value, int result)
        {
            BddRCacheEntry entry = new BddRCacheEntry(u, var, value, result);
            uint key = GenerateKey(value, u, var);

            //################## Just DEBUG #################
            if (cacheArray[key].u != 0 && cacheArray[key].var != 0) collisions++;
            inserts++;
            //###############################################

            cacheArray[key] = entry;
        }

        public bool ContainsKey(int u, int var, bool value)
        {
            uint key = GenerateKey(value, u, var);

            if (cacheArray[key].u == u && cacheArray[key].var == var && cacheArray[key].value == value)
                return true;

            return false;
        }

        public void Clear()
        {
            this.cacheArray = new BddRCacheEntry[this.size];
            cacheArray[0] = new BddRCacheEntry(-1, -1, false, -1);

        }
    }
}
