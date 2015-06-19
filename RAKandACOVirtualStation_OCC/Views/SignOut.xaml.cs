namespace RAKandACOVirtualStation_OCC.Views
{
	using Catel.Windows.Controls;

	using ViewModels;

	/// <summary>
	/// Interaction logic for SignOut.xaml.
	/// </summary>
	public partial class SignOut : Page
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SignOut"/> class.
		/// </summary>
		public SignOut()
		{
			InitializeComponent();
		}

		protected override System.Type GetViewModelType()
		{
			return typeof(SignOutViewModel);
		}
	}
}
