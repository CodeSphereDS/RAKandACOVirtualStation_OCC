using Catel.MVVM;
using Catel.Data;
using System.Configuration;

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

			//check appSettings if (key,value) exist!
			GetKeyValues(new string[]{"VirtualStationServer","StartPort","EndPort"});			
			
			VirtualStationServer =  RAppConfiguration.GetConfigValue<string>("VirtualStationServer");
			StartPort = RAppConfiguration.GetConfigValue<int>("StartPort");
			EndPort = RAppConfiguration.GetConfigValue<int>("EndPort");		
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
		public int StartPort
		{
			get { return GetValue<int>(StartPortProperty); }
			set { SetValue(StartPortProperty, value); }
		}

		/// <summary>
		/// Register the StartPort property so it is known in the class.
		/// </summary>
		public static readonly PropertyData StartPortProperty = RegisterProperty("StartPort", typeof(int), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public int EndPort
		{
			get { return GetValue<int>(EndPortProperty); }
			set { SetValue(EndPortProperty, value); }
		}

		/// <summary>
		/// Register the EndPort property so it is known in the class.
		/// </summary>
		public static readonly PropertyData EndPortProperty = RegisterProperty("EndPort", typeof(int), null);

		
		
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
			//System.Diagnostics.Debug.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().Location);
			var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
			Configuration config = ConfigurationManager.OpenExeConfiguration(path);
			if (config.HasFile)
			{
				config.AppSettings.Settings["VirtualStationServer"].Value = VirtualStationServer;
				config.AppSettings.Settings["StartPort"].Value = StartPort.ToString();
				config.AppSettings.Settings["EndPort"].Value = EndPort.ToString();

				config.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection("appSettings");
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

		#region Methods

		private void GetKeyValues(string[] keys)
		{
			var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
			Configuration config = ConfigurationManager.OpenExeConfiguration(path);
			if (config.HasFile)
			{
				foreach (string s in keys)
				{
					if (ConfigurationManager.AppSettings[s] == null)
					{
						config.AppSettings.Settings.Add(s,"");
						config.Save(ConfigurationSaveMode.Modified);
						ConfigurationManager.RefreshSection("appSettings");
					}
				
				}
				
			}
			else throw new SettingsPropertyNotFoundException();
		}

		
		
		#endregion Methods
	}
}
