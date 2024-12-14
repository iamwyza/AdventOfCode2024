using AdventOfCode2024.GridUtilities;

namespace AdventOfCode2024.Day13;

using Machine = (ulong aX, ulong aY, ulong bX, ulong bY, ulong prizeX, ulong prizeY );

internal partial class Day13 : DayBase
{
    public Day13()
    {
        Ready = true;
    }

    private readonly List<Machine> _machines = new();

    private async Task Init(int part, bool useTestData)
    {
        _machines.Clear();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        Machine currentMachine = new Machine();
        foreach (var line in lines)
        {
            if (line.Length == 0)
            {
                _machines.Add(currentMachine);
                currentMachine = new Machine();
                continue;
            }

            var matches = ButtonRegex().Matches(line);
            if (matches.Count == 0)
            {
                throw new Exception();
            }

            ulong x = ulong.Parse(matches[0].Groups[3].Value);
            ulong y = ulong.Parse(matches[0].Groups[4].Value);

            switch (matches[0].Groups[1].Value)
            {
                case "Button" when matches[0].Groups[2].Value == "A":
                    currentMachine.aX = x;
                    currentMachine.aY = y;
                    break;
                case "Button" when matches[0].Groups[2].Value == "B":
                    currentMachine.bX = x;
                    currentMachine.bY = y;
                    break;
                case "Prize":
                    currentMachine.prizeX = x;
                    currentMachine.prizeY = y;
                    break;
            }
        }

        _machines.Add(currentMachine);
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        ulong totalTokens = 0;
        foreach (var machine in _machines)
        {
            AnsiConsole.MarkupLineInterpolated(
                $"Button [blue]A[/]: X+[yellow]{machine.aX}[/], Y+[yellow]{machine.aY}[/]");
            AnsiConsole.MarkupLineInterpolated(
                $"Button [blue]B[/]: X+[yellow]{machine.bX}[/], Y+[yellow]{machine.bY}[/]");
            AnsiConsole.MarkupLineInterpolated($"Prize: X=[yellow]{machine.prizeX}[/], Y=[yellow]{machine.prizeY}[/]");
            AnsiConsole.WriteLine("==========================================================");

            // ulong remainingX = machine.prizeX;
            // ulong remainingY = machine.prizeY;
            // ulong tokensSpent = 0;
            //
            // bool isBFirst = true;
            //
            // if (machine.prizeX > machine.prizeY)
            // {
            //     if (machine.aX > machine.bX * 3)
            //     {
            //         isBFirst = false;
            //     }
            // }
            // else
            // {
            //     if (machine.aY > machine.bY * 3)
            //     {
            //         isBFirst = false;
            //     }
            // }
            //
            // (bool success, remainingX, remainingY, tokensSpent, ulong firstButtonPresses) = PressButtons(isBFirst,
            //     remainingX, remainingY, tokensSpent, machine);
            //
            // ulong secondButtonPresses = 0;
            //
            // if (!success)
            // {
            //     (success, remainingX, remainingY, tokensSpent, secondButtonPresses) = PressButtons(!isBFirst,
            //         remainingX, remainingY, tokensSpent,  machine);
            // }
            //
            // if (!success)
            // {
            //     ulong firstX = isBFirst ? machine.bX : machine.aX;
            //     ulong firstY = isBFirst ? machine.bY : machine.aY;
            //     ulong secondX = isBFirst ? machine.aX : machine.bX;
            //     ulong secondY = isBFirst ? machine.aY : machine.bY;
            //     ulong tokenAdd = isBFirst ? 3 : 1;
            //     ulong tokenRemove = isBFirst ? 1 : 3;
            //     
            //
            //     while (firstButtonPresses > 0)
            //     {
            //         firstButtonPresses--;
            //         remainingX += firstX;
            //         remainingY += firstY;
            //         tokensSpent -= tokenRemove;
            //
            //         if (remainingX % secondX == 0 && remainingY % secondY == 0)
            //         {
            //             ulong countX = remainingX / secondX;
            //             ulong countY = remainingY / secondY;
            //
            //             if (countX != countY || (countX + secondButtonPresses) > 100)
            //             {
            //                 break;
            //             }
            //
            //             success = true;
            //             tokensSpent += (countX * tokenAdd);
            //             break;
            //         }
            //
            //         //AnsiConsole.MarkupLineInterpolated($"Spent Undid 1 button X+[green]{firstX}[/], Y+[green]{firstY}[/]. Remaining X: [blue]{remainingX}[/], Y: [blue]{remainingY}[/], T: [red]{firstButtonPresses}[/]");
            //     }
            // }

            bool success = false;
            ulong tokensSpent = 0;

            if (!success)
            {
                ulong best = int.MaxValue;
                ulong besta = 0;
                ulong bestb = 0;

                for (ulong a = 0; a <= 100; a++)
                {
                    for (ulong b = 0; b <= 100; b++)
                    {
                        if (machine.prizeX == machine.aX * a + machine.bX * b &&
                            machine.prizeY == machine.aY * a + machine.bY * b)
                        {
                            if (best > b + a * 3)
                            {
                                best = b + a * 3;
                                besta = a;
                                bestb = b;
                                success = true;
                            }
                        }
                    }
                }

                if (success)
                {
                    AnsiConsole.MarkupLineInterpolated(
                        $"Fallback attempt success: A=[red]{besta}[/] B=[red]{bestb}[/]");
                    tokensSpent = best;
                }
            }

            if (success)
            {
                totalTokens += tokensSpent;
                AnsiConsole.MarkupLineInterpolated($"Got a prize! Cost [green]{tokensSpent}[/]");
                AnsiConsole.WriteLine();
            }
        }

        AnsiConsole.MarkupLineInterpolated($"Spent a total of [green]{totalTokens}[/] tokens.");
    }

