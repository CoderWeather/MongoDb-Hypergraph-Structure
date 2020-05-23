using System.Windows;

namespace DesktopApp.Views
{
	public partial class MainView
	{
		public MainView() => InitializeComponent();

		private void UpdateGraphBtn_OnClick(object sender, RoutedEventArgs e)
		{
			GraphLayout.InvalidateMeasure();
		}
	}
}