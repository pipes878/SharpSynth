using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SynthTest
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		#region Overrides of Application

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var window = new MainWindow();

			// Create the ViewModel to which 
			// the main window binds.
			var viewModel = new MainWindowViewModel();
			Exit += (_, __) => viewModel.Dispose();

			// When the ViewModel asks to be closed, 
			// close the window.
			//EventHandler handler = null;
			//handler = delegate
			//{
			//    viewModel.RequestClose -= handler;
			//    window.Close();
			//};
			//viewModel.RequestClose += handler;

			// Allow all controls in the window to 
			// bind to the ViewModel by setting the 
			// DataContext, which propagates down 
			// the element tree.
			window.DataContext = viewModel;
			viewModel.Play();
			window.Show();
		}

		#endregion
	}
}
