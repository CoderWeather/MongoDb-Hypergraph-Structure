using MongoDB.Driver;

namespace AppLib.MongoDb.Samples
{
    public class SampleTraining : BaseSampleDatabase
    {
        public SampleTraining(IMongoClient client) : base(client, "sample_training")
        {
        }
    }
}