using Catel.MVVM;
using Catel.IoC;
using Catel.MVVM.Services;
using Catel.Messaging;

namespace RAKandACOVirtualStation_OCC.ViewModels
{

	/// <summary>
	/// UserControl view model.
	/// </summary>
	public class SignOutViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SignOutViewModel"/> class.
		/// </summary>
		public SignOutViewModel()
		{
			CommandSignOut = new Command(OnCommandSignOutExecute, OnCommandSignOutCanExecute);

		}

		/// <summary>
		/// Gets the title of the view model.
		/// </summary>
		/// <value>The title.</value>
		public override string Title { get { return "View model title"; } }
		/// <summary>
		/// Gets the CommandSignOut command.
		/// </summary>
		public Command CommandSignOut { get; private set; }

		// TODO: Move code below to constructor

		// TODO: Move code above to constructor

		/// <summary>
		/// Method to check whether the CommandSignOut command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool OnCommandSignOutCanExecute()
		{
			return true;
		}

		/// <summary>
		/// Method to invoke when the CommandSignOut command is executed.
		/// </summary>
		private void OnCommandSignOutExecute()
		{
			var service = Catel.IoC.DependencyResolverExtensions.Resolve<IMessageMediator>(this.DependencyResolver);
			service.SendMessage<bool>(false, "Authentication");
		}
	}
}
