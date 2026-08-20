using UnLimitedSorter.Core.Models;

namespace UnLimitedSorter.Core.Helpers;

public static class SortHelper
{
    public static RowElement[] Sort(Dictionary<RowElement, int> dictionary)
    {
        RowElement[] mas = new RowElement[dictionary.Count];

        dictionary.Keys.CopyTo(mas, 0);

        Array.Sort(mas);

        return mas;
    }

}
