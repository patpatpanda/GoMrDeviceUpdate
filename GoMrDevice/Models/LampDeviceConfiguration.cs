using Microsoft.Azure.Devices.Client;
using System;

namespace GoMrDevice.Models
{
	public class LampDeviceConfiguration
	{
		private readonly string _connectionString;

		public DeviceClient DeviceClient { get; }

		public LampDeviceConfiguration(string connectionString)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");
			}

			_connectionString = connectionString;
			DeviceClient = DeviceClient.CreateFromConnectionString(_connectionString);
		}

		// Other properties and methods in the class...
	}

}