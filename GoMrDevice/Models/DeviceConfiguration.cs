using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Win32.SafeHandles;
namespace GoMrDevice.Models
{
	public class DeviceConfiguration
	{
		private readonly string _connectionString;
		public DeviceClient DeviceClient { get; }

		public DeviceConfiguration(string connectionString)
		{
			_connectionString = connectionString;
			DeviceClient = DeviceClient.CreateFromConnectionString(_connectionString);
		}

		public string DeviceId => _connectionString.Split(";")[1].Split("=")[1];
		public string ConnectionString => _connectionString;
		public bool AllowSending { get; set; } = false;
		public int TelemetryInterval { get; set; } = 10000;
	}

}
