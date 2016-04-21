using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BddSharp.Kernel
{
    public class Bdd : IDisposable
    {
        private int u;
        public int U
        {
            get { return u; }
            set { u = value; }
        }

        internal static Bdd CreateBdd(int u)
        {
            Bdd bdd = new Bdd(true);
            bdd.U = u;
            return bdd;
        }

        public Bdd(int var)
        {
            U = Kernel.Ithvar(var);
        }

        public Bdd(bool constant)
        {
            U = (constant?Kernel.bddtrue:Kernel.bddfalse);
        }

        public bool IsTerminal()
        {
            return Kernel.IsTerminal(U);
        }

        public Bdd Low
        {
            get
            {
                return Bdd.CreateBdd(Kernel.Low(u));
            }
        }

        public Bdd High
        {
            get
            {
                return Bdd.CreateBdd(Kernel.High(U));
            }
        }

        public int Var
        {
            get
            {
                return Kernel.Var(U);
            }
        }

        public override int GetHashCode()
        {
            return U;
        }

        public bool Equals(Bdd bdd)
        {
            return this.GetHashCode() == bdd.GetHashCode();       //assuming GetHashCode is unique
        }

        public void Dispose()
        {
            Kernel.DelRef(u);
            System.GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            return U.ToString();
        }

        ~Bdd()
        {
            Kernel.DelRef(u);
        }
    }

	public struct BddNode
	{
		public readonly int low;
		public readonly int high;
		public readonly int var;
        public int refcount;

		public BddNode(int low, int high, int var)
		{
			this.low = low;
			this.high = high;
			this.var = var;
            this.refcount = 0;
		}
	}

	delegate int BoolOperator(int b1, int b2);

	public class Kernel
	{
		internal static Dictionary<long, int> H = new Dictionary<long, int>();
        //private static BDDHash H = new BDDHash();
		internal static Dictionary<int, BddNode> T = new Dictionary<int, BddNode>();
		private static Dictionary<long, int> G = new Dictionary<long, int>();
		private static Dictionary<int, int> R = new Dictionary<int, int>();
		private static Dictionary<long, long> S = new Dictionary<long, long>();

		private static int MaxU = 1;
		public const int bddfalse = 0;
		public const int bddtrue = 1;
		private static int maxVar = 1;

		public static void Setup()
		{  
            T.Clear();
            H.Clear();
            G.Clear();
            maxVar = 1;
            MaxU = 1;
            T.Add(bddfalse, new BddNode(bddfalse, bddfalse, int.MaxValue));
            T.Add(bddtrue, new BddNode(bddtrue, bddtrue, int.MaxValue));
		}

        public static void Done()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

		public static int Ithvar(int var)
		{
			if (var > maxVar)
				maxVar = var;
            return Mk(var, bddfalse, bddtrue);
		}
        


		private static int Mk(int var, int l, int h)
		{
            lock (padlock)
            {
                if (l == h)
                    return l;

                long key = ((long)var << 48) + ((long)l << 24) + ((long)h);

                if (H.ContainsKey(key))
                    return H[key];          //return lookup

                MaxU++;
                H.Add(key, MaxU);
                //H.Add(var, l, h, MaxU);
                T.Add(MaxU, new BddNode(l, h, var));

                return MaxU;
            }
		}

		private static int Apply(BoolOperator op, int u1, int u2)
		{
			int u;
			long key = gKey(u1, u2);

			if (G.ContainsKey(key))
				return G[key];
			if (IsTerminal(u1) && IsTerminal(u2))
			{
				u = op(u1, u2);
			}
			else
			{
				switch (Var(u1).CompareTo(Var(u2)))
				{
					case 0:     //equals
						u = Mk(Var(u1), Apply(op, Low(u1), Low(u2)), Apply(op, High(u1), High(u2)));
						break;
					case -1:    //less than
						u = Mk(Var(u1), Apply(op, Low(u1), u2), Apply(op, High(u1), u2));
						break;
					default:    //greater than
						u = Mk(Var(u2), Apply(op, u1, Low(u2)), Apply(op, u1, High(u2)));
						break;
				}
			}

			G.Add(key, u);
			return u;
		}

		public static bool IsTerminal(int u)
		{
			return u == bddfalse || u == bddtrue;
		}

        public static bool IsIthNode(int u)
        {
            return Low(u) == bddfalse && High(u) == bddtrue;
        }

		private static bool ToBool(int u)
		{
			return u == 1;
		}

		private static int Terminal(bool b)
		{
			return b ? bddtrue : bddfalse;
		}

        public static Bdd And(Bdd u1, Bdd u2)
		{
            lock (padlock)
            {
                int u = Apply(delegate(int b1, int b2) { return Terminal(ToBool(b1) && ToBool(b2)); }, u1.U, u2.U);
                G.Clear();
                //AddRef(u);
                GarbageCollect(u, u1.U, u2.U);
                return Bdd.CreateBdd(u);
            }
		}

        public static Bdd Nand(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(!(ToBool(b1) && ToBool(b2))); }, u1.U, u2.U);
			G.Clear();
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

        public static Bdd Equal(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(ToBool(b1) == ToBool(b2)); }, u1.U, u2.U);
            G.Clear();
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

        public static Bdd Or(Bdd u1, Bdd u2)
		{
            lock (padlock)
            {
                int u = Apply(delegate(int b1, int b2) { return Terminal(ToBool(b1) || ToBool(b2)); }, u1.U, u2.U);
                G.Clear();
                //AddRef(u);
                GarbageCollect(u, u1.U, u2.U);
                return Bdd.CreateBdd(u);
            }
		}

        public static Bdd Xor(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(ToBool(b1) != ToBool(b2)); }, u1.U, u2.U);
            G.Clear();
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

        public static Bdd Nor(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(!(ToBool(b1) || ToBool(b2))); }, u1.U, u2.U);
            G.Clear();
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

        public static Bdd Imp(Bdd u1, Bdd u2)
		{
            lock (padlock)
            {
                int u = Apply(delegate(int b1, int b2) { return Terminal(!(ToBool(b1) && (!ToBool(b2)))); }, u1.U, u2.U);
                G.Clear();
                //AddRef(u);
                GarbageCollect(u, u1.U, u2.U);
                return Bdd.CreateBdd(u);
            }
		}

        public static Bdd InvImp(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(!(!ToBool(b1) && (ToBool(b2)))); }, u1.U, u2.U);
            G.Clear();
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

        public static Bdd Greater(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(b1 > b2); }, u1.U, u2.U);
            G.Clear();
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

        public static Bdd Less(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(b1 < b2); }, u1.U, u2.U);
            G.Clear();
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

		public static Bdd Not(Bdd u1)
		{
            lock (padlock)
            {
                int u = Apply(delegate(int b1, int b2) { return Terminal(!ToBool(b1)); }, u1.U, u1.U);
                G.Clear();
                AddRef(u);
                //DelRef(u1.U);
                return Bdd.CreateBdd(u);
            }
		}

		public static int Low(int u)
		{
            
                return T[u].low;
           
		}

		public static int High(int u)
		{
			return T[u].high;
		}

		public static int Var(int u)
		{
			return T[u].var;
		}

		private static long gKey(int u1, int u2)
		{
			return (long)(((long)u1 << 32) + ((long)u2));
		}

		private static int Res(int u, int restrictVar, bool restrictValue)
		{
			int newU;
			if (R.ContainsKey(u))
				return R[u];
			else if (Var(u) > restrictVar)
				newU = u;
			else if (Var(u) < restrictVar)
				newU = Mk(Var(u),
						Res(Low(u), restrictVar, restrictValue),
						Res(High(u), restrictVar, restrictValue));
			else if (restrictValue == false)/* root.Level = restrictNodeLevel*/
				newU = Res(Low(u), restrictVar, restrictValue);
			else /* root.Level = restrictNodeLevel and restrictValue is true*/
				newU = Res(High(u), restrictVar, restrictValue);
			R.Add(u, newU);
			return newU;
		}

		public static int Restrict(int root, int restrictVar, bool restrictValue)
		{
			int u = Res(root, restrictVar, restrictValue);
			R.Clear();
			return u;
		}

		public static long SatCount(int root)
		{
			long result = Count(root);
			S.Clear();
			return result;
		}

		private static long Count(int u)
		{
			if (S.ContainsKey(u))
				return S[u];
			else
			{
				long result = 0;
				if (IsTerminal(u))
					result = u;
				else
				{
					if (IsTerminal(Low(u)))
						result += (long)Math.Pow(2, maxVar + 1 - Var(u) - 1) * Count(Low(u));
					else
						result += (long)Math.Pow(2, Var(Low(u)) - Var(u) - 1) * Count(Low(u));

					if (IsTerminal(High(u)))
						result += (long)Math.Pow(2, maxVar + 1 - Var(u) - 1) * Count(High(u));
					else
						result += (long)Math.Pow(2, Var(High(u)) - Var(u) - 1) * Count(High(u));
				}
				S.Add(u, result);
				return result;
			}
		}

		public static string AnySat(int u)
		{
			if (u == bddfalse)
				return "Error: The Bdd only consists of one Terminalnode 0";
			else if (u == bddtrue)
				return "[]";
			else if (Low(u) == bddfalse)
				return "x" + Var(u).ToString() + "=1 " + AnySat(High(u));
			else
				return "x" + Var(u).ToString() + "=0 " + AnySat(Low(u));
		}

		public static string AllSat(int u)
		{
			if (u == bddtrue)
				return Environment.NewLine;
			else if (Low(u) != bddfalse && High(u) != bddfalse)
				return "x" + Var(u) + "=0 " + AllSat(Low(u)) +
						"x" + Var(u) + "=1 " + AllSat(High(u));
			else if (Low(u) == bddfalse)
				return "x" + Var(u) + "=1 " + AllSat(High(u));
			else
				return "x" + Var(u) + "=0 " + AllSat(Low(u));
		}

        public static int TCount()
        {
            return T.Count;
        }

        public static string TNumbers()
        {
            lock (padlock)
            {
                int below = 0;
                int zero = 0;
                int above = 0;
                foreach (KeyValuePair<int, BddNode> pair in T)
                {
                    if (pair.Value.refcount < 0)
                        below++;
                    else if (pair.Value.refcount == 0)
                        zero++;
                    else
                        above++;
                }
                return "Below =" + below + " Zero =" + zero + " Above =" + above;
            }
        }

        static Object padlock = new Object();

        public static void GarbageCollect(int u, int u1, int u2)
        {
            AddRef(u);
            //DelRef(u1);
            //DelRef(u2);
        }

        public static void AddRef(int u)
        {
            ReferenceCount.Ref(u);
            ReferenceCount.Visitor.Clear();
        }

        public static void DelRef(int u)
        {
            lock (padlock)
            {
                ReferenceCount.UnRef(u);
                ReferenceCount.Visitor2.Clear();
            }
        }
    }
}
