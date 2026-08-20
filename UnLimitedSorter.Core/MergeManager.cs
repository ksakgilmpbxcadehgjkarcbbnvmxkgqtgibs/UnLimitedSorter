using UnLimitedSorter.Core.Helpers;
using UnLimitedSorter.Core.Models;

namespace UnLimitedSorter.Core;

public class MergeManager
{
    private readonly int MaxOpenFiles;

    private readonly string NameSuperBatch = "SuperBatch";

    public MergeManager(int maxOpenFiles = 100)
    {
        MaxOpenFiles = maxOpenFiles;
    }

    public void Merge(List<string> batchFiles, string outputFilePath)
    {
        while (batchFiles.Count > MaxOpenFiles)
        {
            var nextLevelBatching = new List<string>();

            for (int i = 0; i < batchFiles.Count; i += MaxOpenFiles)
            {
                var batchToMerge = batchFiles.Skip(i).Take(MaxOpenFiles).ToList();

                string superBatchPath = Path.Combine(NameDirHelper.GetNameDir(), $"{NameSuperBatch}_{Guid.NewGuid()}.tmp");

                MergeFiles(batchToMerge, superBatchPath);

                foreach (var file in batchToMerge)
                {
                    File.Delete(file);
                }

                nextLevelBatching.Add(superBatchPath);

            }

            batchFiles = nextLevelBatching;
        }

        MergeFiles(batchFiles, outputFilePath);

        foreach (var file in batchFiles)
        {
            File.Delete(file);
        }
    }

    private void MergeFiles(List<string> batchs, string path)
    {
        var priorityQueue = new PriorityQueue<(StreamReader Reader, string line), RowElement>();

        foreach (var file in batchs)
        {
            var reader = new StreamReader(file);

            ReadLineFromFile(priorityQueue, reader);
        }

        var fileCreate = File.Create(path);

        using var streamWriter = new StreamWriter(fileCreate);

        while (priorityQueue.Count > 0)
        {
            priorityQueue.TryDequeue(out var item, out var currentKey);

            streamWriter.WriteLine(item.line);

            ReadLineFromFile(priorityQueue, item.Reader);
        }
    }

    public void ReadLineFromFile(PriorityQueue<(StreamReader Reader, string line), RowElement> priorityQueue, StreamReader reader)
    {
        string? line = reader.ReadLine();

        if (line != null)
        {
            var row = ParseHelper.TryParseLine(line);

            priorityQueue.Enqueue((reader, line), row);
        }
        else
        {
            reader.Dispose();
        }
    }

}
