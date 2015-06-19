using System;
using RAKandACOVirtualStation_OCC.Common;
using System.Web.Services.Protocols;
using SOAPService;
using System.Collections.Generic;

namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class IGuest:VBoxInterface
	{
		public IGuest(string _this, vboxService access)
			: base(_this, access)
		{
		}
		public void setCredentials(string username, string password, string domain, bool allowInteractiveLogon)
		{
			try
			{
				this.access.IGuest_setCredentials(this._this, username, password, domain, allowInteractiveLogon);
			}
			catch(Exception ex)
			{
				throw ex;

			}
		}
		public IGuestSession createSession(string username,string password,string domain,string sessionName)
		{
			try
			{
				
				string value = this.access.IGuest_createSession(_this, username, password, domain, sessionName);
				return new IGuestSession(value, this.access);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
		public AdditionsRunLevelType getAdditionsRunLevel()
		{
			try
			{
				return this.access.IGuest_getAdditionsRunLevel(this._this);
			}
			catch (Exception ex)
			{
				
				throw ex;
			}
		}
		public List<IGuestSession> getGuestSessions()
		{
			try
			{
				List<IGuestSession> guestSessionList = new List<IGuestSession>();
				string[] guestSession = this.access.IGuest_getSessions(this._this);
				foreach (string g in guestSession)
				{
					guestSessionList.Add(new IGuestSession(g, access));
				}
				return guestSessionList;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public string getAdditionsVersion()
		{
			try
			{
				return this.access.IGuest_getAdditionsVersion(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

	}
}
