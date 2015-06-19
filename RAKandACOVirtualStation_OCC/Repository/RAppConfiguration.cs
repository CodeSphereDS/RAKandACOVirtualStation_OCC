using System;
using System.Configuration;

namespace RAKandACOVirtualStation_OCC.Repository
{
	public class RAppConfiguration
	{

		public RAppConfiguration()
		{
		}

		public static T GetConfigValue<T>(string configName)
		{
			var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
			Configuration config = ConfigurationManager.OpenExeConfiguration(path);
			if (config.HasFile)
			{
				
				if (config.AppSettings.Settings[configName]==null)
				{					
					config.AppSettings.Settings.Add(configName, null);
					config.Save(ConfigurationSaveMode.Full);
					ConfigurationManager.RefreshSection("appSettings");
				}

				var r = config.AppSettings.Settings[configName].Value;

				if (r == null)
				{
					return default(T);
				}
				else if (typeof(T) == typeof(int))
				{
						int x;
						int.TryParse(r, out x);
						return (T)System.Convert.ChangeType(x, typeof(T));
				}
				return (T)System.Convert.ChangeType(r, typeof(T));
			}

			else throw new Exception("Configuration file not found!");


		}

		public static void SaveConfigValue<T>(string configname, T configvalue)
		{
			var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
			Configuration configFile = ConfigurationManager.OpenExeConfiguration(path);
			if (configFile.HasFile)
			{
				string c = configvalue.ToString();
				if (!string.IsNullOrWhiteSpace(c))
				{
					try
					{

						configFile.AppSettings.Settings[configname].Value = c;
						configFile.Save(ConfigurationSaveMode.Full);
						ConfigurationManager.RefreshSection("appSettings");
					}


					catch (Exception ex)
					{
						throw new Exception("Error saving configuration.");
					}
				}
			}
			else
				throw new Exception("Unable to locate configuration file.");
		}
	}

}
