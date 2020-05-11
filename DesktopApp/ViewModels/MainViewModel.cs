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
		}

		#endregion

		#region Public Properties

		[Reactive] public HyperGraph HyperGraph { get; set; }

		#endregion

		#region Private Methods

		#endregion

		#region Public Methods

		#endregion
	}
}