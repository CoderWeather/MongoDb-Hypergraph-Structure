using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppLib;
using AppLib.MongoDb;
using HyperGraphSharp.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Utf8Json;

namespace TestConsole
{
	internal static class Program
	{
		private static readonly DirectoryInfo WorkDir =
			new DirectoryInfo(@"..\..\..\..\Logs\" + DateTime.Now.ToString("yyyy-MM-dd"));

		private static readonly DirectoryInfo DumpDir =
			new DirectoryInfo(@"..\..\..\..\HyperGraphDumps");

		private static int _allCount;
		private static int _curCount;
		private static readonly object Lock = new object();

		private static void Main(string[] args)
		{
			Foo1();
			// var list = LoadHyperGraphsFromDump(ReadCollectionNamesFromFile());
			//
			// foreach (var hg in list)
			// {
			// 	Console.WriteLine("".PadRight(100, '_'));
			// 	Console.WriteLine($"Caption: {hg.Caption}");
			// 	Console.WriteLine($"HyperEdges: {hg.HyperEdges.Count}");
			// 	Console.WriteLine($"Vertices: {hg.Vertices.Count}");
			// }
		}

		private static void Foo1()
		{
			// var db = new MongoDbContext();
			// db.AllCollections.AsParallel().ForAll(async col => await ParseCollectionAsync(col));
			if (WorkDir.Exists is false) WorkDir.Create();
			if (DumpDir.Exists is false) DumpDir.Create();
			var client = new MongoClient("mongodb://localhost:27017");
			var samplesDb = client.GetDatabase("Samples");

			_allCount = samplesDb.ListCollectionNames().ToList().Count;

			samplesDb
			   .ListCollectionNames()
			   .ToList()
			   .Select(name => samplesDb.GetCollection<BsonDocument>(name))
			   .AsParallel()
				// .ForAll(async col => await ParseCollectionAsync(col));
			   .ForAll(async col => await DumpHyperGraphFromCollection(col));

			Console.WriteLine("All done");

			Console.ReadLine();
		}

		private static async Task ParseCollectionAsync(IMongoCollection<BsonDocument> collection)
		{
			var watch = new Stopwatch();
			Console.WriteLine($"Parse {collection.CollectionNamespace.FullName} Start!");
			watch.Start();
			var file = new FileInfo($@"{WorkDir.FullName}\{collection.CollectionNamespace.FullName}.log");
			if (file.Exists) file.Delete();
			var hg = await BsonParser.CollectionToHyperGraphTaskAsync(collection);
			await using var fs = new FileStream(file.FullName, FileMode.Create);
			await using var sw = new StreamWriter(fs) {AutoFlush = true};
			sw.WriteLine("Vertices:");
			foreach (var vertex in hg.Vertices)
			{
				sw.WriteLine("Data: " + vertex.Data);
				sw.WriteLine($"Vertex edges: {vertex.HyperEdges.Count}");
				sw.WriteLine("".PadLeft(50, '-'));
			}

			sw.WriteLine("Edges:");
			foreach (var edge in hg.HyperEdges)
			{
				sw.WriteLine("Edge weight: " + edge.Weight);
				sw.WriteLine($"Edge vertices {edge.Vertices.Count}:");
				foreach (var vertex in edge.Vertices)
					sw.WriteLine('\t' + vertex.Data);
				sw.WriteLine("".PadLeft(50, '-'));
			}

			sw.WriteLine("Vertices count: " + hg.Vertices.Count);
			sw.WriteLine("Edges count: " + hg.HyperEdges.Count);

			watch.Stop();
			_curCount++;
			Console.WriteLine(
				$"Parse {collection.CollectionNamespace.FullName} Done![{_curCount}\\{_allCount}] {watch.ElapsedMilliseconds} ms");
		}

		private static async Task DumpHyperGraphFromCollection(IMongoCollection<BsonDocument> collection)
		{
			var watch = new Stopwatch();
			Console.WriteLine($"Parse {collection.CollectionNamespace.FullName} Start!");
			watch.Start();

			var file = new FileInfo($@"{DumpDir.FullName}\{collection.CollectionNamespace.FullName}.bin");
			if (file.Exists) file.Delete();
			var hg = await BsonParser.CollectionToHyperGraphTaskAsync(collection);
			await using var fs = new FileStream(file.FullName, FileMode.Create);

			fs.Write(hg.Serialize());
			watch.Stop();
			lock (Lock)
			{
				_curCount++;
				Console.WriteLine(
					$"Parse {collection.CollectionNamespace.FullName} Done![{_curCount}\\{_allCount}] {watch.ElapsedMilliseconds} ms");
				Console.Beep();
			}
		}

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

		private static IEnumerable<string> ReadCollectionNamesFromFile()
		{
			using var sr = new StreamReader("Collections for loading.txt");
			while (sr.EndOfStream is false)
			{
				var name = sr.ReadLine();
				if (name != null) yield return name;
			}
		}
	}
}