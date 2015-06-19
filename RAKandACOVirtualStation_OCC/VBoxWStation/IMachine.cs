using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RAKandACOVirtualStation_OCC.Common;
using SOAPService;
using System.Web.Services.Protocols;

namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class IMachine : VBoxInterface
	{
		private string _machineName;
		public IMachine(string _this, vboxService access)
			: base(_this, access)
		{
			MachineName = this.getMachineName();
		}
		
	
		public string getMachineName()
		{
			try
			{
				return access.IMachine_getName(this._this);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public Guid getId()
		{
			try
			{
				string value = access.IMachine_getId(this._this);
				return new Guid(value);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public MachineState getState()
		{
			try
			{
				return this.access.IMachine_getState(this._this);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
		public string sessionType()
		{
			try
			{
				return this.access.IMachine_getSessionType(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}
		public SessionState getSessionState()
		{
			try
			{
				return this.access.IMachine_getSessionState(this._this);
			}
			catch (Exception ex)
			{				
				throw ex;
			}
		}
		public IVRDEServer getVRDEServer()
		{
			try
			{
				string value = this.access.IMachine_getVRDEServer(this._this);
				return new IVRDEServer(value, this.access);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public string MachineName
		{
			get
			{
				if(string.IsNullOrEmpty(_machineName))
				{
					_machineName = getMachineName();
				}
				return _machineName;
			}
			private set
			{
				if (value != null)
				{
					_machineName = value;
				}
			}
		}

		public MachineState MachineState
		{
			get
			{
				System.Diagnostics.Debug.WriteLine(DateTime.Now + " Getting MachineState..." );
				return getState();								
			}
		}

		//IMachine_launchVMProcess(_this, session, type, environment);
		public IProgress LaunchVM(ISession session, string type, string environment)
		{
			try
			{
				string value = access.IMachine_launchVMProcess(this._this,session.getReference(), type, "");
				return new IProgress(value, this.access);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public void lockMachine(ISession session, LockType locktype)
		{
			try
			{
				//System.Diagnostics.Debug.WriteLine("imachine.this : " + this._this);
				//System.Diagnostics.Debug.WriteLine("session.getReference(): " + session.getReference()); 
				this.access.IMachine_lockMachine(this._this, session.getReference(), locktype);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public IVirtualBox getParent()
		{
			try
			{
				string value = this.access.IMachine_getParent(this._this);
				return new IVirtualBox(value, this.access);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		//public IVRDEServer getVRDEServer()
		//{
		//    try
		//    {
		//        string value = this.access.IMachine_getVRDEServer(this._this)
		//        return new IVRDEServer(value,this.access);
		//    }
		//    catch (Exception ex)
		//    {
		//        throw ex;
		//    }
		//}
		public string getOSDescription()
		{
			try
			{
				return this.access.IMachine_getOSTypeId(this._this);							
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
				

		public long getMemorySize()
		{
			try
			{
				return this.access.IMachine_getMemorySize(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}

}
