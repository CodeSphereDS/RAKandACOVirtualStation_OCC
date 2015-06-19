namespace RAKandACOVirtualStation_OCC.Views
{
	using Catel.Windows.Controls;

	using ViewModels;

	/// <summary>
	/// Interaction logic for Login.xaml.
	/// </summary>
	public partial class Login : Page
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Login"/> class.
		/// </summary>
		public Login()
		{
			InitializeComponent();
		}
		protected override System.Type GetViewModelType()
		{
			return typeof(LoginViewModel);
		}
	}
}
