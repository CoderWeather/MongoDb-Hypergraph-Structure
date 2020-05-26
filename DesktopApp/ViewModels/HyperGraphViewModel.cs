using System.Reactive;
using DesktopApp.Models;
using HyperGraphSharp.Models;
using ReactiveUI;

namespace DesktopApp.ViewModels
{
	public class HyperGraphViewModel : ViewModelBase
	{
		public HyperGraph Graph { get; }
		public ReactiveCommand<PocHyperGraphLayout, Unit> InvalidateMeasureButtonCommand { get; }
		public ReactiveCommand<PocHyperGraphLayout, Unit> InvalidateArrangeButtonCommand { get; }
		public ReactiveCommand<PocHyperGraphLayout, Unit> ReLayoutCommand { get; }
		public ReactiveCommand<PocHyperGraphLayout, Unit> SaveGraphImageCommand { get; }


		public HyperGraphViewModel(HyperGraph graph)
		{
			Graph = graph;

			InvalidateMeasureButtonCommand =
				ReactiveCommand.Create<PocHyperGraphLayout>(OnInvalidateMeasureButtonCommand);
			InvalidateArrangeButtonCommand =
				ReactiveCommand.Create<PocHyperGraphLayout>(OnInvalidateArrangeButtonCommand);
			ReLayoutCommand = ReactiveCommand.Create<PocHyperGraphLayout>(OnReLayoutCommand);
			SaveGraphImageCommand = ReactiveCommand.Create<PocHyperGraphLayout>(OnSaveGraphImageCommand);
		}

		private static void OnInvalidateMeasureButtonCommand(PocHyperGraphLayout layout)
		{
			layout.InvalidateMeasure();
		}

		private static void OnInvalidateArrangeButtonCommand(PocHyperGraphLayout layout)
		{
			// layout.InvalidateArrange();
			layout.InvalidateVisual();
		}

		private static void OnReLayoutCommand(PocHyperGraphLayout layout)
		{
			layout.Layout();
		}

		private static async void OnSaveGraphImageCommand(PocHyperGraphLayout layout)
		{
			await layout.ConvertGraphToImage();
		}
	}
}