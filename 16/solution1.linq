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
	var input = "10111100110001111";
	var length = 272;
		
	var diskData = GetDiskData(input, length);
	var checksum = GetChecksum(diskData);
	
	checksum.Dump();
}

public string GetDiskData(string input, int length)
{
	var a = input;
	while (a.Length < length)
	{
		// reverse b
		var b = a.ToCharArray();
		Array.Reverse(b);
		
		// invert b 
		for(var i = 0; i < b.Length; i++)
			b[i] = b[i] == '0' ? '1' : '0';
			
		a = a + '0' + new string(b);
	}
	
	return a.Substring(0, length);
}

public string GetChecksum(string data)
{
	var curr = data;
	while (curr.Length % 2 == 0)
	{
		var newLength = curr.Length / 2;
		var newCurr = new char[newLength];
		
		for(var i = 0; i < newLength; i++)
			newCurr[i] = curr[i*2] == curr[i*2+1] ? '1' : '0';
			
		curr = new string(newCurr);
	}
	
	return curr;
}
