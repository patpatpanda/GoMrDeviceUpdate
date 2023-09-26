using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoMrDevice.MVVM.ViewModels
{
	public class WeatherModel
	{



		public Main main { get; set; }
		public List<Weather> weather { get; set; }

	}



	public class Weather
	{
		public string description { get; set; }

	}

	public class Main
	{
		public double temp { get; set; }

		public double TempCelsius => temp - 273.15;
	}
}
