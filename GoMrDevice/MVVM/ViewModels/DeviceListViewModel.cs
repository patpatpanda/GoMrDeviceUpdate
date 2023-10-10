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
using GoMrDevice.Data;
using GoMrDevice.Models;
using Microsoft.EntityFrameworkCore;

namespace GoMrDevice.MVVM.ViewModels
{
	public class DeviceListViewModel : INotifyPropertyChanged
	{


		private readonly IConfiguration _configuration;
		private readonly ApplicationDbContext _db;

		// ...

		public DeviceListViewModel()
		{
			_configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();
			_db = new ApplicationDbContext();
		}

		// ...



		public event PropertyChangedEventHandler PropertyChanged;

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

				
				IConfiguration configuration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json")
					.Build();

				
				var connectionString = configuration["ConnectionStrings:IoTHubConnectionString"];

				if (string.IsNullOrEmpty(connectionString))
				{
					Debug.WriteLine("IoTHubConnectionString is missing or empty in configuration.");
					return devices; 
				}

				var registryManager = RegistryManager.CreateFromConnectionString(connectionString);
				var result = registryManager.CreateQuery(sqlQuery);

				if (result != null && result.HasMoreResults)
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

			return Enumerable.Empty<Twin>();
		}

		
		public async Task RemoveDeviceFromIoTHub(string deviceId)
		{
			var iotHubConnectionString = _configuration.GetConnectionString("IoTHubConnectionString");

			var registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);

			try
			{
				await registryManager.RemoveDeviceAsync(deviceId);

				var removedDevice = new RemovedDevice
				{
					Name = deviceId,
					Message = "Was deleted",
					Date = DateTime.Now
				};

				
				_db.RemovedDevices.Add(removedDevice);
				await _db.SaveChangesAsync();
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
}

