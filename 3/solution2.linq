<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

void Main()
{
	// source data
	var path = Path.Combine(Path.GetDirectoryName (Util.CurrentQueryPath), "input.txt");
	var lines = File.ReadAllLines(path);
	
	var sourceTriangles = lines.Select(x => x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
	var triangles = TransposeTriangles(sourceTriangles);
	
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
}

public IEnumerable<int[]> TransposeTriangles(IEnumerable<int[]> source)
{
	var fixedSource = source.ToArray();

	for (var x = 0; x < 3; x++)
	{
		for (var y = 0; y < fixedSource.Length; y += 3)
		{
			// this is safe because number of rows always divides by 3
			yield return new[]
			{
				fixedSource[y][x],
				fixedSource[y+1][x],
				fixedSource[y+2][x],
			};
		}
	}
}

// Define other methods and classes here
