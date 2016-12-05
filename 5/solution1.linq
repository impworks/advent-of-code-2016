<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>JsonFormatting = Newtonsoft.Json.Formatting</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
</Query>

void Main()
{
	// input
	var input = "ugkcyxxp";
	
	// data
	long num = 0;
	string pass = "";
	
	// processing
	while (true)
	{
		var doorId = input + num;
		var hash = GetHash(doorId);
		
		// faster compare
		if(hash[0] == 0 && hash[1] == 0 && hash[2] <= 15)
			pass = pass + hash[2].ToString("x2").Substring(1);
			
		num++;
			
		if(pass.Length == 8)
			break;
	}
	
	pass.Dump();
}

private byte[] GetHash(string input)
{
	using (var md5 = MD5.Create())
	{
		var inBytes = Encoding.UTF8.GetBytes(input);
		return md5.ComputeHash(inBytes);
	}
}

// Define other methods and classes here
