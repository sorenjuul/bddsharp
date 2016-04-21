using System;
using System.Collections.Generic;
using System.Text;
using BddSharp.Kernel;

namespace Examples
{
    class TowersOfHanoi : Examples.IExampleForm
    {
        int N;
        Bdd I, E, T;
        Bdd[] l, lp, m, mp, r, rp;
        Dictionary<Bdd, string> transList;
        BddPairList pairList;
        public TowersOfHanoi(int n)
        {
            N = n;
            l = new Bdd[N];
            lp = new Bdd[N];
            m = new Bdd[N];
            mp = new Bdd[N];
            r = new Bdd[N];
            rp = new Bdd[N];
            transList = new Dictionary<Bdd, string>();
            pairList = new BddPairList();
        }

        public string Run(bool reachableStates, bool traceLength, bool findtrace)
        {
            Kernel.Setup();
            for (int i = 0; i < N; i++)
            {
                l[i] = new Bdd(i * 6);
                lp[i] = new Bdd(i * 6 + 1);
                m[i] = new Bdd(i * 6 + 2);
                mp[i] = new Bdd(i * 6 + 3);
                r[i] = new Bdd(i * 6 + 4);
                rp[i] = new Bdd(i * 6 + 5);
            }
            for (int i = 0; i < N * 3; i++)
            {
                pairList.Add(i * 2, i * 2 + 1);
            }
            I = InitialState();
            E = EndState();
            T = Transistions();
            string answer = string.Empty;
            if (reachableStates)
            {
                Bdd R = Transition.ReachableStates(I, T, pairList);
                Bdd result = E >= R;
                answer += "ReachableStates: " + Kernel.SatCountVarSet(R) + Environment.NewLine;
                answer += "E >= R: " + result.U + Environment.NewLine;
            }

            int length = Transition.Tracelength(I, E, T, pairList);
            if (traceLength)
            {
                answer += "Tracelength: " + length + Environment.NewLine;
            }
            
            if (findtrace)
            {
                string trace;
                if(traceLength)
                    trace = Transition.FindTrace(I, E, T, transList, pairList, length);
                else
                    trace = Transition.FindTrace(I, E, T, transList, pairList, Transition.Tracelength(I, E, T, pairList));
                answer += "Trace: " + trace;
            }
            
            return answer;
                
        }

        private Bdd InitialState()
        {
            Bdd I = new Bdd(true);
            for (int i = 0; i < N; i++)
            {
                I &= l[i];
            }
            for (int i = 0; i < N; i++)
            {
                I &= !m[i] & !r[i];
            }
            return I;
        }

        private Bdd EndState()
        {
            Bdd E = new Bdd(true);
            for (int i = 0; i < N; i++)
            {
                E &= r[i];
            }
            for (int i = 0; i < N; i++)
            {
                E &= !m[i] & !l[i];
            }
            return E;
        }

        private Bdd Transistions()
        {
            Bdd T = new Bdd(false);
            Bdd LTM = MoveBricks(l, lp, m, mp, r, rp, "LTM");
            Bdd LTR = MoveBricks(l, lp, r, rp, m, mp, "LTR");
            Bdd MTL = MoveBricks(m, mp, l, lp, r, rp, "MTL");
            Bdd MTR = MoveBricks(m, mp, r, rp, l, lp, "MTR");
            Bdd RTM = MoveBricks(r, rp, m, mp, l, lp, "RTM");
            Bdd RTL = MoveBricks(r, rp, l, lp, m, mp, "RTL");
            T = LTM | LTR | MTL | MTR | RTM | RTL;
            return T;
        }

        private Bdd MoveBricks(Bdd[] s, Bdd[] sp, Bdd[] e, Bdd [] ep, Bdd[] o, Bdd[] op, string move)
        {
            Bdd M = new Bdd(false);
            for (int i = 0; i < N; i++)
            {
                Bdd rule = new Bdd(true);
                for (int j = 0; j < i; j++)
                {
                    rule &= !s[j] & !e[j];
                }
                
                Bdd temp = s[i] & !sp[i] & ep[i] & Assign(s, sp, i) & Assign(e, ep, i) & Assign(o, op, N);
                rule &= temp;
                transList.Add(rule, "Brick " + (i+1) + " " + move + " - ");
                M |= rule;
            }
            return M;
        }

        private Bdd Assign(Bdd[] x, Bdd[] xp, int n)
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
    }
}

