using System.Reactive;
using DesktopApp.Models;
using HyperGraphSharp.Models;
using ReactiveUI;

namespace DesktopApp.ViewModels
{
	public class HyperGraphViewModel : ViewModelBase
	{
		public HyperGraph Graph { get; }
		public ReactiveCommand<PocHyperGraphLayout, Unit> InvalidateButtonCommand { get; }

		public HyperGraphViewModel(HyperGraph graph)
		{
			Graph = graph;
			InvalidateButtonCommand = ReactiveCommand.Create<PocHyperGraphLayout>(OnInvalidateButtonCommand);
		}

		private static void OnInvalidateButtonCommand(PocHyperGraphLayout layout)
		{
			layout.InvalidateMeasure();
		}
	}
}