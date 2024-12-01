namespace AdventOfCode2024;
public interface IDay
{
    bool Ready { get; set; }
    int Day { get; set; }
    Task RunPart1();
    Task RunPart2();
}
