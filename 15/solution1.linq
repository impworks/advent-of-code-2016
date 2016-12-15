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
	var disks = new[]
	{
		new Disk(positions: 17, initial: 1),
		new Disk(positions: 7, initial: 0),
		new Disk(positions: 19, initial: 2),
		new Disk(positions: 5, initial: 0),
		new Disk(positions: 3, initial: 0),
		new Disk(positions: 13, initial: 5),
	};

	var offset = 0;
	while (true)
	{
		var isAligned = true;
		for (var i = 0; i < disks.Length; i++)
		{
			var disk = disks[i];
			var time = offset + i + 1;
			if (!disk.IsAligned(time))
			{
				isAligned = false;
				break;
			}
		}

		if (isAligned)
			break;
			
		offset++;
	}
	
	offset.Dump();
}

class Disk
{
	public Disk(int positions, int initial)
	{
		Positions = positions;
		Initial = initial;
	}
	
	public int Positions;
	public int Initial;
	
	public bool IsAligned(int time)
	{
		return (time + Initial) % Positions == 0;
	}
}
