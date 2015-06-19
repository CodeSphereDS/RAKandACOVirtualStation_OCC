namespace RAKandACOVirtualStation_OCC.Views
{
	using Catel.Windows.Controls;

	using ViewModels;

	/// <summary>
	/// Interaction logic for BlankPage.xaml.
	/// </summary>
	public partial class BlankPage : Page
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BlankPage"/> class.
		/// </summary>
		public BlankPage()
		{
			InitializeComponent();
		}

		protected override System.Type GetViewModelType()
		{
			return typeof(BlankPageViewModel);
		}
	}
}
