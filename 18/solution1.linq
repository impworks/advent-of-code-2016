<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

void Main()
{
    // input data
    var rowCount = 40;
    var input = @".^^^.^.^^^^^..^^^..^..^..^^..^.^.^.^^.^^....^.^...^.^^.^^.^^..^^..^.^..^^^.^^...^...^^....^^.^^^^^^^";
    var row = input.ToCharArray()
                   .Select(x => x == '.')
                   .ToArray();
                                                                                                                       
    // state
    var safeCells = row.Count(x => x);

    // process floors
    for (var rowId = 1; rowId < rowCount; rowId++)
    {
        row = GetNextRow(row);
        safeCells += row.Count(x => x);
    }
    
    safeCells.Dump();
}

private bool[] GetNextRow(bool[] row)
{
    var result = new bool[row.Length];

    for (var pos = 0; pos < row.Length; pos++)
    {
        // rules in the task are superfluous!
        // we only need to check left == right
        var left = IsSafe(row, pos - 1);
        var right = IsSafe(row, pos + 1);
        
        result[pos] = left == right;
    }
    
    return result;
}

private bool IsSafe(bool[] row, int pos)
{
    return pos < 0 || pos >= row.Length || row[pos];
}
