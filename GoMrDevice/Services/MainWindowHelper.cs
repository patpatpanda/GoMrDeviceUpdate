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
using System.Linq;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Configuration;
using Message = Microsoft.Azure.Devices.Client.Message;

public class MainWindowHelper
{
	private readonly DeviceManager _deviceManager;
	private readonly FanService _fanService;
	private readonly IoTHubManager _iotHub;
	private readonly MainWindowHelper _helper;
	private readonly IConfiguration _config;
	public MainWindowHelper(IoTHubManager iotHub, FanService fanService, DeviceConfiguration configuration, IConfiguration config)
	{
		_iotHub = iotHub;
		_fanService = fanService;
		Configuration = configuration;
		_config = config;
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

				if (twins != null)
				{
					// Clear the existing items in DeviceTwinList
					DeviceTwinList.Clear();

					// Add the new twins to DeviceTwinList
					foreach (var twin in twins)
					{
						DeviceTwinList.Add(twin);
					}
				}
				else
				{
					// Handle the case where twins is null
					Debug.WriteLine("No twins returned from the IoT Hub.");
				}

				await Task.Delay(1000);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error in GetDevicesTwinAsync: {ex.Message}");
		}
	}





	public async Task UpdateDeviceTwinListAsync(ObservableCollection<Twin> deviceTwinList)
	{
		try
		{
			while (true)
			{
				var twins = await _iotHub.GetDevicesAsTwinAsync();

				if (twins != null)
				{
					var existingDeviceIds = deviceTwinList.Select(twin => twin.DeviceId).ToList();

					// Lägg till nya enheter
					foreach (var twin in twins)
					{
						if (!existingDeviceIds.Contains(twin.DeviceId))
						{
							deviceTwinList.Add(twin);
						}
					}

					// Ta bort enheter som har tagits bort från IoT-hubb
					var removedDeviceIds = existingDeviceIds.Except(twins.Select(twin => twin.DeviceId)).ToList();
					foreach (var removedDeviceId in removedDeviceIds)
					{
						var twinToRemove = deviceTwinList.FirstOrDefault(twin => twin.DeviceId == removedDeviceId);
						if (twinToRemove != null)
						{
							deviceTwinList.Remove(twinToRemove);
						}
					}
				}
				else
				{
					// Handle the case where twins is null
					Debug.WriteLine("No twins returned from the IoT Hub.");
				}

				await Task.Delay(1000);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
		}
	}





}