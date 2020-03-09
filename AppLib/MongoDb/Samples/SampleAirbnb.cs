using MongoDB.Bson;
using MongoDB.Driver;

namespace AppLib.MongoDb.Samples
{
    public class SampleAirbnb : BaseSampleDatabase
    {
        public IMongoCollection<BsonDocument> ListingAndReviews { get; }

        public SampleAirbnb(IMongoClient client) : base(client, "sample_airbnb")
        {
            ListingAndReviews = DbObject.GetCollection<BsonDocument>("listingsAndReviews");
        }
    }
}