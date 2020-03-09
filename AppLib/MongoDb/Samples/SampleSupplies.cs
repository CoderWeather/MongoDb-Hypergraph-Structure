using MongoDB.Bson;
using MongoDB.Driver;

namespace AppLib.MongoDb.Samples
{
    public class SampleSupplies : BaseSampleDatabase
    {
        public IMongoCollection<BsonDocument> Sales { get; }
        public SampleSupplies(IMongoClient client) : base(client, "sample_supplies")
        {
            Sales = DbObject.GetCollection<BsonDocument>(nameof(Sales).ToLower());
        }
    }
}