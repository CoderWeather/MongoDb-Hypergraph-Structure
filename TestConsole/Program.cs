using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppLib;
using AppLib.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbContext = AppLib.MongoDb.MongoDbContext;

namespace TestConsole
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var db = new MongoDbContext();

            var result = BsonParser.ParseCollectionAsync(db.SampleMflix.Movies);
            result.Wait();
            foreach (var (key, value) in result.Result)
            {
                Console.WriteLine($"{key}: {value}");
            }

            Console.ReadLine();
        }
    }
}