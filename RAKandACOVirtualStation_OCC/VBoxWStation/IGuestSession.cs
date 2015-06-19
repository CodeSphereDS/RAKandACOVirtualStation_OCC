using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RAKandACOVirtualStation_OCC.Common;
using RAKandACOVirtualStation_OCC.VBoxWStation;
using SOAPService;

namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class IGuestSession:VBoxInterface
	{
		public IGuestSession(string _this, vboxService access)
			:base(_this,access){}	

		public void close()
		{
			try
			{
				this.access.IGuestSession_close(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public IProgress copyFrom(string source, string dest, CopyFileFlag[] copyArgs)
		{
			try
			{
				string value = this.access.IGuestSession_copyFrom(this._this, source, dest, copyArgs);
				IProgress p = new IProgress(value, this.access);
				return p;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public IProgress copyTo(string source, string dest, CopyFileFlag[] copyArgs)
		{
			try
			{
				string value = this.access.IGuestSession_copyTo(this._this, source, dest, copyArgs);
				return new IProgress(value, this.access);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public IGuestFsObjInfo directoryQueryInfo(string path)
		{
			try
			{
				string value = this.access.IGuestSession_directoryQueryInfo(this._this, path);
				return new IGuestFsObjInfo(value, this.access);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

	
		public void directoryCreate(string path, uint mode, DirectoryCreateFlag[] createFlags)
		{
			try
			{
				this.access.IGuestSession_directoryCreate(this._this, path, mode, createFlags);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public bool directoryExists(string path)
		{
			try
			{				
				return this.access.IGuestSession_directoryExists(this._this, path);			
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public IGuestDirectory directoryOpen(string path,string filter,DirectoryOpenFlag[] directoryOpenFlag)
		{
			try
			{
				string value = this.access.IGuestSession_directoryOpen(this._this, path, filter, directoryOpenFlag);
				System.Diagnostics.Debug.WriteLine("directoryopen return value: " + value);
				return new IGuestDirectory(value, this.access);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public bool fileExists(string path)
		{
			try
			{
				return this.access.IGuestSession_fileExists(this._this, path);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		//enum GuestSessionWaitForFlag { GuestSessionWaitForFlag_None = 0, GuestSessionWaitForFlag_Start = 1, GuestSessionWaitForFlag_Terminate = 2, GuestSessionWaitForFlag_Status = 4 }

		public GuestSessionWaitResult guestSessionWaitFor(GuestSessionWaitForFlag waitFlag, uint timeout)
		{
			uint flag=0;
			switch (waitFlag)
			{
				case GuestSessionWaitForFlag.None: 
					flag = 0;
					break;
				case GuestSessionWaitForFlag.Start:
					flag = 1;
					break;
				case GuestSessionWaitForFlag.Status:
					flag = 2;
					break;
				case GuestSessionWaitForFlag.Terminate:
					flag = 3;
					break;
				default:
					break;
			}		

			return this.access.IGuestSession_waitFor(this._this, flag, timeout);
		}
		
		public GuestSessionStatus getGuestSessionStatus()
		{
			try
			{
				return this.access.IGuestSession_getStatus(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		
		}

	

	
	}
}
