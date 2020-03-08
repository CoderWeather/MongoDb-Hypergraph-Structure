﻿using System.Collections.Generic;
using System.Linq;
using AppLib.MongoDb.Samples;
using MongoDB.Driver;

namespace AppLib.MongoDb
{
    public class MongoDbContext
    {
        #region Private Static Fields

        private const string ClusterConnString =
            @"mongodb+srv://coderweather:8VDY6YKuRU2SaN0S@cluster0-qmcvv.mongodb.net/test?authSource=admin&replicaSet=Cluster0-shard-0&readPreference=primary&appname=MongoDB%20Compass%20Community&ssl=true";

        private const string LocalConnString = @"mongodb://localhost:27017";

        #endregion

        #region Private Fields

        private readonly IMongoClient _mongoClient;

        #endregion

        #region Public Properties

        public SampleAirbnb SampleAirbnb { get; }
        public SampleAnalytics SampleAnalytics { get; }
        public SampleGeospatial SampleGeospatial { get; }
        public SampleMflix SampleMflix { get; }
        public SampleSupplies SampleSupplies { get; }
        public SampleTraining SampleTraining { get; }
        public SampleWeatherdata SampleWeatherdata { get; }

        #endregion

        #region Public Constructor

        public MongoDbContext()
        {
            _mongoClient = new MongoClient(ClusterConnString);
            SampleAirbnb = new SampleAirbnb(_mongoClient);
            SampleAnalytics = new SampleAnalytics(_mongoClient);
            SampleGeospatial = new SampleGeospatial(_mongoClient);
            SampleMflix = new SampleMflix(_mongoClient);
            SampleSupplies = new SampleSupplies(_mongoClient);
            SampleTraining = new SampleTraining(_mongoClient);
            SampleWeatherdata = new SampleWeatherdata(_mongoClient);
        }

        #endregion
    }
}