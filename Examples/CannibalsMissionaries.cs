using System;
using System.Collections.Generic;
using System.Text;
using BddSharp.Kernel;
using BddSharp.Serializer;

namespace TransitionSystems
{
    class CannibalsMissionaries
    {
        private int N, K, count = 0; // number of missionaries and cannibals put together
        
        public CannibalsMissionaries(int n) 
        {
            this.N = 3;
            this.K = 2;
        }

        // we are NOT using these rules
        /*
         * Regler:
         *
         * One location at a time:
         * -----------------------
         * if m1r then !m1b & !m1l
         * if m2r then !m2b & !m2l
         * if m3r then !m3b & !m3l
         * 
         * Shore rule
         * ----------
         * if (m1r & m2r & !m3r) then 
         * ((c1r & c2r & !c3r) | (!c1r & c2r & c3r) | (c1r & !c2r & c3r) | 
         * (!c1r &!c2r & c3r) | (c1r & !c2r & !c3r) | (!c1r & c2r & !c3r) |
         * (!c1r & !c2r & !c3r))
         * if (m1r & m2r & m3r) then ((c1r & c2r & c3r) & (above rule))
         * 
         * Boat rule (left)
         * ---------
         * if (m1bl & !m2bl & !m3bl) then
         * ((c1bl & !c2bl & !c3bl) | (!c1bl & !c2bl & c3bl) |!c1bl & c2bl & !c3bl) |
         * (!c1bl & !c2bl))
         * if (!m1bl & !m2bl & m3b1) then
         * (c1bl & c2bl & !c3bl) | (!c1bl & c2bl & c3bl) | (c1bl & !c2bl & c3bl) | (above rule)
         * if (m1bl & m2bl & !m3b1) then
         * (!c1bl & !c2bl & !c3bl) 
         * 
         * */


        public string Run()
        {
            Kernel.Setup();
            Bdd I, E, T, R;

            Bdd[] ml = new Bdd[N];  // missionaries on left side
            Bdd[] mr = new Bdd[N];  // missionaries on right side
            Bdd[] mbr = new Bdd[N]; // missionaries on boat on right side
            Bdd[] mbl = new Bdd[N]; // missionaries on boat on left side

            Bdd[] mlp = new Bdd[N];
            Bdd[] mrp = new Bdd[N];
            Bdd[] mbrp = new Bdd[N];
            Bdd[] mblp = new Bdd[N];

            Bdd[] cl = new Bdd[N];  // cannibles on left side
            Bdd[] cr = new Bdd[N];  // cannibles on right side
            Bdd[] cbr = new Bdd[N]; // cannibles on boat on right side
            Bdd[] cbl = new Bdd[N]; // cannibles on boat on left side

            Bdd[] clp = new Bdd[N];
            Bdd[] crp = new Bdd[N];
            Bdd[] cbrp = new Bdd[N];
            Bdd[] cblp = new Bdd[N];

            Bdd br, brp, bl, blp; // the boat on right, the boat on right', the boat one the left, etc.


            //List<int> PreState = new List<int>();
            //List<int> PostState = new List<int>();

            BddPairList pairList = new BddPairList();

            // *2 because we have both missionaries and cannibals
            // *4 because they all have four variables
            for (int i = 0; i < N*2*4; i++)
            {
                pairList.Add(i * 2, i * 2 + 1);
            }

            //for (int i = 0; i < N*2*4; i++)
            //{
            //    PreState.Add(i * 2);
            //    PostState.Add(i * 2 + 1);
            //}

            for (int i = 0; i < N; i++)
            {
                ml[i] = new Bdd(i * 16);
                mlp[i] = new Bdd(i * 16 + 1);
                mbl[i] = new Bdd(i * 16 + 2);
                mblp[i] = new Bdd(i * 16 + 3);
                mr[i] = new Bdd(i * 16 + 4);
                mrp[i] = new Bdd(i * 16 + 5);
                mbr[i] = new Bdd(i * 16 + 6);
                mbrp[i] = new Bdd(i * 16 + 7);
                cl[i] = new Bdd(i * 16 + 8);
                clp[i] = new Bdd(i * 16 + 9);
                cbl[i] = new Bdd(i * 16 + 10);
                cblp[i] = new Bdd(i * 16 + 11);
                cr[i] = new Bdd(i * 16 + 12);
                crp[i] = new Bdd(i * 16 + 13);
                cbr[i] = new Bdd(i * 16 + 14);
                cbrp[i] = new Bdd(i * 16 + 15);
            }
            bl = new Bdd(N * 16);
            blp = new Bdd(N * 16 + 1);
            pairList.Add(N * 16, N * 16 + 1);
            br = new Bdd(N * 16 + 2);
            brp = new Bdd(N * 16 + 3);
            pairList.Add(N * 16 + 2, N * 16 + 3);



            I = InitialState(mr, cr, ml, cl, mbr, cbr, mbl, cbl, bl, br);
            E = EndState(mr, cr, ml, cl, mbr, cbr, mbl, cbl, bl, br);
            T = Transitions(mr, cr, ml, cl, mbr, cbr, mbl, cbl, mrp, crp, mlp, clp, mbrp, cbrp, mblp, cblp, bl, blp, br, brp);
            //R = ReachableStates(I, T, PreState, PostState);
            R = ReachableStates(I, T, pairList);
            Bdd result = E & R;
            //BddSerializer.Serialize(R, "output2");
            return "R&E-satcount: " + Kernel.SatCount(result).ToString() +
                Environment.NewLine +
                "R&E-anysat: " + Kernel.AnySat(result) +
                Environment.NewLine +
                "R-satcount: " + Kernel.SatCount(R).ToString() +
                Environment.NewLine +
                "LevelCount: " + count.ToString();
        }



