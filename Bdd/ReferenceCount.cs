using System;
using System.Collections.Generic;
using System.Text;

namespace BddSharp.Kernel
{
    public class ReferenceCount
    {
        internal static Dictionary<int, int> Visitor = new Dictionary<int, int>();
        internal static Dictionary<int, int> Visitor2 = new Dictionary<int, int>();
        
        public static void Ref(int u)
        {

            if (!Kernel.IsIthNode(u) && !Kernel.IsTerminal(u))
            {
                if (Visitor.ContainsKey(u))
                {
                    AddCount(u);
                }
                else
                {
                    if (!Kernel.IsTerminal(Kernel.Low(u)))
                        Ref(Kernel.Low(u));

                    AddCount(u);

                    if (!Kernel.IsTerminal(Kernel.High(u)))
                        Ref(Kernel.High(u));
                    if (!Visitor.ContainsKey(u))
                        Visitor.Add(u, 1);
                }
            }
        }

        static void AddCount(int u)
        {
            BddNode n = Kernel.T[u];
            n.refcount++;
            Kernel.T[u] = n;
        }

        public static void UnRef(int u)
        {
            if (!Kernel.IsIthNode(u) && !Kernel.IsTerminal(u))
            {
                if (Visitor2.ContainsKey(u))
                {
                    DelCount(u);
                    if (Kernel.T[u].refcount == 0)
                    {
                        DelNode(u);
                    }
                }
                else
                {
                if (!Kernel.IsTerminal(Kernel.Low(u)))
                    UnRef(Kernel.Low(u));

                DelCount(u);

                if (!Kernel.IsTerminal(Kernel.High(u)))
                    UnRef(Kernel.High(u));
                
                if (Kernel.T[u].refcount == 0)
                {
                    DelNode(u);
                }
                if (!Visitor2.ContainsKey(u))
                    Visitor2.Add(u, 1);
                }  
            }
        }

        static void DelCount(int u)
        {
            BddNode n = Kernel.T[u];
            n.refcount--;
            Kernel.T[u] = n;
        }

        static void DelNode(int u)
        {
            BddNode n = Kernel.T[u];
            long key = ((long)n.var << 48) + ((long)n.low << 24) + ((long)n.high);
            Kernel.H.Remove(key);
            Kernel.T.Remove(u);
        }
    }
}
