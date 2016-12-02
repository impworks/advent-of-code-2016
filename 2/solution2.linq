<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

// source data
var path = Path.Combine(Path.GetDirectoryName (Util.CurrentQueryPath), "input.txt");
var lines = File.ReadAllLines(path);

// numpad
var x = 0;
var y = 2;
var numpad = new[,]
{
	{' ', ' ', '1', ' ', ' '},
	{' ', '2', '3', '4', ' '},
	{'5', '6', '7', '8', '9'},
	{' ', 'A', 'B', 'C', ' '},
	{' ', ' ', 'D', ' ', ' '},
};

var code = "";
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
		if (newx >= 0 && newx <= 4 && newy >= 0 && newy <= 4)
		{
			// check if the key is a number
			if (numpad[newy, newx] != ' ')
			{
				x = newx;
				y = newy;
			}
		}
	}
	
	// store current number
	code += numpad[y, x];
}

// result
code.Dump();