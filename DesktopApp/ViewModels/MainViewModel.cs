using AppLib.MongoDb;
using DesktopApp.Models;
using HyperGraphSharp.Models;
using ReactiveUI.Fody.Helpers;

namespace DesktopApp.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		#region Public Constructor

		public MainViewModel()
		{
			HyperGraph = TestGraphFactory.GenerateTest1();
			// InitGraph();
		}

		#endregion

		#region Public Properties

		[Reactive] public HyperGraph? HyperGraph { get; set; }

        #endregion

        #region Private Methods

        private void InitGraph()
        {
            var db = new MongoDbContext();
            var collection = db.SampleWeatherdata.Data;
            var graphTask = BsonParser.CollectionToHyperGraphTaskAsync(collection);
            graphTask.Wait();
            HyperGraph = graphTask.Result;
        }

        #endregion

        #region Public Methods

        #endregion
    }
}