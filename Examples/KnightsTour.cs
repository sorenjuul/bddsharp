using System;
using System.Collections.Generic;
using System.Text;
using BddSharp.Kernel;

namespace Examples
{
    public class Knight : Examples.IExampleForm
    {
        private Bdd T = new Bdd(true);      //Predicate for getting to next state
        private Bdd I;      //Initial states
        //private Bdd R;      //Reachable states
        private int N;
        private Bdd E;
        private Bdd[,] X;
        private Bdd[,] Xp;
        private Bdd[,] K;
        private Bdd[,] Kp;
        private Dictionary<Bdd, String> transList = new Dictionary<Bdd, string>();

        public Knight(int n)
        {
            N = n;
            X = new Bdd[N, N];
            Xp = new Bdd[N, N];
            K = new Bdd[N, N];
            Kp = new Bdd[N, N];
            Kernel.Setup();

            int var = 0;
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                {
                    X[i, j] = new Bdd(var);
                    var++;
                    Xp[i, j] = new Bdd(var);
                    var++;
                    //    }
                    //      REMOVING THIS COMMENT WOULD CHANGE var order from x0,k0..xn,kn -> x0..xn,k0..kn
                    //for (int i = 0; i < N; i++)
                    //    for (int j = 0; j < N; j++)
                    //    {
                    K[i, j] = new Bdd(var);
                    var++;
                    Kp[i, j] = new Bdd(var);
                    var++;
                }
        }

        public string Run(bool reachableStates, bool traceLength, bool findTrace)
        {
            //Console.WriteLine(String.Format("Start: {0}", DateTime.Now.ToString()));
            BuildT();

            BddPairList pairList = BuildStates();

            I = BuildInitialState();
           
            E = BuildEndState();

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

            if (findTrace)
            {
                string trace;
                if (traceLength)
                    trace = Transition.FindTrace(I, E, T, transList, pairList, length);
                else
                    trace = Transition.FindTrace(I, E, T, transList, pairList, Transition.Tracelength(I, E, T, pairList));
                answer += "Trace: " + trace;
            }

            return answer;
        }

        //The Initial knight placed in [0,0]
        public Bdd BuildInitialState()
        {
            Bdd result = new Bdd(true);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    if (i == 0 && j == 0)
                        result &= X[i, j] & K[i, j];
                    else
                        result &= !X[i, j] & !K[i, j];

            return result;
        }

        //Endstate = All squares visited
        public Bdd BuildEndState()
        {
            Bdd result = new Bdd(true);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    result &= X[i, j];

            return result;
        }


        /*
         *  1 6 3           For N=3 the endstate is that the middle isn't visited
         *  4 * 8
         *  7 2 5           Function for test
         */
        public Bdd BuildEndState3()
        {
            if (N != 3)
                throw new Exception("N must be 3 if you call this function");

            return X[0, 0] & X[0, 1] & X[0, 2] &
                    X[1, 0] & !X[1, 1] & X[1, 2] &
                    X[2, 0] & X[2, 1] & X[2, 2];
        }

        public BddPairList BuildStates()
        {
            BddPairList result = new BddPairList();
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                {
                    result.Add(X[i, j].Var, Xp[i, j].Var);
                    result.Add(K[i, j].Var, Kp[i, j].Var);
                }

            return result;
        }

