using AdventOfCode2024.GridUtilities;

namespace AdventOfCode2024.Day09;
internal class Day09 : DayBase
{

    public Day09()
    {
        Ready = true;
    }

    private int[] _map;

    [MemberNotNull(nameof(_map))]
    private async Task Init(int part, bool useTestData)
    {
        

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        _map = lines[0].Select(c => int.Parse(c.ToString())).ToArray();

    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        List<int?> diskMap = new List<int?>();
        List<int> emptyLocations = new List<int>();

        var id = 0;
        var index = 0;

        for (int i = 0; i < _map.Length; i++)
        {
            var val = (int?)null;
            
            if (i % 2 == 0)
            {
                val = id;
                id++;
                
            }
            
            for (int z = 0; z < _map[i]; z++)
            {
                
                diskMap.Add(val);
                if (val is null)
                {
                    emptyLocations.Add(index);
                }
                index++;
            }
        }

        var emptyLocationsArray = emptyLocations.ToArray();

        index = 0;
        var lastLocation = 0;
        for (var i = diskMap.Count-1; i > 0; i--) 
        {
            var item = diskMap[i];
            if (item is not null)
            {
                diskMap[emptyLocationsArray[index++]] = item;
                diskMap[i] = null;
                //DebugAnsiConsole.WriteLine(string.Join("", diskMap.Select(c => c.HasValue ? c.Value.ToString() : ".")));

                lastLocation = i;
                if (index == emptyLocationsArray.Length+1) break;
                if (emptyLocationsArray[index] > i) break; // We moved past the next available empty location, so we're done.
            }
        }

        // I have no earthly idea why the code doesn't do the very last swap, but here we are.
        diskMap[lastLocation] = diskMap[lastLocation+1];
        diskMap[lastLocation+1] = null;

        AnsiConsole.WriteLine(string.Join("", diskMap.Select(c => c.HasValue ? "#" : ".")));

        var total = 0L;
        
        for (int i = 0; i < diskMap.Count; i++)
        {
            if (diskMap[i] is not null)
            {
                total += diskMap[i]!.Value * i;
            }
        }
        
        AnsiConsole.MarkupLineInterpolated($"The checksum is [green]{total}[/]");


    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, false);

        List<int?> diskMap = new List<int?>();
        List<(int location, int size)> emptyLocations = new();
        List<(int location, int size, int id)> fileLocations = new();

        var id = 0;
        var index = 0;

        for (int i = 0; i < _map.Length; i++)
        {
            var val = (int?)null;

            if (i % 2 == 0)
            {
                val = id;
                id++;
            }
            
            for (int z = 0; z < _map[i]; z++)
            {
                diskMap.Add(val);
            }


            if (_map[i] == 0) continue;

            if (val is null)
            {
                emptyLocations.Add((index, _map[i]));
            }
            else
            {
                fileLocations.Add((index, _map[i], val.Value));
            }

            index += _map[i];


        }

        fileLocations.Reverse();
        
        for (var i = 0; i < fileLocations.Count; i++)
        {
            var (location, size, fileId) = fileLocations[i];

            for (var index1 = 0; index1 < emptyLocations.Count; index1++)
            {
                var emptyLocation = emptyLocations[index1];
                if (emptyLocation.size >= size  && emptyLocation.location < location)
                {
                    var moveTo = emptyLocation.location;
                    fileLocations[i] = (moveTo, size, fileId);
                    var remaining = emptyLocation.size - size;
                    if (remaining > 0)
                    {
                        emptyLocations[index1] = (emptyLocation.location + size, remaining);
                    }
                    else
                    {
                        emptyLocations.RemoveAt(index1);
                    }

                    AnsiConsole.MarkupLineInterpolated($"Moving {fileId} of {size} from {location} to {moveTo}");
                    break;
                }
            }
        }
        AnsiConsole.WriteLine(string.Join("", diskMap.Select(c => c.HasValue ? c.Value.ToString() : ".")));

        var total = 0L;
        for (var i = 0; i < diskMap.Count; i++)
        {
            diskMap[i] = null;
        }

        foreach (var (location, size, fileId) in fileLocations)
        {
            for (int i = 0; i < size; i++)
            {
                diskMap[location + i] = fileId;
                total += (location + i) * fileId;
            }
        }


        AnsiConsole.WriteLine(string.Join("", diskMap.Select(c => c.HasValue ? c.Value.ToString() : ".")));
        
        AnsiConsole.MarkupLineInterpolated($"The checksum is [green]{total}[/]");


    }
}