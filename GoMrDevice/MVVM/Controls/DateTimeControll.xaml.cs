using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace GoMrDevice.MVVM.Controls
{
	/// <summary>
	/// Interaction logic for DateTimeControll.xaml
	/// </summary>
	public partial class DateTimeControll : UserControl
	{
		private string? _currentTime;
		private string? _currentDate;

		public string? CurrentTime { get => _currentTime; set { _currentTime = value; OnPropertyChanged(nameof(CurrentTime)); } }
		public string? CurrentDate { get => _currentDate; set { _currentDate = value; OnPropertyChanged(nameof(CurrentDate)); } }


		public event PropertyChangedEventHandler? PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}








		public DateTimeControll()
		{
			InitializeComponent();
			DataContext = this;
			SetDateTime();

			DispatcherTimer timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(1),
			};
			timer.Tick += (s, e) => SetDateTime();
			timer.Start();
		}



		private void SetDateTime()
		{
			CurrentTime = DateTime.Now.ToString("HH:mm");
			CurrentDate = DateTime.Now.ToString("dddd, d MMMM yyyy");
		}
	}
}
