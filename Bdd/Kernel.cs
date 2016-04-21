#define GC

using System;
using System.Collections.Generic;
using System.Text;


namespace BddSharp.Kernel
{
    /// <summary>
    /// The Bdd-object is a bddnode in a ROBDD (Reduced Ordered Binary Desicsion Diagram).
    /// </summary>
    public class Bdd : IDisposable
    {
        internal static Dictionary<int, int> bddCollection = new Dictionary<int, int>();
        internal static object padlock = new object();
        private int u;
        //private int count = 1;
        private int gen;
        /// <summary>
        /// Gets the nodenumber
        /// </summary>
        public int U
        {
            get { return u; }
        }

        /// <summary>
        /// Makes a BddNode from a int. Adds a externel reference to this object. 
        /// For internal package usage.
        /// </summary>
        /// <param name="u">Node number</param>
        /// <returns>Bdd with number u</returns>
        internal static Bdd CreateBdd(int u)
        {
            Bdd bdd = new Bdd(u, true);
            Ref(u);
            return bdd;
        }

        /// <summary>
        /// Used by CreateBdd.
        /// </summary>
        /// <param name="u">Node number</param>
        /// <param name="dif">Dummy bool - not used</param>
        private Bdd(int u, bool dif)
        {
            this.u = u;
            gen = Kernel.generation;
        }

        /// <summary>
        /// For creating initial variable node.
        /// </summary>
        /// <param name="var">Variable number</param>
        public Bdd(int var)
        {
            u = Kernel.Ithvar(var);
            gen = Kernel.generation;
            Ref(u);
        }

        /// <summary>
        /// For creating true or false node
        /// </summary>
        /// <param name="constant">Truth value</param>
        public Bdd(bool constant)
        {
            u = (constant ? Kernel.bddtrue : Kernel.bddfalse);
            gen = Kernel.generation;
        }

        /// <summary>
        /// Finds out if node is Terminal
        /// </summary>
        /// <returns></returns>
        public bool IsTerminal()
        {
            return Kernel.IsTerminal(u);
        }

        /// <summary>
        /// Gets the Low node
        /// </summary>
        public Bdd Low
        {
            get
            {
                return Bdd.CreateBdd(Kernel.Low(u));
            }
        }

        /// <summary>
        /// Gets the High node
        /// </summary>
        public Bdd High
        {
            get
            {
                return Bdd.CreateBdd(Kernel.High(u));
            }
        }

        /// <summary>
        /// Gets the node's variable
        /// </summary>
        public int Var
        {
            get
            {
                return Kernel.Var(u);
            }
        }

        /// <summary>
        /// Gets the BddNode's hashcode.
        /// </summary>
        /// <returns>hashcode</returns>
        public override int GetHashCode()
        {
            return u;
        }

        /// <summary>
        /// Compares two BddNodes.
        /// </summary>
        /// <param name="obj">BddNode</param>
        /// <returns>result</returns>
        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        /// <summary>
        /// Compares two BddNodes.
        /// </summary>
        /// <param name="bdd">BddNode</param>
        /// <returns>result</returns>
        public bool Equals(Bdd bdd)
        {
            return this.GetHashCode() == bdd.GetHashCode();       //assuming GetHashCode is unique
        }

        /// <summary>
        /// Conjunction of two Bdds
        /// </summary>
        /// <param name="l">Left Bdd</param>
        /// <param name="r">Right Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd operator &(Bdd l, Bdd r)
        {
            return Kernel.And(l, r);
        }

        /// <summary>
        /// Disjunction of two Bdds
        /// </summary>
        /// <param name="l">Left Bdd</param>
        /// <param name="r">Right Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd operator |(Bdd l, Bdd r)
        {
            return Kernel.Or(l, r);
        }

        /// <summary>
        /// Greater than operator on two Bdds
        /// </summary>
        /// <param name="l">Left Bdd</param>
        /// <param name="r">Right Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd operator >(Bdd l, Bdd r)
        {
            return Kernel.Greater(l, r);
        }

