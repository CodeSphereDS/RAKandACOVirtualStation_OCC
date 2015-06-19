using Catel.MVVM;
using Catel.Data;
using System.Configuration;
using System;
using System.Diagnostics;
using RAKandACOVirtualStation_OCC.VBoxWStation;
using RAKandACOVirtualStation_OCC.Repository;

namespace RAKandACOVirtualStation_OCC.ViewModels
{

	/// <summary>
	/// UserControl view model.
	/// </summary>
	public class SettingsViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
		/// </summary>
		public SettingsViewModel()
		{
			SettingsOK = new Command(OnSettingsOKExecute, OnSettingsOKCanExecute);
			SettingsCancel = new Command(OnSettingsCancelExecute);
			SelectBackupFolder = new Command(OnSelectBackupFolderExecute);


			//check appSettings if (key,value) exist!
			//GetKeyValues(new string[]{"VirtualStationServer","StartPort","EndPort"});			

			VirtualStationServer = RAppConfiguration.GetConfigValue<string>("VirtualStationServer");
			VirtualStationServerPort = RAppConfiguration.GetConfigValue<int>("VirtualStationServerPort");
			VirtualStationServerUsername = RAppConfiguration.GetConfigValue<string>("VirtualStationServerUsername");
			VirtualStationServerPassword = RAppConfiguration.GetConfigValue<string>("VirtualStationServerPassword");
			BackupFolderpath = RAppConfiguration.GetConfigValue<string>("VirtualStationBackupPath");
		}

		/// <summary>
		/// Gets the title of the view model.
		/// </summary>
		/// <value>The title.</value>
		public override string Title { get { return "View model title"; } }

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public string VirtualStationServer
		{
			get { return GetValue<string>(VirtualStationServerProperty); }
			set { SetValue(VirtualStationServerProperty, value); }
		}

		/// <summary>
		/// Register the VirtualStationServer property so it is known in the class.
		/// </summary>
		public static readonly PropertyData VirtualStationServerProperty = RegisterProperty("VirtualStationServer", typeof(string), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public int VirtualStationServerPort
		{
			get { return GetValue<int>(VirtualStationServerPortProperty); }
			set { SetValue(VirtualStationServerPortProperty, value); }
		}

		/// <summary>
		/// Register the VirtualStationServerPort property so it is known in the class.
		/// </summary>
		public static readonly PropertyData VirtualStationServerPortProperty = RegisterProperty("VirtualStationServerPort", typeof(int), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public string VirtualStationServerUsername
		{
			get { return GetValue<string>(VirtualStationServerUsernameProperty); }
			set { SetValue(VirtualStationServerUsernameProperty, value); }
		}

		/// <summary>
		/// Register the VirtualStationServerUsername property so it is known in the class.
		/// </summary>
		public static readonly PropertyData VirtualStationServerUsernameProperty = RegisterProperty("VirtualStationServerUsername", typeof(string), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public string VirtualStationServerPassword
		{
			get { return GetValue<string>(VirtualStationServerPasswordProperty); }
			set { SetValue(VirtualStationServerPasswordProperty, value); }
		}

		/// <summary>
		/// Register the VirtualStationServerPassword property so it is known in the class.
		/// </summary>
		public static readonly PropertyData VirtualStationServerPasswordProperty = RegisterProperty("VirtualStationServerPassword", typeof(string), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public string BackupFolderpath
		{
			get { return GetValue<string>(BackupFolderpathProperty); }
			set { SetValue(BackupFolderpathProperty, value); }
		}

		/// <summary>
		/// Register the BackupFolderpath property so it is known in the class.
		/// </summary>
		public static readonly PropertyData BackupFolderpathProperty = RegisterProperty("BackupFolderpath", typeof(string), null);

		#region commands
		/// <summary>
		/// Gets the SettingsOK command.
		/// </summary>
		public Command SettingsOK { get; private set; }


		/// <summary>
		/// Method to check whether the SettingsOK command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool OnSettingsOKCanExecute()
		{
			return true;
		}

		/// <summary>
		/// Method to invoke when the SettingsOK command is executed.
		/// </summary>
		private void OnSettingsOKExecute()
		{
			////System.Diagnostics.Debug.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().Location);	
			////var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
			////Configuration config = ConfigurationManager.OpenExeConfiguration(path);
			////if (config.HasFile)
			//{
			//    config.AppSettings.Settings["VirtualStationServer"].Value = VirtualStationServer;
			//    config.AppSettings.Settings["StartPort"].Value = StartPort.ToString();
			//    config.AppSettings.Settings["EndPort"].Value = EndPort.ToString();
			//    config.Save(ConfigurationSaveMode.Modified);
			//    ConfigurationManager.RefreshSection("appSettings");
			//}
			try
			{
				RAppConfiguration.SaveConfigValue<string>("VirtualStationServer", VirtualStationServer);
				RAppConfiguration.SaveConfigValue<int>("VirtualStationServerPort", VirtualStationServerPort);
				RAppConfiguration.SaveConfigValue<string>("VirtualStationServerUsername", VirtualStationServerUsername);
				RAppConfiguration.SaveConfigValue<string>("VirtualStationServerPassword", VirtualStationServerPassword);
				Debug.WriteLine("BAckupFolder Path: " + BackupFolderpath);
				Debug.WriteLine("password: " + VirtualStationServerPassword);
				RAppConfiguration.SaveConfigValue<string>("VirtualStationBackupPath", BackupFolderpath);

				FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(
					"Settings saved.", "Settings", System.Windows.MessageBoxButton.OK);
				if (VirtualBox.IsConnectedVBox)
				{
					VirtualBox.DisconnectVBox();
					var service = Catel.IoC.DependencyResolverExtensions.Resolve<Catel.Messaging.IMessageMediator>(this.DependencyResolver);
					service.SendMessage<bool>(false, "Authentication");
				}
			}
			catch (Exception ex)
			{
				FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(string.Format(
				"Error occured while saving changes. Please contact your system developer.\n\nError Description:\n	{0}", ex.Message),
				"Error", System.Windows.MessageBoxButton.OK);
			}
		}

		/// <summary>
		/// Gets the SettingsCancel command.
		/// </summary>
		public Command SettingsCancel { get; private set; }

		/// <summary>
		/// Method to invoke when the SettingsCancel command is executed.
		/// </summary>
		private void OnSettingsCancelExecute()
		{


		}

		/// <summary>
		/// Gets the SelectBackupFolder command.
		/// </summary>
		public Command SelectBackupFolder { get; private set; }

		/// <summary>
		/// Method to invoke when the SelectBackupFolder command is executed.
		/// </summary>
		private void OnSelectBackupFolderExecute()
		{
			System.Windows.Forms.FolderBrowserDialog folderDlg = new System.Windows.Forms.FolderBrowserDialog();
			folderDlg.ShowNewFolderButton = true;
			// Show the FolderBrowserDialog.
			System.Windows.Forms.DialogResult result = folderDlg.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				BackupFolderpath = folderDlg.SelectedPath;
			}
		}
		#endregion commands
		#region Methods

		#endregion Methods
	}
}
