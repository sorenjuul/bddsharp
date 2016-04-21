using System;
using System.Collections.Generic;
using System.Text;

namespace BddSharp.Kernel
{
    public class StopAndCopy
    {
        internal static Dictionary<int, int> dic = new Dictionary<int, int>();

        public static void Ref(int u)
        {
            if(!dic.ContainsKey(u))
                dic.Add(u, 0);
        }

        public static void UnRef(int u)
        {
            dic.Remove(u);
        }

        public static void GarbageCollect(ref BDDHash H, ref BddTable T)
        {
            foreach (KeyValuePair<int, int> root in dic)
            {
                int u = root.Key;
                Mark(u);
            }
            int size = H.Size;
            H = new BDDHash(size * 2);
            Kernel.G.Clear();
            BddTable newT = new BddTable(size * 2);
            
            foreach (BddTableEntry entry in T)
            {
                if (entry.node.mark == true)
                {
                    BddNode n = entry.node;
                    if(!Kernel.IsIthNode(entry.u) && !Kernel.IsTerminal(entry.u))
                        n.mark = false;
                    newT.Add(entry.u, n);
                    H.Add(entry.node.var, entry.node.low, entry.node.high, entry.u);
                }
            }
            T = newT;
        }

        static void Mark(int u)
        {
            if (Kernel.T[u].mark != true)
            { 
                if (!Kernel.IsTerminal(Kernel.Low(u)))
                    Mark(Kernel.Low(u));

                AddMark(u);

                if (!Kernel.IsTerminal(Kernel.High(u)))
                    Mark(Kernel.High(u));  
            }
        }

        static void AddMark(int u)
        {
            BddNode n = Kernel.T[u];
            n.mark = true;
            Kernel.T[u] = n;
        }
    }
}
