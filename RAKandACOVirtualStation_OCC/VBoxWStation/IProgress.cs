using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RAKandACOVirtualStation_OCC.Common;
using SOAPService;
using System.Web.Services.Protocols;

namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class IProgress : VBoxInterface
	{
		public IProgress(string _this, vboxService access)
			: base(_this, access)
		{
		}

		public void cancel()
		{
			try
			{
				this.access.IProgress_cancel(this._this);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public void waitForCompletion(int timeout)
		{
			try
			{
				this.access.IProgress_waitForCompletion(this._this, timeout);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public string getID()
		{
			try
			{
				return this.access.IProgress_getId(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public string description()
		{
			try
			{
				return this.access.IProgress_getDescription(this._this);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public bool completed()
		{
			try
			{
				return this.access.IProgress_getCompleted(this._this);
				
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public bool canceled()
		{
			try
			{
				return this.access.IProgress_getCanceled(this._this);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public bool cancelable()
		{
			try
			{
				return this.access.IProgress_getCancelable(this._this);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public long resultCode()
		{
			try
			{
				return this.access.IProgress_getResultCode(this._this);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
		public long getTimeout()
		{
			try
			{
				return this.access.IProgress_getTimeout(this._this);
			}
			catch (SoapException ex)
			{
				throw ex;			
			}
		}
	}
}