    // Unsolved....still not sure how to approach this.
    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, true);

        ulong totalTokens = 0;
        foreach (var machine in _machines)
        {
            ulong prizeX = machine.prizeX + 10000000000000UL;
            ulong prizeY = machine.prizeY + 10000000000000UL;
            AnsiConsole.MarkupLineInterpolated(
                $"Button [blue]A[/]: X+[yellow]{machine.aX}[/], Y+[yellow]{machine.aY}[/]");
            AnsiConsole.MarkupLineInterpolated(
                $"Button [blue]B[/]: X+[yellow]{machine.bX}[/], Y+[yellow]{machine.bY}[/]");
            AnsiConsole.MarkupLineInterpolated(
                $"Prize: X=[yellow]{prizeX}[/], Y=[yellow]{prizeY}[/]");
            AnsiConsole.WriteLine("==========================================================");


            bool success = false;
            ulong tokensSpent = 0;


            ulong best = int.MaxValue;
            ulong besta = 0;
            ulong bestb = 0;


            // now see if the buttons can handle the rest of the values.
            var prizeOne = 0UL;
            var prizeTwo = 0UL;
            var oneX = 0UL;
            var oneY = 0UL;
            var twoX = 0UL;
            var twoY = 0UL;
            var oneTokenCost = 0UL;
            var twoTokenCost = 0UL;

            if (prizeX < prizeY)
            {
                if (machine.aY > machine.bY * 3)
                {
                    oneTokenCost = 3;
                    twoTokenCost = 1;
                    oneY = machine.aY;
                    twoY = machine.bY;
                    oneX = machine.aX;
                    twoX = machine.bX;
                }
                else
                {
                    oneTokenCost = 1;
                    twoTokenCost = 3;
                    oneY = machine.bY;
                    twoY = machine.aY;
                    oneX = machine.bX;
                    twoX = machine.aX;
                }
            }
            else
            {
                if (machine.aX > machine.bX * 3)
                {
                    oneTokenCost = 3;
                    twoTokenCost = 1;
                    oneY = machine.aY;
                    twoY = machine.bY;
                    oneX = machine.aX;
                    twoX = machine.bX;
                }
                else
                {
                    oneTokenCost = 1;
                    twoTokenCost = 3;
                    oneY = machine.bY;
                    twoY = machine.aY;
                    oneX = machine.bX;
                    twoX = machine.aX;
                }
            }

            ulong remainingPrizeY, remainingPrizeX;
            ulong tokensSoFar = 0;

            // start by pressing the "best button" as many times as needed to get the number down to a manageable amount
            if (prizeX < prizeY)
            {
                var buttonPresses = prizeY / oneY;
                remainingPrizeX = prizeX - (buttonPresses * oneX);
                remainingPrizeY = prizeY - (buttonPresses * oneY);
                tokensSoFar += buttonPresses * oneTokenCost;
            }
            else
            {
                var buttonPresses = prizeX / oneX;
                remainingPrizeX = prizeX - (buttonPresses * oneX);
                remainingPrizeY = prizeY - (buttonPresses * oneY);
                tokensSoFar += buttonPresses * oneTokenCost;
            }

            // Now with the remainders, lets go ahead and see if the other button can do anything

            if (prizeX < prizeY)
            {
                var buttonPresses = remainingPrizeY / twoY;
                remainingPrizeX -= (buttonPresses * twoX);
                remainingPrizeY -= (buttonPresses * twoY);
                tokensSoFar += buttonPresses * twoTokenCost;
            }
            else
            {
                var buttonPresses = remainingPrizeX / twoX;
                remainingPrizeX -= (buttonPresses * twoX);
                remainingPrizeY -= (buttonPresses * twoY);
                tokensSoFar += buttonPresses * twoTokenCost;
            }

            if (remainingPrizeX == 0 && remainingPrizeY == 0)
            {
                success = true;
            }
            
            AnsiConsole.MarkupLineInterpolated($"Remainder is X=[blue]{remainingPrizeX}[/], Y=[blue]{remainingPrizeY}[/]");

            if (!success)
            {
                // wasn't an even spilt,so lets try pushing out and switching the values a little to see if there is any number of presses that might get us there. (capped at 10k tries)
                for (int i = 0; i < 10000; i++)
                {
                    remainingPrizeX += oneX;
                    remainingPrizeY += oneY;
                    tokensSoFar -= oneTokenCost;

                    remainingPrizeX -= twoX;
                    remainingPrizeY -= twoY;
                    tokensSoFar += twoTokenCost;

                    if (remainingPrizeX == 0 && remainingPrizeY == 0)
                    {
                        success = true;
                        break;
                    }

                    if (remainingPrizeX % oneX == 0 && remainingPrizeY % oneY == 0)
                    {
                        var presses = remainingPrizeX / oneX;
                        if (remainingPrizeY == oneY * presses)
                        {
                            success = true;
                            tokensSoFar += oneTokenCost * presses;
                            break;
                        }
                    }

                    if (remainingPrizeX % twoX == 0 && remainingPrizeY % twoY == 0)
                    {
                        var presses = remainingPrizeX / twoX;
                        if (remainingPrizeY == twoY * presses)
                        {
                            success = true;
                            tokensSoFar += twoTokenCost * presses;
                            break;
                        }
                    }
                }
            }

            tokensSpent = tokensSoFar;

            if (success)
            {
                totalTokens += tokensSpent;
                AnsiConsole.MarkupLineInterpolated($"Got a prize! Cost [green]{tokensSpent}[/]");
                AnsiConsole.WriteLine();
            }
        }

        AnsiConsole.MarkupLineInterpolated($"Spent a total of [green]{totalTokens}[/] tokens.");
    }

    [GeneratedRegex(@"(Button|Prize) ?(\w)?: X.(\d+), Y.(\d+)")]
    private static partial Regex ButtonRegex();
}