using System;
using System.Collections.Generic;
using System.Text;
using BddSharp.Serializer;
using BddSharp.Kernel;


namespace TTest
{
    public struct ListEntry
    {
        public int i1;
        public int i2;
        public int i3;
        public int i4;
        public ListEntry(int i1, int i2, int i3, int i4)
        {
            this.i1 = i1;
            this.i2 = i2;
            this.i3 = i3;
            this.i4 = i4;
        }
    }

    public class ListEntryPointer
    {
        public ListEntry[] entry;
        public ListEntryPointer next = null;

        public ListEntryPointer(int size)
        {
            this.entry = new ListEntry[size];
        }
    }


    public struct IntPair
    {
        public int u1;
        public int u2;

        public IntPair(int u1, int u2)
        {
            this.u1 = u1;
            this.u2 = u2;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //CheckMemory2();
            //SudokuLoop();
            //CheckNewBddHash();
            //BddStressTest(4000000);
            //test();
            //FddTest();
            //CheckRestrict();
            //TEntryTest();
            //CheckT();
            //EnforceTest();
            Hello();
        }

        static public void Hello()
        {
            Knight k = new Knight();
            k.Solve();
        }

        static int Abs(int value)
        {
            if (value == int.MinValue)
                throw new OverflowException(String.Format("Cannot return Abs of {0}", int.MinValue));

            return value < 0 ? value * -1 : value;
        }

        static public void FddTest()
        {
            //bool[] B = FddKernel.ToBitPattern(10);
        }

        static public ulong GenerateKey(int i, int j)
        {
            return ((((ulong)i + (ulong)j) * ((ulong)i + (ulong)j + 1)) / 2 + (ulong)i) % 15485863;
        }


        static void test()
        {
            FddKernel.Setup();
            Fdd[] X = new Fdd[3];
            for (int i = 0; i < 3; i++)
            {
                X[i] = new Fdd(8);
            }
            
            Bdd result = new Bdd(true);

            DateTime startSetup = DateTime.Now;

            //for(int i=0;i<8;i++)
            //    for(int j=i+1;j<9;j++)
            //        result = Kernel.And(result, FddKernel.Xor(X[i], X[j]));

            result = Kernel.And(result, FddKernel.Xor(X[0], X[1]));
            result = Kernel.And(result, FddKernel.Xor(X[0], X[2]));
            //result = Kernel.And(result, FddKernel.Xor(X[0], X[3]));
            //result = Kernel.And(result, FddKernel.Xor(X[0], X[4]));
            //result = Kernel.And(result, FddKernel.Xor(X[0], X[5]));
            //result = Kernel.And(result, FddKernel.Xor(X[0], X[6]));
            //result = Kernel.And(result, FddKernel.Xor(X[0], X[7]));
            //result = Kernel.And(result, FddKernel.Xor(X[0], X[8]));

            result = Kernel.And(result, FddKernel.Xor(X[1], X[2]));
            //result = Kernel.And(result, FddKernel.Xor(X[1], X[3]));
            //result = Kernel.And(result, FddKernel.Xor(X[1], X[4]));
            //result = Kernel.And(result, FddKernel.Xor(X[1], X[5]));
            //result = Kernel.And(result, FddKernel.Xor(X[1], X[6]));
            //result = Kernel.And(result, FddKernel.Xor(X[1], X[7]));
            //result = Kernel.And(result, FddKernel.Xor(X[1], X[8]));

            //result = Kernel.And(result, FddKernel.Xor(X[2], X[3]));
            //result = Kernel.And(result, FddKernel.Xor(X[2], X[4]));
            //result = Kernel.And(result, FddKernel.Xor(X[2], X[5]));
            //result = Kernel.And(result, FddKernel.Xor(X[2], X[6]));
            //result = Kernel.And(result, FddKernel.Xor(X[2], X[7]));
            //result = Kernel.And(result, FddKernel.Xor(X[2], X[8]));

            //result = Kernel.And(result, FddKernel.Xor(X[3], X[4]));
            //result = Kernel.And(result, FddKernel.Xor(X[3], X[5]));
            //result = Kernel.And(result, FddKernel.Xor(X[3], X[6]));
            //result = Kernel.And(result, FddKernel.Xor(X[3], X[7]));
            //result = Kernel.And(result, FddKernel.Xor(X[3], X[8]));

            //result = Kernel.And(result, FddKernel.Xor(X[4], X[5]));
            //result = Kernel.And(result, FddKernel.Xor(X[4], X[6]));
            //result = Kernel.And(result, FddKernel.Xor(X[4], X[7]));
            //result = Kernel.And(result, FddKernel.Xor(X[4], X[8]));

            //result = Kernel.And(result, FddKernel.Xor(X[5], X[6]));
            //result = Kernel.And(result, FddKernel.Xor(X[5], X[7]));
            //result = Kernel.And(result, FddKernel.Xor(X[5], X[8]));

            //result = Kernel.And(result, FddKernel.Xor(X[6], X[7]));
            //result = Kernel.And(result, FddKernel.Xor(X[6], X[8]));

            //result = Kernel.And(result, FddKernel.Xor(X[7], X[8]));

            DateTime finishSetup = DateTime.Now;
            TimeSpan timeSetup = finishSetup - startSetup;

            //result = FddKernel.Restrict(result, X[1], 0);
            result = Kernel.And(result, FddKernel.Equal(X[1], 6));

            int[] foo = FddKernel.AnySat(result, 8);        //returns 0,1,2,3
            
            //BddSerializer.Serialize(result, "foo");

        }

