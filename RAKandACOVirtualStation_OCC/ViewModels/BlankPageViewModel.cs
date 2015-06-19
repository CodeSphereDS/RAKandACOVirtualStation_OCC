namespace RAKandACOVirtualStation_OCC.ViewModels
{
	using Catel.MVVM;

	/// <summary>
	/// UserControl view model.
	/// </summary>
	public class BlankPageViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BlankPageViewModel"/> class.
		/// </summary>
		public BlankPageViewModel()
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
