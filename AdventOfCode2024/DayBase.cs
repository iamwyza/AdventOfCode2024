﻿using System.Reflection;

namespace AdventOfCode2024;
public abstract class DayBase : IDay
{
    public static Dictionary<int, IDay> Days = new ();
    
    public static bool DebugOutput = false;

    // Poor man's DI ;) 
    public static void ScanForSolutions()
    {
        var solutions = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType == typeof(DayBase));
        foreach (var solution in solutions)
        {
            var day = int.Parse(solution.Namespace?.Replace("AdventOfCode2024.Day", "") ?? "-1");
            if (day <= 0) continue;

            // Don't show future days yet even though we found classes for them.  They are just from last year copied over.
            if (DateTime.Now.Year == 2024 && day > DateTime.Now.Day) continue;

            if (Activator.CreateInstance(solution) is not IDay instance)
            {
                throw new Exception($"Couldn't create instance for type {solution.FullName}");
            }

            if (instance.Ready)
            {
                instance.Day = day;

                Days.Add(day, instance);
            }
        }
    }

    public bool Ready { get; set; }

    public int Day { get; set; }

    public void PrintStart(int part)
    {
        Console.WriteLine($"Day {Day} - Part {part}:");
    }

    public async Task<string[]> GetLines()
    {
        return await File.ReadAllLinesAsync($"..\\..\\..\\Day{Day:00}\\day{Day}input.txt");
    }

    public async Task<string[]> GetTestLines(int part)
    {
        return await File.ReadAllLinesAsync($"..\\..\\..\\Day{Day:00}\\day{Day}.{part}testinput.txt");
    }


    public abstract Task RunPart1();
    public abstract Task RunPart2();
}
