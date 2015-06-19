using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RAKandACOVirtualStation_OCC.Common;
using SOAPService;

namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class IEventSource :VBoxInterface
		
	{
		public IEventSource(string _this, vboxService access)
			:base(_this,access)			
		{
		
		}

		public void CreateAggregator(System.Array aSubordinates)
		{

		}
	

	}
}
