using Catel.MVVM;
using Catel.Data;
using Catel;
using RAKandACOVirtualStation_OCC.RDP;
using RAKandACOVirtualStation_OCC.Views;
using RAKandACOVirtualStation_OCC.Views.VirtualViews;
using Catel.MVVM.Views;
using Catel.MVVM.Services;
using AxMSTSCLib;
using System;
using System.Windows;
using System.Diagnostics;
using RAKandACOVirtualStation_OCC.Repository;
using RAKandACOVirtualStation_OCC.VBoxWStation;
using System.ComponentModel;
using System.Windows.Threading;
using SOAPService;
using FirstFloor.ModernUI.Windows.Controls;
using System.IO;
using System.Collections.Generic;

namespace RAKandACOVirtualStation_OCC.ViewModels.VirtualViewModels
{
	/// <summary>
	/// UserControl view model.
	/// </summary>
	public class VirtualStationViewModel : ViewModelBase
	{
		#region Variables
		IPleaseWaitService pleasewaitservice { get; set; }
		BackgroundWorker worker;
		public VirtualStationView rdpxview { get; set; }
		public MsRdpClient8 rdpx { get; set; }

		//IWebSessionManager _iwebMngr;
		//ISession _isession;
		//IVirtualBox _ivirtualBox;
		//IConsole _iconsole;
		//IGuest _iguest;
		//IProgress _iprogress;
		//IVRDEServer _ivrdeServer;
		//IVRDEServerInfo _ivrdeServerInfo;
		#endregion Variables

		/// <summary>
		/// Initializes a new instance of the <see cref="VirtualStationViewModel"/> class.
		/// </summary>
		public VirtualStationViewModel(IMachine virtualstation)
		{
			VirtualStation = virtualstation;

			if (pleasewaitservice == null)
			{
				pleasewaitservice = Catel.IoC.DependencyResolverExtensions.Resolve<IPleaseWaitService>(this.DependencyResolver);
			}
			VirtualStationConnectDisconnect = new Command(OnVirtualStationConnectDisconnectExecute, OnVirtualStationConnectDisconnectCanExecute);
			VirtualStationFullScreenView = new Command(OnVirtualStationFullScreenViewExecute, OnVirtualStationFullScreenViewCanExecute);
			VirtualStationSwitchPower = new Command(OnVirtualStationSwitchPowerExecute, OnVirtualStationSwitchPowerCanExecute);
			UserControlLoaded = new Command(OnUserControlLoadedExecute);
			WfhLoaded = new Command(OnWfhLoadedExecute);
			ShowAboutInfo = new Command(OnShowAboutInfoExecute);

			CopyFileFrom = new Command(OnCopyFileFromExecute);

		}

		protected override void OnClosing()
		{
			Debug.WriteLine("Closing VM");
			if (rdpx != null && rdpx.Connected == 1)
			{
				pleasewaitservice.Show(string.Format("Disconnecting {0} session...", VirtualStation.MachineName));
				rdpx.Disconnect();
			}
			base.OnClosing();
		}
		private BackgroundWorker CreateWorker()
		{
			BackgroundWorker thisWorker = new BackgroundWorker();
			thisWorker.WorkerSupportsCancellation = true;

			thisWorker.DoWork += delegate(object sender, DoWorkEventArgs args)
			{
				if (worker.CancellationPending)
				{
					args.Cancel = true;
					return;
				}

				if (args != null)
				{
					ISession session = (ISession)args.Argument;
					try
					{
						args.Result = VirtualStation.getState();
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex.Message);
					}
					finally
					{
						if (session.sessionState() == SessionState.Locked)
						{
							session.unlockMachine();
						}
					}

				}
			};

			thisWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs args)
			{
				if (args.Result == null)
				{
					VirtualStationPowerStatus = false;
				}

				else
				{
					VirtualStationPowerStatus = (MachineState)args.Result == SOAPService.MachineState.Running ||
						(MachineState)args.Result == SOAPService.MachineState.Stopping ||
						(MachineState)args.Result == SOAPService.MachineState.Saving ? true : false;
				}
			};
			return thisWorker;
		}

		#region Properties

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public bool VirtualStationPowerStatus
		{
			get { return GetValue<bool>(VirtualStationPowerStatusProperty); }
			set { SetValue(VirtualStationPowerStatusProperty, value); }
		}

		/// <summary>
		/// Register the VirtualStationPowerStatus property so it is known in the class.
		/// </summary>
		public static readonly PropertyData VirtualStationPowerStatusProperty = RegisterProperty("VirtualStationPowerStatus", typeof(bool), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>

		/// <summary>
		/// Register the IsConnectingToVirtualStation property so it is known in the class.
		/// </summary>
		public static readonly PropertyData IsConnectingToVirtualStationProperty = RegisterProperty("IsConnectingToVirtualStation", typeof(bool), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public bool IsConnectedToVirtualStation
		{
			get { return GetValue<bool>(IsConnectedToVirtualStationProperty); }
			set { SetValue(IsConnectedToVirtualStationProperty, value); }
		}

		/// <summary>
		/// Register the IsConnectedToVirtualStation property so it is known in the class.
		/// </summary>
		public static readonly PropertyData IsConnectedToVirtualStationProperty = RegisterProperty("IsConnectedToVirtualStation", typeof(bool), null);

		/// <summary>
		/// Gets the title of the view model.
		/// </summary>
		/// <value>The title.</value>
		public override string Title { get { return "View model title"; } }

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public IMachine VirtualStation
		{
			get { return GetValue<IMachine>(VirtualStationProperty); }
			private set { SetValue(VirtualStationProperty, value); }
		}

		/// <summary>
		/// Register the VirtualStation property so it is known in the class.
		/// </summary>
		public static readonly PropertyData VirtualStationProperty = RegisterProperty("VirtualStation", typeof(IMachine));

		#endregion Properties

		#region Commands

		/// <summary>
		/// Gets the VirtualStationSwitchPower command.
		/// </summary>
		public Command VirtualStationSwitchPower { get; private set; }

		/// <summary>
		/// Method to check whether the VirtualStationSwitchPower command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool OnVirtualStationSwitchPowerCanExecute()
		{
			ISession _isession = getSessionObject();
			SessionState _isessionState = _isession.sessionState();
			if (worker != null && !worker.IsBusy && _isessionState==SessionState.Unlocked)
			{
				VirtualStation.lockMachine(_isession, LockType.Shared);
				worker.RunWorkerAsync(_isession);
			}
			else if(worker!=null && worker.IsBusy)
			{
				Debug.WriteLine("worker is busy"); 
			}
			else if (_isessionState != SessionState.Unlocked)
			{
				Debug.WriteLine("Session is still locked");
			}
			return true;
		}

		/// <summary>
		/// Method to invoke when the VirtualStationSwitchPower command is executed.
		/// </summary>
		private void OnVirtualStationSwitchPowerExecute()
		{
			if (!VirtualStationPowerStatus)
			{
				VirtualStationPowerOn();
			}
			else if (VirtualStationPowerStatus)
			{
				MachineState vstate = VirtualStation.getState();
				if (vstate == MachineState.Saving || vstate == MachineState.Aborted
					|| vstate == MachineState.Stopping)
				{
					if (ModernDialog.ShowMessage(string.Format("{0} is unresponsive. Force shutdown?",
						VirtualStation.MachineName), "Confirm", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
					{
						VirtualStationAbort();
					}
				}
				else if (vstate == MachineState.Running)
				{
					if (ModernDialog.ShowMessage(string.Format("Shutdown {0} VirtualStation?",
						VirtualStation.MachineName), "Confirm",
						System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
					{
						VirtualStationShutDown();
					}
				}
			}
		}


		/// <summary>
		/// Gets the VirtualStationConnectDisconnect command.
		/// </summary>
		public Command VirtualStationConnectDisconnect { get; private set; }

		/// <summary>
		/// Method to check whether the VirtualStationConnectDisconnect command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool OnVirtualStationConnectDisconnectCanExecute()
		{
			return VirtualStationPowerStatus;
			//return true;
		}

		/// <summary>
		/// Method to invoke when the VirtualStationConnectDisconnect command is executed.
		/// </summary>
		private void OnVirtualStationConnectDisconnectExecute()
		{
			if (VirtualStationPowerStatus)
			{
				if (rdpx == null)
				{
					OnWfhLoadedExecute();
				}
				try
				{

					if (rdpx.Connected == 0)
					{
						rdpx.AdvancedSettings9.AuthenticationLevel = 2;
						rdpx.UserName = RAppConfiguration.GetConfigValue<string>("VirtualStationServerUsername");
						rdpx.AdvancedSettings9.ClearTextPassword = RAppConfiguration.GetConfigValue<string>("VirtualStationServerPassword");
						rdpx.AdvancedSettings9.RDPPort = getVRDPPort();
						rdpx.Server = RAppConfiguration.GetConfigValue<string>("VirtualStationServer");

						pleasewaitservice.Show(string.Format("Connecting to {0} VirtualStation...", VirtualStation.MachineName));

						rdpx.Connect();

					}
					else if (rdpx.Connected == 1)
					{
						pleasewaitservice.Show(string.Format("Disconnecting from {0} VirtualStation...", VirtualStation.MachineName));
						rdpx.Disconnect();
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Error on rpdx.Connect(). ");
					Debug.WriteLine("Error message: " + ex.Message);
				}
			}
		}

		/// <summary>
		/// Gets the VirtualStationFullScreenView command.
		/// </summary>
		public Command VirtualStationFullScreenView { get; private set; }

		/// <summary>
		/// Method to check whether the VirtualStationFullScreenView command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool OnVirtualStationFullScreenViewCanExecute()
		{
			return IsConnectedToVirtualStation;
		}

		/// <summary>
		/// Method to invoke when the VirtualStationFullScreenView command is executed.
		/// </summary>
		private void OnVirtualStationFullScreenViewExecute()
		{
			rdpx.AdvancedSettings9.SmartSizing = true;
			rdpx.FullScreen = true;
		}

		/// <summary>
		/// Gets the OnWfhLoaded command.
		/// </summary>
		public Command WfhLoaded { get; private set; }

		/// <summary>
		/// Method to invoke when the OnWfhLoaded command is executed.
		/// </summary>
		private void OnWfhLoadedExecute()
		{
			if (rdpxview == null)
			{
				var viewservice = Catel.IoC.DependencyResolverExtensions.Resolve<IViewManager>(this.DependencyResolver);
				var v = viewservice.GetViewsOfViewModel(this);

				foreach (VirtualStationView view in v)
				{
					if (view.GetType().Equals(typeof(VirtualStationView)))
					{
						rdpxview = (VirtualStationView)view;
						Debug.WriteLine("VirtualStationView found!");
						break;
					}

				}
				if (rdpxview == null)
					Debug.WriteLine("VirtualStationView NOT found!");
			}
			if (rdpx == null)
			{
				rdpx = new MsRdpClient8();
				rdpx.Dock = System.Windows.Forms.DockStyle.Fill;
				rdpxview.wfh.Child = rdpx;
				InitRDP8(rdpx);
			}
			rdpxview.wfh.Child = rdpx;

			if (worker == null)
			{
				worker = CreateWorker();
			}
		}

		/// <summary>
		/// Gets the ShowAboutInfo command.
		/// </summary>
		public Command ShowAboutInfo { get; private set; }
		/// <summary>
		/// Method to invoke when the ShowAboutInfo command is executed.
		/// </summary>
		private void OnShowAboutInfoExecute()
		{
			ModernDialog.ShowMessage(string.Format("RAK & ACO VirtualStation Client 2.0\n\n{0}\nOperating System: {1}\nMemory Size: {2} MB\n\n\n" +
			"Software Developer:\n" +
			"     Mark Vincent R. Magbero\n" +
			"     mvmagbero@gmail.com\n" +
			"     (+63)-920-659-6459",
				VirtualStation.MachineName, VirtualStation.getOSDescription(), VirtualStation.getMemorySize()), "About", MessageBoxButton.OK);

		}
		/// <summary>
		/// Gets the UserControlUnloaded command.
		/// </summary>
		public Command UserControlLoaded { get; private set; }

		/// <summary>
		/// Method to invoke when the UserControlUnloaded command is executed.
		/// </summary>
		private void OnUserControlLoadedExecute()
		{
			var xx = new EventHandler((object sender, EventArgs e) =>
					{
						Debug.WriteLine("xtype: " + sender.GetType());
						Debug.WriteLine("xx.type: " + e.GetType());
						if (worker != null)
						{
							worker.CancelAsync();
							Debug.WriteLine("worker is busy: " + worker.IsBusy);
							worker.Dispose();
						}

						if (rdpx != null)
						{
							if (rdpx.Connected == 1)
							{
								rdpx.Disconnect();
								//shutdownEvent.WaitOne(5000);
								Debug.WriteLine("RDC disconnected.");
							}
							rdpx.Dispose();
						}

						if (!VirtualStationHasConnectedUsers())
						{
							ISession _isession = getSessionObject();
							pleasewaitservice.Show(() =>VirtualStationShutDown(),
								string.Format("Shutting down {0}...", VirtualStation.MachineName));
							_isession.releaseRemote();
						}
					});

			Dispatcher.CurrentDispatcher.ShutdownStarted -= xx;
			Dispatcher.CurrentDispatcher.ShutdownStarted += xx;
		}

		/// <summary>
		/// Gets the CopyFileFrom command.
		/// </summary>
		public Command CopyFileFrom { get; private set; }

		/// <summary>
		/// Method to invoke when the CopyFileFrom command is executed.
		/// </summary>

		private void OnCopyFileFromExecute()
		{
			DirectoryInfo backupDirPath = new DirectoryInfo(@RAppConfiguration.GetConfigValue<string>("VirtualStationBackupPath"));
			DirectoryInfo sourceDirPath = new DirectoryInfo(@"C:\Payroll");
			if (string.IsNullOrWhiteSpace(backupDirPath.FullName))
			{
				ModernDialog.ShowMessage("Unable to create backup. Backup folder path is not set.", "Error", MessageBoxButton.OK);
				return;
			}

			ISession _isession = getSessionObject();
			IGuestSession _iguestSession=null;
			
			int timeout = 0;
			while (_isession.sessionState() != SessionState.Unlocked)
			{
				Debug.WriteLine("waiting for session to unlock...");
				System.Threading.Thread.Sleep(200);
				timeout += 1;
				if (timeout >= 15)
				{
					return;
				}
			}

			if (_isession.sessionState() == SessionState.Unlocked && VirtualStation.getState() == MachineState.Running)
			{
				try
				{
					pleasewaitservice.Show("Settings Credentials...");
					VirtualStation.lockMachine(_isession, LockType.Shared);
					IConsole _iconsole = _isession.getConsole();

					if (!_iconsole.getGuestEnteredACPIMode())
					{
						throw new Exception("VirtualStation has not yet entered ACPI Mode");
					}

					IGuest _iguest = _iconsole.getGuest();
					Debug.WriteLine("iguest session: " + _iguest.getReference()); 
					//if(_iguest.
					//_iguest.setCredentials("VStation01", "user123", "", true);
					_iguestSession = CreateGuestSession(_iguest,"VStation01", "user123");
					if (_iguestSession == null)
					{
						throw new Exception("Error while executing backup procedures.");
					}
					if (!_iguestSession.directoryExists(sourceDirPath.FullName))
					{
						throw new Exception("Error locating Payroll System file(s).");
					}
					Directory.CreateDirectory(backupDirPath.FullName);
					backupDirPath.CreateSubdirectory(sourceDirPath.Name);

					DirectoryOpenFlag[] dirOpenFlag = { };
					List<string> fsObj = new List<string>();

					Debug.WriteLine("opening target dir");
					IGuestDirectory iGuestDir = _iguestSession.directoryOpen("C:\\Windows\\", "", dirOpenFlag);
					Debug.WriteLine("success opening target dir");
					pleasewaitservice.UpdateStatus("Creating list of files to backup...");
					traverseGuestDir(fsObj, iGuestDir, _iguestSession);

					foreach (string ff in fsObj)
					{
						Debug.WriteLine(ff);
						DirectoryInfo d = new DirectoryInfo(ff);
						//pleasewaitservice.UpdateStatus("Backing up file(s)... "+ d.Name);
						pleasewaitservice.UpdateStatus(ff.IndexOf(ff) + 1, fsObj.Count, "{0} out of {1} items");
						System.Threading.Thread.Sleep(500);
					}
					pleasewaitservice.Hide();
				}
				catch (Exception ex)
				{
					pleasewaitservice.Hide();
					Debug.WriteLine(ex.Message);
				}
				finally
				{
					if (_iguestSession != null && _iguestSession.getGuestSessionStatus()==GuestSessionStatus.Started)
					{
						_iguestSession.close();
					}
					if (_isession.sessionState() == SessionState.Locked)
					{
						_isession.unlockMachine();
					}
				}
			}
		}

		#endregion Commands

		#region Methods

		private void traverseGuestDir(List<string> fsObjCollection, IGuestDirectory guestFsObjInfo, IGuestSession guestSession)
		{
			if (fsObjCollection == null)
			{
				fsObjCollection = new List<string>();
			}
			while (true)
			{
				try
				{
					IFsObjInfo fsObjInfo = guestFsObjInfo.read();
					string fsObjName = fsObjInfo.getName();
					FsObjType fsObjType = fsObjInfo.getObjType();
					if (fsObjName != "." && fsObjName != ".." && fsObjType == FsObjType.Directory)
					{
						IGuestDirectory _guestFsObjInfo = guestSession.directoryOpen(guestFsObjInfo.getDirectoryName() + fsObjInfo.getName() + "\\", "", null);
						traverseGuestDir(fsObjCollection, _guestFsObjInfo, guestSession);
					}
					else if (fsObjType == FsObjType.File)
					{
						string dirName = guestFsObjInfo.getDirectoryName();
						fsObjCollection.Add(dirName + fsObjName);
					}
				}
				catch (Exception ex)
				{
					break;
				}
			}
		}
		private IGuestSession CreateGuestSession(IGuest _iguest, string username, string password)
		{
			try
			{
				IGuestSession _iguestSession = _iguest.createSession(username, password, "", "");
				int timeout = 0;
				while (_iguestSession.getGuestSessionStatus() != GuestSessionStatus.Started)
				{
					System.Threading.Thread.Sleep(200);
					timeout += 1;
					if (timeout >= 15)
					{
						Debug.WriteLine("unable to create iguestsession");
						return null;
					}
				}
				return _iguestSession;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return null;
			}
			
		}
		#region RDP EventHandlers
		public void rdpxx_OnDisconnected(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
		{
			pleasewaitservice.Hide();
			if (e != null && e.discReason != 1 && rdpx.DisconnectReason().ContainsKey(e.discReason))
			{
				string errorDescription = rdpx.DisconnectReason()[e.discReason];
				ModernDialog.ShowMessage(string.Format("Error connecting to {0}. If problems persist, please contact your system developer.\n\nError code:	0x{1}\nDescription:	{2}.",
				VirtualStation.MachineName, e.discReason.ToString("X"), errorDescription), "Error", MessageBoxButton.OK);
			}

			//switch (e.discReason)
			//{
			//    case 264:
			//        {
			//            ModernDialog.ShowMessage(string.Format("Error connecting to {0}. If problems persist, please contact your system developer. Error code 0x108",
			//            VirtualStation.MachineName), "Error", MessageBoxButton.OK);
			//            break;
			//        }

			//    default: break;
			//}
			IsConnectedToVirtualStation = false;
			Debug.WriteLine("OnDisconnected");
			Debug.WriteLine("Disconnect Reason " + e.discReason);

		}
		public void rdpxx_OnServiceMessageReceived(object sender, AxMSTSCLib.IMsTscAxEvents_OnServiceMessageReceivedEvent e)
		{
			Debug.WriteLine("OnServiceMessageReceived");
		}
		public void rdpxx_OnLoginComplete(object sender, EventArgs e)
		{
			IsConnectedToVirtualStation = true;
			pleasewaitservice.Hide();
			Debug.WriteLine("OnLoginComplete");
		}
		public void rdpxx_OnLogonError(object sender, AxMSTSCLib.IMsTscAxEvents_OnLogonErrorEvent e)
		{
			pleasewaitservice.Hide();
			ModernDialog.ShowMessage("Logon error encountered. Please contact your System Developer.", "Error",
				MessageBoxButton.OK);
			Debug.WriteLine("OnLogonError");
			IsConnectedToVirtualStation = false;
		}
		public void rdpxx_OnConnected(object sender, EventArgs e)
		{
			Debug.WriteLine("OnConnected");
			IsConnectedToVirtualStation = true;
			pleasewaitservice.Hide();
		}
		public void rdpxx_OnFatalError(object sender, AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEvent e)
		{
			Debug.WriteLine("OnFatalError");
		}
		public void rdpxx_OnIdleTimeoutNotification(object sender, EventArgs e)
		{
			Debug.WriteLine("OnIdleTimeout");
			ModernDialog.ShowMessage("Connection idle timeout. Please reconnect..", "Idle Timeout",
				MessageBoxButton.OK);
		}
		public void rdpxx_OnFocusReleased(object sender, AxMSTSCLib.IMsTscAxEvents_OnFocusReleasedEvent e)
		{
			Debug.WriteLine("OnFocusReleased");
		}
		public void rdpxx_OnConnecting(object s, EventArgs e)
		{
			//pleasewaitservice.Show(string.Format("Connecting to {0}", VirtualStation.VSName));
			//IsConnectingToVirtualStation = true;
			Debug.WriteLine("OnConnecting...");
		}
		public void rdpxx_OnAuthenticationWarningDisplayed(object s, EventArgs e)
		{
			Debug.WriteLine("RDPx_onauthenticationwarningdisplay");
		}
		public void rdpx_OnEnterFullScreenMode(object sender, EventArgs e)
		{
			MSTSCLib.IMsRdpClientNonScriptable5 xx = (MSTSCLib.IMsRdpClientNonScriptable5)rdpx.GetOcx();
			xx.ConnectionBarText = VirtualStation.MachineName;
		}
		public void rdpx_OnWarning(object sender, IMsTscAxEvents_OnWarningEvent e)
		{
			Debug.WriteLine("OnWarning ... ");
		}
		#endregion RDP EventHandlers
		private MsRdpClient8 InitRDP8(MsRdpClient8 rdpx)
		{
			rdpx.OnAuthenticationWarningDisplayed += new EventHandler(rdpxx_OnAuthenticationWarningDisplayed);
			rdpx.OnConnecting += new EventHandler(rdpxx_OnConnecting);
			rdpx.OnFatalError += new AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEventHandler(rdpxx_OnFatalError);
			rdpx.OnLogonError += new AxMSTSCLib.IMsTscAxEvents_OnLogonErrorEventHandler(rdpxx_OnLogonError);
			rdpx.OnConnected += new EventHandler(rdpxx_OnConnected);
			rdpx.OnIdleTimeoutNotification += new EventHandler(rdpxx_OnIdleTimeoutNotification);
			rdpx.OnLoginComplete += new EventHandler(rdpxx_OnLoginComplete);
			rdpx.OnServiceMessageReceived += new AxMSTSCLib.IMsTscAxEvents_OnServiceMessageReceivedEventHandler(rdpxx_OnServiceMessageReceived);
			rdpx.OnDisconnected += new AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEventHandler(rdpxx_OnDisconnected);
			rdpx.OnEnterFullScreenMode += new EventHandler(rdpx_OnEnterFullScreenMode);
			rdpx.OnWarning += new IMsTscAxEvents_OnWarningEventHandler(rdpx_OnWarning);

			rdpx.AdvancedSettings9.ConnectionBarShowMinimizeButton = false;
			rdpx.AdvancedSettings9.AudioRedirectionMode = 2;

			rdpx.AdvancedSettings9.BitmapPersistence = 1;
			rdpx.AdvancedSettings9.BitmapVirtualCache16BppSize = 32;
			rdpx.AdvancedSettings9.CachePersistenceActive = 1;
			rdpx.AdvancedSettings9.PerformanceFlags = 8;
			rdpx.AdvancedSettings9.RedirectDrives = false;
			rdpx.AdvancedSettings9.RedirectPOSDevices = false;
			rdpx.AdvancedSettings9.RedirectPrinters = false;
			rdpx.AdvancedSettings9.RedirectSmartCards = false;
			rdpx.AdvancedSettings9.RedirectDevices = false;
			rdpx.AdvancedSettings9.AudioCaptureRedirectionMode = false;
			rdpx.AdvancedSettings8.NetworkConnectionType = 6;

			rdpx.AdvancedSettings9.EnableAutoReconnect = false;
			rdpx.AdvancedSettings9.singleConnectionTimeout = 3;
			rdpx.AdvancedSettings9.overallConnectionTimeout = 3;
			rdpx.AdvancedSettings9.shutdownTimeout = 5;

			rdpx.AdvancedSettings9.ConnectionBarShowPinButton = false;
			rdpx.ConnectedStatusText = "Connected";
			rdpx.AdvancedSettings9.EnableAutoReconnect = true;
			rdpx.AdvancedSettings9.GrabFocusOnConnect = true;
			rdpx.AdvancedSettings9.DisplayConnectionBar = true;
			rdpx.AdvancedSettings9.ConnectionBarShowMinimizeButton = false;
			rdpx.AdvancedSettings9.PinConnectionBar = true;

			//rdpx.AdvancedSettings9.ContainerHandledFullScreen = 1;

			rdpx.Name = "RDPx";
			rdpx.DesktopWidth = 1366;
			rdpx.DesktopHeight = 768;
			rdpx.AdvancedSettings9.SmartSizing = true;

			rdpx.AdvancedSettings8.EnableCredSspSupport = true;


			return rdpx;
		}
		public void VirtualStationAbort()
		{
			ISession _isession = getSessionObject();
			int timeout = 0;
			while (_isession.sessionState() != SessionState.Unlocked)
			{
				Debug.WriteLine("waiting for session to unlock...");
				System.Threading.Thread.Sleep(200);
				timeout += 1;
				if (timeout >= 15)
				{
					return;
				}
			}
			if (VirtualStation.getState() == MachineState.Stopping || VirtualStation.getState() == MachineState.Saving)
			{
				try
				{
					VirtualStation.LaunchVM(_isession, "emergencystop", "");
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
				finally
				{
					if (_isession.sessionState() == SessionState.Locked)
					{
						_isession.unlockMachine();
					}
				}
			}

		}
		public void VirtualStationShutDown()
		{
			ISession _isession = getSessionObject();
			int timeout = 0;
			while (_isession.sessionState() != SessionState.Unlocked)
			{
				Debug.WriteLine("waiting for session to unlock...");
				System.Threading.Thread.Sleep(200);
				timeout += 1;
				if (timeout >= 15)
				{
					return;
				}
			}

			if (_isession.sessionState() == SessionState.Unlocked)
			{
				try
				{
					VirtualStation.lockMachine(_isession, LockType.Shared);
					IConsole _iconsole = _isession.getConsole();
					IVRDEServerInfo _ivrdeServerInfo = _iconsole.getVRDEServerInfo();
					if (_ivrdeServerInfo.active)
					{
						if (ModernDialog.ShowMessage("Another User is still using this VirtualStation. Force shutdown?", "Confirm",
							System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
						{
							return;
						}
					}
					pleasewaitservice.Show(() =>
					{
						if (VirtualStation.getState() == SOAPService.MachineState.Running)
						{
							IProgress p = _iconsole.saveState();
							int saveTimeout = 0;
							while (!p.completed())
							{
								saveTimeout += 1;
								System.Threading.Thread.Sleep(500);
								if (saveTimeout >= 20)
								{
									pleasewaitservice.UpdateStatus("Timeout occured. Forcing shutdown...");
									VirtualStation.LaunchVM(_isession, "emergencystop", "");
									break;
								}
							}
						}
					}, "Shutting Down VirtualStation...");

				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
				finally
				{
					if (_isession.sessionState() == SessionState.Locked)
					{
						_isession.unlockMachine();
					}
				}
			}
		}
		public void VirtualStationPowerOn()
		{
			ISession _isession = getSessionObject();
			int timeout = 0;
			while (_isession.sessionState() != SessionState.Unlocked)
			{
				System.Threading.Thread.Sleep(200);
				timeout += 1;
				if (timeout >= 15)
				{
					return;
				}
			}
			if (_isession.sessionState() == SessionState.Unlocked)
			{
				pleasewaitservice.Show(string.Format("Starting {0} VirtualStation", VirtualStation.MachineName));
				try
				{
					IProgress p = VirtualStation.LaunchVM(_isession, "headless", "");
					while (!p.completed())
					{
						System.Threading.Thread.Sleep(500);
					};
				}
				catch (Exception ex)
				{
					pleasewaitservice.Hide();
					Debug.WriteLine(ex.Message);
					ModernDialog.ShowMessage(string.Format
						("An Error has been encountered while trying to Power-on the {0} VirtualStation. " +
					"Please contact your system developer to resolve the issue.", VirtualStation.MachineName), "Error",
				MessageBoxButton.OK);
				}
				finally
				{
					pleasewaitservice.Hide();
					if (_isession.sessionState() == SessionState.Locked)
					{
						_isession.unlockMachine();
					}
				}
			}
		}
		public void SwitchSaveStateVirtualStation()
		{
			ISession _isession = getSessionObject();
			int timeout = 0;
			while (_isession.sessionState() != SessionState.Unlocked)
			{
				System.Threading.Thread.Sleep(200);
				timeout += 1;
				if (timeout >= 15)
				{
					return;
				}
			}
			if (_isession.sessionState() == SessionState.Unlocked)
			{
				try
				{
					VirtualStation.lockMachine(_isession, SOAPService.LockType.Shared);
					IConsole _iconsole = _isession.getConsole();
					IProgress p = _iconsole.saveState();
					while (!p.completed())
					{
						System.Threading.Thread.Sleep(500);
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}

				finally
				{
					if (_isession.sessionState() == SessionState.Locked)
					{
						_isession.unlockMachine();
					}
				}
			}
		}

		public bool VirtualStationHasConnectedUsers()
		{
			ISession _isession = getSessionObject();
			bool vrdeServerActive = true;
			try
			{
				VirtualStation.lockMachine(_isession, SOAPService.LockType.Shared);
				IConsole _iconsole = _isession.getConsole();
				IVRDEServerInfo _ivrdeServerInfo = _iconsole.getVRDEServerInfo();
				vrdeServerActive = _ivrdeServerInfo.active;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return false;
			}
			finally
			{
				if (_isession.sessionState() == SOAPService.SessionState.Locked)
				{
					_isession.unlockMachine();
				}
			}
			return vrdeServerActive;
		}

		private int getVRDPPort()
		{
			ISession _isession = getSessionObject();
			int vrdeServerPort = 0;
			try
			{
				VirtualStation.lockMachine(_isession, SOAPService.LockType.Shared);
				IConsole _iconsole = _isession.getConsole();
				IVRDEServerInfo _ivrdeServerInfo = _iconsole.getVRDEServerInfo();
				vrdeServerPort = _ivrdeServerInfo.port;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			finally
			{
				if (_isession.sessionState() == SOAPService.SessionState.Locked)
				{
					_isession.unlockMachine();
				}
			}
			return vrdeServerPort;
		}
		private ISession getSessionObject()
		{
			if (!VirtualBox.IsConnectedVBox)
			{
				VirtualBox.LogonVBox();
				IVirtualBox _ivirtualBox = VirtualBox.GetVBox;
				VirtualStation = _ivirtualBox.findMachine(VirtualStation.MachineName);
				VirtualBox.getVirtualStations();
				Debug.WriteLine("Acquired new session...");
			}
			return VirtualBox.WebMgnr.getSessionObject();
		}
	}

		#endregion Methods




}
