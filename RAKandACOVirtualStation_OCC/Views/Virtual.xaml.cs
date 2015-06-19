using Catel.Windows.Controls;
using System;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;


namespace RAKandACOVirtualStation_OCC.Views
{	

	/// <summary>
	/// Interaction logic for Virtual.xaml.
	/// </summary>
	public partial class Virtual : Page,IContent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Virtual"/> class.
		/// </summary>
		public Virtual()
		{
			//this.DataContext = new RAKandACOVirtualStation_OCC.ViewModels.VirtualViewModel();
			InitializeComponent();
			//System.Diagnostics.Debug.WriteLine("Virtual xaml");		
		}
		
	
		protected override Type GetViewModelType()
		{
			return typeof(RAKandACOVirtualStation_OCC.ViewModels.VirtualViewModel);
		}

		public void OnFragmentNavigation(FragmentNavigationEventArgs e)
		{
		}
		public void OnNavigatedFrom(NavigationEventArgs e)
		{
		}
		public void OnNavigatedTo(NavigationEventArgs e)
		{
		}
		public void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			// ask user if navigating away is ok
			if (FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage("Navigate away?", "Navigate",
			  System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
			{
				e.Cancel = true;
			}
		}


	}
}
