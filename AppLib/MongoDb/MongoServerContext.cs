using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HyperGraphSharp.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AppLib.MongoDb
{
	public class MongoServerContext
	{
		#region Private Properties

		private IList<string>? CachedDatabaseNames { get; set; }
		private IList<IMongoDatabase>? CachedDatabases { get; set; }
		private IList<IMongoCollection<BsonDocument>>? CachedBsonCollections { get; set; }

		#endregion

		#region Public Properties

		public bool FileLogging { get; set; }

		public string ConnectionString { get; private set; }
		public IMongoClient Client { get; private set; }
		public DirectoryInfo LogsDirectory { get; }
		public DirectoryInfo DumpsDirectory { get; }

		public string[]? CollectionsWhiteList { get; set; }

		public IEnumerable<string> DatabaseNames => CachedDatabaseNames ??= LoadDatabaseNames();
		public IEnumerable<IMongoDatabase> Databases => CachedDatabases ??= LoadDatabasesList();

		public IEnumerable<IMongoCollection<BsonDocument>> BsonCollections =>
			CachedBsonCollections ??= LoadBsonCollections();

		#endregion

		#region Public Constructor

		public MongoServerContext(string connectionString)
		{
			ConnectionString = connectionString;
			FileLogging = false;
			Client = new MongoClient(ConnectionString);
			CollectionsWhiteList = null;
			LogsDirectory = new DirectoryInfo("Logs");
			if (LogsDirectory.Exists is false)
				LogsDirectory.Create();
			DumpsDirectory = new DirectoryInfo("Dumps");
			if (DumpsDirectory.Exists is false)
				DumpsDirectory.Create();
		}

		#endregion

		#region Public Methods

		public IList<string> LoadDatabaseNames() =>
			Client.ListDatabaseNames().ToList();

		public IList<IMongoDatabase> LoadDatabasesList() =>
			LoadDatabaseNames()
			   .Select(name => Client.GetDatabase(name))
			   .ToList();

		public IList<IMongoCollection<TDoc>> LoadCollections<TDoc>(string? databaseName = null)
		{
			if (databaseName is null)
				return LoadCollections<TDoc>(db: null);

			var db = Client.GetDatabase(databaseName);
			return LoadCollections<TDoc>(db ?? null);
		}

		public IList<IMongoCollection<TDoc>> LoadCollections<TDoc>(IMongoDatabase? db = null)
		{
			var collections = new List<IMongoCollection<TDoc>>();
			if (db is null)
			{
				foreach (var database in LoadDatabasesList())
				{
					collections
					   .AddRange(database.ListCollectionNames()
						   .ToList()
						   .Select(collectionName => database.GetCollection<TDoc>(collectionName)));
				}
			}
			else
			{
				collections.AddRange(db.ListCollectionNames()
				   .ToList()
				   .Select(colName => db.GetCollection<TDoc>(colName)));
			}

			return CollectionsWhiteList != null && CollectionsWhiteList.Length > 0
				? collections
				   .Where(col =>
						CollectionsWhiteList.Contains(col.CollectionNamespace.CollectionName))
				   .ToList()
				: collections;
		}

		public IList<IMongoCollection<BsonDocument>> LoadBsonCollections(string? databaseName = null) =>
			LoadCollections<BsonDocument>(databaseName);

		public IList<IMongoCollection<BsonDocument>> LoadBsonCollection(IMongoDatabase? db = null) =>
			LoadCollections<BsonDocument>(db);

		public IMongoCollection<BsonDocument>? LoadBsonCollection(string collectionName,
			string? databaseName = null) =>
			databaseName is null
				? LoadBsonCollections()
				   .FirstOrDefault(collection => collection.CollectionNamespace.CollectionName == collectionName)
				: LoadBsonCollection(collectionName, Client.GetDatabase(databaseName) ?? null);

		public IMongoCollection<BsonDocument>? LoadBsonCollection(string collectionName,
			IMongoDatabase? database = null) =>
			database is null
				? LoadBsonCollection(collectionName, databaseName: null)
				: database.GetCollection<BsonDocument>(collectionName, new MongoCollectionSettings());

		#endregion

		#region Private Methods

		#endregion

		#region Bson Parsing

		private static async Task<HyperGraph> CollectionToHyperGraphTaskAsync(
			IMongoCollection<BsonDocument> bsonDocCollection)
		{
			var hyperGraph = new HyperGraph(bsonDocCollection.CollectionNamespace.CollectionName);

			await foreach (var keys in ParseCollectionAsync(bsonDocCollection.AsQueryable()))
			{
				var edge = new HyperEdge {Weight = 1};

				foreach (var key in keys)
				{
					var vertex = hyperGraph.Vertices.FirstOrDefault(v => v.Data?.Equals(key) ?? false);
					if (vertex is null)
					{
						vertex = new Vertex(key);
						hyperGraph.Vertices.Add(vertex);
					}

					vertex.HyperEdges.Add(edge);
					edge.Vertices.Add(vertex);
				}

				var existHyperEdge = hyperGraph.HyperEdges.FirstOrDefault(he => he.Vertices.SetEquals(edge.Vertices));
				if (existHyperEdge is null)
				{
					hyperGraph.HyperEdges.Add(edge);
				}
				else
				{
					foreach (var vertex in edge.Vertices)
					{
						vertex.HyperEdges.RemoveWhere(e => e.Id == edge.Id);
						vertex.HyperEdges.Add(existHyperEdge);
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