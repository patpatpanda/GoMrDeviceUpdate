using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using GoMrDevice.Models;
using GoMrDevice.MVVM.Core;
using GoMrDevice.Services;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GoMrDevice;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly DeviceManager _deviceManager;
	private readonly FanService _fanService;
	private readonly IoTHubManager _iotHub;
	private readonly MainWindowHelper _helper;
	private readonly IConfiguration _config;


	public MainWindow(DeviceManager deviceManager, NavigationStore navigationStore, IoTHubManager iotHub,
		FanService fanService, DeviceConfiguration configuration,IConfiguration config)
	{
		InitializeComponent();
		Configuration = configuration;
		_deviceManager = deviceManager;
		_iotHub = iotHub;
		_fanService = fanService;
		_config = config;

		DeviceListView.ItemsSource = DeviceTwinList;

		DataContext = navigationStore;
		_helper = new MainWindowHelper(iotHub, fanService, configuration,_config);


		Task.WhenAll(ToggleFanStateAsync(), _helper.GetDevicesTwinAsync(),_helper.SendTelemetryAsync(Configuration, _fanService),
			_helper.UpdateDeviceTwinListAsync(DeviceTwinList));
	}

	public ObservableCollection<Twin> DeviceTwinList { get; set; } = new();
	public DeviceConfiguration Configuration { get; set; }

	private void StartButton_Click(object sender, RoutedEventArgs e)
	{
		_helper.StartDevice(sender as Button);
	}

	private void StopButton_Click(object sender, RoutedEventArgs e)
	{
		_helper.StopDevice(sender as Button);
	}


	
	
	private async Task ToggleFanStateAsync()
	{
		
		var fan = (Storyboard)FindResource("FanStoryboard");

		while (true)
		{
			var areDevicesOn = _deviceManager.AllowSending();

			if (areDevicesOn)
			{
				fan.Begin();
				SetMessage("The fan is ON.");
			}
			else
			{
				fan.Stop();
				SetMessage("The fan is OFF.");
			}
			
			await Task.Delay(1000);
		}
	}


	private void SetMessage(string message)
	{
		Dispatcher.Invoke(() => { MessageTextBlock.Text = message; });
	}


	
	
	
}