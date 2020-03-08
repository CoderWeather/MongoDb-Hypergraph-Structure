using MongoDB.Driver;

namespace AppLib.MongoDb.Samples
{
    public class SampleSupplies : BaseSampleDatabase
    {
        public SampleSupplies(IMongoClient client) : base(client, "sample_supplies")
        {
        }
    }
}