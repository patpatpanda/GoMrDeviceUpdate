using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoMrDevice.MVVM.ViewModels
{
	public class DeviceViewModel : INotifyPropertyChanged
	{
		private string _lampStatus;

		public string LampStatus
		{
			get { return _lampStatus; }
			set
			{
				if (_lampStatus != value)
				{
					_lampStatus = value;
					OnPropertyChanged(nameof(LampStatus));
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
