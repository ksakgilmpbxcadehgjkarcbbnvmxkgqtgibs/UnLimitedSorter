using UnLimitedSorter.Core;

string input = "input.txt";
string output = "output.txt";

if (!File.Exists(input))
{
    Console.WriteLine($"Ошибка: файл '{input}' не найден в текущей директории ({Directory.GetCurrentDirectory()}).");
    Console.WriteLine("Поместите входной файл рядом с исполняемым файлом или укажите правильный путь.");
    return;
}

Console.WriteLine("Начинаем чтение и первичная сортировка");

BatchCreatorManager batchCreatorManager = new BatchCreatorManager();
var batchMas = batchCreatorManager.ReadFileAndSplit(input);

Console.WriteLine($"Создано батчей {batchMas.Count}");
Console.WriteLine("Начинаем мерж батчей");

MergeManager mergeManager = new MergeManager();
mergeManager.Merge(batchMas, output);

Console.WriteLine("Сортировка завершена");
Console.ReadLine();
