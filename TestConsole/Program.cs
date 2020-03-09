using System;
using System.IO;
using System.Linq;
using AppLib.GraphDataStructure;
using AppLib.MongoDb;
using MongoDbContext = AppLib.MongoDb.MongoDbContext;

namespace TestConsole
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var db = new MongoDbContext();

            // var result = BsonParser.ParseCollectionTaskAsync(db.SampleAirbnb.ListingAndReviews);
            // result.Wait();
            // foreach (var (key, value) in result.Result)
            // {
            //     Console.WriteLine($"{key}: {value}");
            // }
            var hgTask = BsonParser.ParseCollectionToHyperGraphAsync(db.SampleTraining.Tweets);
            hgTask.Wait();
            var hg = hgTask.Result;
            using var fs = new FileStream($@"Logs\{DateTime.Now:yy-MM-dd HH-mm-ss}.log", FileMode.Create);
            using var sw = new StreamWriter(fs) {AutoFlush = true};
            sw.WriteLine("Vertices:");
            foreach (var vertex in hg.Vertices)
            {
                sw.WriteLine("Data: " + vertex.Data);
                sw.WriteLine($"Vertex edges {vertex.Edges.Count}:");
                foreach (var edge in vertex.Edges)
                    sw.Write(edge.Weight + " ");
                sw.WriteLine();
                sw.WriteLine("".PadLeft(50, '-'));
            }

            sw.WriteLine("Edges:");
            foreach (var edge in hg.Edges)
            {
                sw.WriteLine("Edge weight: " + edge.Weight);
                sw.WriteLine($"Edge vertices {edge.Vertices.Count}:");
                foreach (var vertex in edge.Vertices)
                    sw.WriteLine('\t' + vertex.Data);
                sw.WriteLine();
                sw.WriteLine("".PadLeft(50, '-'));
            }

            sw.WriteLine("Vertices count: " + hg.Vertices.Count);
            sw.WriteLine("Edges count: " + hg.Edges.Count);
            
            Console.WriteLine("Done!");
        }
    }
}