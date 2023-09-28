using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using GoMrDevice.Models;
using GoMrDevice.MVVM.Core;
using GoMrDevice.Services;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

namespace GoMrDevice;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly DeviceManager _deviceManager;
	private readonly FanService _fanService;
	private readonly IoTHubManager _iotHub;

	public MainWindow(DeviceManager deviceManager, NavigationStore navigationStore, IoTHubManager iotHub,
		FanService fanService, DeviceConfiguration configuration)
	{
		InitializeComponent();
		Configuration = configuration;
		_deviceManager = deviceManager;
		_iotHub = iotHub;
		_fanService = fanService;

		DeviceListView.ItemsSource = DeviceTwinList;

		DataContext = navigationStore;

		Task.WhenAll(ToggleFanStateAsync(), GetDevicesTwinAsync(), SendTelemetryAsync());
	}

	public ObservableCollection<Twin> DeviceTwinList { get; set; } = new();
	public DeviceConfiguration Configuration { get; set; }


	private async Task ToggleFanStateAsync()
	{
		
		var fan = (Storyboard)FindResource("FanStoryboard");

		while (true)
		{
			var areDevicesOn = _deviceManager.AllowSending();

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


	private void SetMessage(string message)
	{
		Dispatcher.Invoke(() => { MessageTextBlock.Text = message; });
	}


	private async Task UpdateFanStatusAsync(string status)
	{
		try
		{
			if (Configuration.DeviceClient != null)
				await DeviceTwinManager.UpdateReportedTwinAsync(Configuration.DeviceClient, "fanStatus", status);
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
			var button = sender as Button;
			if (button != null)
			{
				var twin = button.DataContext as Twin;
				if (twin != null)
				{
					var deviceId = twin.DeviceId;
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
			var button = sender as Button;
			if (button != null)
			{
				var twin = button.DataContext as Twin;
				if (twin != null)
				{
					var deviceId = twin.DeviceId;
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
		}
	}


	private async Task SendTelemetryAsync()
	{
		while (true)
		{
			if (Configuration.AllowSending)
			{
				// Check the lamp state and include it in telemetry data
				var lampState = _fanService.IsOn();

				var deviceMessage = lampState ? "The device is on" : "The device is off";

				var telemetryData = new DeviceItem
				{
					Date = DateTime.Now,


					DeviceMessage = deviceMessage
				};

				var telemetryJson = JsonConvert.SerializeObject(telemetryData);

				await SendDataAsync(telemetryJson);
			}

			await Task.Delay(Configuration.TelemetryInterval);
		}
	}
}