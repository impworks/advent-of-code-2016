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
	var intervals = LoadIntervals().ToList();
	var allPoints = GetAllPoints(intervals).ToList();
	var goodPoints = allPoints.Where(p => !intervals.Any(y => y.Item1 <= p && y.Item2 >= p)).ToList();
	var result = goodPoints.OrderBy(x => x).First();
	result.Dump();
}

private IEnumerable<Tuple<uint, uint>> LoadIntervals()
{
	var path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt");
	var lines = File.ReadAllLines(path);
	
	foreach (var line in lines)
	{
		var limits = line.Split('-').Select(uint.Parse).ToArray();
		yield return Tuple.Create(limits[0], limits[1]);
	}
}

private IEnumerable<uint> GetAllPoints(IEnumerable<Tuple<uint, uint>> intervals)
{
	foreach (var interval in intervals)
	{
		if(interval.Item1 != uint.MinValue)
			yield return interval.Item1 - 1;

		if (interval.Item2 != uint.MaxValue)
			yield return interval.Item2 + 1;
	}
}