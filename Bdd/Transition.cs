using System;
using System.Collections.Generic;
using System.Text;

namespace BddSharp.Kernel
{
    /// <summary>
    /// Class for verifying transition systems.
    /// </summary>
    public class Transition
    {

        private static Bdd ExecuteTransition(Bdd R, Bdd T, BddPairList bpl)
        {
            Bdd temp = Kernel.ApplyExists(Op.CON, T, R, bpl);
            return Kernel.VarListCompose(bpl, temp);
        }

        /// <summary>
        /// Evaluates the reachable state space of a transition system given a initial state.
        /// </summary>
        /// <param name="I">The Bdd representing the initial state of the system.</param>
        /// <param name="T">The Bdd representing all possible transition in the system.</param>
        /// <param name="bpl">A list of variable pairs that represent pre- and post-states.</param>
        /// <returns>The Bdd representing the reachable state space.</returns>
        public static Bdd ReachableStates(Bdd I, Bdd T, BddPairList bpl)
        {
            Bdd Rp;
            Bdd R = new Bdd(false);
            do
            {
                Rp = R;
                R = I | ExecuteTransition(R, T, bpl);
            }
            while (!R.Equals(Rp));
            return R;
        }

        /// <summary>
        /// Evaluates the number of transitions it takes to reach a state from a given initial
        /// state in a transition system.
        /// </summary>
        /// <param name="I">The Bdd representing the initial state of the system.</param>
        /// <param name="E">The state that what to be reached.</param>
        /// <param name="T">The Bdd representing all possible transition in the system.</param>
        /// <param name="bpl">A list of variable pairs that represent pre- and post-states.</param>
        /// <returns>Number of transitions needed. -1 if no trace exists.</returns>
        public static int Tracelength(Bdd I, Bdd E, Bdd T, BddPairList bpl)
        {
            int tracelength = 0;
            Bdd Rp, test;
            Bdd R = I;
            do
            {
                Rp = R;
                R = I | ExecuteTransition(R, T, bpl);
                test = E >= R;
                if (R.Equals(Rp) & test.U != Kernel.bddtrue)
                    return -1;
                tracelength++;
            }
            while (test.U != Kernel.bddtrue);
            return tracelength;
        }

        /// <summary>
        /// List a possible sequence of transitions to reach a state from a given initial
        /// state in a transition system.
        /// </summary>
        /// <param name="I">The Bdd representing the initial state of the system.</param>
        /// <param name="E">The state that what to be reached.</param>
        /// <param name="T">The Bdd representing all possible transition in the system.</param>
        /// <param name="transitionlist">A collection of all possible transition in the 
        /// transition system and a string that describes each transition.</param>
        /// <param name="bpl">A list of variable pairs that represent pre- and post-states.</param>
        /// <param name="tracelength">The number of transitions it takes to reach a state from a given initial
        /// state.</param>
        /// <returns>A string representing a possible sequence of transitions. "No trace to state" if no trace exists.</returns>
        public static string FindTrace(Bdd I, Bdd E, Bdd T, Dictionary<Bdd, string> transitionlist, BddPairList bpl, int tracelength)
        {
            Bdd R;
            if (tracelength == 0)
                return "[]";
            else if(tracelength > 0)
            {
                foreach (KeyValuePair<Bdd, string> trans in transitionlist)
                {
                    R = ExecuteTransition(I, trans.Key, bpl);
                    if (IsOnTrace(R, E, T, bpl, tracelength - 1))
                        return trans.Value + " " + FindTrace(R, E, T, transitionlist, bpl, tracelength - 1);
                }
            }
            return "No trace to the state.";
        }

        private static bool IsOnTrace(Bdd I, Bdd E, Bdd T, BddPairList bpl, int tracelength)
        {
            int count = 0;
            Bdd R = I;
            while (count < tracelength)
            {
                R = ExecuteTransition(R, T, bpl);
                count++;
            }
            Bdd test = E >= R;
            return test.U == Kernel.bddtrue;
        }
    }
}
