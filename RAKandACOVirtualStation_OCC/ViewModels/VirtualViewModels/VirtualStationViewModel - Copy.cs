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
		IWebSessionManager webMngr { get; set; }
		ISession session { get; set; }
		IVirtualBox ivirtualBox { get; set; }
		IConsole iconsole { get; set; }
		IProgress progress { get; set; }
		IVRDEServer ivrdeServer { get; set; }
		IVRDEServerInfo ivrdeServerInfo { get; set; }
		BackgroundWorker worker;

		public VirtualStationView rdpxview { get; set; }
		public MsRdpClient8 rdpx { get; set; }
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
			VirtualStationPowerOn = new Command(OnVirtualStationPowerOnExecute, OnVirtualStationPowerOnCanExecute);
			UserControlLoaded = new Command(OnUserControlLoadedExecute);
			WfhLoaded = new Command(OnWfhLoadedExecute);
			ShowAboutInfo = new Command(OnShowAboutInfoExecute);

			ShareFolderPath = new Command(OnShareFolderPathExecute);

			CopyFileFrom = new Command(OnCopyFileFromExecute);


			if (worker == null)
			{
				worker = CreateWorker();
			}

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
					IMachine machine = (IMachine)args.Argument;
					args.Result = machine.getState();
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
					VirtualStationPowerStatus = (SOAPService.MachineState)args.Result == SOAPService.MachineState.Running ? true : false;
					//Debug.WriteLine("VirtualStationPowerStatus : " + VirtualStationPowerStatus);
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
		[Model]
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
		/// Gets the VirtualStationPowerOn command.
		/// </summary>
		public Command VirtualStationPowerOn { get; private set; }

		/// <summary>
		/// Method to check whether the VirtualStationPowerOn command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool OnVirtualStationPowerOnCanExecute()
		{
			if (!worker.IsBusy)
			{
				//	Debug.WriteLine("Worker is busy!");
				worker.RunWorkerAsync(VirtualStation);
			}
			return true;
		}

		/// <summary>
		/// Method to invoke when the VirtualStationPowerOn command is executed.
		/// </summary>
		private void OnVirtualStationPowerOnExecute()
		{
			VirtualStationShutDown();
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
							pleasewaitservice.Show(() => SwitchSaveStateVirtualStation(),
								string.Format("Shutting down {0}...", VirtualStation.MachineName));
							session.releaseRemote();
						}
					});

			Dispatcher.CurrentDispatcher.ShutdownStarted -= xx;
			Dispatcher.CurrentDispatcher.ShutdownStarted += xx;
		}

		/// <summary>
		/// Gets the ShareFolderPath command.
		/// </summary>
		public Command ShareFolderPath { get; private set; }

		/// <summary>
		/// Method to invoke when the ShareFolderPath command is executed.
		/// </summary>
		private void OnShareFolderPathExecute()
		{
			webMngr = new IWebSessionManager(VirtualStation.getWsAcces());
			ivirtualBox = VirtualStation.getParent();
			session = webMngr.getSessionObject(ivirtualBox);

			if (this.VirtualStation.getState() == MachineState.Running)
			{
				if (!(session.sessionState() == SessionState.Locked))
				{
					VirtualStation.lockMachine(session, SOAPService.LockType.Shared);
				}
				iconsole = session.getConsole();
				iconsole.setShareFolder();
			}

			if (session.sessionState() == SessionState.Locked)
			{
				session.unlockMachine();
			}
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
			//Debug.WriteLine("replaced path: " + @RAppConfiguration.GetConfigValue<string>("VirtualStationBackupPath").Replace(@"\", @"/"));
			DirectoryInfo backupDirPath = new DirectoryInfo(@RAppConfiguration.GetConfigValue<string>("VirtualStationBackupPath"));
			Debug.WriteLine("escaped backupDirPath: " + backupDirPath.FullName);
			DirectoryInfo sourceDirPath = new DirectoryInfo(@"C:\Payroll");
			if (string.IsNullOrWhiteSpace(backupDirPath.FullName))
			{
				ModernDialog.ShowMessage("Unable to create backup. Backup folder path is not set.", "Error", MessageBoxButton.OK);
				return;
			}

			webMngr = new IWebSessionManager(VirtualStation.getWsAcces());
			ivirtualBox = VirtualStation.getParent();
			session = webMngr.getSessionObject(ivirtualBox);

			if (this.VirtualStation.getState() == MachineState.Running)
			{
				if (!(session.sessionState() == SessionState.Locked))
				{
					VirtualStation.lockMachine(session, SOAPService.LockType.Shared);
				}
				iconsole = session.getConsole();
				IGuest guest = iconsole.getGuest();
				//guest.setCredentials("VStation01", "user123", "", true);
				IGuestSession guestSession = guest.createSession("VStation01", "user123", "", "");
				//Debug.WriteLine("Session status: " + guestSession.getGuestSessionStatus())

				pleasewaitservice.Show(() =>
					{
						GuestSessionWaitResult waitresult = GuestSessionWaitResult.None;
						int retry = 0;
						while (waitresult != GuestSessionWaitResult.Start)
						{
							waitresult = guestSession.guestSessionWaitFor(GuestSessionWaitForFlag.Start, 5000);
							retry += 1;
							if (retry >= 3)
							{
								ModernDialog.ShowMessage("Error while creating backup file(s). If problem persists, please contact your system developer", "Error", MessageBoxButton.OK);
								break;
							}
						}
						if (waitresult == GuestSessionWaitResult.Start)
						{
							//check if source dir exist
							if (!guestSession.directoryExists(sourceDirPath.FullName))
							{
								ModernDialog.ShowMessage("Error locating Payroll System file(s). Please contact your system developer.", "Error", MessageBoxButton.OK);
							}
							else
							{
								if (!Directory.Exists(backupDirPath.FullName))
								{
									try
									{
										System.IO.Directory.CreateDirectory(backupDirPath.FullName);
									}
									catch (Exception ex)
									{
										pleasewaitservice.Hide();
										Debug.WriteLine(ex.Message);
										ModernDialog.ShowMessage(string.Format("Unable to create target directory.\nDescription:\n\t{0}", ex.ToString()), "Error", MessageBoxButton.OK);
										return;
									}
								}
								if (!Directory.Exists(backupDirPath.FullName + @"\\" + sourceDirPath.Name))
								{
									try
									{
										backupDirPath.CreateSubdirectory(sourceDirPath.Name);
									}
									catch (Exception ex)
									{
										pleasewaitservice.Hide();
										Debug.WriteLine(ex.Message);
										ModernDialog.ShowMessage(string.Format("Unable to create target subdirectory.\nDescription:\n\t{0}", ex.ToString()), "Error", MessageBoxButton.OK);
										return;
									}
								}
							//test
							Debug.WriteLine("waitresult: " + waitresult);
							//System.Threading.Thread.Sleep(1000);
							DirectoryOpenFlag[] dirOpenFlag = { DirectoryOpenFlag.None };
							Debug.WriteLine("sourceDir.FullName: " + sourceDirPath.FullName);
							IGuestDirectory iGuestDir = guestSession.directoryOpen(sourceDirPath.FullName + "\\", "", null); 

							Dictionary<IFsObjInfo, string> fsObj = new Dictionary<IFsObjInfo, string>();
							int totalFile = 0;
							pleasewaitservice.UpdateStatus("Creating list of files to backup...");
							traverseGuestDir(fsObj, iGuestDir, guestSession,totalFile);
							pleasewaitservice.UpdateStatus(string.Format("{0} file(s) added to list...",totalFile));
							int counter = 1;
								foreach (KeyValuePair<IFsObjInfo, string> ff in fsObj)
								{									
									Debug.WriteLine(string.Format("{0} path: {1}", ff.Key.getName(), ff.Value));
									pleasewaitservice.UpdateStatus(counter,totalFile,string.Format("Copying file(s)... {0}{0}",ff.Value,ff.Key.getName()));
									CopyFileFlag[] copyFlags = { CopyFileFlag.Recursive, CopyFileFlag.Update, CopyFileFlag.FollowLinks };
									
									IProgress p = guestSession.copyFrom(sourceDirPath.FullName + "\\", backupDirPath.FullName, copyFlags);
									while (!p.completed())
									{
										System.Threading.Thread.Sleep(500);
									}
								}
							}
							//IGuestFsObjInfo iGuestFsObjInfo = guestSession.directoryQueryInfo(sourceDirPath.FullName + "\\");
							//Debug.WriteLine("iGuestFsObjInfo hardlinks : " + iGuestFsObjInfo.getHardLinks());
							//Debug.WriteLine("iGuestFsObjInfo nodeID : " + iGuestFsObjInfo.getNodeID());

							//while (true)
							//{
							//    int cnt = 0;
							//    try
							//    {
							//        IFsObjInfo fsobjinfo = iGuestDir.read();
							//        Debug.WriteLine("fsobjFullName: " + iGuestDir.getDirectoryName() + fsobjinfo.getName());
							//        Debug.WriteLine("fsobjinfoNodeID: " + fsobjinfo.getNodeID());
							//        Debug.WriteLine("fsobjGenerationID: " + fsobjinfo.getGenerationID());
							//        Debug.WriteLine("fsobjGID: " + fsobjinfo.getGID());
							//        Debug.WriteLine("fsobjtype: " + fsobjinfo.getObjType());
							//        //Debug.WriteLine(string.Format("ifsobj{0} file/folder name: {1}",cnt, ifsobj[cnt].getName()));
							//        //cnt += 1;
							//    }
							//    catch (Exception ex)
							//    {
							//        break;
							//    }
							//}
							//test
						}
					}, "Setting credentials...");
				guestSession.close();
			}

			if (session.sessionState() == SessionState.Locked)
			{
				session.unlockMachine();
			}
		}
		#endregion Commands

		#region Methods

		private void traverseGuestDir(Dictionary<IFsObjInfo, string> fsObjCollection, IGuestDirectory guestFsObjInfo, IGuestSession guestSession,int totalFile)
		{
			if (fsObjCollection == null)
			{
				fsObjCollection = new Dictionary<IFsObjInfo, string>();
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
						traverseGuestDir(fsObjCollection, _guestFsObjInfo, guestSession,totalFile);
					}
					else if (fsObjType == FsObjType.File)
					{						
						string dirName = guestFsObjInfo.getDirectoryName();
						fsObjCollection.Add(fsObjInfo, dirName);
						totalFile += 1;
					}
				}
				catch (Exception ex)
				{
					break;
				}
			}
		}

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

		public void VirtualStationShutDown()
		{
			webMngr = new IWebSessionManager(VirtualStation.getWsAcces());
			ivirtualBox = VirtualStation.getParent();
			session = webMngr.getSessionObject(ivirtualBox);

			if (VirtualStationPowerStatus == true)
			{
				if (!(session.sessionState() == SOAPService.SessionState.Locked))
				{
					VirtualStation.lockMachine(session, SOAPService.LockType.Shared);
				}
				iconsole = session.getConsole();
				ivrdeServerInfo = iconsole.getVRDEServerInfo();


				if (ModernDialog.ShowMessage("Shutdown VirtualStation?", "Confirm",
		  System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
				{
					//check if multipleusers are connected?

					if (!ivrdeServerInfo.active)
					{
						pleasewaitservice.Show(() =>
						{
							Debug.WriteLine(session.sessionState().ToString());
							if (VirtualStation.getState() == SOAPService.MachineState.Running)
							{
								//if (iconsole.getGuestEnteredACPIMode())
								//{
								//    iconsole.powerButton();
								//}
								//else iconsole.powerDown();
								//while (VirtualStation.getState() == SOAPService.MachineState.Running)
								//{
								//    System.Threading.Thread.Sleep(1000);
								//}
								IProgress p = iconsole.saveState();
								while (!p.completed())
								{
									System.Threading.Thread.Sleep(1000);
								}
							}
						}, "Shutting Down VirtualStation...");
					}
					else if (ivrdeServerInfo.active)
					{
						if (ModernDialog.ShowMessage("Another User is still using this VirtualStation. Force shutdown?", "Confirm",
		  System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
						{
							pleasewaitservice.Show(() =>
							{
								Debug.WriteLine(session.sessionState().ToString());
								if (VirtualStation.getState() == SOAPService.MachineState.Running)
								{
									//if (iconsole.getGuestEnteredACPIMode())
									//{
									//    iconsole.powerButton();
									//}
									//else iconsole.powerDown();

									//while (VirtualStation.getState() == SOAPService.MachineState.Running)
									//{
									//    System.Threading.Thread.Sleep(1000);
									//}
									IProgress p = iconsole.saveState();
									while (!p.completed())
									{
										System.Threading.Thread.Sleep(1000);
									}
								}
							}, "Shutting Down VirtualStation...");
						}
					}
				}

				if (session.sessionState() == SessionState.Locked)
				{
					session.unlockMachine();
				}
			}
			else if (VirtualStationPowerStatus == false)
			{
				pleasewaitservice.Show(string.Format("Starting {0} VirtualStation", VirtualStation.MachineName));
				//if(!(session.sessionState()==SessionState.Unlocked))
				//{
				//    ModernDialog.ShowMessage("Session State locked .. either busy or spawning. MachineState =" + VirtualStation.getState()
				//        ,"Error",MessageBoxButton.OK);

				//}

				Debug.WriteLine("session state: " + session.sessionState());
				Debug.WriteLine("machine state: " + VirtualStation.getState());
				try
				{

					IProgress p = VirtualStation.LaunchVM(VirtualStation.getReference(), session, "headless", "");
					while (!p.completed())
					{
						System.Threading.Thread.Sleep(1000);
					};
					Debug.WriteLine("session state after launchvm: " + session.sessionState());
					Debug.WriteLine("machine state after launchvm: " + VirtualStation.getState());
				}
				catch (Exception ex)
				{
					pleasewaitservice.Hide();
					ModernDialog.ShowMessage(string.Format
						("An Error has been encountered while trying to Power-on the {0} VirtualStation. " +
					"Please contact your system developer to resolve the issue.", VirtualStation.MachineName), "Error",
				MessageBoxButton.OK);
				}
				finally
				{
					pleasewaitservice.Hide();
					if (session.sessionState() == SessionState.Locked)
					{
						//session.unlockMachine();
					}
				}
			}

		}
		public void SwitchSaveStateVirtualStation()
		{
			webMngr = new IWebSessionManager(VirtualStation.getWsAcces());
			ivirtualBox = VirtualStation.getParent();
			session = webMngr.getSessionObject(ivirtualBox);

			if (this.VirtualStation.getState() == MachineState.Running)
			{
				if (!(session.sessionState() == SessionState.Locked))
				{
					VirtualStation.lockMachine(session, SOAPService.LockType.Shared);
				}
				iconsole = session.getConsole();
				IProgress p = iconsole.saveState();
				while (!p.completed())
				{
					System.Threading.Thread.Sleep(1000);
				}
			}

			if (session.sessionState() == SessionState.Locked)
			{
				session.unlockMachine();
			}
		}

		public bool VirtualStationHasConnectedUsers()
		{
			try
			{
				webMngr = new IWebSessionManager(VirtualStation.getWsAcces());
				ivirtualBox = VirtualStation.getParent();
				session = webMngr.getSessionObject(ivirtualBox);

				if (!(session.sessionState() == SOAPService.SessionState.Locked))
				{
					VirtualStation.lockMachine(session, SOAPService.LockType.Shared);
				}
				IConsole iconsole = session.getConsole();
				ivrdeServerInfo = iconsole.getVRDEServerInfo();
				if (session.sessionState() == SOAPService.SessionState.Locked)
				{
					session.unlockMachine();
				}
				return ivrdeServerInfo.active;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		private int getVRDPPort()
		{
			webMngr = new IWebSessionManager(VirtualStation.getWsAcces());
			ivirtualBox = VirtualStation.getParent();
			session = webMngr.getSessionObject(ivirtualBox);

			if (!(session.sessionState() == SOAPService.SessionState.Locked))
			{
				VirtualStation.lockMachine(session, SOAPService.LockType.Shared);
			}
			IConsole iconsole = session.getConsole();
			ivrdeServerInfo = iconsole.getVRDEServerInfo();
			if (session.sessionState() == SOAPService.SessionState.Locked)
			{
				session.unlockMachine();
			}
			return ivrdeServerInfo.port;
		}

		#endregion Methods


	}

}
