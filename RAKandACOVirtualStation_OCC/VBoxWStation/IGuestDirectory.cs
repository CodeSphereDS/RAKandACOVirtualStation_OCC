using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RAKandACOVirtualStation_OCC.VBoxWStation;
using RAKandACOVirtualStation_OCC.Common;
using RAKandACOVirtualStation_OCC.VBoxWStation;
using SOAPService;

namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class IGuestDirectory : IDirectory
	{
		public IGuestDirectory(string _this, vboxService access)
			:base(_this,access)
		{		
		}
	
	}
}