        /// <summary>
        /// Lesser than operator on two Bdds
        /// </summary>
        /// <param name="l">Left Bdd</param>
        /// <param name="r">Right Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd operator <(Bdd l, Bdd r)
        {
            return Kernel.Lesser(l, r);
        }

        /// <summary>
        /// Not operator on Bdd
        /// </summary>
        /// <param name="r">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd operator !(Bdd r)
        {
            return Kernel.Not(r);
        }

        /// <summary>
        /// Equals operator on two Bdds
        /// </summary>
        /// <param name="l">Left Bdd</param>
        /// <param name="r">Right Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd operator ==(Bdd l, Bdd r)
        {
            return Kernel.Equal(l, r);
        }

        /// <summary>
        /// Not equal operator on two Bdds
        /// </summary>
        /// <param name="l">Left Bdd</param>
        /// <param name="r">Right Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd operator !=(Bdd l, Bdd r)
        {
            return Kernel.Xor(l, r);
        }

        //c# does not allow flexible overload of <<
        /// <summary>
        /// Inverse implication operator on two Bdds
        /// </summary>
        /// <param name="l">Left Bdd</param>
        /// <param name="r">Right Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd operator <=(Bdd l, Bdd r)
        {
            return Kernel.InvImp(l, r);
        }

        //c# does not allow flexible overload of >>
        /// <summary>
        /// Implication operator on two Bdds
        /// </summary>
        /// <param name="l">Left Bdd</param>
        /// <param name="r">Right Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd operator >=(Bdd l, Bdd r)
        {
            return Kernel.Imp(l, r);
        }

        /// <summary>
        /// Send the object to garbage collection and calls external unreferencing of object.
        /// </summary>
        public void Dispose()
        {
#if GC
            ///Unref(u);
#endif
            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns nodenumber in string
        /// </summary>
        /// <returns>Node number</returns>
        public override string ToString()
        {
            return U.ToString();
        }

        /// <summary>
        /// Finalizes object and calls external unreferencing of object.
        /// </summary>
        ~Bdd()
        {
#if GC
                Unref(u);
#endif
        }

        private void Unref(int u)
        {
            if (u != 0 && u != 1 && gen == Kernel.generation)
            {
                lock (padlock)
                {
                    if (bddCollection[u] > 1)
                    {
                        bddCollection[u]--;
                    }
                    else
                    {
                        bddCollection.Remove(u);
                    }
                }
            }
        }

        static private void Ref(int u)
        {
            lock (padlock)
            {
                if (bddCollection.ContainsKey(u))
                {
                    bddCollection[u]++;
                }
                else
                {
                    bddCollection.Add(u, 1);
                }
            }
            if (Kernel.data.NeedResize())
                Kernel.data.MoreSpace();
        }

        /// <summary>
        /// Serializes the Bdd to XML and makes a picture of the Bdd using Graphviz.
        /// </summary>
        /// <param name="filename">The name of the XML and picture file.</param>
        /// <param name="pictureSize">Size of the picture. 12 is good for smaller examples.</param>
        public void Serialize(string filename, int pictureSize)
        {
            BddSerializer.Serialize(this, pictureSize, filename);
        }
    }


    internal struct BddNode
    {
        internal readonly int low;
        internal readonly int high;
        internal readonly int var;

        internal BddNode(int low, int high, int var)
        {
            this.low = low;
            this.high = high;
            this.var = var;
        }

    }

	delegate int BoolOperator(int b1, int b2);

    /// <summary>
    /// Boolean operators. Used as parameters in Kernel class.
    /// </summary>
    public enum Op : byte 
    { CON, XOR, DIS, NAND, NOR, IMPL, BIMP, GREATER, LESSER, INV_IMPL, NOT }

    /// <summary>
    /// Kernel holds all Bdd-operation. 
    /// </summary>
	public static class Kernel
	{
        internal static System.IO.TextWriter tw = new System.IO.StreamWriter("GCDEBUG.txt");
        internal static Data data;                               
        //used with Apply() with delegates
        private static Dictionary<long, int> G2 = new Dictionary<long, int>();
        //used with SatCount()
        private static Dictionary<int, int> varSet = new Dictionary<int, int>();

