using UnLimitedSorter.Core.Helpers;
using UnLimitedSorter.Core.Models;

namespace UnLimitedSorter.Core;

public class BatchCreatorManager
{
    private readonly int BatchSizeRow;
    private readonly long BatchSizeBytesCap;

    private readonly string NameBatch = "TempBatch";

    public BatchCreatorManager(int batchSizeRow = 500_000, long batchSizeBytesCap = 500_000_000)
    {
        BatchSizeRow = batchSizeRow;
        BatchSizeBytesCap = batchSizeBytesCap;

        if (!Directory.Exists(NameDirHelper.GetNameDir()))
        {
            Directory.CreateDirectory(NameDirHelper.GetNameDir());
        }
    }

    public List<string> ReadFileAndSplit(string inputFilePath)
    {
        var batchFiles = new List<string>();

        var dictionary = new Dictionary<RowElement, int>();

        int batchIndex = 0;

        int lineCount = 0;

        long approxBytes = 0;

        using var reader = new StreamReader(inputFilePath);

        string? line;

        while ((line = reader.ReadLine()) != null)
        {
            var rowKey = ParseHelper.TryParseLine(line);

            if (!dictionary.TryGetValue(rowKey, out int count))
            {
                dictionary[rowKey] = 1;
            }
            else
            {
                dictionary[rowKey] = count + 1;
            }

            lineCount++;
            approxBytes += line.Length * 2L;

            if (lineCount >= BatchSizeRow || approxBytes >= BatchSizeBytesCap)
            {
                BatchPackaging(batchFiles, dictionary, ref batchIndex);

                lineCount = 0;
                approxBytes = 0;
            }
        }

        // Если считали и в обьем батча не дозаполнили
        if (lineCount > 0)
        {
            BatchPackaging(batchFiles, dictionary, ref batchIndex);
        }

        return batchFiles;
    }

    private void BatchPackaging(List<string> batchFiles, Dictionary<RowElement, int> dictionary, ref int batchIndex)
    {
        var mas = SortHelper.Sort(dictionary);

        string batchPath = SaveInFile(dictionary, mas, batchIndex);

        batchIndex++;

        batchFiles.Add(batchPath);

        dictionary.Clear();
    }

    private string SaveInFile(Dictionary<RowElement, int> dictionary, RowElement[] mas, int batchIndex)
    {
        string tempFilePath = Path.Combine(NameDirHelper.GetNameDir(), $"{NameBatch}_{batchIndex}.tmp");

        using var stream = new StreamWriter(tempFilePath);

        foreach (var key in mas)
        {
            int countRow = dictionary[key];

            for (int i = 0; i < countRow; i++)
            {
                stream.WriteLine($"{key.RowNumber}.{key.RowLine}");
            }
        }

        return tempFilePath;
    }
}
