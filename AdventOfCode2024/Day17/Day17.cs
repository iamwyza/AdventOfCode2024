using System.Diagnostics;
using AdventOfCode2024.GridUtilities;

namespace AdventOfCode2024.Day17;
internal class Day17 : DayBase
{
    private long RegisterA;
    private long RegisterB;
    private long RegisterC;
    
    
    public Day17()
    {
        Ready = true;
    }

    private enum Instruction
    {
        ADV = 0,
        BXL,
        BST,
        JNZ,
        BXC,
        OUT,
        BDV,
        CDV
    }

    private List<Instruction> _instructions = new();

    private async Task Init(int part, bool useTestData)
    {
        _instructions.Clear();
        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        RegisterA = int.Parse(lines[0].Split(' ')[2]);
        RegisterB = int.Parse(lines[1].Split(' ')[2]);
        RegisterC = int.Parse(lines[2].Split(' ')[2]);
        _instructions.AddRange(lines[4].Split(' ')[1].Split(',').Select(Enum.Parse<Instruction>));
   
        PrintMachineState(0);
   
    }

    private void PrintMachineState(int instructionPointer)
    {
        AnsiConsole.MarkupLineInterpolated($"Register A: [green]{RegisterA}[/]");
        AnsiConsole.MarkupLineInterpolated($"Register B: [red]{RegisterB}[/]");
        AnsiConsole.MarkupLineInterpolated($"Register C: [blue]{RegisterC}[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.Write("Program:");

        for (var index = 0; index < _instructions.Count; index++)   
        {
            if (index == instructionPointer)
            {
                AnsiConsole.MarkupInterpolated($"[red]{_instructions[index]}[/]");
            }
            else if (index == instructionPointer + 1)
            {
                AnsiConsole.MarkupInterpolated($"[green]{(int)_instructions[index]}[/]");

            }else
            {
                AnsiConsole.MarkupInterpolated($"[yellow]{(int)_instructions[index]}[/]");
            }
            AnsiConsole.Write(',');
        }
        AnsiConsole.WriteLine();
    }
    
    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        int instructionPointer = 0;
        sbyte[] output = new sbyte[_instructions.Count];

        int outputCount = 0;
        while (instructionPointer < _instructions.Count) {
            PrintMachineState(instructionPointer);
            var instruction = _instructions[instructionPointer];
            
            // if (instruction != Instruction.JNZ && instructionPointer + 1 > _instructions.Count)
            //         break;
            
            var operand = (int)_instructions[instructionPointer + 1];
            (instructionPointer, outputCount) = PerformOperation(instruction, instructionPointer, operand, ref output, outputCount);
        }
        
        AnsiConsole.MarkupLineInterpolated($"Program Output: [green]{string.Join(',', output)}[/]");
    }
    
    // Nope, still don't have it.
    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);
        
        int instructionPointer = 0;
        int maxOutputSize = _instructions.Count;
        sbyte[] output = new sbyte[maxOutputSize];
        for (int i = 0; i < maxOutputSize; i++)
            output[i] = 0;
        
        sbyte[] target = _instructions.Select(x => (sbyte)x).ToArray();

        RegisterA = 0;
        bool success = false;
        int outputCount = 0;
        long lastChecked = 0;
        while (maxOutputSize != outputCount && !success)
        {
            RegisterA = lastChecked + 1;
            if (lastChecked % 10_000_000 == 0)
            {
                AnsiConsole.MarkupLineInterpolated($"Processed [yellow]{lastChecked}[/]");
            }
            lastChecked = RegisterA;
            RegisterB = RegisterC = outputCount = instructionPointer = 0;
            while (instructionPointer < _instructions.Count) {
                var instruction = _instructions[instructionPointer];
                bool checkAfter = instruction == Instruction.OUT;
            
                var operand = (int)_instructions[instructionPointer + 1];
                (instructionPointer, outputCount) = PerformOperation(instruction, instructionPointer, operand, ref output, outputCount);

                if (checkAfter)
                {
                    if (target[outputCount-1] != output[outputCount-1])
                    {
                        break;
                    }

                    if (outputCount == maxOutputSize)
                    {
                        success = true;
                        break;
                    }

                }
                
                
                if (outputCount > maxOutputSize)
                    break;
            }
            
        }
      
        AnsiConsole.MarkupLineInterpolated($"Register A is: [green]{lastChecked}[/]");
    }

    private (int instructionPointer, int outputCount) PerformOperation(Instruction instruction, int instructionPointer, int operand, ref sbyte[] output, int outputCount)
    {
        var operandValue = operand switch
        {
            < 4 => operand,
            4 => RegisterA,
            5 => RegisterB,
            6 => RegisterC
        };
        
        // DebugAnsiConsole.WriteLine("");
        switch (instruction)
        {
            case Instruction.ADV:
                // DebugAnsiConsole.MarkupLineInterpolated($"ADV Operation: [green]{RegisterA}[/] / [orange1]{Math.Pow(2, operandValue)}[/]");
                RegisterA /= (int)Math.Pow(2, operandValue);
                break;
            case Instruction.BXL:
                // DebugAnsiConsole.MarkupLineInterpolated($"BXL Operation: [red]{RegisterB}[/] ^ [orange1]{operand}[/]");
                RegisterB ^= operand;
                break;
            case Instruction.BST:
                // DebugAnsiConsole.MarkupLineInterpolated($"BST Operation: [red]{operandValue}[/] % [orange1]8[/]");
                RegisterB = operandValue % 8;
                break;
            case Instruction.JNZ:
                // DebugAnsiConsole.MarkupLineInterpolated($"JNZ Operation: [green]{operandValue}[/] > 0 ? Jump to  [orange1]{operand}[/]");
                if (RegisterA > 0)
                {
                    return (operand, outputCount);
                }
                break;
            case Instruction.BXC:
                // DebugAnsiConsole.MarkupLineInterpolated($"BXC Operation: [red]{RegisterB}[/] ^ [blue]{RegisterC}[/]");
                RegisterB ^= RegisterC;
                break;
            case Instruction.OUT:
                // DebugAnsiConsole.MarkupLineInterpolated($"OUT Operation: [red]{operandValue}[/] % [orange1]8[/]");
                output[outputCount] = (sbyte)(operandValue % 8);
                outputCount++;
                break;
            case Instruction.BDV:
                // DebugAnsiConsole.MarkupLineInterpolated($"BDV Operation: [green]{RegisterA}[/] / [orange1]{Math.Pow(2, operandValue)}[/]");
                RegisterB = RegisterA / (int)Math.Pow(2, operandValue);
                break;
            case Instruction.CDV:
                // DebugAnsiConsole.MarkupLineInterpolated($"BDC Operation: [green]{RegisterA}[/] / [orange1]{Math.Pow(2, operandValue)}[/]");
                RegisterC = RegisterA / (int)Math.Pow(2, operandValue);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
        }

        // DebugAnsiConsole.WriteLine("");

        return (instructionPointer + 2, outputCount);
    }

   
}