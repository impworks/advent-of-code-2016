<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

void Main()
{
	// input
	var path = Path.Combine(Path.GetDirectoryName (Util.CurrentQueryPath), "input.txt");
	var lines = File.ReadAllLines(path).Select(x => x.Trim()).ToArray();

	// variables
	var count = 0;
	
	foreach (var line in lines)
	{
		var parts = line.Split('[', ']');
		var outerParts = parts.Where((value, idx) => idx % 2 == 0).ToList();
		var innerParts = parts.Where((value, idx) => idx % 2 == 1).ToList();
		
		var babs = outerParts.SelectMany(GetBABs).ToList();
		var isCorrect = innerParts.Any(x => babs.Any(y => x.Contains(y)));
		
		if(isCorrect)
			count++;
	}
	
	count.Dump();
}

private List<string> GetBABs(string str)
{
	// allow overlapping matches in cases like 'ababa' => aba, bab, aba
	var rx = new Regex(@"([a-z])(?=(?<cap>[a-z]\1))");
	var matches = rx.Matches(str);
	return matches.OfType<Match>()
				  .Select(x => x.Groups["cap"].Value)
				  .Where(x => x[0] != x[1]) // a != b
				  .Select(x => x + x[0])    // ba => bab
				  .ToList();
}

// Define other methods and classes here
