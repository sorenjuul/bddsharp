using System;
using System.Collections.Generic;
using System.Text;

namespace BddSharp.Kernel
{
    /// <summary>
    /// Collection for holding pairs of variables representing pre- and post-state.
    /// This collection is used when doing quantification (only the first number in 
    /// the pair is used) and composition (first number is the one that is replaced
    /// with the last). The class replaces the BddPairList when working with Fdd's.
    /// </summary>
    public class FddPairList
    {
        private BddPairList list = new BddPairList();

        /// <summary>
        /// Add a variable pair to the List.
        /// </summary>
        /// <param name="x">Variable used in quantification and variable that is replaced
        /// in composition.</param>
        /// <param name="xp">Variable representing the post state.</param>
        public void Add(int x, int xp)
        {
            list.Add(x, xp);
        }

        /// <summary>
        /// Remove a pair of variables.
        /// </summary>
        /// <param name="x">First variable in the pair to be removed.</param>
        public void Remove(int x)
        {
            list.Remove(x);
        }

        /// <summary>
        /// Clear the FddPairList.
        /// </summary>
        public void Clear()
        {
            list.Clear();
        }

        /// <summary>
        /// IEnumerator for iterating over the FddPairList
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<int, int>> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        internal IEnumerator<KeyValuePair<int, bool>> GetEnumeratorExists()
        {
            return list.GetEnumeratorExists();
        }
    }
}
