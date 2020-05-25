using System.Collections.Generic;
using System.Linq;
using AppLib;
using ReactiveUI.Fody.Helpers;

namespace DesktopApp.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		#region Public Properties

		// [Reactive] public HyperGraph? HyperGraph { get; set; }

		[Reactive] public List<HyperGraphViewModel>? HyperGraphs { get; set; }
		[Reactive] public int SelectedTabIndex { get; set; }

		#endregion

		#region Public Commands

		#endregion

		#region Public Constructor

		public MainViewModel()
		{
			// HyperGraph = TestGraphFactory.GenerateTest1();
			InitGraphs();
		}

		#endregion

		#region Private Methods

		private void InitGraphs()
		{
			HyperGraphs = CachedHyperGraphs.Cached
			   .Select(hg => new HyperGraphViewModel(hg))
			   .ToList();
		}

		#endregion

		#region Public Methods

		#endregion
	}
}