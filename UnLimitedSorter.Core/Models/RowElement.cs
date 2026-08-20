namespace UnLimitedSorter.Core.Models;

public struct RowElement : IComparable<RowElement>, IEquatable<RowElement>
{
    public int RowNumber;
    public string RowLine;

    public RowElement(string rowLine, int rowNumber)
    {
        RowLine = rowLine;
        RowNumber = rowNumber;
    }

    public int CompareTo(RowElement other)
    {
        int textCmp = string.Compare(RowLine, other.RowLine, StringComparison.Ordinal);
        if (textCmp != 0) return textCmp;
        return RowNumber.CompareTo(other.RowNumber);
    }

    public bool Equals(RowElement other)
        => RowNumber == other.RowNumber && RowLine.Equals(other.RowLine);

    public override int GetHashCode()
        => HashCode.Combine(RowNumber, RowLine);
}
