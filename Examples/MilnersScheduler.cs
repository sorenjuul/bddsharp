using System;
using System.Collections.Generic;
using System.Text;
using BddSharp.Kernel;

namespace Examples
{
    class MilnersScheduler : Examples.IExampleForm
    {
        int N;

        public MilnersScheduler(int n)
        {
            this.N = n;
        }

        public string Run(bool reachableStates, bool traceLength, bool findtrace)
        {
            Kernel.Setup();

            Bdd[] c = new Bdd[N];
            Bdd[] cp = new Bdd[N];
            Bdd[] h = new Bdd[N];
            Bdd[] hp = new Bdd[N];
            Bdd[] t = new Bdd[N];
            Bdd[] tp = new Bdd[N];
            Bdd I, T, R;

            BddPairList pairList = new BddPairList();

            for (int n = 0; n < N*3; n++)
            {
                pairList.Add(n * 2, n * 2 + 1);
            }

            for (int n = 0; n < N; n++)
            {
                c[n] = new Bdd(n*6);
                cp[n] = new Bdd(n*6 + 1);
                t[n] = new Bdd(n*6 + 2);
                tp[n] = new Bdd(n*6 + 3);
                h[n] = new Bdd(n*6 + 4);
                hp[n] = new Bdd(n*6 + 5);
            }

            I = InitialState(t, h, c);
            T = Transitions(t, tp, h, hp, c, cp);
            string answer = string.Empty;
           
            if (reachableStates)
            {
                R = Transition.ReachableStates(I, T, pairList);
                answer += "ReachableStates: " + Kernel.SatCountVarSet(R) + Environment.NewLine;
            }
            return answer;
            
        }

        public Bdd InitialState(Bdd[] t, Bdd[] h, Bdd[] c)
        {
            Bdd I = c[0] & !h[0] & !t[0];

            for (int i = 1; i < N; i++)
            {
                I &= !c[i] & !h[i] & !t[i];
            }
            return I;
        }

        public Bdd Assign(Bdd[] x, Bdd[] xp, int n)
        {
            Bdd result = new Bdd(true);
            for (int i = 0; i < N; i++)
            {
                if (i != n)
                {
                    result &= x[i] == xp[i];
                }
            }
            return result;
        }

        public Bdd Transitions(Bdd[] t, Bdd[] tp, Bdd[] h, Bdd[] hp, Bdd[] c, Bdd[] cp)
        {
            Bdd P, E;
            Bdd T = new Bdd(false);

            for (int i = 0; i < N; i++)
            {
                P = ((c[i] > cp[i]) & (tp[i] > t[i]) & hp[i] & Assign(c, cp, i)
                    & Assign(t, tp, i) & Assign(h, hp, i))
                    | ((h[i] > hp[i]) & cp[(i + 1) % N]
                    & Assign(c, cp, (i + 1) % N) & Assign(h, hp, i) & Assign(t, tp, N));

                E = t[i] & !tp[i] & Assign(t, tp, i) & Assign(c, cp, N) & Assign(h, hp, N);

                T |= P | E;
            }
            return T;
        }
    }
}
