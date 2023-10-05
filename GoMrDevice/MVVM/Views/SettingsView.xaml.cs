﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
 using GoMrDevice.MVVM.ViewModels;

 namespace GoMrDevice.MVVM.Views
{
	/// <summary>
	/// Interaction logic for SettingsViews.xaml
	/// </summary>
	public partial class SettingsView : UserControl
	{
		public SettingsView()
		{
			InitializeComponent();
			var deviceListViewModel = new DeviceListViewModel();

			// Set the DataContext of your DeviceListControl
			DeviceListControl.DataContext = deviceListViewModel;

			// Fetch and set the data
			Task.WhenAll(deviceListViewModel.LoadDeviceTwinDataAsync());
		}
	}
}
