using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Configuration;

namespace GoMrDevice.MVVM.ViewModels
{
	public class DeviceListViewModel : INotifyPropertyChanged
	{
		private ObservableCollection<Twin> _deviceTwinList;

		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableCollection<Twin> DeviceTwinList
		{
			get { return _deviceTwinList; }
			set
			{
				if (_deviceTwinList != value)
				{
					_deviceTwinList = value;
					OnPropertyChanged(nameof(DeviceTwinList));
					Debug.WriteLine("DeviceTwinList property set.");
				}
			}
		}

		// This method is used to raise the PropertyChanged event
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		public async Task LoadDeviceTwinDataAsync()
		{
			var twins = await GetDevicesAsTwinAsync();
			DeviceTwinList = new ObservableCollection<Twin>(twins);
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
	// Rest of your ViewModel code
}
