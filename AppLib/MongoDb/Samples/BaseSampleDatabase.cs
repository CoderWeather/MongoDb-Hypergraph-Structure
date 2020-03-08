using MongoDB.Driver;

namespace AppLib.MongoDb.Samples
{
    public abstract class BaseSampleDatabase
    {
        #region Public Properties

        protected IMongoDatabase DbObject { get; }

        #endregion

        #region Public Constructor

        protected BaseSampleDatabase(IMongoClient client, string dbName) =>
            DbObject = client.GetDatabase(dbName);

        #endregion
    }
}