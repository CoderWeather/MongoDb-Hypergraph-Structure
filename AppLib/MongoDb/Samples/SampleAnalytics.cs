using MongoDB.Driver;

namespace AppLib.MongoDb.Samples
{
    public class SampleAnalytics : BaseSampleDatabase
    {
        public SampleAnalytics(IMongoClient client) : base(client, "sample_analytics")
        {
        }
    }
}