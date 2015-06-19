using Catel.Data;
using Catel.MVVM;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Catel.IoC;
using Catel.MVVM.Services;
using System.Net;
using System.ComponentModel;
using System;
using RAKandACOVirtualStation_OCC.Views;
using RAKandACOVirtualStation_OCC.Repository;
using RAKandACOVirtualStation_OCC.ViewModels.VirtualViewModels;
using System.Windows.Threading;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
using System.Diagnostics;
using RAKandACOVirtualStation_OCC.RDP;
using System.Threading;
using RAKandACOVirtualStation_OCC.HelperClasses;
using AxMSTSCLib;
using RAKandACOVirtualStation_OCC.VBoxWStation;
using FirstFloor.ModernUI.Windows.Controls;

namespace RAKandACOVirtualStation_OCC.ViewModels
{
	/// <summary>
	/// UserControl view model.
	/// </summary>
	public class VirtualViewModel : ViewModelBase
	{
		#region variables
		private IPleaseWaitService pleasewaitservice { get; set; }
		private IMessageService messageservice { get; set; }
		IMachine SelectedMachine { get; set; }

		#endregion variables

		/// <summary>
		/// Initializes a new instance of the <see cref="VirtualViewModel"/> class.
		/// </summary>
		public VirtualViewModel()
		{
			LoadVirtualStations = new Command(OnLoadVirtualStationsExecute, OnLoadVirtualStationsCanExecute);
			SelectVirtualStationChanged = new Command<object>(OnSelectVirtualStationChangedExecute);
			pleasewaitservice = Catel.IoC.DependencyResolverExtensions.Resolve<IPleaseWaitService>(this.DependencyResolver);
			messageservice = Catel.IoC.DependencyResolverExtensions.Resolve<IMessageService>(this.DependencyResolver);
			VMListBoxPreviewMouseLeftButtonDown = new Command<object>(OnVMListBoxPreviewMouseLeftButtonDownExecute);
			ListBoxKeyNav = new Command<object>(OnListBoxKeyNavExecute);
			
			//Optimizations
			

		}

		/// <summary>
		/// Gets the title of the view model.
		/// </summary>
		/// <value>The title.</value>
		public override string Title { get { return "View model title"; } }

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public string ProgressDetails
		{
			get { return GetValue<string>(ProgressDetailsProperty); }
			set { SetValue(ProgressDetailsProperty, value); }
		}

		/// <summary>
		/// Register the ProgressDetails property so it is known in the class.
		/// </summary>
		public static readonly PropertyData ProgressDetailsProperty = RegisterProperty("ProgressDetails", typeof(string), null);
		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public string VirtualErrorMessage
		{
			get { return GetValue<string>(VirtualErrorMessageProperty); }
			set { SetValue(VirtualErrorMessageProperty, value); }
		}

