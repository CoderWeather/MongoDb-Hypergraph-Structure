using MongoDB.Driver;

namespace AppLib.MongoDb.Samples
{
    public class SampleWeatherdata : BaseSampleDatabase
    {
        public SampleWeatherdata(IMongoClient client) : base(client, "sample_weatherdata")
        {
        }
    }
}