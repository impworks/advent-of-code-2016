<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>JsonFormatting = Newtonsoft.Json.Formatting</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Net.Http</Namespace>
</Query>

void Main()
{
	// input
	var nodes = LoadNodes().ToList();
	
	// state
	var count = 0;

	// processing
	for (var idx1 = 0; idx1 < nodes.Count - 1; idx1++)
	{
		for (var idx2 = idx1 + 1; idx2 < nodes.Count; idx2++)
		{
			var node1 = nodes[idx1];
			var node2 = nodes[idx2];
			
			if(node1.UsedSpace != 0 && node2.FreeSpace >= node1.UsedSpace)
				count++;
				
			if(node2.UsedSpace != 0 && node1.FreeSpace >= node2.UsedSpace)
				count++;
		}
	}
	
	count.Dump();
}

private IEnumerable<Node> LoadNodes()
{
	var path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt");
	var lines = File.ReadAllLines(path).Skip(2);

	foreach (var line in lines)
	{
		var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		var nameParts = parts[0].Split('-');

		yield return new Node
		{
			X = int.Parse(nameParts[1].Substring(1)),
			Y = int.Parse(nameParts[2].Substring(1)),
			
			UsedSpace = int.Parse(parts[2].Replace("T", "")),
			FreeSpace = int.Parse(parts[3].Replace("T", ""))
		};
	}
}

private class Node
{
	public int X;
	public int Y;
	
	public int UsedSpace;
	public int FreeSpace;
}
