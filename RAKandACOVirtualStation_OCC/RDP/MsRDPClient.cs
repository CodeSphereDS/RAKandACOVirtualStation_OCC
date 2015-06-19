using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSTSCLib;
using AxMSTSCLib;

namespace RAKandACOVirtualStation_OCC.RDP
{
	public class MsRdpClient2 : AxMSTSCLib.AxMsRdpClient2NotSafeForScripting
	{
		public MsRdpClient2()
			: base()
		{
		}


		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			//Fix for the missing focus issue on the rdp client component
			if (m.Msg == 0x0021) //WM_MOUSEACTIVATE ref:http://msdn.microsoft.com/en-us/library/ms645612(VS.85).aspx
				this.Focus();
			base.WndProc(ref m);
		}
	}
	public class MsRdpClient3 : AxMSTSCLib.AxMsRdpClient3NotSafeForScripting
	{
		public MsRdpClient3()
			: base()
		{
		}

		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			//Fix for the missing focus issue on the rdp client component
			if (m.Msg == 0x0021) //WM_MOUSEACTIVATE ref:http://msdn.microsoft.com/en-us/library/ms645612(VS.85).aspx
				this.Focus();
			base.WndProc(ref m);
		}
	}
	public class MsRdpClient4 : AxMSTSCLib.AxMsRdpClient4NotSafeForScripting
	{
		public MsRdpClient4()
			: base()
		{
		}

		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			//Fix for the missing focus issue on the rdp client component
			if (m.Msg == 0x0021) //WM_MOUSEACTIVATE ref:http://msdn.microsoft.com/en-us/library/ms645612(VS.85).aspx
				this.Focus();
			base.WndProc(ref m);
		}
	}
	public class MsRdpClient5 : AxMSTSCLib.AxMsRdpClient5NotSafeForScripting
	{
		public MsRdpClient5()
			: base()
		{
		}

		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			//Fix for the missing focus issue on the rdp client component
			if (m.Msg == 0x0021) //WM_MOUSEACTIVATE ref:http://msdn.microsoft.com/en-us/library/ms645612(VS.85).aspx
				this.Focus();
			base.WndProc(ref m);
		}
	}
	public class MsRdpClient6 : AxMSTSCLib.AxMsRdpClient6NotSafeForScripting
	{
		//Fix for the missing focus issue on the rdp client component
		public MsRdpClient6()
			: base()
		{
		}

		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			//Fix for the missing focus issue on the rdp client component
			if (m.Msg == 0x0021) //WM_MOUSEACTIVATE ref:http://msdn.microsoft.com/en-us/library/ms645612(VS.85).aspx
				this.Focus();
			base.WndProc(ref m);
		}
	}
	public class MsRdpClient7 : AxMSTSCLib.AxMsRdpClient7NotSafeForScripting
	{
		public MsRdpClient7()
			: base()
		{
		}

		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			//Fix for the missing focus issue on the rdp client component
			if (m.Msg == 0x0021) //WM_MOUSEACTIVATE ref:http://msdn.microsoft.com/en-us/library/ms645612(VS.85).aspx
				this.Focus();
			base.WndProc(ref m);
		}

	}

	public class MsRdpClient8 : AxMSTSCLib.AxMsRdpClient8NotSafeForScripting
	{
		public MsRdpClient8()
			: base()
		{
		}

		//protected override void WndProc(ref System.Windows.Forms.Message m)
		//{
		//    //Fix for the missing focus issue on the rdp client component
		//    if (m.Msg == 0x0021) //WM_MOUSEACTIVATE ref:http://msdn.microsoft.com/en-us/library/ms645612(VS.85).aspx
		//        this.Focus();		
		//        if (m!=null)
		//            base.WndProc(ref m);

		//}
		private Dictionary<int, string> disconnectReason;

		public Dictionary<int, string> DisconnectReason()
		{
			if (disconnectReason == null)
			{
				disconnectReason = new Dictionary<int, string>
				{
				{2308, "Socket closed"},
				{3, "Remote disconnection by server. This is not an error code."},
				{3080, "Decompression error"},
				{264, "Connection timed out"},
				{3078, "Decryption error"},
{260, "DNS name lookup failure"},
{1288, "DNS lookup failed"},
{2822, "Encryption error"},
{1540, "Windows Sockets gethostbyname call failed."},
{520, "Host not found error"},
{1032, "Internal error"},
{2310, "Internal security error"},
{2566, "Internal security error"},
{1286, "The encryption method specified is not valid"},
{2052, "Bad IP address specified"},
{1542, "Server security data is not valid"},
{1030, "Security data is not valid"},
{776, "The IP address specified is not valid"},
{2056, "License negotiation failed"},
{2312, "Licensing time-out"},
{1, "Local disconnection. This is not an error code."},
{0, "No information is available"},
{262, "Out of memory"},
{518, "Out of memory"},
{774, "Out of memory"},
{2, "Remote disconnection by user.This is not an error code."},
{1798, "Failed to unpack server certificate"},
{516, "Windows Sockets connect failed"},
{1028, "Windows Sockets recv call failed"},
{1796, "Time-out occurred"},
{1544, "Internal timer error"},
{772, "Windows Sockets send call failed"},
{2823, "The account is disabled"},
{3591, "The account is expired"},
{3335, "The account is locked out"},
{3079, "The account is restricted"},
{6919, "The received certificate is expired"},
{5639, "The policy does not support delegation of credentials to the target server"},
{8455, "The server authentication policy does not allow connection requests using saved credentials. The user must enter new credentials"},
{2055, "Login failed"},
{6151, "No authority could be contacted for authentication. The domain name of the authenticating party could be wrong, the domain could be unreachable, or there might have been a trust relationship failure"},
{2567, "The specified user has no account"},
{3847, "The password is expired"},
{4615, "The user password must be changed before logging on for the first time"},
{5895, "Delegation of credentials to the target server is not allowed unless mutual authentication has been achieved"},
{8711, "The smart card is blocked"},
{7175, "An incorrect PIN was presented to the smart card"}
				};
			}
			return disconnectReason;
		}
	
	}
}
