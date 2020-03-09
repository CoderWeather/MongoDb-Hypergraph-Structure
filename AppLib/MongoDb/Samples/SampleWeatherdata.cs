using MongoDB.Bson;
using MongoDB.Driver;

namespace AppLib.MongoDb.Samples
{
    public class SampleWeatherdata : BaseSampleDatabase
    {
        public IMongoCollection<BsonDocument> Data { get; }

        public SampleWeatherdata(IMongoClient client) : base(client, "sample_weatherdata")
        {
            Data = DbObject.GetCollection<BsonDocument>(nameof(Data).ToLower());
        }
    }
}