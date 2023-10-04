using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using GoMrDevice.Models;
using GoMrDevice.Services;
using Microsoft.Azure.Devices.Shared;

namespace GoMrDevice.MVVM.Controls
{
	public partial class DeviceControl : UserControl
	{
		private DeviceManager _deviceManager;
		private FanService _fanService;
		private IoTHubManager _iotHub;
		private MainWindowHelper _helper;

		public ObservableCollection<Twin> DeviceTwinList { get; set; } = new ObservableCollection<Twin>();
		public DeviceConfiguration Configuration { get; set; }
		public static readonly DependencyProperty FanStatusProperty =
			DependencyProperty.Register(nameof(FanStatus), typeof(bool), typeof(DeviceControl), new PropertyMetadata(false));

		public bool FanStatus
		{
			get { return (bool)GetValue(FanStatusProperty); }
			set { SetValue(FanStatusProperty, value); }
		}
		public void SetServices(DeviceManager deviceManager, FanService fanService, IoTHubManager ioTHubManager, DeviceConfiguration deviceConfiguration)
		{
			_deviceManager = deviceManager;
			_fanService = fanService;
			_iotHub = ioTHubManager;
			Configuration = deviceConfiguration;

			_helper = new MainWindowHelper(_iotHub, _fanService, Configuration);

			// Start fan toggle and other tasks
			ToggleFanStateAsync();
			_ = _helper.GetDevicesTwinAsync();
			_ = _helper.SendTelemetryAsync(Configuration, _fanService);
			_ = _helper.UpdateDeviceTwinListAsync(DeviceTwinList);
		}

		

		


		private async Task ToggleFanStateAsync()
		{
			var fan = (Storyboard)FindResource("FanStoryboard");

			while (true)
			{
				var areDevicesOn = _deviceManager?.AllowSending() ?? false;

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
			MessageTextBlock.Text = message;
		}

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			_helper.StartDevice(sender as Button);
		}

		private void StopButton_Click(object sender, RoutedEventArgs e)
		{
			_helper.StopDevice(sender as Button);
		}
	}
}
