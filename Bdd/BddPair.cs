using System;
using System.Collections.Generic;
using System.Text;


namespace BddSharp.Kernel
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
