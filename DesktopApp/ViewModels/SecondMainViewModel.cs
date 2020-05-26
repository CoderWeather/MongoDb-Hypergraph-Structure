using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using AppLib;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

#pragma warning disable 8618

namespace DesktopApp.ViewModels
{
	[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
	public class SecondMainViewModel : ViewModelBase
	{
		#region Public Properties

		[Reactive] public int SelectedGraphIndex { get; set; }
		public HyperGraphViewModel SelectedGraph { [ObservableAsProperty] get; }
		public List<HyperGraphViewModel> HyperGraphs { get; }

		#endregion

		#region Public Commands

		#endregion

		#region Public Constructor

		public SecondMainViewModel()
		{
			HyperGraphs = CachedHyperGraphs.Cached
			   .Select(hg => new HyperGraphViewModel(hg))
			   .ToList();

			this.WhenAnyValue(x => x.SelectedGraphIndex)
			   .Where(index => index > 0 && index < HyperGraphs.Count)
			   .Select(index => HyperGraphs[index])
			   .ToPropertyEx(this, x => x.SelectedGraph);
		}

		#endregion

		#region Private Methods

		#endregion
	}
}