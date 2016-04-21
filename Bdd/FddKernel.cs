using System;
using System.Collections.Generic;
using System.Text;

namespace BddSharp.Kernel
{
   /// <summary>
   /// Class for handling Finite Desition diagrams (FDD's), BDD's with natural numbers
   /// </summary>
    public class FddKernel
    {
        /// <summary>
        /// Setup initializer the Fdd Kernel and must be run instead of the Bdd Setup when
        /// working with FDD's
        /// </summary>
        public static void Setup()
        {
            Kernel.Setup();
            FddDictionary.Clear();
            RestrictDictionary.Clear();
            MaxVar = 1;
        }

        private static Dictionary<int, int> FddDictionary = new Dictionary<int, int>(); //key beeing var, value beeing size
        private static Dictionary<int, int> RestrictDictionary = new Dictionary<int, int>(); //key beeing var, value beeing restricted value

        private static int MaxVar;

        internal static int IthVar(int states)         //MinValue always beeing 0
        {
            int size = Log2(states);
            FddDictionary.Add(MaxVar, size);
            for (int i = MaxVar; i < MaxVar + size; i++)
                Kernel.Ithvar(i);

            int result = MaxVar;

            MaxVar += size;

            return result;                          //result beeing the StartVar.
        }
        /// <summary>
        /// Returns an array of valid solutions to the given problem. Notice that the result is
        /// ordered based on when Fdd's are declared. An error will occur if the function cannot
        /// uphold the maxvalue passed into the function as the second parameter.
        /// </summary>
        /// <param name="root">The Bdd result</param>
        /// <param name="MaxValue">The function will uphold any max value passed via this function</param>
        /// <returns></returns>
        public static int[] AnySat(Bdd root, int MaxValue)
        {
            int[] result = new int[FddDictionary.Count];

            string s = Kernel.CompactAnySat(root); //returns a big bitpattern "100110001110010101010101"

            if (s.Contains("Error") || "".CompareTo(s) == 0) return new int[0];

            int i = 0;
            foreach (KeyValuePair<int, int> entry in FddDictionary)
            {
                if (RestrictDictionary.ContainsKey(entry.Key))
                {
                    result[i] = RestrictDictionary[entry.Key];
                }
                else
                {
                    while (entry.Key + entry.Value > s.Length)          //should be fixed in CompactAnySat.
                    {
                        s += "0";
                    }

                    result[i] = ToValue(s.Substring(entry.Key, entry.Value));
                }
                if (result[i] > MaxValue)
                    return new int[0];      //no valid solution

                i++;
            }

            return result;
        }

        /// <summary>
        /// returns the bit-size of a given FDD.
        /// </summary>
        /// <param name="fdd">The FDD to get the size of</param>
        /// <returns></returns>
        public static int Size(Fdd fdd)
        {
            return FddDictionary[fdd.Var];
        }

        private static int ToValue(string BitPattern)   //e.g. turns "1111" into 15
        {
            int result = 0;
            foreach (char bit in BitPattern)
            {
                result <<= 1;
                if (bit == '1') result++;
            }
            return result;
        }

        private static bool[] ToBitPattern(int value)
        {
            return ToBitPattern(value, Log2(value));
        }

        private static bool[] ToBitPattern(int value, int bitSize)    //e.g. turns 14 into {true,true,true,false}
        {
            bool[] result = new bool[bitSize];
            for (int i = result.Length-1; i >= 0; i--)
            {
                result[i] = ((value & 1) == 1);
                value >>= 1;
            }
            return result;
        }

