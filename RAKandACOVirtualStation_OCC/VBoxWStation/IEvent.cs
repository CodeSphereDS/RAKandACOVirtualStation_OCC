using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RAKandACOVirtualStation_OCC.Common;
using RAKandACOVirtualStation_OCC.VBoxWStation;
using SOAPService;

namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class IEvent : VBoxInterface
	{
		public IEvent(string _this, vboxService access)
			: base(_this, access)
		{}

		public VBoxEventType type 
		{ 
			get
			{
				return this.access.IEvent_getType(_this);
			}
		}

		public IEventSource source 
		{
			get
			{
				string value = this.access.IEvent_getSource(_this);
				return new IEventSource(_this, access);
			}
		}

		bool _waitable;
		public bool waitable
		{
			get
			{
				return _waitable;
			}
		}


		public void setProcessed()
		{

		}

		public void waitProcessed(int timeout, bool result)
		{

		}
	}
}
