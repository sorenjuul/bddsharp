using System;
using System.Collections.Generic;
using System.Text;
using BddSharp.Kernel;

namespace Examples
{
/* This program creates a transition relation for a finite state machine.
 * This transition relation is then used to find the reachable statespace
 * of the state machine. The state machine has 8 states with state 0 being
 * the initial state. The transitions form a ring:
 *
 *    0 -> 1 -> 2 -> 3 -> 4 -> 5 -> -> 7 -> 0
 */
    public class FDDStatemachine : IExampleForm
    {
        private int N = 8;
        private Fdd F;
        private Fdd Fp;
        Bdd I;
        Bdd R;
        Bdd T;
        Bdd E;
        FddPairList pairs = new FddPairList();
        private Dictionary<Bdd, String> transList;

        public FDDStatemachine()
        {
            FddKernel.Setup();
            transList = new Dictionary<Bdd, string>();

            F = new Fdd(N);
            Fp = new Fdd(N);

            BuildInitialState();
            BuildEndState();
            BuildTransitions();
            pairs.Add(F.Var,Fp.Var);        //only pair to add.
            R = ReachableStates();
        }

        private void BuildInitialState()
        {
            I = FddKernel.Equal(F, 0);
        }

        private void BuildEndState()
        {
            E = FddKernel.Equal(F, N-1);
        }


        public string Run()
        {
            int i = Tracelength();

          //  int tracelength = Transition.Tracelength(I, E, T, pairs);
            
            string path = FindTrace(I, i);
            
            //Bdd endStateReached = E >= R;

            return path;
        }

        public void BuildTransitions()
        {
            T = new Bdd(false);
            Bdd temp;

            for (int i = 0; i < N; i++)
            {
                int j = (i+1)%N;
                temp = Kernel.And(FddKernel.Equal(F, i), FddKernel.Equal(Fp, j));

                T |= temp;
                transList.Add(temp, String.Format("from {0} to {1}", i, j));
            }
        }

        public Bdd ReachableStates()
        {
            int count = 0;
            Bdd Rp, tmp, t;
            Bdd result = new Bdd(false);
            do
            {
                Rp = result;
                tmp = T & result;
                t = FddKernel.Exists(pairs, tmp);
                count++;
                //Console.WriteLine(String.Format("time: {0}, count: {1}", DateTime.Now.ToString(), count));
                result = I | FddKernel.Compose(t, pairs);
            }
            while (!result.Equals(Rp));
            return result;
        }

        private int Tracelength()
        {
            int count = 0;
            Bdd Rp, tmp, t;
            Bdd R = I;
            do
            {
                Bdd test = E >= R;
                if (test.U == Kernel.bddtrue)
                    break;
                Rp = R;
                tmp = T & R;
                t = FddKernel.Exists(pairs, tmp);

                R = I | FddKernel.Compose(t, pairs);
                count++;
                //R.Serialize(String.Format("TracelengthLoop{0}",count),40);
            }
            while (!R.Equals(Rp));
            return count;
        }

        private string FindTrace(Bdd I2, int tracelength)
        {
            Bdd tmp, t;
            Bdd R2 = I2;
            if (tracelength == 0)
                return "[]";
            else
            {
                foreach (KeyValuePair<Bdd, string> trans in transList)
                {
                    tmp = trans.Key & I2;
                    t = FddKernel.Exists(pairs, tmp);
                    R2 = FddKernel.Compose(t, pairs);
                    if (IsOnTrace(R2, tracelength - 1))
                        return " " + trans.Value + " " + FindTrace(R2, tracelength - 1);
                }
                return "Not good";
            }
        }

        private bool IsOnTrace(Bdd I2, int tracelength)
        {
            int count = 0;
            Bdd tmp, t;
            Bdd R2 = I2;
            while (count <= tracelength)
            {
                tmp = T & R2;
                t = FddKernel.Exists(pairs, tmp);

                R2 = I2 | FddKernel.Compose(t, pairs);
                count++;
            }
            Bdd test = E >= R2;
            return test.U == Kernel.bddtrue;
        }

        public string Run(bool reachableStates, bool traceLength, bool findtrace)
        {
            return Run();
        }

    }
}
