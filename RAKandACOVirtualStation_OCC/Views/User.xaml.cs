namespace RAKandACOVirtualStation_OCC.Views
{
	using Catel.Windows.Controls;

	/// <summary>
	/// Interaction logic for User.xaml.
	/// </summary>
	public partial class User : Page
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="User"/> class.
		/// </summary>
		public User()
		{
			InitializeComponent();
		}
		protected override System.Type GetViewModelType()
		{
			return typeof(RAKandACOVirtualStation_OCC.ViewModels.UserViewModel);
		}
	}
}
