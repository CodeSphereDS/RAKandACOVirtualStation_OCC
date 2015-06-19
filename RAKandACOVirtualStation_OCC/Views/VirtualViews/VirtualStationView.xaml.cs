using RAKandACOVirtualStation_OCC.ViewModels.VirtualViewModels;
using Catel.Windows.Controls;
using RAKandACOVirtualStation_OCC.RDP;
using System;
using System.Diagnostics;
using System.Threading;
namespace RAKandACOVirtualStation_OCC.Views.VirtualViews
{
	/// <summary>
	/// Interaction logic for VirtualStationView.xaml.
	/// </summary>
	public partial class VirtualStationView : UserControl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VirtualStationView"/> class.
		/// </summary>
		public VirtualStationView()		
		{
			InitializeComponent();
			if (wfh == null)
			{
				wfh = new System.Windows.Forms.Integration.WindowsFormsHost();
			}
					
		}
		protected override System.Type GetViewModelType()
		{
			return typeof(VirtualStationViewModel);
		}		
	}
}
