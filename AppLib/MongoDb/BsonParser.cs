using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AppLib.MongoDb
{
    public static class BsonParser
    {
        private const string mapFunc = @"";
        private const string reduceFunc = @"";
        private const string finalizeFunc = @"";

        public static async Task<IEnumerable<KeyValuePair<string, int>>> MapReduce(
            IMongoCollection<BsonDocument> collection)
        {
            return await (await collection
                    .MapReduceAsync<KeyValuePair<string, int>>(mapFunc, reduceFunc))
                .ToListAsync();
        }

        public static async Task<IEnumerable<KeyValuePair<string, int>>> ParseCollectionAsync(
            IMongoCollection<BsonDocument> collection)
        {
            var collectionFields = new Dictionary<string, int>();

            foreach (var doc in collection.AsQueryable())
            {
                collectionFields.AddOrModify(await ParseDocumentAsync(doc));
            }

            return collectionFields;
        }

        public static IEnumerable<KeyValuePair<string, int>> ParseDocument(BsonDocument doc)
        {
            var collectionFields = new Dictionary<string, int>();
            foreach (var el in doc.Elements)
                collectionFields.AddOrModify(ParseElement(el));
            return collectionFields;
        }

        public static async Task<IEnumerable<KeyValuePair<string, int>>> ParseDocumentAsync(BsonDocument doc) =>
            await Task.Run(() => ParseDocument(doc));

        private static IEnumerable<KeyValuePair<string, int>> ParseElement(BsonElement el)
        {
            var collectionFields = new Dictionary<string, int>();
            if (el.Value.IsBsonDocument)
                collectionFields.AddOrModify(ParseDocument(el.Value.AsBsonDocument));
            else
                collectionFields.AddOrModify(el.Name);
            return collectionFields;
        }

        private static async Task<IEnumerable<KeyValuePair<string, int>>> ParseElementAsync(BsonElement el) =>
            await Task.Run(() => ParseElement(el));
    }
}