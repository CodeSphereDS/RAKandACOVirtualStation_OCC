using System;
using SOAPService;

namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class VBoxWStation
	{
		IMachine _machine;
		IWebSessionManager _webMngr;
		ISession _session;
		IVirtualBox _ivirtualBox ;
		IConsole _iconsole ;
		IProgress _progress ;
		IVRDEServer _ivrdeServer ;
		IVRDEServerInfo _ivrdeServerInfo ;

		public VBoxWStation(IMachine machine)
		{
			if(machine==null)
			{
				throw new ArgumentNullException("IMachine cannot be null");
			}
			else
			{
				_machine = machine;
			}
		}
		public IVirtualBox getIVirtualBox()
		{
			if (_ivirtualBox == null)
			{
				_ivirtualBox = _machine.getParent();
			}
			return _ivirtualBox;
		}
		public IWebSessionManager getWebMngr()
		{
			if (_webMngr == null)
			{
				_webMngr = new IWebSessionManager(this._ivirtualBox.getReference(), this._ivirtualBox.getWsAcces());
			}
			return _webMngr;
		}

		public ISession getISession()
		{
			if (_session == null)
			{
				_session = _webMngr.getSessionObject();
			}
			return _session;
		}
	}
}