        //internal because garbage collecion clears.
        internal static BddGCache G;
        internal static BddGCache GAppEx; 
        internal static BddRCache R; 
		
        //used with SatCount()
        private static Dictionary<long, long> S = new Dictionary<long, long>();
        internal static int[,] Lookup;

		private static int MaxU = 1;
        /// <summary>
        /// The nodenumber of the falsenode.
        /// </summary>
        public const int bddfalse = 0;
		/// <summary>
		/// The nodenumber of the truenode.
		/// </summary>
        public const int bddtrue = 1;
		private static int vars = 0;
        private static int maxVar = 0;
        internal static int generation = 0;

        /// <summary>
        /// Sets up all collections and other values before beginning to solve a bdd problem.
        /// Must be called before solving a bdd problem.
        /// </summary>
        /// <param name="startTableAllocation">Size of unique node collection.</param>
        /// <param name="startCacheAllocation">Size of cache.</param>
        public static void Setup(int startTableAllocation, int startCacheAllocation)
        {
            data = new Data(startTableAllocation);
            G = new BddGCache(startCacheAllocation);
            GAppEx = new BddGCache(startCacheAllocation);
            R = new BddRCache(startCacheAllocation);
            AllSetup();
        }

        /// <summary>
        ///Sets up all collections and other values before beginning to solve a bdd problem.
        /// Must be called before solving a bdd problem.
        /// Sets collection and cache to default sizes.
        /// </summary>
        public static void Setup()
        {
            data = new Data();
            G = new BddGCache();
            GAppEx = new BddGCache();
            R = new BddRCache();
            AllSetup();
        }

        private static void AllSetup()
        {
            generation++;
            Bdd.bddCollection = new Dictionary<int, int>();
            G2.Clear();
            quantiHash = new Dictionary<int, int>();
            composeHash = new Dictionary<int, int>();
            MaxU = 1;
            maxVar = 0;
            data.Add(int.MaxValue, bddfalse, bddfalse, bddfalse);
            Bdd.bddCollection.Add(0, 1);
            data.Add(int.MaxValue, bddtrue, bddtrue, bddtrue);
            Bdd.bddCollection.Add(1, 1);
            Lookup = new int[11, 4]  
            { 
              {0,0,0,1},  /* and                       ( & )         */
              {0,1,1,0},  /* xor                       ( ^ )         */
              {0,1,1,1},  /* or                        ( | )         */
              {1,1,1,0},  /* nand                                    */
              {1,0,0,0},  /* nor                                     */
              {1,1,0,1},  /* implication               ( >> )        overloaded >=*/
              {1,0,0,1},  /* bi-implication                          */
              {0,0,1,0},  /* difference /greater than  ( - ) ( > )   */
              {0,1,0,0},  /* less than                 ( < )         */
              {1,0,1,1},  /* inverse implication       ( << )        overloaded <=*/
              {1,1,0,0}   /* not                       ( ! )         */
            };
        }

		internal static int Ithvar(int var)
		{
            if (Lookup == null)
                throw new Exception("Setup in kernel must be run before anything else.");
            if (var > maxVar)
                maxVar = var;
            return Mk(var, bddfalse, bddtrue);
		}

		private static int Mk(int var, int l, int h)
		{
            if (l == h)
				return l;
           			
			if (data.ContainsKey(var, l, h))
				return data[var, l, h];          //return lookup

			MaxU++;
            data.Add(var,l,h,MaxU);

			return MaxU;
		}

        private static int Apply(BoolOperator op, int u1, int u2)
        {
            int u;
            long key = gKey(u1, u2);

            if (G2.ContainsKey(key))
                return G2[key];
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

            G2.Add(key, u);
            return u;
        }

        private static long gKey(int u1, int u2)
        {
            return (long)(((long)u1 << 32) + ((long)u2));
        }

