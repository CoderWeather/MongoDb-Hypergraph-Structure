using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppLib.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TestConsole
{
	internal static class Program
	{
		private static readonly DirectoryInfo WorkDir =
			new DirectoryInfo(@"..\..\..\..\Logs\" + DateTime.Now.ToString("yyyy-MM-dd"));

		private static void Main(string[] args)
		{
			var db = new MongoDbContext();
			if (WorkDir.Exists is false) WorkDir.Create();

			db.AllCollections.AsParallel().ForAll(async col => await ParseCollectionAsync(col));
			Console.WriteLine("All done");

			Console.ReadLine();
		}

		private static async Task ParseCollectionAsync(IMongoCollection<BsonDocument> collection)
		{
			Console.WriteLine($"Parse {collection.CollectionNamespace.FullName} Start!");
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

			Console.WriteLine($"Parse {collection.CollectionNamespace.FullName} Done!");
		}
	}
}