        public static void SudokuLoop()
        {
            int count = 0;
            for (int i = 0; i < 9; i++)               //line
                for (int j = 0; j < 8; j++)           //left operand
                    for (int k = j+1; k < 9; k++)     //right operand
                    {
                        Console.WriteLine(String.Format("Line: {0} Left: {1} Right: {2}", i, j, k));
                        count++;
                    }
        }

        public static void CheckNewBddHash()
        {
            BDDHash hash = new BDDHash();
            for (int i = 0; i < 1000; i++)
            {
                hash.Add(i, i, i, i);
            }

            for (int j = 0; j < 1000; j++)
            {
                Console.WriteLine(hash[j, j, j].ToString());
            }
        }

        public static void CheckMemory()
        {
            //HEntry[][] Clusters;
            //Clusters = new HEntry[50][];

            //for(int i = 0; i < 50 ; i++)
            //    Clusters[i] = new HEntry[0x1000000];
        }

        public static void CheckMemory2()
        {
            int size = 0x100000;
            ListEntryPointer first = new ListEntryPointer(size);
            int i = 1;
            ListEntryPointer last = first;

            while (true)
            {
                last.next = new ListEntryPointer(size);
                i++;
                last = last.next;
                for (int j = 0; j < size; j++)
                {
                    last.entry[j] = new ListEntry(j,j,j,j);
                }
                size *= 2;
            }
        }

        public static void CheckT()
        {
            BddTable table = new BddTable(0x200000);
            for (int i = 2; i < int.MaxValue; i++)
            {
                table.Add(i,i,i,i);
            }
        }

        public static void CheckRestrict()
        {
            Kernel.Setup();
            Bdd a = new Bdd(1);
            Bdd b = new Bdd(2);
            Bdd c = new Bdd(3);

            Bdd result = new Bdd(false);

            result = Kernel.Or(Kernel.Equal(a, b), c);

            string s1 = Kernel.AnySat(result);

            result = Kernel.Restrict(result, b.Var, true);

            string s2 = Kernel.AnySat(result);

            BddSerializer.Serialize(result, "foo");
        }

        public static void BddStressTest(int n)
        {
            Kernel.Setup();
            Bdd[] X = new Bdd[n];
            Bdd result = new Bdd(true);

            for (int i = 0; i < n; i++)
            {
                X[i] = new Bdd(i);
            }

            for (int i = 1; i < n; i++)
            {
                result = Kernel.Or(result, Kernel.Equal(X[i-1],X[i]));
            }
        }

        public static void TEntryTest()
        {
            TEntry entry = new TEntry(1, 2, 3, 4);

            entry.SetMark(true);
            entry.SetMark(false);
            entry.SetMark(true);
            entry.SetMark(true);


        }


