using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SOAPService;
using RAKandACOVirtualStation_OCC.Common;
using System.Web.Services.Protocols;

namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class ISession : VBoxInterface
	{
		public ISession(string _this, vboxService access)
			: base(_this, access)
		{
		}

		public SessionState sessionState()
		{
			try
			{
				return this.access.ISession_getState(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public SessionType SessionType()
		{
			try
			{
				return this.access.ISession_getType(this._this);
			}
			catch (Exception ex)
			{				
				throw ex;
			}
		}

		public void unlockMachine()
		{
			try
			{
				this.access.ISession_unlockMachine(this._this);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
		public IConsole getConsole()
		{
			try
			{
				string value = this.access.ISession_getConsole(this._this);
				
				return new IConsole(value, this.access);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public IMachine getMachine()
		{
			try
			{				
				string value = this.access.ISession_getMachine(this._this);
				return new IMachine(value, this.access);
			}
			catch (SoapException ex)
			{
				//if session is not locked for a particular imachine
				throw ex;
			}
		}

	}
}
