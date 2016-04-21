using System;
using System.Collections.Generic;
using System.Text;
using BddSharp.Kernel;

namespace TransitionSystems
{
    class Verification
    {

        public static Bdd ReachableStates(Bdd I, Bdd T, List<int> PreState, List<int> PostState)
        {
            Bdd Rp, tmp, t;
            Bdd R = new Bdd(false);
            do
            {
                Rp = R;
                tmp = Kernel.And(T, R);
                t = Kernel.Exists(PreState, tmp);

                R = Kernel.Or(I, Kernel.Compose(t, PreState, PostState));
            }
            while (!R.Equals(Rp));
            return R;
        }

        //public Boolean HasDeadLocks(Bdd R, Bdd T, int N)
        //{
        //    Bdd C = new Bdd(true);
        //    Bdd f = new Bdd(false);

        //    for (int i = 0; i < N; i++)
        //    {
        //        C = Kernel.And(C, Kernel.Exists(T, PostState.U));
        //    }

        //    if (C != f && R != f)
        //        return false;
        //    return true;
        //}

    }
}
