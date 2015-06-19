using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RAKandACOVirtualStation_OCC.Common;
using SOAPService;

namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class IDirectory : VBoxInterface
	{
		public IDirectory(string _this, vboxService access)
			: base(_this, access)
		{
		}
		public void close()
		{
			try
			{
				this.access.IDirectory_close(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public string getDirectoryName()
		{
			try
			{
				return this.access.IDirectory_getDirectoryName(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public IFsObjInfo read()
		{
			try
			{
				string value = this.access.IDirectory_read(this._this);
				//System.Diagnostics.Debug.WriteLine("IDirectory_read value: " + value);
				return new IFsObjInfo(value, this.access);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public string getFilter()
		{
			try
			{
				return this.access.IDirectory_getFilter(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		

	}
}