        private static int Apply(Op op, int u1, int u2)
        {
            int u;
            if (G.ContainsKey(u1, u2, op))
                return G[u1,u2,op];

            if (IsTerminal(u1) && IsTerminal(u2))
            {
                return Lookup[(int)(byte)op, u1*2+u2];
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

            G.Add(u1, u2, op, u);
            return u;
        }
        
        internal static bool IsTerminal(int u)
		{
			return u == bddfalse || u == bddtrue;
		}

		private static bool ToBool(int u)
		{
			return Convert.ToBoolean(u);
		}

		private static int Terminal(bool b)
		{
			return b ? bddtrue : bddfalse;
		}
//############################################################
        /// <summary>
        /// Boolean operator equal.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd Equal(Bdd u1, Bdd u2)
        {
            int u = Apply(Op.BIMP, u1.U, u2.U);
            return Bdd.CreateBdd(u);
        }

        /// <summary>
        /// Boolean operator conjunction.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd And(Bdd u1, Bdd u2)
        {
            int u = Apply(Op.CON, u1.U, u2.U);
            return Bdd.CreateBdd(u);
        }

        /// <summary>
        /// Boolean operator disjunction.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd Or(Bdd u1, Bdd u2)
        {
            int u = Apply(Op.DIS, u1.U, u2.U);
            return Bdd.CreateBdd(u);
        }

        /// <summary>
        /// Boolean operator greater than.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd Greater(Bdd u1, Bdd u2)
        {
            int u = Apply(Op.GREATER, u1.U, u2.U);
            return Bdd.CreateBdd(u);
        }

        /// <summary>
        /// Boolean operator implication.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd Imp(Bdd u1, Bdd u2)
        {
            int u = Apply(Op.IMPL, u1.U, u2.U);
            return Bdd.CreateBdd(u);
        }

        /// <summary>
        /// Boolean operator inverse implication.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd InvImp(Bdd u1, Bdd u2)
        {
            int u = Apply(Op.INV_IMPL, u1.U, u2.U);
            return Bdd.CreateBdd(u);
        }

        /// <summary>
        /// Boolean operator lesser than.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd Lesser(Bdd u1, Bdd u2)
        {
            int u = Apply(Op.LESSER, u1.U, u2.U);
            return Bdd.CreateBdd(u);
        }

        /// <summary>
        /// Boolean operator Nand.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd Nand(Bdd u1, Bdd u2)
        {
            int u = Apply(Op.NAND, u1.U, u2.U);
            return Bdd.CreateBdd(u);
        }

        /// <summary>
        /// Boolean operator Nor.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd Nor(Bdd u1, Bdd u2)
        {
            int u = Apply(Op.NOR, u1.U, u2.U);
            return Bdd.CreateBdd(u);
        }

        /// <summary>
        /// Boolean operator Xor.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd Xor(Bdd u1, Bdd u2)
        {
            int u = Apply(Op.XOR, u1.U, u2.U);
            return Bdd.CreateBdd(u);
        }

        /// <summary>
        /// Boolean operator not.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd Not(Bdd u1)
        {
            int u = Apply(Op.NOT, u1.U, u1.U);
            return Bdd.CreateBdd(u);
        }

//############################################################

        /// <summary>
        /// Boolean operator conjunction. Uses delegate to evaluate result.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd DelegateAnd(Bdd u1, Bdd u2)
		{
            int u = Apply(delegate(int b1, int b2) { return Terminal(ToBool(b1) && ToBool(b2)); }, u1.U, u2.U);
            G2.Clear();
            return Bdd.CreateBdd(u);
		}

        /// <summary>
        /// Boolean operator Nand. Uses delegate to evaluate result.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd DelegateNand(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(!(ToBool(b1) && ToBool(b2))); }, u1.U, u2.U);
			G2.Clear();
            return Bdd.CreateBdd(u);
		}

        /// <summary>
        /// Boolean operator equal. Uses delegate to evaluate result.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd DelegateEqual(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(ToBool(b1) == ToBool(b2)); }, u1.U, u2.U);
            G2.Clear();
            return Bdd.CreateBdd(u);
		}

        /// <summary>
        /// Boolean operator disjunction. Uses delegate to evaluate result.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd DelegateOr(Bdd u1, Bdd u2)
		{
            int u = Apply(delegate(int b1, int b2) { return Terminal(ToBool(b1) || ToBool(b2)); }, u1.U, u2.U);
            G2.Clear();
            return Bdd.CreateBdd(u);
 		}

