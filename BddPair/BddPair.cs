using System;
using System.Collections.Generic;
using System.Text;
using BDD = BddSharp.Kernel.Kernel;
using BddSharp.Kernel;

namespace BddPair
{
    public class BddPair
    {
        public Bdd data;
        public int id;
        public int last;
        public BddPair next;

        public BddPair(Bdd Data, int Id, int Last, BddPair Next)
        {
            this.data = Data;
            this.id = Id;
            this.last = Last;
            this.next = Next;
        }
    }
}
