using MongoDB.Bson;
using MongoDB.Driver;

namespace AppLib.MongoDb.Samples
{
    public class SampleTraining : BaseSampleDatabase
    {
        public IMongoCollection<BsonDocument> Companies { get; }
        public IMongoCollection<BsonDocument> Grades { get; }
        public IMongoCollection<BsonDocument> Inspections { get; }
        public IMongoCollection<BsonDocument> Posts { get; }
        public IMongoCollection<BsonDocument> Routes { get; }
        public IMongoCollection<BsonDocument> Stories { get; }
        public IMongoCollection<BsonDocument> Trips { get; }
        public IMongoCollection<BsonDocument> Tweets { get; }
        public IMongoCollection<BsonDocument> Zips { get; }

        public SampleTraining(IMongoClient client) : base(client, "sample_training")
        {
            Companies = DbObject.GetCollection<BsonDocument>(nameof(Companies).ToLower());
            Grades = DbObject.GetCollection<BsonDocument>(nameof(Grades).ToLower());
            Inspections = DbObject.GetCollection<BsonDocument>(nameof(Inspections).ToLower());
            Posts = DbObject.GetCollection<BsonDocument>(nameof(Posts).ToLower());
            Routes = DbObject.GetCollection<BsonDocument>(nameof(Routes).ToLower());
            Stories = DbObject.GetCollection<BsonDocument>(nameof(Stories).ToLower());
            Trips = DbObject.GetCollection<BsonDocument>(nameof(Trips).ToLower());
            Tweets = DbObject.GetCollection<BsonDocument>(nameof(Tweets).ToLower());
            Zips = DbObject.GetCollection<BsonDocument>(nameof(Zips).ToLower());
        }
    }
}