        public void BuildT()
        {
            T = new Bdd(false);

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Bdd oneMove = new Bdd(false);
                    for (int movetype = 1; movetype <= 8; movetype++)
                    {
                        int k = i;
                        int l = j;
                        if (move(movetype, ref k, ref l))
                        {
                            Bdd temp = X[i, j] & !X[k, l] & Xp[i, j] & Xp[k, l] &
                                 K[i, j] & !K[k, l] & !Kp[i, j] & Kp[k, l] &
                                AssignK(K[i, j].Var, K[k, l].Var) &
                                AssignX(X[i, j].Var, X[k, l].Var);

                            T |= temp;
                            transList.Add(temp, String.Format("Moved from {0}, {1} to {2}, {3}", i, j, k, l));

                            //oneMove |= X[i, j] & !X[k, l] & Xp[i, j] & Xp[k, l] &
                            //     K[i, j] & !K[k, l] & !Kp[i, j] & Kp[k, l] &
                            //     AssignK(K[i, j].Var, K[k, l].Var) &
                            //     AssignX(X[i, j].Var, X[k, l].Var);
                        }
                    }
                    //T &= oneMove;
                }
            }
        }

        /*     8 . 1 
         *   7 . . . 2
         *   . . x . .     Chess board, x marks the knight
         *   6 . . . 3     Returns false if move isn't possible
         *     5 . 4
         */
        public bool move(int movetype, ref int i, ref int j)
        {
            switch (movetype)
            {
                case 1: i += 1; j -= 2; break;
                case 2: i += 2; j -= 1; break;
                case 3: i += 2; j += 1; break;
                case 4: i += 1; j += 2; break;
                case 5: i -= 1; j += 2; break;
                case 6: i -= 2; j += 1; break;
                case 7: i -= 2; j -= 1; break;
                case 8: i -= 1; j -= 2; break;
            }
            if (i < 0 || j < 0 || i >= N || j >= N)
                return false;

            return true;

        }

        // let all variables stay the same except for n and m
        public Bdd AssignX(int n, int m)
        {
            Bdd result = new Bdd(true);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    if (X[i, j].Var != n && X[i, j].Var != m)
                    {
                        result &= X[i, j] == Xp[i, j];
                    }

            return result;
        }

        public Bdd AssignK(int n, int m)
        {
            Bdd result = new Bdd(true);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    if (K[i, j].Var != n && K[i, j].Var != m)
                    {
                        result &= K[i, j] == Kp[i, j];
                        //result &= !K[i, j];       //ToDo: Check if you can optimize
                    }

            return result;
        }

        private Bdd KEndState()
        {
            return !K[0, 0] & !K[0, 1] & !K[0, 2] & !K[1, 0] & !K[1, 1] & K[1, 2] & !K[2, 0] & !K[2, 1] & !K[2, 2];
        }
    }

    public class KnightEasy : Examples.IExampleForm
    {
        private Bdd T = new Bdd(true);      //Predicate for getting to next state
        private Bdd I;      //Initial states
        //private Bdd R;      //Reachable states
        private int N;
        private Bdd E;
        private Bdd[,] X;
        private Bdd[,] Xp;
        private Dictionary<Bdd, String> transList = new Dictionary<Bdd, string>();

        public KnightEasy(int n)
        {
            N = n;
            X = new Bdd[N, N];
            Xp = new Bdd[N, N];
            Kernel.Setup();

            int var = 0;
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                {
                    X[i, j] = new Bdd(var);
                    var++;
                    Xp[i, j] = new Bdd(var);
                    var++;
                }
        }

        public string Run(bool reachableStates, bool traceLength, bool findTrace)
        {
            
            BuildT();

            BddPairList pairList = BuildStates();

            I = BuildInitialState();

            E = BuildEndState();

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

            if (findTrace)
            {
                string trace;
                if (traceLength)
                    trace = Transition.FindTrace(I, E, T, transList, pairList, length);
                else
                    trace = Transition.FindTrace(I, E, T, transList, pairList, Transition.Tracelength(I, E, T, pairList));
                answer += "Trace: " + trace;
            }

            return answer;
        }

        //The Initial knight placed in [0,0]
        public Bdd BuildInitialState()
        {
            Bdd result = new Bdd(true);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    if (i == 0 && j == 0)
                        result &= X[i, j];
                    else
                        result &= !X[i, j];

            return result;
        }

        //Endstate = All squares visited
        public Bdd BuildEndState()
        {
            Bdd result = new Bdd(true);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    result &= X[i, j];

            return result;
        }

        public BddPairList BuildStates()
        {
            BddPairList result = new BddPairList();
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                {
                    result.Add(X[i, j].Var, Xp[i, j].Var);
                }

            return result;
        }

        public void BuildT()
        {
            T = new Bdd(false);

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Bdd oneMove = new Bdd(false);
                    for (int movetype = 1; movetype <= 8; movetype++)
                    {
                        int k = i;
                        int l = j;
                        if (move(movetype, ref k, ref l))
                        {
                            Bdd temp = X[i, j] & !X[k,l] & Xp[k, l] &
                                AssignX(X[i, j].Var, X[k, l].Var);

                            T |= temp;
                            transList.Add(temp, String.Format("Moved from {0}, {1} to {2}, {3}", i, j, k, l));
                        }
                    }
                }
            }
        }

        /*     8 . 1 
         *   7 . . . 2
         *   . . x . .     Chess board, x marks the knight
         *   6 . . . 3     Returns false if move isn't possible
         *     5 . 4
         */
        public bool move(int movetype, ref int i, ref int j)
        {
            switch (movetype)
            {
                case 1: i += 1; j -= 2; break;
                case 2: i += 2; j -= 1; break;
                case 3: i += 2; j += 1; break;
                case 4: i += 1; j += 2; break;
                case 5: i -= 1; j += 2; break;
                case 6: i -= 2; j += 1; break;
                case 7: i -= 2; j -= 1; break;
                case 8: i -= 1; j -= 2; break;
            }
            if (i < 0 || j < 0 || i >= N || j >= N)
                return false;

            return true;

        }

        // let all variables stay the same except for n and m
        public Bdd AssignX(int n, int m)
        {
            Bdd result = new Bdd(true);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    if (X[i, j].Var != n && X[i, j].Var != m)
                    {
                        result &= X[i, j] == Xp[i, j];
                    }

            return result;
        }
    }
}
