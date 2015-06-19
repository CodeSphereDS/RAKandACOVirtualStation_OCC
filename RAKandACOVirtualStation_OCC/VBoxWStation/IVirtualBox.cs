using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RAKandACOVirtualStation_OCC.Common;
using SOAPService;
using System.Web.Services.Protocols;


namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class IVirtualBox : VBoxInterface
	{
		public IVirtualBox(string _this, vboxService acess)
			: base(_this, acess)
		{			
		}

		public string getVersion()
		{
			try
			{
				return this.access.IVirtualBox_getVersion(this._this);
				
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public IMachine findMachine(string nameOrID)
		{
			try
			{
				string value = this.access.IVirtualBox_findMachine(this._this, nameOrID);
				return new IMachine(value, this.access);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public List<IMachine> getMachines()
		{
			try
			{
				List<IMachine> iMachines = new List<IMachine>();
				string[] machines = this.access.IVirtualBox_getMachines(this._this);
				foreach (string machine in machines)
				{
					iMachines.Add(new IMachine(machine, this.access));
				}

				return iMachines;
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public void setExtraData(string key,string value)
		{
			try
			{
				this.access.IVirtualBox_setExtraData(this._this, key, value);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public string getExtraData(string key)
		{
			try
			{
				return this.access.IVirtualBox_getExtraData(this._this, key);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<IProgress> progressOperations()
		{
			try
			{
				List<IProgress> progress=new List<IProgress>();
				string[] values = this.access.IVirtualBox_getProgressOperations(this._this);
				foreach (string pValue in values)
				{
					IProgress p = new IProgress(pValue, this.access);
					progress.Add(p);
				}
				return progress;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public string getAPIVersion()
		{
			try
			{
				return this.access.IVirtualBox_getAPIVersion(this._this);
			}
			catch (Exception ex)
			{				
				throw ex;
			}
		}
		
	}
}
