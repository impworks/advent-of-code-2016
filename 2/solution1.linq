<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

// source data
var path = Path.Combine(Path.GetDirectoryName (Util.CurrentQueryPath), "input.txt");
var lines = File.ReadAllLines(path);

// numpad
var x = 1;
var y = 1;
var numpad = new[,]
{
	{1, 2, 3},
	{4, 5, 6},
	{7, 8, 9}
};

var code = "";

var visitedPlaces = new HashSet<Tuple<int, int>>();

// path traversal
foreach (var line in lines)
{
	// walk along the numpad
	foreach (var cmd in line)
	{
		var newx = x;
		var newy = y;

		if (cmd == 'U')
			newy--;
		else if (cmd == 'D')
			newy++;
		else if (cmd == 'L')
			newx--;
		else if (cmd == 'R')
			newx++;

		// check bounds
		if (newx >= 0 && newx <= 2 && newy >= 0 && newy <= 2)
		{
			x = newx;
			y = newy;
		}
	}

	// store current number
	code += numpad[y, x].ToString();
}

code.Dump();