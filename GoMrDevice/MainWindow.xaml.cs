using GoMrDevice.Models;
using GoMrDevice.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using GoMrDevice.Services;
using GoMrDevice.MVVM.Core;
using System.Windows.Media.Animation;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Microsoft.Graph.Models.ExternalConnectors;


namespace GoMrDevice
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly IoTHubManager _iotHub;
		public ObservableCollection<Twin> DeviceTwinList { get; set; } = new ObservableCollection<Twin>();

		private readonly DeviceManager _deviceManager;
		


		public MainWindow(DeviceManager deviceManager, NavigationStore navigationStore, IoTHubManager iotHub)
		{
			InitializeComponent();

			_deviceManager = deviceManager;
			_iotHub = iotHub;

			DeviceListView.ItemsSource = DeviceTwinList;

			DataContext = navigationStore;

			Task.WhenAll(ToggleFanStateAsync(), GetDevicesTwinAsync());
		}


		private async Task ToggleFanStateAsync()
{
    Storyboard fan = (Storyboard)this.FindResource("FanStoryboard");

    while (true)
    {
        bool areDevicesOn = _deviceManager.AllowSending();

        // Check if the status of IoT devices has changed
        if (areDevicesOn)
        {
            fan.Begin();
            SetMessage("The fan is ON.");
        }
        else
        {
            fan.Stop();
            SetMessage("The fan is OFF.");
        }

        await Task.Delay(1000);
    }
}

// Helper method to set the message in the UI
private void SetMessage(string message)
{
    // Ensure that this code runs on the UI thread
    Dispatcher.Invoke(() =>
    {
        MessageTextBlock.Text = message;
    });
}


private async void StartButton_Click(object sender, RoutedEventArgs e)
{
	try
	{
		Button? button = sender as Button;
		if (button != null)
		{
			Twin? twin = button.DataContext as Twin;
			if (twin != null)
			{
				string deviceId = twin.DeviceId;
				if (!string.IsNullOrEmpty(deviceId))
					await _iotHub.SendMethodAsync(new MethodDataRequest
					{
						DeviceId = deviceId,
						MethodName = "start"
					});
			}
		}
	}
	catch (Exception ex) { Debug.WriteLine(ex.Message); }
}

private async void StopButton_Click(object sender, RoutedEventArgs e)
{
	try
	{
		Button? button = sender as Button;
		if (button != null)
		{
			Twin? twin = button.DataContext as Twin;
			if (twin != null)
			{
				string deviceId = twin.DeviceId;
				if (!string.IsNullOrEmpty(deviceId))
					await _iotHub.SendMethodAsync(new MethodDataRequest
					{
						DeviceId = deviceId,
						MethodName = "stop"
					});
			}
		}
	}
	catch (Exception ex) { Debug.WriteLine(ex.Message); }
}




		private async Task GetDevicesTwinAsync()
		{
			try
			{
				while (true)
				{
					var twins = await _iotHub.GetDevicesAsTwinAsync();
					DeviceTwinList.Clear();

					foreach (var twin in twins)
						DeviceTwinList.Add(twin);

					await Task.Delay(1000);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}


		





	}
}