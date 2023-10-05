using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
using GoMrDevice.Models;
using GoMrDevice.Services;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace GoMrDevice.MVVM.Controls
{
	/// <summary>
	/// Interaction logic for DeviceListControl.xaml
	/// </summary>
	public partial class DeviceListControl : UserControl
	{
		private readonly ObservableCollection<Twin> _deviceTwinList = new ObservableCollection<Twin>();
		


		public DeviceListControl()
		{
			InitializeComponent();

			// Start your method when your UserControl loads
			Loaded += async (sender, e) =>
			{
				await GetDevicesTwinAsync();
			};
		}

		public async Task GetDevicesTwinAsync()
		{
			try
			{
				while (true)
				{
					var twins = await GetDevicesAsTwinAsync();
					_deviceTwinList.Clear();

					foreach (var twin in twins)
						_deviceTwinList.Add(twin);

					await Task.Delay(1000);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		public async Task<IEnumerable<Twin>> GetDevicesAsTwinAsync(string sqlQuery = "select * from devices")
		{
			try
			{
				var devices = new List<Twin>();

				// Access the configuration
				IConfiguration configuration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json")
					.Build();

				// Retrieve the connection string
				var connectionString = configuration["ConnectionStrings:IoTHubConnectionString"];

				if (string.IsNullOrEmpty(connectionString))
				{
					Debug.WriteLine("IoTHubConnectionString is missing or empty in configuration.");
					return devices; // Return an empty list or handle the error as needed.
				}

				var registryManager = RegistryManager.CreateFromConnectionString(connectionString);
				var result = registryManager.CreateQuery(sqlQuery);

				if (result.HasMoreResults)
				{
					foreach (var device in await result.GetNextAsTwinAsync())
					{
						devices.Add(device);
					}
				}

				return devices;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error in GetDevicesAsTwinAsync: {ex.Message}");
			}

			return null!;
		}
		

	}
}
