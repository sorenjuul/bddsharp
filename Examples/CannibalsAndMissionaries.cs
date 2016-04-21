using System;
using System.Collections.Generic;
using System.Text;
using BddSharp.Kernel;

namespace Examples
{
    class CannibalsAndMissionaries : Examples.IExampleForm
    {
        private int N; // number of missionaries and cannibals put together
        private Dictionary<Bdd, string> transList = new Dictionary<Bdd, string>();

        public CannibalsAndMissionaries() 
        {
            this.N = 3;
        }

        public string Run(bool fake1, bool fake2, bool fake3)
        {
            Kernel.Setup();
            Bdd I, E, T, R;

            Bdd[] ml = new Bdd[N];  // missionaries on left side
            Bdd[] mr = new Bdd[N];  // missionaries on right side

            Bdd[] mlp = new Bdd[N]; // missionaries on left'
            Bdd[] mrp = new Bdd[N];

            Bdd[] cl = new Bdd[N];  // cannibles on left side
            Bdd[] cr = new Bdd[N];  // cannibles on right side
            
            Bdd[] clp = new Bdd[N];
            Bdd[] crp = new Bdd[N];
            
            Bdd br, brp, bl, blp; // the boat on right, the boat on right', the boat one the left, etc.

            BddPairList pairList = new BddPairList();

            // *2 because we have both missionaries and cannibals
            // *2 because they all have two variables
            for (int i = 0; i < N*2*2; i++)
            {
                pairList.Add(i * 2, i * 2 + 1);
            }

            for (int i = 0; i < N; i++)
            {
                ml[i] = new Bdd(i * 8);
                mlp[i] = new Bdd(i * 8 + 1);
                
                mr[i] = new Bdd(i * 8 + 2);
                mrp[i] = new Bdd(i * 8 + 3);
                
                cl[i] = new Bdd(i * 8 + 4);
                clp[i] = new Bdd(i * 8 + 5);
                
                cr[i] = new Bdd(i * 8 + 6);
                crp[i] = new Bdd(i * 8 + 7);
            }

            bl = new Bdd(N * 8);
            blp = new Bdd(N * 8 + 1);
            pairList.Add(N * 8, N * 8 + 1);
            br = new Bdd(N * 8 + 2);
            brp = new Bdd(N * 8 + 3);
            pairList.Add(N * 8 + 2, N * 8 + 3);



            I = InitialState(mr, cr, ml, cl, bl, br);
            E = EndState(mr, cr, ml, cl, bl, br);
            T = Transitions(mr, cr, ml, cl, mrp, crp, mlp, clp, bl, blp, br, brp);
            R = Transition.ReachableStates(I, T, pairList);
            
            Bdd result = E >= R;
            int tracelength = Transition.Tracelength(I, E, T, pairList);
            string trace = Transition.FindTrace(I, E, T, transList, pairList, tracelength);
            return "R=>E-satcount: " + Kernel.SatCountVarSet(result).ToString() +
                Environment.NewLine +
                "R-satcount: " + Kernel.SatCountVarSet(R).ToString() +
                Environment.NewLine +
                "Solution: " + trace +
                Environment.NewLine +
                "Tracelength: " + tracelength;
        }


        // 3 missionaries on left side and 3 cannibals on left side
        private Bdd InitialState(Bdd[] mr, Bdd[] cr, Bdd[] ml, Bdd[] cl, Bdd bl, Bdd br)
        {
            Bdd I = new Bdd(true);

            for (int i = 0; i < N; i++)
            {
                I &= ml[i] & cl[i] & !mr[i] & !cr[i]; 
            }
            I &= bl & !br;

            return I;
        }

        // 3 missionaries on right side and 3 cannibals on right side
        private Bdd EndState(Bdd[] mr, Bdd[] cr, Bdd[] ml, Bdd[] cl, Bdd bl, Bdd br)
        {
            Bdd E = new Bdd(true);

            for (int i = 0; i < N; i++)
            {
                E &= !ml[i] & !cl[i] & mr[i] & cr[i];
            }
            E &= !bl & br;

            return E;
        }

