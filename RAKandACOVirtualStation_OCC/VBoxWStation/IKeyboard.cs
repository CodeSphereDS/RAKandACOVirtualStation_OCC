using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RAKandACOVirtualStation_OCC.Common;
using SOAPService;
using System.Web.Services.Protocols;

namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class IKeyboard : VBoxInterface
	{
		public IKeyboard(string _this, vboxService access)
			: base(_this, access)
		{
		}

		public void putScancode(int scancode)
		{
			try
			{
				this.access.IKeyboard_putScancode(this._this, scancode);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public long putScancodes(List<int> scancodes)
		{
			try
			{
				long value = this.access.IKeyboard_putScancodes(this._this, scancodes.ToArray());
				return value;
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}

		public void putCAD()
		{
			try
			{
				this.access.IKeyboard_putCAD(this._this);
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
	}
}
