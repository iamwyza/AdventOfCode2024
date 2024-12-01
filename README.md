# AdventOfCode2024

When copying from last year to this year.  Use the following to reset the files:

_note: replace 2023 and 2024 with the correct values from last year your copying from to this year._
```bash
# Replace all instances of the current year for the new one in names of files.  Note you may need to run this twice
find . -name '*2023*' -exec sh -c 'for path; do mv "$path" "$(dirname "$path")/$(basename "$path" | sed "s/2023/2024/g")"; done' _ {} +

# Replace all instances of the current year for the new one in the contents of the files.
find . -type f -exec grep -l '2023' {} \; -exec sed -i 's/2023/2024/g' {} +

cd AdventOfCode2024

# Clear out each days input files
find . -name "*.txt" -exec truncate --size 0 {} \;

# Copies Day00/Day00.cs to all Days
find . -name "Day*.cs" ! -name "Day00.cs" ! -name "DayBase.cs" ! -exec cp ./Day00/Day00.cs {} \;

# Copies Replaces Day00 with the appropriate Day## in each file's contents 
find . -name "Day*.cs" ! -name "Day00.cs" ! -name "DayBase.cs" ! -exec sh -c 'for file; do name=${file##*/}; base=${name%.cs}; sed -i "s/Day00/$base/g" "$file"; done' _ {} +
```
