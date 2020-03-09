using MongoDB.Bson;
using MongoDB.Driver;

namespace AppLib.MongoDb.Samples
{
    public class SampleGeospatial : BaseSampleDatabase
    {
        public IMongoCollection<BsonDocument> Shipwrecks { get; }

        public SampleGeospatial(IMongoClient client) : base(client, "sample_geospatial")
        {
            Shipwrecks = DbObject.GetCollection<BsonDocument>(nameof(Shipwrecks).ToLower());
        }
    }
}