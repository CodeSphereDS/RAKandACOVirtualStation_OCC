using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RAKandACOVirtualStation_OCC.VBoxWStation;

namespace RAKandACOVirtualStation_OCC.Models
{
	public class VirtualStation
	{
		public IMachine Machine{get;set;}
		public string MachineName { get; set; }
		public VirtualStation(IMachine machine)
		{
			Machine = machine;
			MachineName = machine.getName(); 
		}
	}
}