        /// <summary>
        /// Boolean operator Xor. Uses delegate to evaluate result.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd DelegateXor(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(ToBool(b1) != ToBool(b2)); }, u1.U, u2.U);
            G2.Clear();
            return Bdd.CreateBdd(u);
		}

        /// <summary>
        /// Boolean operator Nor. Uses delegate to evaluate result.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd DelegateNor(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(!(ToBool(b1) || ToBool(b2))); }, u1.U, u2.U);
            G2.Clear();
            return Bdd.CreateBdd(u);
		}

        /// <summary>
        /// Boolean operator implication. Uses delegate to evaluate result.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd DelegateImp(Bdd u1, Bdd u2)
		{            
            int u = Apply(delegate(int b1, int b2) { return Terminal(!(ToBool(b1) && (!ToBool(b2)))); }, u1.U, u2.U);
            G2.Clear();
            return Bdd.CreateBdd(u);     
		}

        /// <summary>
        /// Boolean operator inverse implication. Uses delegate to evaluate result.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd DelegateInvImp(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(!(!ToBool(b1) && (ToBool(b2)))); }, u1.U, u2.U);
            G2.Clear();
            return Bdd.CreateBdd(u);
		}

        /// <summary>
        /// Boolean operator greater than. Uses delegate to evaluate result.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd DelegateGreater(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(b1 > b2); }, u1.U, u2.U);
            G2.Clear();
            return Bdd.CreateBdd(u);
		}

        /// <summary>
        /// Boolean operator lesser than. Uses delegate to evaluate result.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <param name="u2">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd DelegateLess(Bdd u1, Bdd u2)
		{
			int u = Apply(delegate(int b1, int b2) { return Terminal(b1 < b2); }, u1.U, u2.U);
            G2.Clear();
            return Bdd.CreateBdd(u);
		}

        /// <summary>
        /// Boolean operator not. Uses delegate to evaluate result.
        /// </summary>
        /// <param name="u1">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd DelegateNot(Bdd u1)
		{            
            int u = Apply(delegate(int b1, int b2) { return Terminal(!ToBool(b1)); }, u1.U, u1.U);
            G2.Clear();
            return Bdd.CreateBdd(u);           
		}


//################################################################################################


       

        /// <summary>
        /// Existential quantification on a list of variables.
        /// </summary>
        /// <param name="varList">List of variables</param>
        /// <param name="root">bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd VarListExists(BddPairList varList, Bdd root)
        {
            int u = VarListQuantification(ref varList, root.U, Op.DIS);
            return Bdd.CreateBdd(u);
        }
        
        internal static Dictionary<int, int> quantiHash = new Dictionary<int, int>();
        
        private static int VarListQuantification(ref BddPairList varList, int u, Op op)
        {
            if (quantiHash.ContainsKey(u))
                return quantiHash[u];
            int low, high;

            if (!IsTerminal(Low(u)))
                low = VarListQuantification(ref varList, Low(u), op);
            else
                low = Low(u);

            if (!IsTerminal(High(u)))
                high = VarListQuantification(ref varList, High(u), op);
            else
                high = High(u);

            int newU;
            if (varList.QuantificationContainsKey(Var(u)))
                newU = Apply(op, low, high);
            else
                newU = Mk(Var(u), low, high);

            quantiHash.Add(u, newU);
            return newU;
        }

        

        
        private static int Exists(int var, int u)
        {
            return Apply(Op.DIS, InternalRes(u, var, true), InternalRes(u, var, false));
        }

        /// <summary>
        /// Existential quantification on one variable.
        /// </summary>
        /// <param name="var">Variable number</param>
        /// <param name="root">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd Exists(int var, Bdd root)
        {
            int u = Exists(var, root.U);
            return Bdd.CreateBdd(u);
        }

