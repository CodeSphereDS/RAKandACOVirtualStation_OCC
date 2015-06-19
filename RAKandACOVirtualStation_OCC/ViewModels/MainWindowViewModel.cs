using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Navigation;
using Catel.MVVM;
using Catel.Data;
using Catel.Services;
using Catel;
using Catel.MVVM.Services;
using Catel.Messaging;
using System;


namespace RAKandACOVirtualStation_OCC.ViewModels
{
	/// <summary>
	/// MainWindow view model.
	/// </summary>
	public class MainWindowViewModel : ViewModelBase
	{
		#region Variables
		#endregion

		#region Constructor & destructor
		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
		/// </summary>
		public MainWindowViewModel()
			: base()
		{
			UserIsAuthenticated = true;
			MainWindowLoaded = new Command(OnMainWindowLoadedExecute);
			var service = Catel.IoC.DependencyResolverExtensions.Resolve<IMessageMediator>(this.DependencyResolver);
			service.Register<bool>(this, AuthenticateUser, "Authentication");
			ModelBase.SuspendValidationForAllModels = true;

			//GlobalLeanAndMeanModel = true;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the title of the view model.
		/// </summary>
		/// <value>The title.</value>
		public override string Title { get { return "RAK & ACO Virtual Solutions 2.0"; } }

		#endregion
		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public LinkCollection TitleLinkCollection
		{
			get { return GetValue<LinkCollection>(TitleLinkCollectionProperty); }
			set { SetValue(TitleLinkCollectionProperty, value); }
		}

		/// <summary>
		/// Register the TitleLinkCollection property so it is known in the class.
		/// </summary>
		public static readonly PropertyData TitleLinkCollectionProperty = RegisterProperty("TitleLinkCollection", typeof(LinkCollection), null);


		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public LinkGroupCollection ActiveLinkGroup
		{
			get { return GetValue<LinkGroupCollection>(ActiveLinkGroupProperty); }
			private set { SetValue(ActiveLinkGroupProperty, value); }
		}

		/// <summary>
		/// Register the ActiveLinkGroup property so it is known in the class.
		/// </summary>
		public static readonly PropertyData ActiveLinkGroupProperty = RegisterProperty("ActiveLinkGroup", typeof(LinkGroupCollection), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public bool UserIsAuthenticated
		{
			get { return GetValue<bool>(UserIsAuthenticatedProperty); }
			set { SetValue(UserIsAuthenticatedProperty, value); }
		}

		/// <summary>
		/// Register the UserIsAuthenticated property so it is known in the class.
		/// </summary>
		public static readonly PropertyData UserIsAuthenticatedProperty = RegisterProperty("UserIsAuthenticated", typeof(bool), null, (sender, e) => ((MainWindowViewModel)sender).OnUserIsAuthenticatedChanged());

		/// <summary>
		/// Called when the UserIsAuthenticated property has changed.
		/// </summary>
		private void OnUserIsAuthenticatedChanged()
		{
			MenuDisplay();
		}

		#region Commands
		/// <summary>
		/// Gets the MainWindowLoaded command.
		/// </summary>
		public Command MainWindowLoaded { get; private set; }

		/// <summary>
		/// Method to invoke when the MainWindowLoaded command is executed.
		/// </summary>
		private void OnMainWindowLoadedExecute()
		{
			MenuDisplay();
		}
		#endregion Commands

		#region Methods
		private void MenuDisplay()
		{
			if (ActiveLinkGroup == null)
			{
				ActiveLinkGroup = new LinkGroupCollection();
			}
			if (TitleLinkCollection == null)
			{
				TitleLinkCollection = new LinkCollection();
			}

			TitleLinkCollection.Clear();
			ActiveLinkGroup.Clear();

			if (UserIsAuthenticated)
			{
				// fill with groups when logged in
				LinkGroup AuthGroup = new LinkGroup();
				AuthGroup.GroupName = "AuthenticatedGroup";
				AuthGroup.DisplayName = "VirtualStation";

				//AuthGroup.Links.Add(new Link() { DisplayName = "User", Source = new System.Uri("/Views/User.xaml", UriKind.Relative) });
				AuthGroup.Links.Add(new Link() { DisplayName = "Stations", Source = new System.Uri("/Views/Virtual.xaml", UriKind.Relative) });
				AuthGroup.Links.Add(new Link() { DisplayName = "Sign Out", Source = new System.Uri("/Views/SignOut.xaml", UriKind.Relative) });
				ActiveLinkGroup.Add(AuthGroup);

				TitleLinkCollection.Add(new Link() { DisplayName = "Settings", Source = new Uri("/Views/Settings.xaml", UriKind.Relative) });
				TitleLinkCollection.Add(new Link() { DisplayName = "About", Source = new Uri("/Views/About.xaml", UriKind.Relative) });
			}
			else if (!UserIsAuthenticated)
			{
				LinkGroup UnAuthGroup = new LinkGroup();
				UnAuthGroup.GroupName = "UnAuthenticatedGroup";
				UnAuthGroup.DisplayName = "VirtualStation";
				UnAuthGroup.Links.Add(new Link() { DisplayName = "Login", Source = new Uri("/Views/Login.xaml", UriKind.Relative) });

				ActiveLinkGroup.Add(UnAuthGroup);
			}

		}

		[MessageRecipient(Tag = "Logout")]
		private void AuthenticateUser(bool value)
		{
			//var service = Catel.IoC.DependencyResolverExtensions.Resolve<IMessageService>(this.DependencyResolver);					
			//service.Show(value);
			UserIsAuthenticated = value;
		}
		#endregion
	}
}
