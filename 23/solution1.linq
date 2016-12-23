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
			["a"] = 7,
			["b"] = 0,
			["c"] = 0,
			["d"] = 0,
		};

		_commands = new Dictionary<string, System.Action<string[]>>
		{
			["cpy"] = Copy,
			["inc"] = Inc,
			["dec"] = Dec,
			["jnz"] = Jnz,
			["tgl"] = Toggle
		};
	}

	private string[] _source;
	private ByteCode[] _compiledSource;

	private Dictionary<string, Action<string[]>> _commands;
	private Dictionary<string, int> _registers;
	private int _cursor;

	public int GetRegister(string name) => _registers[name];

	public void Compile()
	{
		var compiledSource = new ByteCode[_source.Length];

		// pre-parses all commands 
		for (var idx = 0; idx < _source.Length; idx++)
		{
			var instruction = _source[idx].Split(' ');
			compiledSource[idx] = new ByteCode
			{
				Command = instruction[0],
				Args = instruction.Skip(1).ToArray()
			};
		}

		_compiledSource = compiledSource;
	}

	public void Process()
	{
		while (_cursor < _compiledSource.Length)
		{
			var bytecode = _compiledSource[_cursor];
			_commands[bytecode.Command](bytecode.Args);
			_cursor++;
		}
	}
	
	public void Copy(string[] args)
	{
		var from = args[0];
		var to = args[1];
		
		if(IsRegister(to))	
			_registers[to] = GetValue(from);
	}
	
	public void Inc(string[] args)
	{
		var reg = args[0];
		
		if(IsRegister(reg))
			_registers[reg]++;
	}
	
	public void Dec(string[] args)
	{
		var reg = args[0];

		if (IsRegister(reg))
			_registers[reg]--;
	}
	
	public void Jnz(string[] args)
	{
		var value = GetValue(args[0]);
		var offset = GetValue(args[1]);

		if (value != 0)		
			_cursor += offset - 1;
	}
	
	public void Toggle(string[] args)
	{
		var pos = _cursor + GetValue(args[0]);
		if(pos < 0 || pos >= _compiledSource.Length)
			return;
			
		var bytecode = _compiledSource[pos];
		if (bytecode.Args.Length == 1)
			bytecode.Command = bytecode.Command == "inc" ? "dec" : "inc";
		else
			bytecode.Command = bytecode.Command == "jnz" ? "cpy" : "jnz";
	}
	
	private bool IsRegister(string str)
	{
		return str.Length == 1
			&& str[0] >= 'a'
			&& str[0] <= 'd';
	}
	
	private int GetValue(string val)
	{
		return IsRegister(val) ? _registers[val] : int.Parse(val);
	}
}

class ByteCode
{
	public string Command;
	public string[] Args;
}