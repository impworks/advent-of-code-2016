<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

// input
var path = Path.Combine(Path.GetDirectoryName (Util.CurrentQueryPath), "input.txt");
var lines = File.ReadAllLines(path).Select(x => x.Trim()).ToArray();

// values
var result = "";

for (var pos = 0; pos < lines[0].Length; pos++)
{
	var charDistribution = lines.Select(x => x[pos])
								.GroupBy(x => x)
								.OrderBy(x => x.Count());
								
	result += charDistribution.First().Key;
}

result.Dump();
