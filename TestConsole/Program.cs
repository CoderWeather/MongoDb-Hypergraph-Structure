using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppLib.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbContext = AppLib.MongoDb.MongoDbContext;

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
            var tasks = db.AllCollections.Select(ParseCollectionAsync).ToArray();
            Task.WaitAll(tasks);
            Console.WriteLine("All done");
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
                sw.WriteLine($"Vertex edges: {vertex.Edges.Count}");
                sw.WriteLine("".PadLeft(50, '-'));
            }

            sw.WriteLine("Edges:");
            foreach (var edge in hg.Edges)
            {
                sw.WriteLine("Edge weight: " + edge.Weight);
                sw.WriteLine($"Edge vertices {edge.Vertices.Count}:");
                foreach (var vertex in edge.Vertices)
                    sw.WriteLine('\t' + vertex.Data);
                sw.WriteLine("".PadLeft(50, '-'));
            }

            sw.WriteLine("Vertices count: " + hg.Vertices.Count);
            sw.WriteLine("Edges count: " + hg.Edges.Count);

            Console.WriteLine($"Parse {collection.CollectionNamespace.FullName} Done!");
        }
    }
}