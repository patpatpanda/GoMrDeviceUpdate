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
using System.Linq;

namespace GoMrDevice.MVVM.ViewModels
{
	public class DeviceListViewModel : INotifyPropertyChanged
	{
		private readonly IConfiguration _configuration;
		public event PropertyChangedEventHandler PropertyChanged;
		public DeviceListViewModel()
		{
			// Create the IConfiguration instance once in the constructor
			_configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();
		}



		private ObservableCollection<Twin> _deviceTwinList = new ObservableCollection<Twin>();

		public ObservableCollection<Twin> DeviceTwinList
		{
			get { return _deviceTwinList; }
			set
			{
				if (_deviceTwinList != value)
				{
					_deviceTwinList = value;
					OnPropertyChanged(nameof(DeviceTwinList));
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
		public async Task UpdateDeviceTwinListAsync(ObservableCollection<Twin> deviceTwinList)
		{
			try
			{
				while (true)
				{
					var twins = await GetDevicesAsTwinAsync();
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

					await Task.Delay(1000);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
		public async Task RemoveDeviceFromIoTHub(string deviceId)
		{
			var iotHubConnectionString = _configuration.GetConnectionString("IoTHubConnectionString");

			var registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);

			try
			{

				await registryManager.RemoveDeviceAsync(deviceId);
			}
			catch (Exception ex)
			{

				Debug.WriteLine($"Fel vid borttagning av enhet: {ex.Message}");
				throw;
			}
			finally
			{
				registryManager.CloseAsync().Wait();
			}
		}

	}
	// Rest of your ViewModel code
}
