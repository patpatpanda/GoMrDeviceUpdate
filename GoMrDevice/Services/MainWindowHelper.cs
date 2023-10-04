using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using GoMrDevice.Models;
using Microsoft.Azure.Devices.Shared;
using GoMrDevice.Services;
using Microsoft.Azure.Devices.Client;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

public class MainWindowHelper
{
	private readonly DeviceManager _deviceManager;
	private readonly FanService _fanService;
	private readonly IoTHubManager _iotHub;
	private readonly MainWindowHelper _helper;

	public MainWindowHelper(IoTHubManager iotHub, FanService fanService, DeviceConfiguration configuration)
	{
		_iotHub = iotHub;
		_fanService = fanService;
		Configuration = configuration;
	}
	public ObservableCollection<Twin> DeviceTwinList { get; set; } = new();
	public DeviceConfiguration Configuration { get; set; }
	public async void StartDevice(Button button)
	{
		try
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
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
		}
	}


	private async Task UpdateFanStatusAsync(string status)
	{
		try
		{
			// Implementera din UpdateFanStatusAsync-kod här.
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
		}
	}
	public async void StopDevice(Button button)
	{
		try
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

	public async Task SendTelemetryAsync(DeviceConfiguration configuration, FanService fanService)
	{
		while (true)
		{
			if (configuration.AllowSending)
			{
				// Check the lamp state and include it in telemetry data
				var lampState = fanService.IsOn();

				var deviceMessage = lampState ? "The device is on" : "The device is off";

				var telemetryData = new DeviceItem
				{
					Date = DateTime.Now,
					DeviceMessage = deviceMessage
				};

				var telemetryJson = JsonConvert.SerializeObject(telemetryData);

				await SendDataAsync(telemetryJson);
			}

			await Task.Delay(configuration.TelemetryInterval);
		}
	}


	public async Task GetDevicesTwinAsync()
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

	

	public async Task UpdateDeviceTwinListAsync(ObservableCollection<Twin> deviceTwinList)
	{
		try
		{
			while (true)
			{
				var twins = await _iotHub.GetDevicesAsTwinAsync();
				deviceTwinList.Clear();

				foreach (var twin in twins)
					deviceTwinList.Add(twin);

				await Task.Delay(1000);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
		}
	}


}