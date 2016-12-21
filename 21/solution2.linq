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
	var lines = File.ReadAllLines(path);

	// process
	var scrambler = new Scrambler("fbgdceah");
	var result = scrambler.Unscramble(lines);

	result.Dump();
}

class Scrambler
{
	public Scrambler(string source)
	{
		_source = source;

		_commands = new[]
		{
			new Command
			{
				Regex = new Regex("^swap position (?<pos1>[0-9]) with position (?<pos2>[0-9])"),
				Callback = (data, args) => SwapPosition(data, int.Parse(args["pos1"]), int.Parse(args["pos2"]))
			},
			new Command
			{
				Regex = new Regex("^swap letter (?<ch1>[a-z]) with letter (?<ch2>[a-z])"),
				Callback = (data, args) => SwapLetter(data, args["ch1"][0], args["ch2"][0])
			},
			new Command
			{
				Regex = new Regex("^rotate (?<dir>left|right) (?<dist>[0-9]+) steps?"),
				Callback = (data, args) => Rotate(data, int.Parse(args["dist"]), args["dir"] == "right")
			},
			new Command
			{
				Regex = new Regex("^rotate based on position of letter (?<ch>[a-z])"),
				Callback = (data, args) => RotateByChar(data, args["ch"][0])
			},
			new Command
			{
				Regex = new Regex("^reverse positions (?<start>[0-9]) through (?<end>[0-9])"),
				Callback = (data, args) => Reverse(data, int.Parse(args["start"]), int.Parse(args["end"]))
			},
			new Command
			{
				Regex = new Regex("^move position (?<from>[0-9]) to position (?<to>[0-9])"),
				Callback = (data, args) => Move(data, int.Parse(args["to"]), int.Parse(args["from"]))
			},
		};
	}

	private readonly string _source;
	private readonly Command[] _commands;

	public string Unscramble(string[] lines)
	{
		var data = _source.ToCharArray();

		foreach (var line in lines.Reverse())
		{
			var matched = false;

			foreach (var command in _commands)
			{
				var match = command.Regex.Match(line);
				if (!match.Success)
					continue;

				var args = command.Regex.GetGroupNames().ToDictionary(x => x, x => match.Groups[x].Value);
				command.Callback(data, args);
				matched = true;
				break;
			}

			if (!matched)
				throw new Exception("No command matched for line:\n\n" + line);
		}

		return new string(data);
	}

	private void SwapPosition(char[] data, int pos1, int pos2)
	{
		var tmp = data[pos1];
		data[pos1] = data[pos2];
		data[pos2] = tmp;
	}

	private void SwapLetter(char[] data, char ch1, char ch2)
	{
		for (var idx = 0; idx < data.Length; idx++)
		{
			if (data[idx] == ch1)
				data[idx] = ch2;
			else if (data[idx] == ch2)
				data[idx] = ch1;
		}
	}

	private void Rotate(char[] data, int dist, bool left)
	{	
		var src = data.ToArray();
		var srcStart = (dist * (left ? 1 : -1)) % data.Length;
		if (srcStart < 0)
			srcStart = data.Length + srcStart;

		for (var i = 0; i < data.Length; i++)
			data[i] = src[(srcStart + i) % data.Length];
	}

	private void RotateByChar(char[] data, char ch)
	{
		// original, shift, new:
		// 0         1      1
		// 1         2      3
		// 2         3      5
		// 3         4      7
		// 4         6      2
		// 5         7      4
		// 6         8      6
		// 7         9      0
		
		var idx = Array.IndexOf(data, ch);
		var pos = idx / 2 + (idx % 2 == 0 && idx != 0 ? 5 : 1);
			
		Rotate(data, pos, left: true);
	}

	private void Reverse(char[] data, int start, int end)
	{
		var revCount = (end - start + 1) / 2;
		for (var i = 0; i < revCount; i++)
		{
			var startIdx = start + i;
			var endIdx = end - i;

			SwapPosition(data, startIdx, endIdx);
		}
	}

	private void Move(char[] data, int from, int to)
	{
		var result = new List<char>(data);
		var ch = result[from];
		result.RemoveAt(from);

		// special case for moving the char to the end of the list
		if (to == result.Count)
			result.Add(ch);
		else
			result.Insert(to, ch);

		for (var idx = 0; idx < data.Length; idx++)
			data[idx] = result[idx];
	}
}

class Command
{
	public Regex Regex;
	public Action<char[], Dictionary<string, string>> Callback;
}