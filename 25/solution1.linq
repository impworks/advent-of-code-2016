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

    // compile & optimize source
    var compiler = new Compiler(true);
    var compiled = compiler.Compile(source);

    // sufficient possibility
    var target = new[] { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 }; 
    var a = 0;
    while (true)
    {
        var matches = compiled.Execute(a, target);
        if (matches)
        {
            a.Dump();
            return;
        }
        
        a++;
    }
}

class Compiler
{
    public Compiler(bool optimize = true)
    {
        _allowOptimization = optimize;
    }
    
    private bool _allowOptimization;

    public CompiledSource Compile(string[] source)
    {
        var compiled = new ByteCode[source.Length];

        // pre-parses all commands 
        for (var idx = 0; idx < source.Length; idx++)
        {
            var instruction = source[idx].Split(' ');
            compiled[idx] = new ByteCode
            {
                Command = instruction[0],
                Args = instruction.Skip(1).ToArray()
            };
        }

        Optimize(compiled);

        return new CompiledSource(compiled);
    }

    private void Optimize(ByteCode[] compiledSource)
    {
        if (!_allowOptimization)
            return;

        for (var i = 0; i < compiledSource.Length; i++)
            ApplyMultiplyOptimization(compiledSource, i);
    }

    private void ApplyMultiplyOptimization(ByteCode[] compiledSource, int idx)
    {
        // pattern:
        // (init y), inc x, dec y, jnz y -2, dec z, jnz z -5
        // optimization:
        // x += y * z; y = 0; z = 0

        var jnzZ = compiledSource[idx];
        if (jnzZ.Command != "jnz" || jnzZ.Args[1] != "-5")
            return;

        var decZ = compiledSource[idx - 1];
        var jnzY = compiledSource[idx - 2];
        var decY = compiledSource[idx - 3];
        var incX = compiledSource[idx - 4];

        var matches = decZ.Command == "dec"
                      && jnzY.Command == "jnz"
                      && decY.Command == "dec"
                      && incX.Command == "inc"
                      && decZ.Args[0] == jnzZ.Args[0]
                      && decY.Args[0] == jnzY.Args[0];

        if (!matches)
            return;

        var x = incX.Args[0];
        var y = decY.Args[0];
        var z = decZ.Args[0];

        incX.Optimized = processor =>
        {
            processor.Registers[x] += processor.Registers[y] * processor.Registers[z];
            processor.Registers[y] = processor.Registers[z] = 0;
            processor.Cursor += 4;
        };
    }
}

class CompiledSource
{
    public CompiledSource(ByteCode[] source)
    {
        _source = source;
    }
    
    private ByteCode[] _source;
    
    public bool Execute(int a, int[] targetOutput)
    {
        var processor = new Processor(_source, targetOutput, a);
        return processor.Process();
    }
}

class Processor
{
    public Processor(ByteCode[] source, int[] targetOutput, int a)
    {
        _source = source;
        _targetOutput = targetOutput;
        Registers = new Dictionary<string, int>
        {
            ["a"] = a,
            ["b"] = 0,
            ["c"] = 0,
            ["d"] = 0,
        };

        _commands = new Dictionary<string, Action<string[]>>
        {
            ["cpy"] = Copy,
            ["inc"] = Inc,
            ["dec"] = Dec,
            ["jnz"] = Jnz,
            ["out"] = Out
        };
    }

    private ByteCode[] _source;
    private Dictionary<string, Action<string[]>> _commands;
    
    public Dictionary<string, int> Registers;    
    public int Cursor;
    
    private int[] _targetOutput;
    private int _outputCount;
    private bool? _isMatchingOutput;

    // gets statistics about the hottest loops
    public uint GetSteps() => _source.Aggregate(0u, (a, x) => a + x.Executions);
    public List<Tuple<int, uint, string>> GetStats() => _source.Select((x, idx) => Tuple.Create(idx, x.Executions, x.Command + " " + string.Join(" ", x.Args)))
                                                                       .ToList();

    public bool Process()
    {      
        while (Cursor < _source.Length)
        {
            var bytecode = _source[Cursor];
            bytecode.Executions++;

            if (bytecode.Optimized != null)
                bytecode.Optimized(this);
            else
                _commands[bytecode.Command](bytecode.Args);
                
            if(_isMatchingOutput != null)
                return _isMatchingOutput.Value;

            Cursor++;
        }
        
        return false;
    }

    private void Copy(string[] args)
    {
        var from = args[0];
        var to = args[1];
        
        Registers[to] = GetValue(from);
    }

    private void Inc(string[] args)
    {
        var reg = args[0];
        Registers[reg]++;
    }

    private void Dec(string[] args)
    {
        var reg = args[0];
        Registers[reg]--;
    }

    private void Jnz(string[] args)
    {
        var value = GetValue(args[0]);
        var offset = GetValue(args[1]);

        if (value != 0)
            Cursor += offset - 1;
    }
    
    private void Out(string[] args)
    {
        var result = GetValue(args[0]);
        if (result != _targetOutput[_outputCount])
        {
            _isMatchingOutput = false;
            return;
        }
        
        _outputCount++;
        if(_outputCount == _targetOutput.Length)
            _isMatchingOutput = true;
    }

    private bool IsRegister(string str)
    {
        return str.Length == 1
            && str[0] >= 'a'
            && str[0] <= 'd';
    }

    private int GetValue(string val)
    {
        return IsRegister(val) ? Registers[val] : int.Parse(val);
    }
}

class ByteCode
{
    public uint Executions;
    public string Command;
    public string[] Args;
    public Action<Processor> Optimized;
}