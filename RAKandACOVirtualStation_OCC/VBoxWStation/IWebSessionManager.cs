using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RAKandACOVirtualStation_OCC.Common;
using SOAPService;
using System.Web.Services.Protocols;

namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class IWebSessionManager : VBoxInterface
	{
		public IWebSessionManager(string _this, vboxService access)
			: base(_this, access) { }

		public IVirtualBox logon(Uri uri, string username, string password)
		{
			string value = this.access.IWebsessionManager_logon(username, password);
			return new IVirtualBox(value, this.access);
		}
		public ISession getSessionObject()
		{
			try
			{
				string value = this.access.IWebsessionManager_getSessionObject(this._this); 
				return new ISession(value, this.access);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public void logOff()
		{
			try
			{
				this.access.IWebsessionManager_logoff(this._this);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
	}
}
