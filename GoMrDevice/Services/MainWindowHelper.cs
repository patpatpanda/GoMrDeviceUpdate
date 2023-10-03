using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using GoMrDevice.Models;
using Microsoft.Azure.Devices.Shared;
using GoMrDevice.Services;

public class MainWindowHelper
{
	private readonly IoTHubManager _iotHub;
	private readonly FanService _fanService;

	public MainWindowHelper(IoTHubManager iotHub, FanService fanService)
	{
		_iotHub = iotHub;
		_fanService = fanService;
	}

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
	

}