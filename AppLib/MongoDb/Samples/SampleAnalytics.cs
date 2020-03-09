using MongoDB.Bson;
using MongoDB.Driver;

namespace AppLib.MongoDb.Samples
{
    public class SampleAnalytics : BaseSampleDatabase
    {
        public IMongoCollection<BsonDocument> Accounts { get; }
        public IMongoCollection<BsonDocument> Customers { get; }
        public IMongoCollection<BsonDocument> Transactions { get; } 
            
        public SampleAnalytics(IMongoClient client) : base(client, "sample_analytics")
        {
            Accounts = DbObject.GetCollection<BsonDocument>(nameof(Accounts).ToLower());
            Customers = DbObject.GetCollection<BsonDocument>(nameof(Customers).ToLower());
            Transactions = DbObject.GetCollection<BsonDocument>(nameof(Transactions).ToLower());
        }
    }
}