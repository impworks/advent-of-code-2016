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

	var processor = new Processor(source, true);
	processor.Compile();
	processor.Process();

	processor.GetRegister("a").Dump();
}

class Processor
{
	public Processor(string[] source, bool optimize = true)
	{
		_source = source;
		_allowOptimization = optimize;
		_registers = new Dictionary<string, int>
		{
			["a"] = 12,
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

	private bool _allowOptimization;
	private string[] _source;
	private ByteCode[] _compiledSource;

	private Dictionary<string, Action<string[]>> _commands;
	private Dictionary<string, int> _registers;
	private int _cursor;

	public int GetRegister(string name) => _registers[name];	
	
	// gets statistics about the hottest loops
	public uint GetSteps() => _compiledSource.Aggregate(0u, (a, x) => a + x.Executions);
	public List<Tuple<int, uint, string>> GetStats() => _compiledSource.Select((x, idx) => Tuple.Create(idx, x.Executions, x.Command + " " + string.Join(" ", x.Args)))
																	   .ToList();
	
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
		Optimize();
		
		while (_cursor < _compiledSource.Length)
		{
			var bytecode = _compiledSource[_cursor];
			bytecode.Executions++;
			
			if(bytecode.Optimized != null)
				bytecode.Optimized();
			else
				_commands[bytecode.Command](bytecode.Args);
				
			_cursor++;
		}
	}

	private void Copy(string[] args)
	{
		var from = args[0];
		var to = args[1];

		if (IsRegister(to))
			_registers[to] = GetValue(from);
	}

	private void Inc(string[] args)
	{
		var reg = args[0];

		if (IsRegister(reg))
			_registers[reg]++;
	}

	private void Dec(string[] args)
	{
		var reg = args[0];

		if (IsRegister(reg))
			_registers[reg]--;
	}

	private void Jnz(string[] args)
	{
		var value = GetValue(args[0]);
		var offset = GetValue(args[1]);

		if (value != 0)
			_cursor += offset - 1;
	}

	private void Toggle(string[] args)
	{
		var pos = _cursor + GetValue(args[0]);
		if (pos < 0 || pos >= _compiledSource.Length)
			return;

		var bytecode = _compiledSource[pos];
		if (bytecode.Args.Length == 1)
			bytecode.Command = bytecode.Command == "inc" ? "dec" : "inc";
		else
			bytecode.Command = bytecode.Command == "jnz" ? "cpy" : "jnz";
			
		Optimize();
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
	
	private void Optimize()
	{
		if(!_allowOptimization)
			return;
			
		for (var i = 0; i < _compiledSource.Length; i++)
		{
			// flush old optimizations
			var bc = _compiledSource[i];
			bc.Optimized = null;
			
			ApplyMultiplyOptimization(i);
		}
	}
	
	private void ApplyMultiplyOptimization(int idx)
	{
		// pattern:
		// (init y), inc x, dec y, jnz y -2, dec z, jnz z -5
		// optimization:
		// x += y * z; y = 0; z = 0

		var jnzZ = _compiledSource[idx];
		if (jnzZ.Command != "jnz" || jnzZ.Args[1] != "-5")
			return;
		
		var decZ = _compiledSource[idx - 1];
		var jnzY = _compiledSource[idx - 2];
		var decY = _compiledSource[idx - 3];
		var incX = _compiledSource[idx - 4];
		
		var matches = decZ.Command == "dec"
					  && jnzY.Command == "jnz"
					  && decY.Command == "dec"
					  && incX.Command == "inc"
					  && decZ.Args[0] == jnzZ.Args[0]
					  && decY.Args[0] == jnzY.Args[0];
					  
		if(!matches)
			return;
			
		var x = incX.Args[0];
		var y = decY.Args[0];
		var z = decZ.Args[0];

		incX.Optimized = () =>
		{
			_registers[x] += _registers[y] * _registers[z];
			_registers[y] = _registers[z] = 0;
			_cursor += 4;
		};
	}
}

class ByteCode
{
	public uint Executions;
	public string Command;
	public string[] Args;
	public Action Optimized;
}