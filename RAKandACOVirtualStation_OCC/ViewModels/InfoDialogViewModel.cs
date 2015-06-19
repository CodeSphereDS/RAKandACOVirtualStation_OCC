using System;
using Catel.MVVM;
using Catel.Data;

namespace RAKandACOVirtualStation_OCC.ViewModels 
{
	public class InfoDialogViewModel : ViewModelBase	
	{
		public InfoDialogViewModel()			
		{

		}

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public string InfoDisplay
		{
			get { return GetValue<string>(InfoDisplayProperty); }
			set { SetValue(InfoDisplayProperty, value); }
		}

		/// <summary>
		/// Register the InfoDisplay property so it is known in the class.
		/// </summary>
		public static readonly PropertyData InfoDisplayProperty = RegisterProperty("InfoDisplay", typeof(string), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>

		public string NewTitle { get; set; } 
		public override string Title { get{return NewTitle;}}
	
	}



}
