using System;
using RAKandACOVirtualStation_OCC.Common;
using SOAPService;
using System.Web.Services.Protocols;


namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class IConsole : VBoxInterface
	{
		public IConsole(string _this, vboxService access)
			: base(_this, access)
		{
		}
		public void pause()
		{
			try
			{
				this.access.IConsole_pause(this._this);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
		public void resume()
		{
			try
			{
				this.access.IConsole_resume(this._this);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
		public IProgress  powerDown()
		{
			try
			{
				string value = this.access.IConsole_powerDown(this._this);
				return new IProgress(value, this.access);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
		public void powerButton()
		{
			try
			{
				this.access.IConsole_powerButton(this._this);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
		public IProgress saveState()
		{
			try
			{
				string value = this.access.IConsole_saveState(this._this);
				return new IProgress(value, access);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
		public IProgress powerUp()
		{
			try
			{
				string value = this.access.IConsole_powerUp(this._this);
				return new IProgress(value, this.access);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
		public IKeyboard getKeyboard()
		{
			try
			{
				string value = this.access.IConsole_getKeyboard(this._this);
				return new IKeyboard(value, this.access);
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
				MachineState state = this.access.IConsole_getState(this._this);
				return state;
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
		public bool getGuestEnteredACPIMode()
		{
			try
			{
				return this.access.IConsole_getGuestEnteredACPIMode(_this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public IVRDEServerInfo getVRDEServerInfo()
		{
			try
			{
				return this.access.IConsole_getVRDEServerInfo(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public void setShareFolder()
		{
			try
			{
				this.access.IConsole_createSharedFolder(this._this, "test share", "D:\\Installers", true, true);

			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
		public IGuest getGuest()
		{
			try
			{
				string value = this.access.IConsole_getGuest(_this);
				return new IGuest(value, this.access);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
	}
}
