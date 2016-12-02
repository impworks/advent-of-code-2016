<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

// source data
var path = Path.Combine(Path.GetDirectoryName (Util.CurrentQueryPath), "input.txt");
var data = File.ReadAllText(path).Split(new[] { ", " }, StringSplitOptions.None);

// current coordinates & direction in grades
var x = 0;
var y = 0;
var orientation = 90;

var visitedPlaces = new HashSet<Tuple<int, int>>();

// path traversal
foreach (var elem in data)
{
	// rotate
	var dir = elem[0] == 'L' ? -1 : 1;
	orientation += dir * 90;
	var rads = orientation * Math.PI / 180.0;
	
	// step
	var steps = int.Parse(elem.Substring(1));
	for (var i = 0; i < steps; i++)
	{
		x -= (int)Math.Cos(rads);
		y -= (int)Math.Sin(rads);

		// check visited places
		var coords = Tuple.Create(x, y);
		if (!visitedPlaces.Contains(coords))
		{
			visitedPlaces.Add(coords);
			continue;
		}
		
		(Math.Abs(x) + Math.Abs(y)).Dump();
		return;
	}
}