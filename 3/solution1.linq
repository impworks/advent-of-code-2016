<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

// source data
var path = Path.Combine(Path.GetDirectoryName (Util.CurrentQueryPath), "input.txt");
var lines = File.ReadAllLines(path);
var triangles = lines.Select(x => x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());

// counters
var possibleCount = 0;

// processing data
foreach (var triangle in triangles)
{
	var sum = triangle.Sum();
	var isValid = triangle.All(s => s < sum - s);
	if(isValid)
		possibleCount++;
}

possibleCount.Dump();