//###########################################################################################

        /// <summary>
        /// Boolean operation and existential quantification on a list of variables.
        /// The same operation as first using Apply() and then VarListExists().
        /// </summary>
        /// <param name="operatoR">Boolean operator</param>
        /// <param name="root1">bdd</param>
        /// <param name="root2">bdd</param>
        /// <param name="varList">List of variables</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd ApplyExists(Op operatoR, Bdd root1, Bdd root2, BddPairList varList)
        {
            int u = AppEx(operatoR, root1.U, root2.U, ref varList);
            return Bdd.CreateBdd(u);
        }

        private static int AppEx(Op op, int u1, int u2, ref BddPairList varList)
        {
            int u;
            if (GAppEx.ContainsKey(u1, u2, op))
                return GAppEx[u1, u2, op];

            if (IsTerminal(u1) && IsTerminal(u2))
            {
                return Lookup[(int)(byte)op, u1 * 2 + u2];
            }
            else
            {
                switch (Var(u1).CompareTo(Var(u2)))
                {
                    case 0:     //equals
                        u = Mk(Var(u1), AppEx(op, Low(u1), Low(u2), ref varList), AppEx(op, High(u1), High(u2), ref varList));
                        break;
                    case -1:    //less than
                        u = Mk(Var(u1), AppEx(op, Low(u1), u2, ref varList), AppEx(op, High(u1), u2, ref varList));
                        break;
                    default:    //greater than
                        u = Mk(Var(u2), AppEx(op, u1, Low(u2), ref varList), AppEx(op, u1, High(u2), ref varList));
                        break;
                }
            }
            if(varList.QuantificationContainsKey(Var(u)))
                u = Apply(Op.DIS, Low(u), High(u));
            GAppEx.Add(u1, u2, op, u);
            return u;
        }

        

        

//#######################################################################################




        /// <summary>
        /// Universal quantification on a list of variables.
        /// </summary>
        /// <param name="varList">List of variables</param>
        /// <param name="root">bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd VarListForAll(BddPairList varList, Bdd root)
        {
            int u = VarListQuantification(ref varList, root.U, Op.CON);
            return Bdd.CreateBdd(u);
        }

        
        private static int ForAll(int var, int u)
        {
            return Apply(Op.CON, InternalRes(u, var, true), InternalRes(u, var, false));
        }
        
        /// <summary>
        /// Universal quatification on a variabel. 
        /// </summary>
        /// <param name="var">Variable number</param>
        /// <param name="root">Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd ForAll(int var, Bdd root)
        {
            int u = ForAll(var, root.U);
            return Bdd.CreateBdd(u);
        }

        

//########################################################################################

        
        /// <summary>
        /// Substitution of one Bdd with a variable in a Bdd.
        /// </summary>
        /// <param name="t">The Bdd the substitution is performed on.</param>
        /// <param name="replace">The Bdd that is used for the substitution.</param>
        /// <param name="x">The variable number that is substituted</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd Compose(Bdd t, Bdd replace, int x)
        {
            int u = BddCompose(t.U, replace.U, x);
            return Bdd.CreateBdd(u);
        }

        private static int BddCompose(int u, int tp, int x)
        {
            int tpu = Ithvar(tp);
            return Apply(Op.DIS, Apply(Op.CON, tpu, InternalRes(u, x, true)), Apply(Op.CON, Apply(Op.NOT, tpu, tpu), InternalRes(u, x, false)));
        }

        /// <summary>
        /// Substitution of one variable with another variable in a Bdd.
        /// </summary>
        /// <param name="t">The Bdd the substitution is performed on.</param>
        /// <param name="replace">The variable number that is inserted.</param>
        /// <param name="x">The variable number that is substituted</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd Compose(Bdd t, int replace, int x)
        {
            int u = Compose(t.U, replace, x);
            return Bdd.CreateBdd(u);
        }

        private static int Compose(int u, int tp, int x)
        {
            if (Var(u) == tp)
            {
                return Replace(u, x);
            }
            else if (u != 0 && u != 1)
            {
                return Mk(Var(u), Compose(Low(u), tp, x), Compose(High(u), tp, x));
            }
            else
                return u;
        }

        private static int Replace(int u, int tp)
        {
            return Mk(tp, Low(u), High(u));
        }

        /// <summary>
        /// Substitution of a list of variablepairs in a Bdd.
        /// </summary>
        /// <param name="bpl">Variablepairlist</param>
        /// <param name="root">bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd VarListCompose(BddPairList bpl, Bdd root)
        {
            int u = VarListCompose(ref bpl, root.U);
            return Bdd.CreateBdd(u);
        }

        internal static Dictionary<int, int> composeHash = new Dictionary<int, int>();
        private static int VarListCompose(ref BddPairList bpl, int u)
        {
            if (u == 0 || u == 1)
                return u;
            if (composeHash.ContainsKey(u))
                return composeHash[u];
            int newU = u;
            if (bpl.ComposeContainsKey(Var(u)))
            {
                newU = Mk(bpl[Var(u)], VarListCompose(ref bpl, Low(u)), VarListCompose(ref bpl, High(u)));
            }
            else
            {
                newU = Mk(Var(u), VarListCompose(ref bpl, Low(u)), VarListCompose(ref bpl, High(u)));
            }
            
            composeHash.Add(u, newU);
            return newU;
        }

