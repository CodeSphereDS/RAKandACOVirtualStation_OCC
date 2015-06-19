using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SOAPService;

using System.Web.Services.Protocols;

namespace RAKandACOVirtualStation_OCC.Common
{
	public class VBoxInterface
	{
		protected vboxService access;
		protected string _this;

		public VBoxInterface(string _this, vboxService access)
		{
			this._this = _this;
			this.access = access;
		}

		public vboxService getWsAcces()
		{
			return access;
		}

		public void releaseRemote()
		{
			try
			{
				access.IManagedObjectRef_release(this._this);
				this._this = null;
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public string getReference()
		{
			try
			{
				return this._this;
			}
			catch (Exception ex)
			{				
				throw ex;
			}			
		}
		
	}
}
