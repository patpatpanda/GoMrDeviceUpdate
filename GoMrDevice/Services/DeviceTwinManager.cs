using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoMrDevice.Services
{
	public class DeviceTwinManager
	{
		public static async Task<object> GetDesiredTwiPropnAsync(DeviceClient deviceClient, string key)
		{
			try
			{
				var twin = await deviceClient.GetTwinAsync();
				if (twin.Properties.Desired.Contains(key))
					return twin.Properties.Desired[key];

			}
			catch (Exception e)
			{
				Console.WriteLine(e);

			}

			return null;
		}



		public static async Task UpdateReportedTwinAsync(DeviceClient deviceClient, string key, object value)
		{
			try
			{
				var twinProps = new TwinCollection();
				twinProps[key] = value;
				await deviceClient.UpdateReportedPropertiesAsync(twinProps);
				Console.WriteLine($"Reported property: {key} updated with value: {value}");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}



	}
}