		/// <summary>
		/// Register the VirtualErrorMessage property so it is known in the class.
		/// </summary>
		public static readonly PropertyData VirtualErrorMessageProperty = RegisterProperty("VirtualErrorMessage", typeof(string), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public VirtualStationViewModel _VirtualStationViewModel
		{
			get { return GetValue<VirtualStationViewModel>(_VirtualStationViewModelProperty); }
			set { SetValue(_VirtualStationViewModelProperty, value); }
		}
		/// <summary>
		/// Register the _VirtualStationViewModel property so it is known in the class.
		/// </summary>
		public static readonly PropertyData _VirtualStationViewModelProperty = RegisterProperty("_VirtualStationViewModel", typeof(VirtualStationViewModel), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public bool VirtualStationCollectionIsLoading
		{
			get { return GetValue<bool>(VirtualStationCollectionIsLoadingProperty); }
			set { SetValue(VirtualStationCollectionIsLoadingProperty, value); }
		}
		/// <summary>
		/// Register the VirtualStationCollectionIsLoading property so it is known in the class.
		/// </summary>
		public static readonly PropertyData VirtualStationCollectionIsLoadingProperty = RegisterProperty("VirtualStationCollectionIsLoading", typeof(bool), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public ObservableCollection<IMachine> VirtualStationCollection
		{
			get { return GetValue<ObservableCollection<IMachine>>(VirtualStationCollectionProperty); }
			set { SetValue(VirtualStationCollectionProperty, value); }
		}

		/// <summary>
		/// Register the VirtualStationCollection property so it is known in the class.
		/// </summary>
		public static readonly PropertyData VirtualStationCollectionProperty = RegisterProperty("VirtualStationCollection", typeof(ObservableCollection<IMachine>), null, (sender, e) => ((VirtualViewModel)sender).OnVirtualStationCollectionChanged());

		/// <summary>
		/// Called when the VirtualStationCollection property has changed.
		/// </summary>
		private void OnVirtualStationCollectionChanged()
		{
			if (VirtualStationCollection != null && VirtualStationCollection.Count > 0)
			{
				VirtualStationCollectionIsLoading = false;
			}
		}

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public IMachine VirtualStation
		{
			get { return GetValue<IMachine>(VirtualStationProperty); }
			set { SetValue(VirtualStationProperty, value); }
		}

		/// <summary>
		/// Register the VirtualStation property so it is known in the class.
		/// </summary>
		public static readonly PropertyData VirtualStationProperty = RegisterProperty("VirtualStation", typeof(IMachine), null, (sender, e) => ((VirtualViewModel)sender).OnVirtualStationChanged(sender, e));

		/// <summary>
		/// Called when the VirtualStation property has changed.
		/// </summary>
		private void OnVirtualStationChanged(object sender, AdvancedPropertyChangedEventArgs e)
		{
		}

		protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
		}

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public ViewModelBase CurrentRDesktopContent
		{
			get { return GetValue<ViewModelBase>(CurrentRDesktopContentProperty); }
			set { SetValue(CurrentRDesktopContentProperty, value); }
		}

		/// <summary>
		/// Register the CurrentRDesktopContent property so it is known in the class.
		/// </summary>
		public static readonly PropertyData CurrentRDesktopContentProperty = RegisterProperty("CurrentRDesktopContent", typeof(ViewModelBase), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public bool VirtualStationRDPIsConnected
		{
			get
			{
				if (_VirtualStationViewModel == null || _VirtualStationViewModel.rdpx == null)
				{
					return !false;
				}
				else
				{
					return !Convert.ToBoolean(_VirtualStationViewModel.rdpx.Connected);
				}
			}
		}

		#region Commands

		/// <summary>
		/// Gets the LoadVirtualStations command.
		/// </summary>
		public Command LoadVirtualStations { get; private set; }

		/// <summary>
		/// Method to check whether the LoadVirtualStations command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool OnLoadVirtualStationsCanExecute()
		{
			return true;
		}

		/// <summary>
		/// Method to invoke when the LoadVirtualStations command is executed.
		/// </summary>
		private void OnLoadVirtualStationsExecute()
		{
			if (VirtualBox.IsConnectedVBox)
			{
				if (VirtualStationCollection != null)
				{
					Debug.WriteLine("prior clear - VirtualBox.getVirtualStations() count: " + VirtualBox.getVirtualStations().Count);
					VirtualStationCollection.Clear();
					Debug.WriteLine("prior clear - VirtualBox.getVirtualStations() count: " + VirtualBox.getVirtualStations().Count);
				}
			}
			if (VirtualStationCollection == null || VirtualStationCollection.Count<=0)
			{
				VirtualErrorMessage = "";
				VirtualStationCollectionIsLoading = true;
				BackgroundWorker worker = new BackgroundWorker();

				worker.DoWork += delegate(object s, DoWorkEventArgs args)
				{
					try
					{
						ProgressDetails = "Searching for VirtualStations...";
						VirtualStationCollection = VirtualBox.getVirtualStations();
						VirtualStationCollectionIsLoading = false;
					}
					catch (Exception ex)
					{
						//System.Diagnostics.Debug.WriteLine("Unable to logon");
						VirtualErrorMessage = "Unable to connect to VirtualStations Server. " +
						 "Please ensure that the VirtualStation Server PC is powered-on before attempting to connect. " +
						 "If this problem persist, please contact your System Developer.";
						VirtualStationCollectionIsLoading = false;
					}
				};
				worker.RunWorkerAsync();
			}
		}

		/// <summary>
		/// Gets the OnSelectVirtualStation command.
		/// </summary>
		public Command<object> SelectVirtualStationChanged { get; private set; }

		/// <summary>
		/// Method to invoke when the OnSelectVirtualStation command is executed.
		/// </summary>
		private void OnSelectVirtualStationChangedExecute(object parameter)
		{
			#region handleSelectionChange
			//Debug.WriteLine("parameter type : " + parameter.GetType());

			System.Windows.Controls.SelectionChangedEventArgs e = parameter as System.Windows.Controls.SelectionChangedEventArgs;
			if (e.AddedItems[0] as IMachine == SelectedMachine)
			{
				SelectedMachine = null;
			}
			else if (e.AddedItems[0] as IMachine != SelectedMachine)
			{
				if (e.RemovedItems.Count == 0)
				{
					Debug.WriteLine("VirtualStation: " + VirtualStation.MachineName);
					pleasewaitservice.Show(() =>
						{
							_VirtualStationViewModel = new VirtualStationViewModel(VirtualStation);
							CurrentRDesktopContent = _VirtualStationViewModel;
						}, string.Format("Initializing {0} VirtualStation settings...", VirtualStation.MachineName));
				}

				else if (e.RemovedItems.Count > 0) //if previousItem exist
				{
					var prevSelectedItem = e.RemovedItems[0] as IMachine;

					if (prevSelectedItem.MachineState == SOAPService.MachineState.Saved || prevSelectedItem.MachineState == SOAPService.MachineState.PoweredOff)
					{
						pleasewaitservice.Show(() =>
						{
							_VirtualStationViewModel = new VirtualStationViewModel(VirtualStation);
							CurrentRDesktopContent = _VirtualStationViewModel;
						}, string.Format("Initializing {0} VirtualStation settings...", VirtualStation.MachineName));
					}
					else
					{
						if (ModernDialog.ShowMessage(string.Format("Switch to {0} VirtualStation?", VirtualStation.MachineName),
									"Confirm", System.Windows.MessageBoxButton.OKCancel) != System.Windows.MessageBoxResult.OK)
						{
							Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
								{
									VirtualStation = prevSelectedItem;
								}), null);
							SelectedMachine = prevSelectedItem;
							return;
						}
						else
						{
							Debug.WriteLine("VirtualStation: " + VirtualStation.MachineName);
							if (_VirtualStationViewModel != null)
							{
								if (_VirtualStationViewModel.rdpx != null && _VirtualStationViewModel.rdpx.Connected == 1)
								{
									pleasewaitservice.Show(string.Format("Disconnecting remote session from {0} VirtualStation", prevSelectedItem.MachineName));
									_VirtualStationViewModel.rdpx.Disconnect();
									//while (_VirtualStationViewModel.rdpx.Connected != 0)
									//{
									//    Thread.Sleep(500);
									//}
								}
							}
							pleasewaitservice.Show(string.Format("Checking if {0} VirtualStation has connected users...", prevSelectedItem.MachineName));
							if (prevSelectedItem.MachineState == SOAPService.MachineState.Running) //if State is running
							{
								if (!_VirtualStationViewModel.VirtualStationHasConnectedUsers())
								{
									pleasewaitservice.UpdateStatus(string.Format("Saving state of {0}...", prevSelectedItem.MachineName));
									_VirtualStationViewModel.SwitchSaveStateVirtualStation();
								}
							}
							//if state is starting
							_VirtualStationViewModel = new VirtualStationViewModel(VirtualStation);
							CurrentRDesktopContent = _VirtualStationViewModel;
							pleasewaitservice.Hide();
						}
					}
				}
			}
			#endregion handleSelectionChange
		}

		/// <summary>
		/// Gets the VMListBoxPreviewMouseLeftButtonDown command.
		/// </summary>
		public Command<object> VMListBoxPreviewMouseLeftButtonDown { get; private set; }

		/// <summary>
		/// Method to invoke when the VMListBoxPreviewMouseLeftButtonDown command is executed.
		/// </summary>
		private void OnVMListBoxPreviewMouseLeftButtonDownExecute(object parameter)
		{
			#region handlePreviewLeftMouseButtonDown
			Debug.WriteLine("PreviewMouseLeftButtonDown clicked!");

			System.Windows.Input.MouseButtonEventArgs e = parameter as System.Windows.Input.MouseButtonEventArgs;
			//    if(e.Source!=null)
			//    {
			//        Debug.WriteLine(e.Source);
			//    }

			if (parameter != null)
			{
				Debug.WriteLine(parameter.GetType());
				//if (ModernDialog.ShowMessage(string.Format("Disconnect from {0} VirtualStation session?", prevMachine.MachineName),
				//    "Confirm", System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.OK)
				//{
				//    Debug.WriteLine("Machine Changed!");
				//}	
			}




			#endregion handleSelectionChange
		}

		/// <summary>
		/// Gets the ListBoxKeyNav command.
		/// </summary>
		public Command<object> ListBoxKeyNav { get; private set; }

		/// <summary>
		/// Method to invoke when the ListBoxKeyNav command is executed.
		/// </summary>
		private void OnListBoxKeyNavExecute(object parameter)
		{
			if (parameter != null)
			{
				//	Debug.WriteLine("keyboard nav param type: " + parameter.GetType());
				System.Windows.Input.KeyEventArgs e = parameter as System.Windows.Input.KeyEventArgs;
				e.Handled = true;
			}

		}

		#endregion Commands
		
		#region Methods


		#endregion Methods
	}
}
