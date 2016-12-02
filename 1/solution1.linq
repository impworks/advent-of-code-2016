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

// path traversal
foreach (var elem in data)
{
	// rotate
	var dir = elem[0] == 'L' ? -1 : 1;
	orientation += dir * 90;
	var rads = orientation * Math.PI / 180.0;
	
	// steps
	var steps = int.Parse(elem.Substring(1));	
	x += (int)(steps * Math.Cos(rads));
	y += (int)(steps * Math.Sin(rads));
}

// solution
(Math.Abs(x) + Math.Abs(y)).Dump();