using MongoDB.Driver;

namespace AppLib.MongoDb.Samples
{
    public class SampleAirbnb : BaseSampleDatabase
    {
        public SampleAirbnb(IMongoClient client) : base(client, "sample_airbnb")
        {
        }
    }
}