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
	var length = 3005290;
	
	// state
	var group = new bool[length];
	var currId = 0;

	while (true)
	{
		var newId = StealPresents(currId, group);
		if (newId == null)
			break;
			
		currId = newId.Value;
	}
	
	// result
	(currId + 1).Dump();
}

public int? StealPresents(int currentId, bool[] group)
{
	var stealFrom = FindNextElfWithPresents(currentId, group);
	if(stealFrom == null)
		return null;
		
	group[stealFrom.Value] = true;
	
	return FindNextElfWithPresents(stealFrom.Value, group);
}

public int? FindNextElfWithPresents(int currentId, bool[] group)
{
	var count = group.Length;
	for (var i = 0; i < count - 1; i++)
	{
		var position = (currentId + i + 1) % count;
		if (!group[position])
			return position;
	}
	
	return null;
}
