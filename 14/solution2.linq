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
	var salt = "ahsbgdzn";
	var hashes = 64;
	var windowSize = 1000;

	var patterns = GetChars().ToDictionary(x => new String(x, 3), x => new String(x, 5));

	// state
	var idx = -1L;
	var foundHashes = new List<long>();
	var pendingHashes = new List<PendingHashInfo>();

	while (true)
	{
		idx++;
		var currentString = salt + idx;

		var md5 = GetLongHash(currentString);

		// check pending hashes
		foreach (var pending in pendingHashes)
		{
			// skip outdated or already checked hashes
			if (pending.IsStale || pending.OriginalIndex <= idx - windowSize)
			{
				pending.IsStale = true;
				continue;
			}

			// check if current hash confirms any of the pending hashes
			if (md5.Contains(pending.Confirmation))
			{
				pending.IsStale = true;
				foundHashes.Add(pending.OriginalIndex);
			}
		}

		if (foundHashes.Count >= hashes)
			break;

		// add newly found hashes for checking
		var matchingPattern = patterns.Select(p => new { Index = md5.IndexOf(p.Key), Confirmation = p.Value })
									  .Where(x => x.Index != -1)
									  .OrderBy(x => x.Index)
									  .Take(1)
									  .FirstOrDefault();

		if (matchingPattern != null)
			pendingHashes.Add(new PendingHashInfo(idx, matchingPattern.Confirmation) { OriginalHash = md5 });

		// garbage collection
		if (idx % 2000 == 0)
			pendingHashes = pendingHashes.Where(x => !x.IsStale).ToList();
	}

	foundHashes[63].Dump();
}

private char[] GetChars()
{
	var result = new char[16];

	// generate 0..9
	for (var i = 0; i < 10; i++)
		result[i] = (char)('0' + i);

	// generate a..f
	for (var i = 0; i < 6; i++)
		result[10 + i] = (char)('a' + i);

	return result;
}

private string GetMd5(string value)
{
	using (var md5 = MD5.Create())
	{
		var inBytes = Encoding.UTF8.GetBytes(value);
		var outBytes = md5.ComputeHash(inBytes);
		return string.Concat(outBytes.Select(x => x.ToString("x2")));
	}
}

private string GetLongHash(string value)
{
	var hash = GetMd5(value);
	
	for(var i = 0; i < 2016; i++)
		hash = GetMd5(hash);
		
	return hash;
}

class PendingHashInfo
{
	public PendingHashInfo(long index, string confirmation)
	{
		OriginalIndex = index;
		Confirmation = confirmation;
	}

	public long OriginalIndex;
	public string OriginalHash;
	public string Confirmation;
	public bool IsStale;
}
