using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RAKandACOVirtualStation_OCC.Common;
using RAKandACOVirtualStation_OCC.VBoxWStation;
using SOAPService;

namespace RAKandACOVirtualStation_OCC.VBoxWStation
{
	public class IFsObjInfo : VBoxInterface
	{
		public IFsObjInfo(string _this, vboxService access)
		:base(_this,access)
		{
		}

		public string getName()
		{
			try
			{
				return this.access.IFsObjInfo_getName(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public FsObjType getObjType()
		{
			try
			{
				return this.access.IFsObjInfo_getType(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public long getHardLinks()
		{
			try
			{
				return this.access.IFsObjInfo_getHardLinks(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public long getNodeID()
		{
			try
			{
				return this.access.IFsObjInfo_getNodeId(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public long getGenerationID()
		{
			try
			{
				return this.access.IFsObjInfo_getGenerationId(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public uint getGID()
		{
			try
			{
				return this.access.IFsObjInfo_getGID(this._this);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
