using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using AppLib;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

#pragma warning disable 8618

namespace DesktopApp.ViewModels
{
	[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
	public class MainViewModel : ViewModelBase
	{
		#region Public Properties

		[Reactive] public int SelectedGraphIndex { get; set; }
		public HyperGraphViewModel SelectedGraph { [ObservableAsProperty] get; }
		public List<HyperGraphViewModel> HyperGraphs { get; }

		[Reactive] public Visibility SideGraphInfoPanelVisibility { get; set; }

		#endregion

		#region Public Commands

		public ReactiveCommand<Unit, Unit> SideGraphInfoPanelVisibilityCommand { get; }

		#endregion

		#region Public Constructor

		public MainViewModel()
		{
			SideGraphInfoPanelVisibility = Visibility.Visible;

			HyperGraphs = CachedHyperGraphs.Cached
			   .Select(hg => new HyperGraphViewModel(hg))
			   .ToList();

			this.WhenAnyValue(x => x.SelectedGraphIndex)
			   .Where(index => index > 0 && index < HyperGraphs.Count)
			   .Select(index => HyperGraphs[index])
			   .ToPropertyEx(this, x => x.SelectedGraph);

			SideGraphInfoPanelVisibilityCommand = ReactiveCommand.Create(OnOpenSideGraphInfoPanelCommand);
		}

		#endregion

		#region Private Methods
		
		private void OnOpenSideGraphInfoPanelCommand()
		{
			SideGraphInfoPanelVisibility = SideGraphInfoPanelVisibility switch
			{
				Visibility.Visible   => Visibility.Collapsed,
				Visibility.Collapsed => Visibility.Visible,
				_                    => Visibility.Collapsed
			};
		}

		#endregion
	}
}