        public static void conflict()
        {
            Dictionary<ulong, IntPair> D = new Dictionary<ulong, IntPair>();
            int conflicts = 0;

            for (int i = 0; i < 10000; i++)
            {
                for (int j = 0; j < 10000; j++)
                {
                    IntPair pair = new IntPair(i, j);
                    ulong key = GenerateKey(i, j);
                    if (D.ContainsKey(key))
                    {
                        System.Console.WriteLine(string.Format("A conflict was reached for i:{0}, j:{1}. Conflict was on i:{2}, j:{3}.", i, j, D[key].u1, D[key].u2));
                        conflicts++;
                    }
                    else
                    {
                        D.Add(key, pair);
                    }
                }
            }
            System.Console.WriteLine("Number of conflicts:{0}.",conflicts);

        }

        public static void EnforceTest()
        {
            Kernel.Setup();
            Bdd result;
            Fdd fdd = new Fdd(6);

            result = FddKernel.EnforceMaxValue(fdd, 6);

            BddSerializer.Serialize(result, "TTest");
        }


        //0  |18 |             row1
        // 8 |   |             row2
        //  2|   |             row3
        //--- --- ---
        public static void EnforceTest2()
        {
            FddKernel.Setup();
            Fdd[] X = new Fdd[27];
            for (int i = 0; i < 27; i++)
            {
                X[i] = new Fdd(8);
            }

            Bdd result = new Bdd(true);

            //row1 restricts
            result = Kernel.And(result, FddKernel.Equal(X[0], 0));
            result = Kernel.And(result, FddKernel.Equal(X[3], 1));
            result = Kernel.And(result, FddKernel.Equal(X[4], 8));

            //grid 1 restricts (except row1)
            result = Kernel.And(result, FddKernel.Equal(X[10], 8));
            result = Kernel.And(result, FddKernel.Equal(X[20], 2));

            //row1 maxvalues restricts
            result = Kernel.Or(result, FddKernel.EnforceMaxValue(X[1], 8));
            result = Kernel.Or(result, FddKernel.EnforceMaxValue(X[2], 8));


            //row1
            for(int i=0;i<8;i++)
                for(int j=i+1;j<9;j++)
                    result = Kernel.And(result, FddKernel.Xor(X[i], X[j]));


            //small grid 1
            result = Kernel.And(result, FddKernel.Xor(X[0], X[9]));
            result = Kernel.And(result, FddKernel.Xor(X[0], X[10]));
            result = Kernel.And(result, FddKernel.Xor(X[0], X[11]));
            result = Kernel.And(result, FddKernel.Xor(X[0], X[18]));
            result = Kernel.And(result, FddKernel.Xor(X[0], X[19]));
            result = Kernel.And(result, FddKernel.Xor(X[0], X[20]));
            result = Kernel.And(result, FddKernel.Xor(X[1], X[9]));
            result = Kernel.And(result, FddKernel.Xor(X[1], X[10]));
            result = Kernel.And(result, FddKernel.Xor(X[1], X[11]));
            result = Kernel.And(result, FddKernel.Xor(X[1], X[18]));
            result = Kernel.And(result, FddKernel.Xor(X[1], X[19]));
            result = Kernel.And(result, FddKernel.Xor(X[1], X[20]));
            result = Kernel.And(result, FddKernel.Xor(X[2], X[9]));
            result = Kernel.And(result, FddKernel.Xor(X[2], X[10]));
            result = Kernel.And(result, FddKernel.Xor(X[2], X[11]));
            result = Kernel.And(result, FddKernel.Xor(X[2], X[18]));
            result = Kernel.And(result, FddKernel.Xor(X[2], X[19]));
            result = Kernel.And(result, FddKernel.Xor(X[2], X[20]));

            //row2
            //for (int i = 9; i < 17; i++)
            //    for (int j = i; j < 18; j++)
            //        result = Kernel.And(result, FddKernel.Xor(X[i], X[j]));

            ////row3
            //for (int i = 18; i < 26; i++)
            //    for (int j = i; j < 27; j++)
            //        result = Kernel.And(result, FddKernel.Xor(X[i], X[j]));
        
        }
    }
}
