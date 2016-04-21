using System;
using System.Collections.Generic;
using System.Text;

namespace BddSharp.Kernel
{

    public class BddPairList : IEnumerable<KeyValuePair<int, int>>
    {
        SortedList<int, int> pairList = new SortedList<int, int>(new DescendingComparer());

        public void Add(int first, int second)
        {
            pairList.Add(first, second);
        }

        public void Remove(int first)
        {
            pairList.Remove(first);
        }

        public void Clear()
        {
            pairList = new SortedList<int, int>(new DescendingComparer());
        }

        #region IEnumerable<KeyValuePair<int,int>> Members

        public IEnumerator<KeyValuePair<int, int>> GetEnumerator()
        {
            return pairList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return pairList.GetEnumerator();
        }

        #endregion
    }

    class DescendingComparer : Comparer<int>
    {
        public override int Compare(int x, int y)
        {
            if (x > y)
                return -1;
            else if (x < y)
                return 1;
            else
                return 0;
        }
    }
}
