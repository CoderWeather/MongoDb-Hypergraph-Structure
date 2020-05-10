using System.Windows;
using DesktopApp.ViewModels;

namespace DesktopApp.Views
{
	public partial class MainView
	{
		public MainView()
		{
			InitializeComponent();
		}

		private void UpdateLayoutBtn_OnClick(object sender, RoutedEventArgs e)
		{
			var vm = DataContext as MainViewModel;
			GraphLayout.UpdateLayout();
		}

		private void LayoutBtn_OnClick(object sender, RoutedEventArgs e)
		{
			GraphLayout.Layout();
		}
	}
}