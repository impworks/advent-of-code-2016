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
	var path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt");
	var lines = File.ReadAllLines(path).Select(x => x.Trim()).ToArray();

	// handlers
	var handlers = new Handler[]
	{
		new Handler
		{
			Regex = new Regex("^rect (?<x>[0-9]+)x(?<y>[0-9]+)", RegexOptions.ExplicitCapture),
			Action = (d, args) => Rect(d, args["x"], args["y"])
		},
		new Handler
		{
			Regex = new Regex("^rotate row y=(?<y>[0-9]+) by (?<dist>[0-9]+)", RegexOptions.ExplicitCapture),
			Action = (d, args) => ShiftRow(d, args["y"], args["dist"])
		},
		new Handler
		{
			Regex = new Regex("^rotate column x=(?<x>[0-9]+) by (?<dist>[0-9]+)", RegexOptions.ExplicitCapture),
			Action = (d, args) => ShiftColumn(d, args["x"], args["dist"])
		},
	};
	
	// state
	var data = new bool[6,50];

	// processing
	foreach (var line in lines)
	{
		foreach (var handler in handlers)
		{
			var match = handler.Regex.Match(line);
			if (!match.Success)
				continue;

			var args = handler.Regex.GetGroupNames()
									.Skip(1) // main group
									.ToDictionary(x => x, x => int.Parse(match.Groups[x].Value));
			handler.Action(data, args);
        }
	}
	
	var render = Render(data);
	render.Dump();
}

private void Rect(bool[,] data, int width, int height)
{
	for(var y = 0; y < height; y++)
		for(var x = 0; x < width; x++)
			data[y,x] = true;
}

private void ShiftColumn(bool[,] data, int column, int distance)
{
	var rows = data.GetLength(0);
	var cols = data.GetLength(1);
	
	for (var step = 0; step < distance; step++)
	{
		var last = data[rows-1, column];

		for (var pos = rows - 2; pos >= 0; pos--)
			data[pos+1, column] = data[pos, column];
		
		data[0, column] = last;
	}
}

private void ShiftRow(bool[,] data, int row, int distance)
{
	var rows = data.GetLength(0);
	var cols = data.GetLength(1);

	for (var step = 0; step < distance; step++)
	{
		var last = data[row, cols-1];
		
		for(var pos = cols - 2; pos >= 0; pos --)
			data[row, pos+1] = data[row, pos];
			
		data[row, 0] = last;
	}
}

private char[,] Render(bool [,] data)
{
	var rows = data.GetLength(0);
	var cols = data.GetLength(1);
	var result = new char[rows, cols];

	for (var y = 0; y < rows; y++)
		for (var x = 0; x < cols; x++)
			result[y, x] = data[y, x] ? '#' : ' ';
			
	return result;
}

class Handler
{
	public Regex Regex;
	public Action<bool[,], Dictionary<string, int>> Action;
}

// Define other methods and classes here
