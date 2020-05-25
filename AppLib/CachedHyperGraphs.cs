using System.Collections.Generic;
using System.IO;
using System.Linq;
using HyperGraphSharp.Models;
using Utf8Json;

namespace AppLib
{
	public static class CachedHyperGraphs
	{
		private static readonly FileInfo FileWithNames = new FileInfo("Collections for Loading.txt");
		private static readonly DirectoryInfo DumpDir = new DirectoryInfo(@"..\..\..\..\HyperGraphDumps");

		public static IEnumerable<HyperGraph> Cached =>
			Loaded ??= LoadHyperGraphsFromDump(ReadCollectionNames()).ToList();

		private static List<HyperGraph>? Loaded { get; set; }

		private static IEnumerable<HyperGraph> LoadHyperGraphsFromDump(IEnumerable<string> names)
		{
			var resList = new List<HyperGraph>();
			var dumpFiles = DumpDir.GetFiles("*.bin", SearchOption.TopDirectoryOnly)
			   .Where(fi => names.Contains(fi.Name)).ToArray();
			if (dumpFiles.Length == 0) return resList;

			foreach (var fileName in dumpFiles.Select(fi => fi.FullName))
			{
				using var fs = new FileStream(fileName, FileMode.Open);
				using var ms = new MemoryStream();
				fs.CopyTo(ms);
				var bytes = ms.GetBuffer();
				var hg = JsonSerializer.Deserialize<HyperGraph>(bytes);
				resList.Add(hg);
			}

			return resList;
		}

		private static IEnumerable<string> ReadCollectionNames()
		{
			using var sr = FileWithNames.OpenText();
			while (sr.EndOfStream is false)
			{
				var name = sr.ReadLine();
				if (name != null) yield return name;
			}
		}
	}
}