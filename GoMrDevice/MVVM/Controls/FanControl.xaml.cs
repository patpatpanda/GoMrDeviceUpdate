using GoMrDevice.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
namespace GoMrDevice.MVVM.Controls
{
	/// <summary>
	/// Interaction logic for FanControl.xaml
	/// </summary>
	public partial class FanControl : UserControl
	{
		private readonly DeviceManager _deviceManager;
		private readonly Storyboard _fanStoryboard;

		public FanControl(DeviceManager deviceManager, Storyboard fanStoryboard)
		{
			InitializeComponent();

			_deviceManager = deviceManager;
			_fanStoryboard = fanStoryboard;

			Loaded += FanControl_Loaded;
		}

		private async void FanControl_Loaded(object sender, RoutedEventArgs e)
		{
			await ToggleFanStateAsync();
		}

		private async Task ToggleFanStateAsync()
		{
			while (true)
			{
				if (_deviceManager.AllowSending())
					_fanStoryboard.Begin();
				else
					_fanStoryboard.Stop();

				await Task.Delay(1000);
			}
		}
	}
}
