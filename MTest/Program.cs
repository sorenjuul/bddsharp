using System;
using System.Collections.Generic;
using System.Text;
using BddSharp.Kernel;
using BDD = BddSharp.Kernel.Kernel;
using BddSharp.Serializer;

namespace MTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            BDDHash ht = new BDDHash();
            BddNode b = new BddNode(3, 1, 0);
            BddNode b1 = new BddNode(2, 1, 1);
            BddNode b2 = new BddNode(16, 0, 0);
            int key = ht.GenerateKey(b);
            int key1 = ht.GenerateKey(b1);
            int key2 = ht.GenerateKey(b2);
            Console.WriteLine(key.ToString());

            Console.WriteLine(ht.count.ToString()); // test count for Add

            ht.Add(key, b); // test Add
            ht.Add(key1, b1); // test Add
            ht.Add(key2, b2); // test Add

            Console.WriteLine(ht.count.ToString()); // test count for after Add
            ht.Clear();
            Console.WriteLine(ht.count.ToString()); // test for Clear()
             * */

            Bdd result = new Bdd(true);

            BDD.Setup();
            {
                Bdd a = new Bdd(1);
                Bdd b = new Bdd(2);
                Bdd c = new Bdd(3);
                Bdd d = new Bdd(4);
                Bdd e;

                e = BDD.Equal(a, b);
                Console.WriteLine(BDD.TCount().ToString());
                c = BDD.Equal(c, d);
                Console.WriteLine(BDD.TCount().ToString());
                result = BDD.And(e, c);
                result = BDD.Or(result, e);
            }
            Console.WriteLine(BDD.TCount().ToString());

            BddSerializer.Serialize(result, "foo");


        }
    }
}
