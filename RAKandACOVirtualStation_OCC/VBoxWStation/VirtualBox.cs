using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using System.Net;
using RAKandACOVirtualStation_OCC.Common;
using SOAPService;
using System.Diagnostics;
using System.Collections.ObjectModel;
using RAKandACOVirtualStation_OCC.Repository;
using System.Net.Sockets;

namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class VirtualBox
	{
		#region variables
		private static IVirtualBox _VBox;
		private static IWebSessionManager _WebMngr;
		private static ObservableCollection<IMachine> _ImachineCollection;
		#endregion variables

		private static IVirtualBox connect(Uri url)
		{
			try
			{
				return connect(url, "", "");
			}
			catch (SoapException ex)
			{
				throw ex;
			}
			catch (WebException ex)
			{
				throw ex;
			}
		}
		private static IVirtualBox connect(Uri url, string username, string password)
		{
			try
			{
				vboxService access = new vboxService() { Url = url.ToString() };			
				String value = access.IWebsessionManager_logon(username, password);			
				return new IVirtualBox(value, access);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
			catch (WebException ex)
			{
				throw ex;
			}
		}
		public VirtualBox()
		{
		}

		#region Methods
		
		private static Uri VirtualStationServerUrl()
		{
			string VirtualHost = RAppConfiguration.GetConfigValue<string>("VirtualStationServer");
			int VirtualHostPort = RAppConfiguration.GetConfigValue<int>("VirtualStationServerPort");
			string serverIP = ServerIPAddress(VirtualHost).ToString();
			Debug.WriteLine("server address: " + ServerIPAddress(VirtualHost).ToString());
			var HostUrl = string.Concat("http://", ServerIPAddress(VirtualHost).ToString(), ":", VirtualHostPort.ToString());
			Debug.WriteLine(HostUrl);
			try
			{
				Uri h = new Uri(HostUrl);
				return h;
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Invalid URI/Hostname. Contact your System Developer to resolve this issue.");
			}
			throw new ArgumentException("Invalid URI/Hostname. Contact your System Developer to resolve this issue.");

		}
		private string VirtualStationServerUsername
		{
			get
			{
				return RAppConfiguration.GetConfigValue<string>("VirtualStationServerUsername");
			}
		}
		private string VirtualStationServerPassword
		{
			get
			{
				return RAppConfiguration.GetConfigValue<string>("VirtualStationServerPassword");
			}
		}

		public static ObservableCollection<IMachine> getVirtualStations()
		{
			if (!IsConnectedVBox)
			{
				LogonVBox();
			}
			_ImachineCollection = new ObservableCollection<IMachine>(_VBox.getMachines());
			foreach (IMachine m in _ImachineCollection)
			{
				Debug.WriteLine("imachine: " + m.MachineName + m.getParent().getReference());
			}
			return _ImachineCollection;
		}

		public static bool IsConnectedVBox
		{
			get
			{
				if (_WebMngr == null || string.IsNullOrEmpty(_WebMngr.getSessionObject().getReference()))
				{
					return false;
				}			
				else return true;
			}
		
		}
		public static void LogonVBox()
		{
			if (!IsConnectedVBox)
			{
				Debug.WriteLine("BeginLogon: " + DateTime.Now);
				_VBox = connect(VirtualStationServerUrl(), "", "");
				_WebMngr = new IWebSessionManager(_VBox.getReference(), _VBox.getWsAcces());
				Debug.WriteLine("EndLogon: " + DateTime.Now);
			}
		}
		public static void DisconnectVBox()
		{
			try
			{
				Debug.WriteLine("disconnecting from vbox...");
				_VBox.releaseRemote();
				_VBox = null;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		public static IWebSessionManager WebMgnr
		{
			get
			{
				if (_VBox == null)
				{
					throw new ArgumentNullException("VirtualBox Object is null");
				}
				if (_WebMngr == null)
				{
					_WebMngr = new IWebSessionManager(_VBox.getReference(), _VBox.getWsAcces());
				}
				return _WebMngr;
			}
		}
		public static string getVBoxExtraData(string key)
		{
			return _VBox.getExtraData(key);
		}
		private static IPAddress ServerIPAddress(string hostNameOrAddress)
		{
			if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
			{
				return null;
			}

			try
			{

				//IPAddress[] IPs = Dns.GetHostAddresses(hostNameOrAddress);				
				//foreach (IPAddress ip in IPs)
				//{

				//    if (ip.AddressFamily == AddressFamily.InterNetwork)
				//    {
				//        Debug.WriteLine("IP FOUND!-->" + ip.ToString());
				//        System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
				//        System.Net.NetworkInformation.PingReply pr = p.Send(ip);
				//        if (pr.Status == System.Net.NetworkInformation.IPStatus.Success)
				//        {
				//            return ip;
				//        }
				//    }
				//}

				IPHostEntry host = Dns.GetHostEntry(hostNameOrAddress);
				return host
					.AddressList
					.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
			}
			catch (Exception ex)
			{
				throw ex;
				return null;
			}
		}

		private int GetVirtualStationPort(string vsPort)
		{
			if (string.IsNullOrEmpty(vsPort) || vsPort == null)
			{
				return 0;
			}
			int x;
			int.TryParse(vsPort, out x);
			return x;
		}
		public static IVirtualBox GetVBox
		{
			get
			{
				return _VBox;
			}
		}
		#endregion Methods
	}
}
