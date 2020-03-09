using MongoDB.Bson;
using MongoDB.Driver;

namespace AppLib.MongoDb.Samples
{
    public class SampleMflix : BaseSampleDatabase
    {
        #region Public Properties

        public IMongoCollection<BsonDocument> Comments { get; }
        public IMongoCollection<BsonDocument> Movies { get; }
        public IMongoCollection<BsonDocument> Sessions { get; }
        public IMongoCollection<BsonDocument> Theaters { get; }
        public IMongoCollection<BsonDocument> Users { get; }

        #endregion

        #region Public Constructor

        public SampleMflix(IMongoClient client) : base(client, "sample_mflix")
        {
            Comments = DbObject.GetCollection<BsonDocument>(nameof(Comments).ToLower());
            Movies = DbObject.GetCollection<BsonDocument>(nameof(Movies).ToLower());
            Sessions = DbObject.GetCollection<BsonDocument>(nameof(Sessions).ToLower());
            Theaters = DbObject.GetCollection<BsonDocument>(nameof(Theaters).ToLower());
            Users = DbObject.GetCollection<BsonDocument>(nameof(Users).ToLower());
        }

        #endregion
    }
}