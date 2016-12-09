<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

void Main()
{
	// source data
	var path = Path.Combine(Path.GetDirectoryName (Util.CurrentQueryPath), "input.txt");
	var source = File.ReadAllText(path);
		
	var unpacker = new Unpacker(source);
	unpacker.GetLength().Dump();
}

class Unpacker
{
	public Unpacker(string source)
	{
		_source = source;
		_markerRegex = new Regex(@"\((?<len>[0-9]+)x(?<repeat>[0-9]+)\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
	}
	
	private string _source;
	private Regex _markerRegex;
	
	public long GetLength()
	{
		return GetSegmentLength(0, _source.Length);	
	}
	
	private long GetSegmentLength(int start, int sourceLength)
	{
		var length = 0L;
		var position = start;

		while (true)
		{
			var match = _markerRegex.Match(_source, position, sourceLength);
			if (!match.Success)
				break;
				
			length += match.Index - position;

			var dataLength = int.Parse(match.Groups["len"].Value);
			var repeatCount = int.Parse(match.Groups["repeat"].Value);
			var dataStart = match.Index + match.Length;
			
			var subPartLength = GetSegmentLength(dataStart, dataLength);
			length += subPartLength * repeatCount;
			
			position = dataStart + dataLength;
			
			if(position >= start + sourceLength)
				break;
		}
		
		length += start + sourceLength - position;

		return length;
	}
}

// Define other methods and classes here
