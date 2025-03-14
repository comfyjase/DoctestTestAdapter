using DoctestTestAdapter.Shared.Pdb;
using System.Collections.Generic;

namespace DoctestTestAdapter.Shared.EqualityComparers
{
    internal class PdbLineDataComparer : IComparer<PdbLineData>
    {
        public int Compare(PdbLineData x, PdbLineData y)
        {
            if (x.Number > y.Number)
                return 1;
            if (x.Number < y.Number)
                return -1;
            else
                return 0;
        }
    }
}