        private Bdd InitialState(Bdd[] mr, Bdd[] cr, Bdd[] ml, Bdd[] cl, Bdd[] mbr, Bdd[] cbr, Bdd[] mbl, Bdd[] cbl,
            Bdd bl, Bdd br)
        {
            Bdd I = new Bdd(true);

            for (int i = 0; i < N; i++)
            {
                I = I & ml[i] & cl[i] & !mbl[i] & !cbl[i] & !mbr[i] & !cbr[i] & !mr[i] & !cr[i]; 
            }
            I &= bl & !br;

            return I;
        }

        private Bdd EndState(Bdd[] mr, Bdd[] cr, Bdd[] ml, Bdd[] cl, Bdd[] mbr, Bdd[] cbr, Bdd[] mbl, Bdd[] cbl,
            Bdd bl, Bdd br)
        {
            Bdd E = new Bdd(true);

            for (int i = 0; i < N; i++)
            {
                E = E & !ml[i] & !cl[i] & !mbl[i] & !cbl[i] & !mbr[i] & !cbr[i] & mr[i] & cr[i];
            }
            E &= !bl & br;

            return E;
        }

        private Bdd Transitions(Bdd[] mr, Bdd[] cr, Bdd[] ml, Bdd[] cl, Bdd[] mbr, Bdd[] cbr, Bdd[] mbl, Bdd[] cbl,
                               Bdd[] mrp, Bdd[] crp, Bdd[] mlp, Bdd[] clp, Bdd[] mbrp, Bdd[] cbrp, Bdd[] mblp, Bdd[] cblp,
                                Bdd bl, Bdd blp, Bdd br, Bdd brp)
        {
            // enviroment rules
            Bdd E = ShoreRules (ml, cl) & ShoreRules(mr, cr);
            // left to boat
            Bdd LTB = MoveToBoat(ml, mlp, mbl, mblp, cl, clp, cbl, cblp, bl, blp, br, brp);
            // boat to right
            Bdd BTR = MoveToShore(mbl, mblp, mr, mrp, cbl, cblp, cr, crp, bl, blp, br, brp);
            // right to boat
            Bdd RTB = MoveToBoat(mr, mrp, mbr, mbrp, cr, crp, cbr, cbrp, br, brp, bl, blp);
            // boat to right
            Bdd BTL = MoveToShore(mbr, mbrp, ml, mlp, cbr, cbrp, cl, clp, br, brp, bl, blp);
            // all moves
            Bdd M = LTB | BTR | RTB | BTL;
            // Moves and enviroment = transitions
            Bdd T = M & E;
            return T;
        }

