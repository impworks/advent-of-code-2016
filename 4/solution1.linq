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
	var pattern = new Regex(@"^(?<data>[a-z-]+)-(?<id>[0-9]{3})\[(?<hash>[a-z]{5})\]$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
	
	var sum = 0;	
	foreach (var line in lines)
	{
		// get data from the line
		var match = pattern.Match(line.Trim());
		var data = match.Groups["data"].Value;
		var id = int.Parse(match.Groups["id"].Value);
		var hash = match.Groups["hash"].Value;
		
		// validate hash
		if(GetHash(data) == hash)
			sum += id;
	}
	
	sum.Dump();
}

private string GetHash(string data)
{
	var letters = data.Where(x => x != '-')
					  .GroupBy(x => x)
					  .OrderBy(x => x.Count())
					  .ThenBy(x => x.Key)
					  .Select(x => x.Key)
					  .Take(5)
					  .ToArray();
					  
	return new String(letters);
}

// Define other methods and classes here