        /// <summary>
        /// restricts an FDD out of an expression, by giving it an integer value.
        /// </summary>
        /// <param name="root">the root of the tree the Fdd is a part of</param>
        /// <param name="fdd">the Fdd to restrict</param>
        /// <param name="restrictValue">the given ristrict value, must be between 0 and maxvalue of the Fdd.</param>
        /// <returns>the restricted Bdd</returns>
        public static Bdd Restrict(Bdd root, Fdd fdd, int restrictValue)
        {
            bool[] B = ToBitPattern(restrictValue);

            for (int i = 0; i < B.Length; i++)
            {
                root = Kernel.Restrict(root, fdd.Var + i, B[i]);
            }

            RestrictDictionary.Add(fdd.Var, restrictValue);

            return root;
        }

        private static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        private static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        /// <summary>
        /// Xor function for FDD's, same as a != differ function.
        /// </summary>
        /// <param name="u1">left side of the expression</param>
        /// <param name="u2">right side of the expression</param>
        /// <returns>a new BDD where u1 and u2 are xor'ed</returns>
        public static Bdd Xor(Fdd u1, Fdd u2)
        {
            int u1Size = Size(u1);
            int u2Size = Size(u2);
            int MinSize = Min(u1Size,u2Size);
            Bdd result = new Bdd(false);

            for (int i = 0; i < MinSize; i++)
            {
                result = Kernel.Or(result,Kernel.Xor(new Bdd(u1.Var+i),new Bdd(u2.Var+i)));
            }

            if (u1Size != u2Size)
            {
                if (u1Size > u2Size)        //you reach this state when 2 Fdds does not have the same number
                {                           //of bits. What happens is then the smallest get buffered with 0's
                    for (int i = MinSize; i < u1Size; i++)
                    {
                        result = Kernel.Or(result, Kernel.Xor(new Bdd(u1.Var + i), new Bdd(false)));
                    }
                }
                else
                {
                    for (int i = MinSize; i < u2Size; i++)
                    {
                        result = Kernel.Or(result, Kernel.Xor(new Bdd(false), new Bdd(u2.Var + i)));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Equal function for FDD's
        /// </summary>
        /// <param name="u1">left side of the expression</param>
        /// <param name="u2">right side of the expression</param>
        /// <returns>a new BDD where u1 and u2 are equal</returns>
        public static Bdd Equal(Fdd u1, Fdd u2)
        {
            int u1Size = Size(u1);
            int u2Size = Size(u2);
            int MinSize = Min(u1Size, u2Size);
            Bdd result = new Bdd(true);

            for (int i = 0; i < MinSize; i++)
            {
                result = Kernel.And(result, Kernel.Equal(new Bdd(u1.Var + i), new Bdd(u2.Var + i)));
            }

            if (u1Size != u2Size)
            {
                if (u1Size > u2Size)        //you reach this state when 2 Fdds does not have the same number
                {                           //of bits. What happens is then the smallest get buffered with 0's
                    for (int i = MinSize; i < u1Size; i++)
                    {
                        result = Kernel.And(result, Kernel.Equal(new Bdd(u1.Var + i), new Bdd(false)));
                    }
                }
                else
                {
                    for (int i = MinSize; i < u2Size; i++)
                    {
                        result = Kernel.And(result, Kernel.Equal(new Bdd(false), new Bdd(u2.Var + i)));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// And function for FDD's.
        /// </summary>
        /// <param name="u1">left side of the expression</param>
        /// <param name="u2">right side of the expression</param>
        /// <returns>a new BDD where u1 and u2 are bitwise and'ed</returns>
        public static Bdd And(Fdd u1, Fdd u2)
        {
            int u1Size = Size(u1);
            int u2Size = Size(u2);
            int MinSize = Min(u1Size, u2Size);
            Bdd result = new Bdd(false);

            for (int i = 0; i < MinSize; i++)
            {
                result = Kernel.Or(result, Kernel.And(new Bdd(u1.Var + i), new Bdd(u2.Var + i)));
            }

            if (u1Size != u2Size)
            {
                if (u1Size > u2Size)        //you reach this state when 2 Fdds does not have the same number
                {                           //of bits. What happens is then the smallest get buffered with 0's
                    for (int i = MinSize; i < u1Size; i++)
                    {
                        result = Kernel.Or(result, Kernel.And(new Bdd(u1.Var + i), new Bdd(false)));
                    }
                }
                else
                {
                    for (int i = MinSize; i < u2Size; i++)
                    {
                        result = Kernel.Or(result, Kernel.And(new Bdd(false), new Bdd(u2.Var + i)));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Boolean operation and existential quantification on a list of variables.
        /// The same operation as first using Apply() and then VarListExists().
        /// </summary>
        /// <param name="pairList">The list of pairs.</param>
        /// <param name="root">The BDD to do the Exists operation on.</param>
        /// <returns>The resulting Bdd</returns>
        public static Bdd Exists(FddPairList pairList, Bdd root)
        {
            IEnumerator<KeyValuePair<int,bool>> enumerator = pairList.GetEnumeratorExists();
            enumerator.MoveNext();
            while(enumerator.Current.Key != 0)      //safe because Fdd var always starts on 1.
            {
                for(int i = 0 ; i < FddDictionary[enumerator.Current.Key]; i++)
                    root = Kernel.Exists(enumerator.Current.Key + i, root);
                enumerator.MoveNext();
            }
            return root;
        }

        /// <summary>
        /// Substitution of a list of variablepairs in an Fdd
        /// </summary>
        /// <param name="t">The BDD to apply compose on</param>
        /// <param name="pairList">The list to substitute</param>
        /// <returns></returns>
        public static Bdd Compose(Bdd t, FddPairList pairList)
        {
            foreach (KeyValuePair<int, int> pair in pairList)
            {
                for (int i = 0; i < FddDictionary[pair.Key]; i++)
                    t = Kernel.Compose(t, pair.Key+i, pair.Value+i);
            }

            return t;
            
        }


        /// <summary>
        /// Enforces maxvalues by disallowing values normally room for with the size of bits in an fdd
        /// Value is normally expected to be the same as used in the constructor of the fdd.
        /// Note: (powers of 2) -1) does not need to be constrained: 7, 255 etc.
        /// </summary>
        /// <param name="fdd">The fdd to constrain</param>
        /// <param name="value">The values to enforce</param>
        public static Bdd EnforceMaxValue(Fdd fdd, int value)
        {
            Bdd result;

            //Always allow all bits where most significant bit isn't set
            result = Kernel.Not(new Bdd(fdd.Var));

            int start = (int)Math.Pow(2,Size(fdd)-1);      //lowest number with most significant bit set

            for (int i = start; i <= value; i++)
            {
                result = Kernel.Or(result, Equal(fdd, i));
            }
            return result;
        }
        
        /// <summary>
        /// Creates a hard binding sort of like an fdd restrict.
        /// Difference is that the binding will persist no matter
        /// what changes is later added.
        /// </summary>
        /// <param name="fdd">the Fdd to create a binding on</param>
        /// <param name="value">the constant number that is assigned to the Fdd.</param>
        /// <returns></returns>
        public static Bdd Equal(Fdd fdd, int value)
        {
            if (Log2(value) > FddDictionary[fdd.Var])      //value larger than fdd can hold.
                return new Bdd(false);

            bool[] b = ToBitPattern(value, Size(fdd));
            
            Bdd result = new Bdd(true);

            for (int i = 0; i < b.Length; i++)
            {
                if (b[i])
                {
                    result = Kernel.And(result, new Bdd(fdd.Var + i));
                }
                else
                {
                    result = Kernel.And(result, Kernel.Not(new Bdd(fdd.Var + i)));
                }
            }
            return result;
        }
        /// <summary>
        /// Returns the amount of bits needed to hold a given number
        /// </summary>
        /// <param name="i">The max number to store</param>
        /// <returns></returns>
        public static int Log2(int i)
        {
            if (i == 0 || i == 1)
                return 1;
            return Convert.ToInt32(Math.Ceiling(Math.Log(i, 2)));
        }
    }
}