        // ms = missonaries startpoint (pre)
        // msp = missonaries startpoint (post or ')
        // me = missinaries endpoint (pre)
        // mep = missionaries endpoint (post or ')
        // same for cannibals
        // bs = boat start
        // be = boat end
        private Bdd MoveToShore(Bdd[] ms, Bdd[] msp, Bdd[] me, Bdd[]mep, Bdd[] cs, Bdd[] csp, Bdd[] ce ,Bdd[] cep,
            Bdd bs, Bdd bsp, Bdd be, Bdd bep)
        {
            Bdd M = new Bdd(false);

            // opens boat after move
            Bdd OB = OpenBoat(bs, bsp, be, bep);

            // if one missionary in boat then one missionary move
            for (int i = 0; i < N; i++)
            {
                Bdd temp = Kernel.IfThen(OnePersonBoat(ms, cs, i),
                    (ms[i] & !msp[i] & !me[i] & mep[i] & OB & Assign(ms, msp, i) & Assign(me, mep, i)));
                M |= temp;
                //M = M | (ms[i] & !msp[i] & mep[i] & Assign(ms, msp, i) & Assign(me, mep, i));
            }
            // if one cannibal in boat then move one cannibal
            for (int i = 0; i < N; i++)
            {
                M |= Kernel.IfThen(OnePersonBoat(cs, ms, i),
                    (cs[i] & !csp[i] & !ce[i] & cep[i] & OB & Assign(cs, csp, i) & Assign(ce, cep, i)));
                //M = M | (cs[i] & !csp[i] & cep[i] & Assign(cs, csp, i) & Assign(ce, cep, i));
            }
            // two missionaries move
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
		        {
                    if (i != j)
                    {
                        
                         M |= Kernel.IfThen(TwoPersonBoat(ms, cs, i, j),
                             (ms[i] & ms[j] & !msp[i] & !msp[j]
                            & !me[i] & !me[j] & mep[i] & mep[j]
                            & OB
                            & Assign(ms, msp, i, j) & Assign(me, mep, i, j)));
                        
                          
                         /*M = M | (ms[i] & ms[j] & !msp[i] & !msp[j]
                                & mep[i] & mep[j]
                                & Assign(ms, msp, i, j) & Assign(me, mep, i, j));*/
                    }
                }
            }
            // two cannibals move
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (i != j)
                    {
                        M |= Kernel.IfThen(TwoPersonBoat(cs, ms, i, j),
                            (cs[i] & cs[j] & !csp[i] & !csp[j]
                            & !ce[i] & !ce[j] & cep[i] & cep[j]
                            & OB
                            & Assign(cs, csp, i, j) & Assign(ce, cep, i, j)));
                        /*M = M | (cs[i] & cs[j] & !csp[i] & !csp[j]
                            & cep[i] & cep[j]
                            & Assign(cs, csp, i, j) & Assign(ce, cep, i, j));*/
                    }
                }
            }
            // one of each move
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    M |= Kernel.IfThen(TwoDifPersonBoat(cs, ms, i, j),
                        (cs[i] & ms[j] & !csp[i] & !msp[j] 
                        & !ce[i] & !me[j] & cep[i] & mep[j]
                        & OB
                        & Assign(ms, msp, j) & Assign(cs, csp, i)) 
                        & Assign(me, mep, j) & Assign(ce, cep, i));
                    /*M = M | (cs[i] & ms[j] & !csp[i] & !msp[j]
                        & cep[i] & mep[j]
                        & Assign(ms, msp, j) & Assign(cs, csp, i))
                        & Assign(me, mep, j) & Assign(ce, cep, i);*/
                }
            }

            return M;
        }

        // open boat to move down in
        private Bdd OpenBoat(Bdd bs, Bdd bsp, Bdd be, Bdd bep)
        {
            Bdd OB = !bs & !bsp & !be & bep;
            return OB;
        }

        // only one person on boat
        private Bdd OnePersonBoat(Bdd[] personType, Bdd[] otherType , int number)
        {
            Bdd OPB = personType[number];
            for (int i = 0; i < N; i++)
            {
                if (i != number)
                    OPB &= !personType[i];
            }
            for (int i = 0; i < N; i++)
            {
                OPB &= !otherType[i];
            }
            return OPB;
        }

        // two person of same type on boat
        private Bdd TwoPersonBoat(Bdd [] personType, Bdd [] otherType, int number1, int number2)
        {
            Bdd TPB = personType[number1] & personType[number2];
            for (int i = 0; i < N; i++)
            {
                if (i != number1 && i != number2)
                    TPB &= !personType[i];
            }
            for (int i = 0; i < N; i++)
            {
                TPB &= !otherType[i];
            }
            return TPB;
        }

        // two different persons on boat
        private Bdd TwoDifPersonBoat(Bdd[] type1, Bdd[] type2, int number1, int number2)
        {
            Bdd TDPB = type1[number1] & type2[number2];
            for (int i = 0; i < N; i++)
            {
                if(i != number1)
                    TDPB &= !type1[i];
            }
            for (int i = 0; i < N; i++)
            {
                if (i != number2)
                    TDPB &= !type2[i];
            }
            return TDPB;
        }


        // ms = missonaries startpoint (pre)
        // msp = missonaries startpoint (post or ')
        // me = missinaries endpoint (pre)
        // mep = missionaries endpoint (post or ')
        // same for cannibals
        // bs = boat start
        // be = boat end
        private Bdd MoveToBoat(Bdd[] ms, Bdd[] msp, Bdd[] me, Bdd[] mep, Bdd[] cs, Bdd[] csp, Bdd[] ce, Bdd[] cep, 
            Bdd bs, Bdd bsp, Bdd be, Bdd bep)
        {
            Bdd M = new Bdd(false);
            Bdd LB = LockBoat(bs, bsp, be, bep);
            
            // one missionary move
            for (int i = 0; i < N; i++)
            {
                M = M | (ms[i] & !msp[i] & !me[i] & mep[i] & LB & Assign(ms, msp, i) & Assign(me, mep, i));
                //M = M | (ms[i] & !msp[i] & mep[i] & Assign(ms, msp, i) & Assign(me, mep, i));
            }
            // one cannibal move
            for (int i = 0; i < N; i++)
            {
                M = M | (cs[i] & !csp[i] & !ce[i] & cep[i] & LB & Assign(cs, csp, i) & Assign(ce, cep, i));
                //M = M | (cs[i] & !csp[i] & cep[i] & Assign(cs, csp, i) & Assign(ce, cep, i));
            }
            // two missionaries move
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (i != j)
                    {

                        M = M | (ms[i] & ms[j] & !msp[i] & !msp[j]
                           & !me[i] & !me[j] & mep[i] & mep[j]
                           & LB
                           & Assign(ms, msp, i, j) & Assign(me, mep, i, j));


                        /*M = M | (ms[i] & ms[j] & !msp[i] & !msp[j]
                               & mep[i] & mep[j]
                               & Assign(ms, msp, i, j) & Assign(me, mep, i, j));*/
                    }
                }
            }
            // two cannibals move
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (i != j)
                    {
                        M = M | (cs[i] & cs[j] & !csp[i] & !csp[j]
                            & !ce[i] & !ce[j] & cep[i] & cep[j]
                            & LB
                            & Assign(cs, csp, i, j) & Assign(ce, cep, i, j));
                        /*M = M | (cs[i] & cs[j] & !csp[i] & !csp[j]
                            & cep[i] & cep[j]
                            & Assign(cs, csp, i, j) & Assign(ce, cep, i, j));*/
                    }
                }
            }
            // one of each move
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    M = M | (cs[i] & ms[j] & !csp[i] & !msp[j]
                        & !ce[i] & !me[j] & cep[i] & mep[j]
                        & LB
                        & Assign(ms, msp, j) & Assign(cs, csp, i))
                        & Assign(me, mep, j) & Assign(ce, cep, i);
                    /*M = M | (cs[i] & ms[j] & !csp[i] & !msp[j]
                        & cep[i] & mep[j]
                        & Assign(ms, msp, j) & Assign(cs, csp, i))
                        & Assign(me, mep, j) & Assign(ce, cep, i);*/
                }
            }
            
            // boat empty
            Bdd emptyBoat = new Bdd(true);
            for (int i = 0; i < N; i++)
			{
                emptyBoat &= !me[i] & !ce[i];
			}
            
            // if boat is empty then make move
            M = Kernel.IfThen(emptyBoat, M);

            return M;
        }

        // bs = boat startpoint, be = boat endpoint, p = ')
        private Bdd LockBoat(Bdd bs, Bdd bsp, Bdd be, Bdd bep)
        {
            Bdd MB = bs & !bsp & !be & !bep;
            return MB;
        }

        private Bdd ShoreRules(Bdd[] m, Bdd[] c)
        {
            Bdd SR = new Bdd(true);

            // three missionaries on a shore
            Bdd ThM = ThreePersonShore(m);
            // two misionaries on a shore
            Bdd TwM = TwoPersonShore(m);
            // one missinary on a shore;
            Bdd OM = OnePersonShore(m);
            // no missionary on a shore
            Bdd NM = NoPersonShore(m);

            // three cannibals on a shore
            Bdd ThC = ThreePersonShore(c);
            // two cannibals on a shore
            Bdd TwC = TwoPersonShore(c);
            // one cannibals on a shore
            Bdd OC = OnePersonShore(c);
            // no cannibal on a shore
            Bdd NC = NoPersonShore(c);

            // if three missionaries then all cannibals combinations possible = true
            //SR |= Kernel.IfThen(ThM, ThC | TwC | OC | NC);
            // if two missionaries
            SR &= Kernel.IfThen(TwM, TwC | OC | NC);
            // if one missionary
            SR &= Kernel.IfThen(OM, OC | NC);
            // if no missionary (same as three - can make them together)
            //SR |= Kernel.IfThen(NM, ThC | TwC | OC | NC);

            return SR;
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
        public Bdd Assign(Bdd[] x, Bdd[] xp, int n)
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
        public Bdd Assign(Bdd[] x, Bdd[] xp, int n, int m)
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

        private Bdd ReachableStates(Bdd I, Bdd T, List<int> PreState, List<int> PostState)
        {
            Bdd Rp, tmp, t;
            Bdd R = new Bdd(false);
            do
            {
                Rp = R;
                tmp = T & R;
                t = Kernel.Exists(PreState, tmp);

                R = I | Kernel.Compose(t, PreState, PostState);
                count++;
            }
            while (!R.Equals(Rp));
            return R;
        }

        private Bdd ReachableStates(Bdd I, Bdd T, BddPairList bpl)
        {
            Bdd Rp, tmp, t;
            Bdd R = new Bdd(false);
            do
            {
                Rp = R;
                tmp = T & R;
                t = Kernel.Exists(bpl, tmp);

                R = I | Kernel.Compose(t, bpl);
                count++;
            }
            while (!R.Equals(Rp));
            return R;
        }
    }
}
