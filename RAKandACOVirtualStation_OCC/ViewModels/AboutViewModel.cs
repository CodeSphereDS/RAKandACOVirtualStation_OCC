using System;
using System.Collections.Generic;
using Catel.Data;
using Catel.MVVM;
namespace RAKandACOVirtualStation_OCC.ViewModels
{
	

	/// <summary>
	/// UserControl view model.
	/// </summary>
	public class AboutViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AboutViewModel"/> class.
		/// </summary>
		public AboutViewModel()
		{
			ShowChanges();
		}

		/// <summary>
		/// Gets the title of the view model.
		/// </summary>
		/// <value>The title.</value>
		public override string Title { get { return "View model title"; } }

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public Dictionary<DateTime,string> ChangeLog
		{
			get { return GetValue<Dictionary<DateTime,string>>(ChangeLogProperty); }
			set { SetValue(ChangeLogProperty, value); }
		}

		/// <summary>
		/// Register the ChangeLog property so it is known in the class.
		/// </summary>
		public static readonly PropertyData ChangeLogProperty = RegisterProperty("ChangeLog", typeof(Dictionary<DateTime,string>), null);

		void ShowChanges()
		{
			if (ChangeLog == null)
			{
				ChangeLog = new Dictionary<DateTime, string>();
				ChangeLog.Add(new DateTime(2013,6,6),"Released RAK&ACO VirtualStation Client V1.0");
				ChangeLog.Add(new DateTime(2013, 6, 11), "RAK&ACO VirtualStation Client V1.1 - Added One-click Backup");
				ChangeLog.Add(new DateTime(2013, 6, 11), "RAK&ACO VirtualStation Client V1.1 - Added One-click Backup"); 
			}
		}
	}
}
