using Catel.Windows;
using Catel.Data;
using Catel.MVVM.Properties;
using Catel.MVVM;
using Catel.Windows.Controls;
using System;
using RAKandACOVirtualStation_OCC.ViewModels;

namespace RAKandACOVirtualStation_OCC.Views
{
	
	/// <summary>
	/// Interaction logic for MainWindow.xaml.
	/// </summary>
	public partial class MainWindow : FirstFloor.ModernUI.Windows.Controls.ModernWindow 
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		/// <param name="viewModel">The view model to inject.</param>
		/// <remarks>
		/// This constructor can be used to use view-model injection.
		/// </remarks>
		//public ModernWindow()
		//    : base(new MainWindowViewModel(), 
		//                DataWindowMode.Custom,
		//                null,
		//                DataWindowDefaultButton.None,
		//                true,InfoBarMessageControlGenerationMode.None)			
		//{
		//    Initialize();
		//}

		MainWindowViewModel mainvm { get; set; }
		public MainWindow()					
		{
			InitializeComponent();
			if (mainvm == null)
			{
				mainvm = new MainWindowViewModel();
			}
			DataContext = mainvm;	 
			//Title= " RAK  &  ACO Virtual  Station  Client  V2.0 ";		
			IsTitleVisible = true;
									
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			if (FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("Exit application?",
				"Confirm", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
			{
				e.Cancel = true;
			}
			else
			{
				e.Cancel = true;
				Dispatcher.InvokeShutdown();
			}
			
		}
	
		protected  Type GetViewModelType()
		{
			return typeof(MainWindowViewModel);
		}			
	}
}
