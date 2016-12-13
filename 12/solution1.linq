<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

void Main()
{
	// source data
	var path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt");
	var source = File.ReadAllLines(path);

	var processor = new Processor(source);
	processor.Compile();
	processor.Process();

	processor.GetRegister("a").Dump();
}

class Processor
{
	public Processor(string[] source)
	{
		_source = source;
		_registers = new Dictionary<string, int>
		{
			["a"] = 0,
			["b"] = 0,
			["c"] = 0,
			["d"] = 0,
		};

		_commands = new[]
		{
			new Command
			{
				Regex = new Regex("cpy (?<from>[abcd]) (?<to>[abcd])"),
				Handler = args => () => _registers[args["to"]] = _registers[args["from"]]
			},
			new Command
			{
				Regex = new Regex("cpy (?<value>[0-9]+) (?<to>[abcd])"),
				Handler = args => () => _registers[args["to"]] = int.Parse(args["value"])
			},
			new Command
			{
				Regex = new Regex("inc (?<reg>[abcd])"),
				Handler = args => () => _registers[args["reg"]]++
			},
			new Command
			{
				Regex = new Regex("dec (?<reg>[abcd])"),
				Handler = args => () => _registers[args["reg"]]--
			},
			new Command
			{
				Regex = new Regex("jnz (?<reg>[abcd]) (?<dist>-?[0-9]+)"),
				Handler = args => () => _cursor += _registers[args["reg"]] == 0
					? 0
					: int.Parse(args["dist"]) - 1, // compensate ++ later
			},
			new Command
			{
				Regex = new Regex("jnz (?<value>-?[0-9]+) (?<dist>-?[0-9]+)"),
				Handler = args => () => _cursor += int.Parse(args["value"]) == 0
					? 0
					: int.Parse(args["dist"]) - 1, // compensate ++ later
			},
		};
	}

	private Command[] _commands;

	private string[] _source;
	private Action[] _compiledSource;

	private Dictionary<string, int> _registers;
	private int _cursor;

	public int GetRegister(string name) => _registers[name];
	
	public void Compile()
	{
		var compiledSource = new Action[_source.Length];

		// pre-parses all commands into functions to speed up execution
		for (var idx = 0; idx < _source.Length; idx++)
		{
			var line = _source[idx];
			foreach (var cmd in _commands)
			{
				var match = cmd.Regex.Match(line);
				if (!match.Success)
					continue;

				var args = cmd.Regex.GetGroupNames()
									.Where(x => x != "0")
									.ToDictionary(x => x, x => match.Groups[x].Value);
				compiledSource[idx] = cmd.Handler(args);
			}
		}
		
		_compiledSource = compiledSource;
	}

	public void Process()
	{
		while (_cursor < _compiledSource.Length)
		{
			_compiledSource[_cursor]();
			_cursor++;
		}
	}
}

class Command
{
	public Regex Regex;
	public Func<Dictionary<string, string>, Action> Handler;
}
