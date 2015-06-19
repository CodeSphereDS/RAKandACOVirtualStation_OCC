namespace RAKandACOVirtualStation_OCC.ViewModels
{
	using Catel.MVVM;
	using Catel.MVVM.Services;
	using Catel.IoC;

	/// <summary>
	/// UserControl view model.
	/// </summary>
	public class UserViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UserViewModel"/> class.
		/// </summary>
		public UserViewModel()
		{
			
		}

		/// <summary>
		/// Gets the title of the view model.
		/// </summary>
		/// <value>The title.</value>
		public override string Title { get { return "View model title"; } }

		// TODO: Register models with the vmpropmodel codesnippet
		// TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
	
	}
}
