namespace RAKandACOVirtualStation_OCC.Views
{
	using Catel.Windows.Controls;
	using RAKandACOVirtualStation_OCC.ViewModels;

	/// <summary>
	/// Interaction logic for Settings.xaml.
	/// </summary>
	public partial class Settings : Page
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Settings"/> class.
		/// </summary>
		public Settings()
		{
			InitializeComponent();
		}
		protected override System.Type GetViewModelType()
		{
			return typeof(SettingsViewModel);
		}
	}
}
