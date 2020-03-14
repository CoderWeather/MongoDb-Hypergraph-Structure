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
        #region Async Parse

        public static async Task<HyperGraph> CollectionToHyperGraphTaskAsync(
            IMongoCollection<BsonDocument> bsonDocCollection)
        {
            var hyperGraph = new HyperGraph();

            await foreach (var keys in ParseCollectionAsync(bsonDocCollection.AsQueryable()))
            {
                var edge = new HyperEdge();

                foreach (var key in keys)
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

                var existHyperEdge = hyperGraph.Edges.FirstOrDefault(he => he.Vertices.SetEquals(edge.Vertices));
                if (existHyperEdge is null)
                    hyperGraph.Edges.Add(edge);
                else
                {
                    foreach (var vertex in edge.Vertices)
                    {
                        vertex.Edges.RemoveWhere(e => e.Id == edge.Id);
                        vertex.Edges.Add(existHyperEdge);
                    }

                    existHyperEdge.Weight++;
                }
            }

            return hyperGraph;
        }

        private static async IAsyncEnumerable<IEnumerable<string>> ParseCollectionAsync(
            IEnumerable<BsonDocument> collection)
        {
            foreach (var doc in collection)
                yield return await Task.Run(() => ParseDocument(doc));
        }

        #endregion


        #region Sinchronize Parse

        private static IEnumerable<string> ParseDocument(BsonDocument doc, string? ketPrefix = null) =>
            doc.SelectMany(el => ParseElement(el, ketPrefix));

        private static IEnumerable<string> ParseElement(BsonElement el, string? keyPrefix = null)
        {
            var newKey = $"{(keyPrefix is null ? null : keyPrefix + '.')}{el.Name}";
            if (el.Value.IsBsonDocument)
                foreach (var key in ParseDocument(el.Value.AsBsonDocument, newKey))
                    yield return key;
            else
                yield return newKey;
        }

        #endregion
    }
}