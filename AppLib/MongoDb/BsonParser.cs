using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppLib.GraphDataStructure;
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

        public static async Task<HyperGraph> ParseCollectionToHyperGraphAsync(
            IMongoCollection<BsonDocument> bsonDocCollection)
        {
            var hyperGraph = new HyperGraph();

            await foreach (var pairs in ParseCollectionAsync(bsonDocCollection))
            {
                var edge = new HyperEdge {Weight = 1};

                foreach (var (key, _) in pairs)
                {
                    var vertex = hyperGraph.Vertices.FirstOrDefault(v => v.Data.Equals(key));
                    if (vertex is null)
                    {
                        vertex = new Vertex {Data = key};
                        hyperGraph.Vertices.Add(vertex);
                    }

                    vertex.Edges.Add(edge);
                    edge.Vertices.Add(vertex);
                }

                var testHyperEdge = hyperGraph.Edges.FirstOrDefault(he => he.Vertices.SetEquals(edge.Vertices));
                if (testHyperEdge is null)
                    hyperGraph.Edges.Add(edge);
                else
                {
                    foreach (var vertex in edge.Vertices)
                    {
                        vertex.Edges.RemoveWhere(e => e.Id == edge.Id);
                        vertex.Edges.Add(testHyperEdge);
                    }

                    testHyperEdge.Weight++;
                }
            }

            return hyperGraph;
        }

        private static async IAsyncEnumerable<IEnumerable<KeyValuePair<string, int>>> ParseCollectionAsync(
            IMongoCollection<BsonDocument> bsonCollection)
        {
            foreach (var doc in bsonCollection.AsQueryable())
                yield return await ParseDocumentTaskAsync(doc);
        }

        private static async Task<IEnumerable<KeyValuePair<string, int>>> ParseDocumentTaskAsync(BsonDocument doc,
            string keyPrefix = null)
        {
            var collectionFields = new Dictionary<string, int>();
            foreach (var el in doc)
                collectionFields.AddOrModify(await ParseElementTaskAsync(el, keyPrefix));

            return collectionFields;
        }

        private static async Task<IEnumerable<KeyValuePair<string, int>>> ParseElementTaskAsync(BsonElement el,
            string keyPrefix = null)
        {
            var collectionFields = new Dictionary<string, int>();
            if (el.Value.IsBsonDocument)
                collectionFields.AddOrModify(await ParseDocumentTaskAsync(el.Value.AsBsonDocument,
                    (keyPrefix is null ? null : keyPrefix + '.') + el.Name));
            else
                collectionFields.AddOrModify($"{(keyPrefix is null ? null : keyPrefix + '.')}{el.Name}");
            return collectionFields;
        }
    }
}