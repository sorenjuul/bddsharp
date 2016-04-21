using System;
using System.Collections.Generic;
using System.Text;
using BddSharp.Kernel;
using BddSharp.Serializer;

namespace TransitionSystems
{
    class MilnersScheduler
    {
        int N;

        public MilnersScheduler(int n)
        {
            this.N = n;
        }

        public string Run()
        {
            Bdd[] c = new Bdd[1000];
            Bdd[] cp = new Bdd[1000];
            Bdd[] h = new Bdd[1000];
            Bdd[] hp = new Bdd[1000];
            Bdd[] t = new Bdd[1000];
            Bdd[] tp = new Bdd[1000];
            Bdd I, T, R;

            Kernel.Setup();

            List<int> PreState = new List<int>();
            List<int> PostState = new List<int>();

            for (int n = 1; n < N; n++)
            {
                PreState.Add(n);
                PostState.Add(n + 1);
            }


            for (int n = 0; n < N; n++)
            {
                c[n] = new Bdd(n);
                cp[n] = new Bdd(n + 1);
                t[n] = new Bdd(n + 2);
                tp[n] = new Bdd(n + 3);
                h[n] = new Bdd(n + 4);
                hp[n] = new Bdd(n + 5);
            }

            I = InitialState(t, h, c);
            T = Transitions(t, tp, h, hp, c, cp);
            R = Verification.ReachableStates(I, T, PreState, PostState);
            BddSerializer.Serialize(R, "output");

            //Kernel.Done();
            return Kernel.SatCount(R).ToString();
        }

        public Bdd InitialState(Bdd[] t, Bdd[] h, Bdd[] c)
        {
            Bdd I = Kernel.And(Kernel.Not(t[0]), Kernel.And(c[0], Kernel.Not(h[0])));

            for (int i = 1; i < N; i++)
            {
                I = Kernel.And(I, Kernel.And(Kernel.And(Kernel.Not(c[i]), Kernel.Not(h[i])), Kernel.Not(t[i])));
            }
            return I;
        }

        public Bdd Assign(Bdd[] x, Bdd[] xp, int n)
        {
            Bdd Result = new Bdd(true);
            for (int i = 0; i < N; i++)
            {
                if (i != n)
                {
                    Result = Kernel.And(Result, Kernel.Equal(x[i], xp[i]));
                }
            }
            return Result;
        }

        public Bdd Transitions(Bdd[] t, Bdd[] tp, Bdd[] h, Bdd[] hp, Bdd[] c, Bdd[] cp)
        {
            Bdd P, E2, E;
            Bdd T = new Bdd(false);

            for (int i = 0; i < N; i++)
            {
                //P = ((c[i] > cp[i]) & (tp[i] > t[i]) & hp[i] & A(c, cp, i)
                // & A(t, tp, i) & A(h, hp, i))
                // | ((h[i] > hp[i]) & cp[(i + 1) % N] & A(c, cp, (i + 1) % N) & A(h, hp, i)
                // & A(t, tp, N));

                P = Kernel.Or(Kernel.And(Kernel.And(Kernel.And(Kernel.And
                    (Kernel.And(Kernel.Greater(c[i], cp[i]), Kernel.Greater(tp[i], t[i])),
                    hp[i]), Assign(c, cp, i)), Assign(t, tp, i)), Assign(h, hp, i)),
                    Kernel.And(Kernel.And(Kernel.And(Kernel.And(Kernel.Greater(h[i], hp[i]),
                    cp[(i + 1) % N]), Assign(c, cp, (i + 1) % N)), Assign(h, hp, i)), Assign(t, tp, N)));

                //P2 = Kernel.Or(Kernel.And(Kernel.And(Kernel.And(Kernel.And(
                //    Kernel.And(Kernel.And(Kernel.And(c[i], Kernel.Not(t[i])), tp[i]), Kernel.Not(cp[i])), hp[i]), 
                //    Assign(c, cp, i)), Assign(t, tp, i)), Assign(h, hp, i)),
                //    Kernel.And(Kernel.And(Kernel.And(Kernel.And(Kernel.And(h[i], Kernel.Not(hp[i])), cp[(i % N) + 1]),
                //    Assign(c, cp, (i % N) + 1)), Assign(h, hp, i)), Assign(t, tp, N))); 

                //Denne E er implementeret efter forskrifterne i noterne
                //E = Kernel.And(Kernel.And(t[i], Kernel.Not(tp[i])), Assign(t, tp, i));

                //Denne E er implementeret ligesom i Milners.cxx i BuDDy
                E2 = Kernel.And(Kernel.And(Kernel.And(Kernel.And(t[i], Kernel.Not(tp[i])),
                    Assign(t, tp, i)), Assign(c, cp, N)), Assign(h, hp, N));

                T = Kernel.Or(T, Kernel.Or(P, E2));
            }
            return T;
        }


    }
}
