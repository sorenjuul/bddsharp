using System;
using System.Collections.Generic;
using System.Text;

namespace BddSharp.Kernel
{
    /// <summary>
    /// Collection for holding pairs of variables representing pre- and post-state.
    /// This collection is used when doing quantification (only the first number in 
    /// the pair is used) and composition (first number is the one that is replaced
    /// with the last).
    /// </summary>
    public class BddPairList
    {
        Dictionary<int, int> composeList = new Dictionary<int, int>();
        Dictionary<int, bool> quantificationList = new Dictionary<int, bool>();

        /// <summary>
        /// Add a variable pair to the List.
        /// </summary>
        /// <param name="x">Variable used in quantification and variable that is replaced
        /// in composition.</param>
        /// <param name="xp">Variable representing the post state.</param>
        public void Add(int x, int xp)
        {
            composeList.Add(xp, x);
            quantificationList.Add(x, false);
        }

        internal bool ComposeContainsKey(int xp)
        {
            return composeList.ContainsKey(xp);
        }

        internal bool QuantificationContainsKey(int x)
        {
            return quantificationList.ContainsKey(x);
        }

        internal int this[int xp]
        {
            get
            {
                return composeList[xp];
            }
        }

        /// <summary>
        /// Remove a pair of variables.
        /// </summary>
        /// <param name="x">First variable in the pair to be removed.</param>
        public void Remove(int x)
        {
            quantificationList.Remove(x);
            int keyx = -1;
            foreach (KeyValuePair<int, int> kvp in composeList)
            {
                if (kvp.Value == x)
                {
                    keyx = kvp.Key;
                    break;
                }
            }
            if (keyx != -1)
                composeList.Remove(keyx);
        }

        /// <summary>
        /// Clear the BddPairList.
        /// </summary>
        public void Clear()
        {
            quantificationList = new Dictionary<int,bool> ();
            composeList = new Dictionary<int,int> ();
        }

        internal IEnumerator<KeyValuePair<int, int>> GetEnumerator()
        {
            return composeList.GetEnumerator();
        }

        internal IEnumerator<KeyValuePair<int, bool>> GetEnumeratorExists()
        {
            return quantificationList.GetEnumerator();
        }


    }
}
