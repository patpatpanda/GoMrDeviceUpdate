using GoMrDevice.Models;
using GoMrDevice.MVVM.Core;
using GoMrDevice.MVVM.ViewModels;
using GoMrDevice.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;



namespace GoMrDevice
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static IHost? AppHost { get; set; }

		public App()
		{
			AppHost = Host.CreateDefaultBuilder()
				.ConfigureAppConfiguration((context, config) =>
				{
					config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
				})
				.ConfigureServices((config, services) =>
				{
					services.AddSingleton<MainWindow>();

					// Add configuration for "Device" connection string
					services.AddSingleton(new DeviceConfiguration(config.Configuration.GetConnectionString("Device")!));

					// Add configuration for "Lamp_Device" connection string
					var iotHubConnectionString = config.Configuration.GetConnectionString("IoTHubConnectionString");

					services.AddSingleton<DeviceManager>();
					services.AddSingleton<DateTimeService>();
					services.AddSingleton<NavigationStore>();
					services.AddSingleton<FanService>();

					// Create an instance of IoTHubManager with your options here
					services.AddSingleton(new IoTHubManager(new IotHubManagerOptions
					{
						IotHubConnectionString = "HostName=iot-warrior.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=fUwugjRnWfRPHa5sB+yBDMO7Oqzg7yku6AIoTKh4Z5Q=",
						EventHubEndpoint = "Endpoint=sb://ihsuprodamres117dednamespace.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessKey=fUwugjRnWfRPHa5sB+yBDMO7Oqzg7yku6AIoTKh4Z5Q=;EntityPath=iothub-ehub-iot-warrio-25230142-3ad8e367d4",
						EventHubName = "iothub-ehub-iot-warrio-25230142-3ad8e367d4",
						ConsumerGroup = "serviceapplication"
					}));
				})


				.Build();
		}

		protected override async void OnStartup(StartupEventArgs args)
		{
			var mainWindow = AppHost!.Services.GetRequiredService<MainWindow>();
			var navigationStore = AppHost!.Services.GetRequiredService<NavigationStore>();
			var dateTimeService = AppHost!.Services.GetService<DateTimeService>();
			var anService = AppHost!.Services.GetService<FanService>();
			

			navigationStore.CurrentViewModel = new HomeViewModel(navigationStore, dateTimeService!);

			await AppHost!.StartAsync();
			mainWindow.Show();
			base.OnStartup(args);
		}
	}
}
