using UnLimitedSorter.Core.Models;

namespace UnLimitedSorter.Core.Helpers;

public static class ParseHelper
{

    private static readonly char SeparatorChar = '.';

    public static RowElement TryParseLine(string line)
    {
        var charIndex = line.IndexOf(SeparatorChar, StringComparison.Ordinal);

        if (charIndex < 0)
            throw new Exception("Error: separator symbol not found");

        ReadOnlySpan<char> numberSpan = line.AsSpan(0, charIndex);

        if (!int.TryParse(numberSpan, out int rowNumber))
            throw new Exception("Error: the number specified in the task was not found.");

        string rowString = line.Substring(charIndex + 1);

        return new RowElement(rowString, rowNumber);
    }
}
