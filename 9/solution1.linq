<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>JsonFormatting = Newtonsoft.Json.Formatting</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Net.Http</Namespace>
</Query>

// source data
var path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt");
var source = File.ReadAllText(path);
var markerRegex = new Regex(@"\((?<len>[0-9]+)x(?<repeat>[0-9]+)\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

// state
var position = 0;
var length = 0;

while (true)
{
	var match = markerRegex.Match(source, position);
	if (match.Success)
	{
		// append data unaffected by markers
		if (match.Index > position)
			length += match.Index - position;

		// repeat data
		var dataLength = int.Parse(match.Groups["len"].Value);
		var repeatCount = int.Parse(match.Groups["repeat"].Value);
		length += dataLength * repeatCount;

		// shift position
		var dataStart = match.Index + match.Length;
		position = dataStart + dataLength;
	}
	else
	{
		// append remaining bit
		length += source.Length - position;
		break;
	}
}

length.Dump();