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
		var subnetParts = parts.Where((value, idx) => idx % 2 == 1);
		
		var isCorrect = parts.Any(HasAbba) && !subnetParts.Any(HasAbba);
		if(isCorrect)
			count++;
	}
	
	count.Dump();
}

private bool HasAbba(string str)
{
	var rx = new Regex(@"([a-z])([a-z])\2\1");
	var match = rx.Match(str);
	return match.Success && match.Value[0] != match.Value[1];	
}

// Define other methods and classes here
