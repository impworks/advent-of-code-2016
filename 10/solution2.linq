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
	
	// processing
	var network = new BotNetwork();
	var outputs = network.ProcessNetwork(source);
	var result = outputs[0] * outputs[1] * outputs[2];
	
	// result
	result.Dump();
}

class BotNetwork
{
	public BotNetwork()
	{	
		_bots = new Dictionary<int, Bot>();
		_outputs = new Dictionary<int, int>();
		
		_valueCmd = new Regex("value (?<value>[0-9]+) goes to bot (?<bot>[0-9]+)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		_connectCmd = new Regex("bot (?<bot>[0-9]+) gives low to (?<lotype>bot|output) (?<lo>[0-9]+) and high to (?<hitype>bot|output) (?<hi>[0-9]+)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
	}
	
	private Regex _valueCmd;
	private Regex _connectCmd;
	
	private Dictionary<int, Bot> _bots;
	private Dictionary<int, int> _outputs;	
	
	public Dictionary<int, int> ProcessNetwork(string[] commands)
	{
		// process commands: connections first, values last
		foreach (var cmd in commands.OrderBy(x => x))
		{
			// define bot connection
			var connectMatch = _connectCmd.Match(cmd);
			if (connectMatch.Success)
			{
				var botId = int.Parse(connectMatch.Groups["bot"].Value);
				var bot = GetBot(botId);
				
				bot.LoOutputType = connectMatch.Groups["lotype"].Value;
				bot.LoOutputId = int.Parse(connectMatch.Groups["lo"].Value);
				bot.HiOutputType = connectMatch.Groups["hitype"].Value;
				bot.HiOutputId = int.Parse(connectMatch.Groups["hi"].Value);
				
				continue;
			}

			// set value
			var valueMatch = _valueCmd.Match(cmd);
			if (valueMatch.Success)
			{
				var botId = int.Parse(valueMatch.Groups["bot"].Value);
				var value = int.Parse(valueMatch.Groups["value"].Value);

				PassValue(botId, value);

				continue;
			}

			throw new ArgumentException($"Unknown command: {cmd}");
		}

		return _outputs;
	}
	
	private void PassValue(int botId, int value)
	{
		// add value to bot
		var bot = GetBot(botId);
		bot.Values.Add(value);
		
		if(bot.Values.Count < 2)
			return;
			
		// bot is loaded: pass values forward
		var lo = bot.Values.Min();
		var hi = bot.Values.Max();

		if (bot.LoOutputType == "bot")
			PassValue(bot.LoOutputId, lo);
		else
			_outputs[bot.LoOutputId] = lo;

		if (bot.HiOutputType == "bot")
			PassValue(bot.HiOutputId, hi);
		else
			_outputs[bot.HiOutputId] = hi;
	}
	
	private Bot GetBot(int botId)
	{
		Bot bot;
		if (!_bots.TryGetValue(botId, out bot))
		{
			bot = new Bot { Id = botId };
			_bots[botId] = bot;
		}
		
		return bot;
	}
}

class Bot
{
	public int Id;
	public List<int> Values = new List<int>();
	
	public string LoOutputType;
	public int LoOutputId;
	
	public string HiOutputType;
	public int HiOutputId;
}