//#############################################################################################

        /// <summary>
        /// If-Then operator for Bdds.
        /// Implemented as conjunction.
        /// </summary>
        /// <param name="condition">Condition Bdd</param>
        /// <param name="then">Then Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd IfThen(Bdd condition, Bdd then)
        {
            int u = Apply(Op.CON, condition.U, then.U);
            return Bdd.CreateBdd(u);
        }

        /// <summary>
        /// If-Then-Else operator for Bdds.
        /// </summary>
        /// <param name="condition">Condition Bdd</param>
        /// <param name="then">Then Bdd</param>
        /// <param name="eLse">Else Bdd</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd IfThenElse(Bdd condition, Bdd then, Bdd eLse)
        {
            int u = Apply(Op.CON, Apply(Op.CON, condition.U, then.U), Apply(Op.CON, Apply(Op.NOT, condition.U, condition.U), eLse.U));
            return Bdd.CreateBdd(u);
        }


		internal static int Low(int u)
		{
            return data[u].low;
		}

		internal static int High(int u)
		{
            return data[u].high;
		}

		internal static int Var(int u)
		{
            return data[u].var;
		}
        
        /// <summary>
        /// Restriction on a variable in a Bdd to a truth value.
        /// </summary>
        /// <param name="root">Bdd to perform the restriction on.</param>
        /// <param name="restrictVar">Variable to restrict</param>
        /// <param name="restrictValue">Truth value</param>
        /// <returns>Resulting Bdd</returns>
        public static Bdd Restrict(Bdd root, int restrictVar, bool restrictValue)
        {
            int u = Res(root.U, restrictVar, restrictValue);
            return Bdd.CreateBdd(u);
        }

        private static int InternalRes(int u, int restrictVar, bool restrictValue)
        {
            int newU = Res(u, restrictVar, restrictValue);
            return newU;
        }


		private static int Res(int u, int restrictVar, bool restrictValue)
		{
			int newU;
            if (R.ContainsKey(u, restrictVar, restrictValue))
                return R[u, restrictVar, restrictValue];
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
			R.Add(u, restrictVar, restrictValue, newU);
			return newU;
		}

        /// <summary>
        /// Determins the number of valid truth assigments for a given Bdd.
        /// Takes all initialized variables into consideration.
        /// </summary>
        /// <param name="root">Bdd</param>
        /// <returns>Number of valid truth assigments.</returns>
        public static long SatCount(Bdd root)
        {
            long result = Count(root.U);
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

        /// <summary>
        /// Determins the number of valid truth assigments for a given Bdd.
        /// Takes only variables in the given Bdd into consideration.
        /// </summary>
        /// <param name="root">Bdd</param>
        /// <returns>Number of valid truth assigments.</returns>
		public static long SatCountVarSet(Bdd root)
		{
            vars = VarCount(root);
            long result = CountVarSet(root.U);
			S.Clear();
            varSet.Clear();
            vars = 0;
			return result;
		}

		private static long CountVarSet(int u)
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
                        result += (long)Math.Pow(2, vars + 1 - varSet[Var(u)] - 1) * CountVarSet(Low(u));
					else
                        result += (long)Math.Pow(2, varSet[Var(Low(u))] - varSet[Var(u)] - 1) * CountVarSet(Low(u));

					if (IsTerminal(High(u)))
                        result += (long)Math.Pow(2, vars + 1 - varSet[Var(u)] - 1) * CountVarSet(High(u));
					else
                        result += (long)Math.Pow(2, varSet[Var(High(u))] - varSet[Var(u)] - 1) * CountVarSet(High(u));
				}
				S.Add(u, result);
				return result;
			}
		}
        private static List<int> varsList = new List<int>();
        private static int VarCount(Bdd root)
        {
            Vars(root.U);
            varsList.Sort();
            int orderedVar = 1;
            foreach (int var in varsList)
            {
                varSet.Add(var, orderedVar++);
            }
            int result = varSet.Count;
            varsList.Clear();
            return result;
        }
        
        private static void Vars(int u)
        {
            if (!varsList.Contains(Var(u)))
                varsList.Add(Var(u));

            if (!IsTerminal(Low(u)))
                Vars(Low(u));

            if (!IsTerminal(High(u)))
                Vars(High(u));
        }

        /// <summary>
        /// Returns a satisfying truth assigment for a given Bdd.
        /// Format of the returning string is "var1=1 var2=0 etc.".
        /// </summary>
        /// <param name="root">Bdd</param>
        /// <returns>A satisfying truth assigment.</returns>
        public static string AnySat(Bdd root)
        {
            return RecursiveAnySat(root.U);
        }

		private static string RecursiveAnySat(int u)
		{
			if (u == bddfalse)
				return "Error: The Bdd only consists of one Terminalnode 0";
			else if (u == bddtrue)
				return "[]";
			else if (Low(u) == bddfalse)
				return "var" + Var(u).ToString() + "=1 " + RecursiveAnySat(High(u));
			else
				return "var" + Var(u).ToString() + "=0 " + RecursiveAnySat(Low(u));
		}

        /// <summary>
        /// Returns a satisfying truth assigment for a given Bdd.
        /// Format of the returning string is "101-1 etc." where a "-"
        /// represents a variable where both true and false satisfy a
        /// solution.
        /// </summary>
        /// <param name="root">Bdd</param>
        /// <returns>A satisfying truth assigment.</returns>
        public static string CompactAnySat(Bdd root)
        {
            return RecursiveCompactAnySat(root.U, root.Var);
        }

        private static string RecursiveCompactAnySat(int u, int var)
        {
            if (u == bddfalse)
                return "Error: The Bdd only consists of one Terminalnode 0";
            if (u == bddtrue)
                return "0";

            string placeholders = "";
            while (Var(u) != var++)
                placeholders += "-";        //inserts - for variables that doesnt matter for the sollution

            if (Low(u) == bddfalse)
                return placeholders + "1" + RecursiveCompactAnySat(High(u),var);
            else
                return placeholders + "0" + RecursiveCompactAnySat(Low(u), var);
        }

        /// <summary>
        /// Returns all satisfying truth assigments for a given Bdd.
        /// </summary>
        /// <param name="root">Bdd</param>
        /// <returns>All satisfying truth assigments</returns>
        public static string AllSat(Bdd root)
        {
            return RecursiveAllSat(root.U);
        }

		private static string RecursiveAllSat(int u)
		{
			if (u == bddtrue)
				return Environment.NewLine;
			else if (Low(u) != bddfalse && High(u) != bddfalse)
				return "var" + Var(u) + "=0 " + RecursiveAllSat(Low(u)) +
						"var" + Var(u) + "=1 " + RecursiveAllSat(High(u));
			else if (Low(u) == bddfalse)
				return "var" + Var(u) + "=1 " + RecursiveAllSat(High(u));
			else
				return "var" + Var(u) + "=0 " + RecursiveAllSat(Low(u));
		}

        /// <summary>
        /// Number of nodes in node table.
        /// </summary>
        /// <returns></returns>
        public static int NodeCount()
        {
            return data.Count;
        }

        internal static bool IsIthNode(int u)
        {
            return Low(u) == bddfalse && High(u) == bddtrue;
        }
    }
}
