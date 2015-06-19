using Catel.Windows;
using Catel.Data;
using Catel.MVVM.Properties;
using Catel.MVVM;
using System;
using FirstFloor.ModernUI.Windows.Controls;
using RAKandACOVirtualStation_OCC.ViewModels;

namespace RAKandACOVirtualStation_OCC.Views
{
	/// <summary>
	/// Interaction logic for InfoDialog.xaml
	/// </summary>
	public partial class InfoDialog : ModernDialog
	{		
		
		public InfoDialog(InfoDialogViewModel infoviewModel)
		{
			InitializeComponent();
			DataContext = infoviewModel;
			
		}
	}
}
