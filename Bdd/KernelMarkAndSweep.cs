using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

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
            return Kernel.IsTerminal(u);
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
                return Bdd.CreateBdd(Kernel.High(u));
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
            //Kernel.DelRef(u);
        }
    }

	public struct BddNode
	{
		public readonly int low;
		public readonly int high;
		public readonly int var;
        public bool mark;

		public BddNode(int low, int high, int var)
		{
			this.low = low;
			this.high = high;
			this.var = var;
            this.mark = false;
		}

        // for creating ithvars and terminalnodes
        public BddNode(int var)
        {
            this.low = 0;
            this.high = 1;
            this.var = var;
            this.mark = true;
        }
	}

	delegate int BoolOperator(int b1, int b2);

	public class Kernel
	{
        internal static BDDHash H = new BDDHash(0x100000);
		//internal static Dictionary<int, BddNode> T = new Dictionary<int, BddNode>(0x1000000);
        internal static BddTable T = new BddTable(0x100000);
        internal static BddHashCache<long, int> G = new BddHashCache<long, int>(100000);
        //internal static Dictionary<long, int> G = new Dictionary<long, int>();
        //internal static BddCache<long,int > G = new BddCache<long, int>(1200000);
        private static Dictionary<int, int> R = new Dictionary<int, int>();
		private static Dictionary<long, long> S = new Dictionary<long, long>();

		private static int MaxU = 1;
		public const int bddfalse = 0;
		public const int bddtrue = 1;
		private static int maxVar = 1;
        private static bool resize = false;

		public static void Setup()
		{
            T = new BddTable();
            H = new BDDHash(0x100000);
            MarkAndSweep.dic.Clear();
            G.Clear();
            maxVar = 1;
            MaxU = 1;
            T.Add(bddfalse, new BddNode(int.MaxValue));
            T.Add(bddtrue, new BddNode(int.MaxValue));
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
            if (H.NeedResize)
                resize = true;
            if (H.ContainsKey(var, bddfalse, bddtrue))
            {
                return H[var, bddfalse, bddtrue];          //return lookup
            }
            MaxU++;
            H.Add(var, bddfalse, bddtrue, MaxU);
            T.Add(MaxU, new BddNode(var));
            return MaxU;
		}
        


		private static int Mk(int var, int l, int h)
		{
            if (H.NeedResize)
                resize = true;
            if (l == h)
            {
                return l;
            }
            if (H.ContainsKey(var, l, h))
            {
                return H[var, l, h];          //return lookup
            }
            MaxU++;
            H.Add(var, l, h, MaxU);
            T.Add(MaxU, new BddNode(l, h, var));
            return MaxU;
		}

		private static int Apply(BoolOperator op, int u1, int u2)
		{
            int u;
			long key = gKey(u1, u2);

            if (G.ContainsKey(key))
            {
                return G[key];
            }
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
            int u = Apply(delegate(int b1, int b2) { return Terminal(ToBool(b1) && ToBool(b2)); }, u1.U, u2.U);
           
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

        public static Bdd Nand(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(!(ToBool(b1) && ToBool(b2))); }, u1.U, u2.U);
		
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

        public static Bdd Equal(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(ToBool(b1) == ToBool(b2)); }, u1.U, u2.U);
          
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

        public static Bdd Or(Bdd u1, Bdd u2)
		{
            int u = Apply(delegate(int b1, int b2) { return Terminal(ToBool(b1) || ToBool(b2)); }, u1.U, u2.U);
           
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

        public static Bdd Xor(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(ToBool(b1) != ToBool(b2)); }, u1.U, u2.U);
            
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

        public static Bdd Nor(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(!(ToBool(b1) || ToBool(b2))); }, u1.U, u2.U);
           
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

        public static Bdd Imp(Bdd u1, Bdd u2)
		{
            int u = Apply(delegate(int b1, int b2) { return Terminal(!(ToBool(b1) && (!ToBool(b2)))); }, u1.U, u2.U);
            
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

        public static Bdd InvImp(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(!(!ToBool(b1) && (ToBool(b2)))); }, u1.U, u2.U);
            
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

        public static Bdd Greater(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(b1 > b2); }, u1.U, u2.U);
            
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

        public static Bdd Less(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(b1 < b2); }, u1.U, u2.U);
            
            AddRef(u);
            return Bdd.CreateBdd(u);
		}

		public static Bdd Not(Bdd u1)
		{
            int u = Apply(delegate(int b1, int b2) { return Terminal(!ToBool(b1)); }, u1.U, u1.U);
            AddRef(u);
            return Bdd.CreateBdd(u);
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
			else if (restrictValue == false)/* node.var = restrictNodeVar*/
				newU = Res(Low(u), restrictVar, restrictValue);
			else /* node.var = restrictNodeVar and restrictValue is true*/
				newU = Res(High(u), restrictVar, restrictValue);
			R.Add(u, newU);
			return newU;
		}

		public static Bdd Restrict(Bdd root, int restrictVar, bool restrictValue)
		{
			int u = Res(root.U, restrictVar, restrictValue);
            AddRef(u);
            R.Clear();
            return Bdd.CreateBdd(u);
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

        static Object padlock = new Object();

        public static void AddRef(int u)
        {
            lock (padlock)
            {
                G.Clear();
                MarkAndSweep.Ref(u);
                if (resize)
                {
                    //MarkAndSweep.GarbageCollect(ref H, ref T);
                    resize = false;
                }
            }
        }

        public static void DelRef(int u)
        {
            lock (padlock)
            {
                MarkAndSweep.UnRef(u);
            }
        }

        public static Bdd Exists(Bdd root, int var)
        {
            return And(Restrict(root, var, true), Restrict(root, var, false));
        }


        public static Bdd ForAll(Bdd root, int var)
        {
            return Or(Restrict(root, var, true), Restrict(root, var, false));
        } 

        public static Bdd IfThen(Bdd Condition, Bdd Then)
        {
            return And(Condition, Then);
        }

        public static Bdd IfThenElse(Bdd Condition, Bdd Then, Bdd Else)
        {
            return Or(IfThen(Condition, Then), IfThen(Not(Condition), Else));
        }
    }
}