        private Bdd Transitions(Bdd[] mr, Bdd[] cr, Bdd[] ml, Bdd[] cl, Bdd[] mrp, Bdd[] crp, Bdd[] mlp, Bdd[] clp, 
                                Bdd bl, Bdd blp, Bdd br, Bdd brp)
        {
            Bdd LTR = ShoreRule(ml, cl, mr, cr, mlp, clp, mrp, crp, bl, blp, brp, "l");
            Bdd RTL = ShoreRule(mr, cr, ml, cl, mrp, crp, mlp, clp, br, brp, blp, "r");

            Bdd T = LTR | RTL;
            return T;
        }

        private Bdd ShoreRule(Bdd[] ms, Bdd[] cs, Bdd[] me, Bdd[] ce, Bdd[] msp, Bdd[] csp, Bdd[] mep, Bdd[] cep, 
                                Bdd bs, Bdd bsp, Bdd bep, string side)
        {
            Bdd rule = new Bdd (false);
            Bdd tempTrans;
            // three missionaries on a shore
            Bdd ThM = ThreePersonShore(ms);
            // two misionaries on a shore
            Bdd TwM = TwoPersonShore(ms);
            // one missinary on a shore;
            Bdd OM = OnePersonShore(ms);
            // no missionary on a shore
            Bdd NM = NoPersonShore(ms);

            // three cannibals on a shore
            Bdd ThC = ThreePersonShore(cs);
            // two cannibals on a shore
            Bdd TwC = TwoPersonShore(cs);
            // one cannibals on a shore
            Bdd OC = OnePersonShore(cs);
            // no cannibal on a shore
            Bdd NC = NoPersonShore(cs);

            // boat ready
            Bdd B = bs;

            // move one missionary
            Bdd MOM = MoveOne(ms, msp, me, mep, cs, csp, ce, cep, bs, bsp, bep);
            // move one cannibal
            Bdd MOC = MoveOne(cs, csp, ce, cep, ms, msp, me, mep, bs, bsp, bep);
            // move two missionaries
            Bdd MTM = MoveTwo(ms, msp, me, mep, cs, csp, ce, cep, bs, bsp, bep);
            // move two cannibals
            Bdd MTC = MoveTwo(cs, csp, ce, cep, ms, msp, me, mep, bs, bsp, bep);
            // move one of each
            Bdd MOOE = MoveOneOfEach(ms, msp, me, mep, cs, csp, ce, cep, bs, bsp, bep);

            // m=3 c=3
            //tempTrans = Kernel.IfThen(ThM & ThC & B, MOC | MTC | MOOE);
            tempTrans = ThM & ThC & B & (MOC | MTC | MOOE);
            transList.Add(tempTrans, "33" + side);
            rule |= tempTrans;
            // m=3 c=2
            //tempTrans = Kernel.IfThen(ThM & TwC & B, MOM | MOC | MTC);
            tempTrans = ThM & TwC & B & (MOM | MOC | MTC);
            transList.Add(tempTrans, "32" + side);
            rule |= tempTrans;
            // m=3 c=1
            //tempTrans = Kernel.IfThen(ThM & OC & B, MOC | MTM);
            tempTrans = ThM & OC & B & (MOC | MTM);
            transList.Add(tempTrans, "31" + side);
            rule |= tempTrans;
            // m=2 c=2
            //tempTrans = Kernel.IfThen(TwM & TwC & B, MTM | MOOE);
            tempTrans = TwM & TwC & B & (MTM | MOOE);
            transList.Add(tempTrans, "22" + side);
            rule |= tempTrans;
            // m=1 c=1
            //tempTrans = Kernel.IfThen(OM & OC & B, MOM | MOOE);
            tempTrans = OM & OC & B & (MOM | MOOE);
            transList.Add(tempTrans, "11" + side);
            rule |= tempTrans;
            // m=0 c=3
            //tempTrans = Kernel.IfThen(NM & ThC & B, MOC | MTC);
            tempTrans = NM & ThC & B & (MOC | MTC);
            transList.Add(tempTrans, "03" + side);
            rule |= tempTrans;
            // m=0 c=2
            //tempTrans = Kernel.IfThen(NM & TwC & B, MOC | MTC);
            tempTrans = NM & TwC & B & (MOC | MTC);
            transList.Add(tempTrans, "02" + side);
            rule |= tempTrans;
            // m=0 c=1
            //tempTrans = Kernel.IfThen(NM & OC & B, MOC);
            tempTrans = NM & OC & B & MOC;
            transList.Add(tempTrans, "01" + side);
            rule |= tempTrans;

            return rule;
        }

