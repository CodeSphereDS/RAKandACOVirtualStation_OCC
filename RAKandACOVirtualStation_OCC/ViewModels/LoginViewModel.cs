using System;
using Catel.MVVM;
using Catel.Data;
using System.Security.Cryptography;
using System.Text;
using RAKandACOVirtualStation_OCC.Repository;
using RAKandACOVirtualStation_OCC.VBoxWStation;

namespace RAKandACOVirtualStation_OCC.ViewModels
{
	/// <summary>
	/// UserControl view model.
	/// </summary>
	public class LoginViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LoginViewModel"/> class.
		/// </summary>
		public LoginViewModel()
		{
			CommandSignIn = new Command(OnCommandSignInExecute, OnCommandSignInCanExecute);
		}

		/// <summary>
		/// Gets the title of the view model.
		/// </summary>
		/// <value>The title.</value>
		public override string Title { get { return "View model title"; } }

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public string Username
		{
			get { return GetValue<string>(UsernameProperty); }
			set { SetValue(UsernameProperty, value); }
		}

		/// <summary>
		/// Register the Username property so it is known in the class.
		/// </summary>
		public static readonly PropertyData UsernameProperty = RegisterProperty("Username", typeof(string), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public string Password
		{
			get { return GetValue<string>(PasswordProperty); }
			set { SetValue(PasswordProperty, value); }
		}

		/// <summary>
		/// Register the Password property so it is known in the class.
		/// </summary>
		public static readonly PropertyData PasswordProperty = RegisterProperty("Password", typeof(string), null);

		/// <summary>
		/// Gets the CommandSignIn command.
		/// </summary>
		public Command CommandSignIn { get; private set; }


		/// <summary>
		/// Method to check whether the CommandSignIn command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool OnCommandSignInCanExecute()
		{
			return Username != null && Password != null && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
		}

		/// <summary>
		/// Method to invoke when the CommandSignIn command is executed.
		/// </summary>
		private void OnCommandSignInExecute()
		{
			if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
			{
				return;
			}
			var service = Catel.IoC.DependencyResolverExtensions.Resolve<Catel.Messaging.IMessageMediator>(this.DependencyResolver);
			var pleasewaitservice = Catel.IoC.DependencyResolverExtensions.Resolve<Catel.MVVM.Services.IPleaseWaitService>(this.DependencyResolver);
			
			try
			{				
				pleasewaitservice.Show("Logging in to VirtualStation Server...");
				VirtualBox.LogonVBox();
				//d7cda0ca2c8586e512c425368fcb2bba62e81475bfceb4284f4906de8ec242bc
				//d679bc6852ca239cc080e313bcd4176b8a4dd8f999d0112b468d76bbb6dbc5df
				//var username = RAppConfiguration.GetConfigValue<string>("VirtualStationServerUsername");
				//var password = RAppConfiguration.GetConfigValue<string>("VirtualStationServerPassword");
				var usernameKey = sha256(Username);
				var keyValue = VirtualBox.getVBoxExtraData(usernameKey);
				if (!string.IsNullOrWhiteSpace(keyValue) && sha256(Password) == keyValue)
				{
					service.SendMessage<bool>(true, "Authentication");
					pleasewaitservice.UpdateStatus("Logging in to VirtualStation Server...OK");
					System.Threading.Thread.Sleep(2500);
					pleasewaitservice.Hide();
					return;
				}
				else
				{
					pleasewaitservice.Hide();
					FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(
						"Login error. Please ensure correct Username/Password. If problem persist, please contact your system developer.", "Login", System.Windows.MessageBoxButton.OK);
					VirtualBox.DisconnectVBox();
				}				
			}
			catch (Exception ex)
			{
				pleasewaitservice.Hide();
				FirstFloor.ModernUI.Windows.Controls.ModernDialog.ShowMessage(
					"Login error. Please ensure correct Username/Password. If problem persist, please contact your system developer.", "Login", System.Windows.MessageBoxButton.OK);
				VirtualBox.DisconnectVBox();
			}
		}

		public string CalculateMD5Hash(string input)
		{
			// step 1, calculate MD5 hash from input
			MD5 md5 = MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);

			// step 2, convert byte array to hex string

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}
			return sb.ToString();
		}

		public string sha256(string password)
		{
			SHA256Managed crypt = new SHA256Managed();
			string hash = String.Empty;
			byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
			foreach (byte bit in crypto)
			{
				hash += bit.ToString("x2");
			}
			return hash;
		}

	}
}
