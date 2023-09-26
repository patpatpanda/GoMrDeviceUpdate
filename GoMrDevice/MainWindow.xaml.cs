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
		private readonly FanService _fanService;
		public DeviceConfiguration Configuration { get; set; }

		public MainWindow(DeviceManager deviceManager, NavigationStore navigationStore, IoTHubManager iotHub,FanService fanService,DeviceConfiguration configuration)
		{
			InitializeComponent();
			Configuration = configuration;
			_deviceManager = deviceManager;
			_iotHub = iotHub;
			_fanService = fanService;

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


		// Create a method to update the fan status in Device Twin
		private async Task UpdateFanStatusAsync(string status)
		{
			try
			{
				if (Configuration.DeviceClient != null)
				{
					await DeviceTwinManager.UpdateReportedTwinAsync(Configuration.DeviceClient, "fanStatus", status);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
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
						{
							await _iotHub.SendMethodAsync(new MethodDataRequest
							{
								DeviceId = deviceId,
								MethodName = "start"
							});

							_fanService.TurnOn();

							
							
						}
					
					}
					await UpdateFanStatusAsync("On");
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
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
						{
							await _iotHub.SendMethodAsync(new MethodDataRequest
							{
								DeviceId = deviceId,
								MethodName = "stop"
							});

							_fanService.TurnOff();

							
							
						}
					}
					await UpdateFanStatusAsync("Off");
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
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

		public async Task SendDataAsync(string dataAsJson)
		{


			if (!string.IsNullOrEmpty(dataAsJson))
			{
				var message = new Message(Encoding.UTF8.GetBytes(dataAsJson));
				await Configuration.DeviceClient.SendEventAsync(message);
				//Console.WriteLine($"Message sent at {DateTime.Now} with data {dataAsJson}");

			}


		}



		private async Task SendTelemetryAsync()
		{
			while (true)
			{
				if (Configuration.AllowSending)
				{
					// Check the lamp state and include it in telemetry data
					bool lampState = _fanService.IsOn();

					string deviceMessage = lampState ? "The device is on" : "The device is off";

					var telemetryData = new DeviceItem()
					{

						Date = DateTime.Now,


						DeviceMessage = deviceMessage,

					};

					var telemetryJson = JsonConvert.SerializeObject(telemetryData);

					await SendDataAsync(telemetryJson);

				}

				await Task.Delay(Configuration.TelemetryInterval);
			}
		}


	}
}