        private Bdd MoveOne(Bdd[] ds, Bdd[] dsp, Bdd[] de, Bdd[] dep, Bdd[] ss, Bdd[] ssp, Bdd[] se, Bdd[] sep,
            Bdd bs, Bdd bsp, Bdd bep)
        {
            Bdd M = new Bdd(false);

            // Move boat to other side
            Bdd MB = MoveBoat(bs, bsp, bep);

            // one missionary to other side
            for (int i = 0; i < N; i++)
            {
                M |= ds[i] & !dsp[i] & dep[i] & MB & Assign(ds, dsp, i) & Assign(de, dep, i) &
                    Assign(ss, ssp, N) & Assign(se, sep, N);
            }
            return M;
        }

        private Bdd MoveTwo(Bdd[] ds, Bdd[] dsp, Bdd[] de, Bdd[] dep, Bdd[] ss, Bdd[] ssp, Bdd[] se, Bdd[] sep,
            Bdd bs, Bdd bsp, Bdd bep)
        {
            Bdd M = new Bdd(false);

            // Move boat to other side
            Bdd MB = MoveBoat(bs, bsp, bep);

            // two missionaries move
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (i != j)
                    {
                        M = M | ds[i] & ds[j] & !dsp[i] & !dsp[j]
                               & dep[i] & dep[j]
                               & MB
                               & Assign(ds, dsp, i, j) & Assign(de, dep, i, j)
                               & Assign(ss, ssp, N) & Assign(se, sep, N);
                    }
                }
            }
            return M;
        }

        private Bdd MoveOneOfEach(Bdd[] ms, Bdd[] msp, Bdd[] me, Bdd[] mep, Bdd[] cs, Bdd[] csp, Bdd[] ce, Bdd[] cep,
            Bdd bs, Bdd bsp, Bdd bep)
        {
            Bdd M = new Bdd(false);

            // Move boat to other side
            Bdd MB = MoveBoat(bs, bsp, bep);

            // one of each move
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {  
                    M |= cs[i] & ms[j] & !csp[i] & !msp[j]
                        & cep[i] & mep[j]
                        & MB
                        & Assign(ms, msp, j) & Assign(cs, csp, i)
                        & Assign(me, mep, j) & Assign(ce, cep, i);
                }
            }
            return M;
        }

        // move boat to other side
        private Bdd MoveBoat(Bdd bs, Bdd bsp, Bdd bep)
        {
            Bdd MB = bs & !bsp & bep;
            return MB;
        }

        private Bdd NoPersonShore(Bdd[] p)
        {
            Bdd NPS = new Bdd(true);
            for (int i = 0; i < N; i++)
            {
                NPS &= !p[i];
            }
            
            return NPS;
        }

        private Bdd OnePersonShore(Bdd[] p)
        {
            Bdd OPS = new Bdd(false);
            for (int i = 0; i < N; i++)
            {
                OPS |= p[i % N] & !p[(i + 1) % N] & !p[(i + 2) % N];
            }

            return OPS;
        }

        private Bdd TwoPersonShore(Bdd[] p)
        {
            Bdd TwPS = new Bdd(false);
            for (int i = 0; i < N; i++)
            {
                TwPS |= p[i % N] & p[(i+1) % N] & !p[(i+2) % N];
            }

            return TwPS;
        }

        private Bdd ThreePersonShore(Bdd[] p)
        {
            Bdd ThPS = new Bdd(true);
            for (int i = 0; i < N; i++)
            {
                ThPS &= p[i];
            }
            
            return ThPS;
        }

        // let all variables stay the same except for n
        private Bdd Assign(Bdd[] x, Bdd[] xp, int n)
        {
            Bdd result = new Bdd(true);
            for (int i = 0; i < N; i++)
            {
                if (i != n)
                {
                    result = result & (x[i] == xp[i]);
                }
            }
            return result;
        }

        // let all variables stay the same except for n and m
        private Bdd Assign(Bdd[] x, Bdd[] xp, int n, int m)
        {
            Bdd result = new Bdd(true);
            for (int i = 0; i < N; i++)
            {
                if (i != n && i != m)
                {
                    result = result & (x[i] == xp[i]);
                }
            }
            return result;
        }
    }
}
