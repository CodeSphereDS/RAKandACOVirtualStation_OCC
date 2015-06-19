using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using SOAPService;
using RAKandACOVirtualStation_OCC.Common;


namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class IVRDEServer : VBoxInterface
	{
		public IVRDEServer(string _this, vboxService access)
			: base(_this, access)
		{
		}

		public bool enabled
		{
			set
			{
				try
				{
					this.access.IVRDEServer_setEnabled(this._this, value); //.IVRDPServer_setEnabled(this._this, value);
				}
				catch (SoapException ex)
				{
					throw ex;
				}
			}

			get
			{
				try
				{
					return this.access.IVRDEServer_getEnabled(this._this);
				}
				catch (SoapException ex)
				{
					throw ex;
				}
			}
		}

		public bool allowMultiConnection
		{
			set
			{
				try
				{
					this.access.IVRDEServer_setAllowMultiConnection(this._this, value);
				}
				catch (SoapException ex)
				{
					throw ex;
				}
			}

			get
			{
				try
				{
					return this.access.IVRDEServer_getAllowMultiConnection(this._this);
				}
				catch (SoapException ex)
				{
					throw ex;
				}
			}
		}

		public SOAPService.AuthType authType
		{
			set
			{
				try
				{
					this.access.IVRDEServer_setAuthType(this._this, value);
				}
				catch (SoapException ex)
				{
					throw ex;
				}
			}

			get
			{
				try
				{
					return this.access.IVRDEServer_getAuthType(this._this);
				}
				catch (SoapException ex)
				{
					throw ex;
				}
			}
		}

		public uint authTimeout
		{
			set
			{
				try
				{
					this.access.IVRDEServer_setAuthTimeout(this._this, value);
				}
				catch (SoapException ex)
				{
					throw ex;
				}
			}

			get
			{
				try
				{
					return this.access.IVRDEServer_getAuthTimeout(this._this);
				}
				catch (SoapException ex)
				{
					throw ex;
				}
			}

		}

		public List<string> VRDEProperties()
		{
			try
			{
				return this.access.IVRDEServer_getVRDEProperties(this._this).ToList<string>();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		
	}
}
