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
	var num = 0L;
	var foundChars = 0;
	var pass = new char[8];
	
	// processing
	while (true)
	{
		var doorId = input + num;
		var hash = GetHash(doorId);
		
		num++;

		// fast compare
		if (hash[0] == 0 && hash[1] == 0 && hash[2] <= 15)
		{
			// checking if position is valid
			var position = hash[2].ToString("x2")[1];
			if (position < '0' || position > '7')
				continue;
				
			// checking if position is empty
			var offset = (int)(position - '0');
			if(pass[offset] != 0)
				continue;
			
			// placing character in position
			pass[offset] = hash[3].ToString("x2")[0];
			foundChars++;
		}
			
		if(foundChars == 8)
			break;
	}
	
	new string(pass).Dump();
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
