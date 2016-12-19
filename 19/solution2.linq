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
	var elf = CreateElves(length);
	
	// state
	var remaining = length;
	var acrossElf = FindElfAcross(elf, length);

	// process
	while (remaining > 1)
	{
		// remove elf across
		var removed = acrossElf;
		removed.NextElf.PrevElf = acrossElf.PrevElf;
		removed.PrevElf.NextElf = acrossElf.NextElf;
		
		// step 1 or 2 steps depending on rounding
		acrossElf = remaining % 2 == 0 ? removed.NextElf : removed.NextElf.NextElf;
		
		elf = elf.NextElf;
		
		remaining--;
	}
		
	elf.Id.Dump();
}

private Elf CreateElves(int length)
{
	Elf firstElf = null;
	Elf prevElf = null;

	for (var i = 0; i < length; i++)
	{
		var elf = new Elf { Id = i + 1 };
		
		if(firstElf == null)
			firstElf = elf;

		if (prevElf != null)
		{
			prevElf.NextElf = elf;
			elf.PrevElf = prevElf;
		}
			
		prevElf = elf;
	}
	
	// loop
	prevElf.NextElf = firstElf;
	firstElf.PrevElf = prevElf;
	
	return firstElf;
}

private Elf FindElfAcross(Elf elf, int count)
{
	var curr = elf;
	var target = count / 2;
	for(var i = 0; i < target; i++)
		curr = curr.NextElf;
		
	return curr;
}

private class Elf
{
	public int Id;
	public Elf NextElf;
	public Elf PrevElf;
}
