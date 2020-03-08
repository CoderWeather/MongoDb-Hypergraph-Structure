using MongoDB.Driver;

namespace AppLib.MongoDb.Samples
{
    public class SampleGeospatial : BaseSampleDatabase
    {
        public SampleGeospatial(IMongoClient client) : base(client, "sample_geospatial")
        {
        }
    }
}