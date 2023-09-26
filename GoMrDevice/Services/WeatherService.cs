using GoMrDevice.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GoMrDevice.Services
{
	public class WeatherService
	{
		private readonly HttpClient _httpClient;

		public WeatherService()
		{
			_httpClient = new HttpClient();
		}

		public async Task<WeatherModel> GetWeatherAsync(string city)
		{
			var apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid=6ba62436dd00318990437058362d6a82";
			var response = await _httpClient.GetAsync(apiUrl);
			response.EnsureSuccessStatusCode();
			var jsonResponse = await response.Content.ReadAsStringAsync();
			var weatherData = JsonSerializer.Deserialize<WeatherModel>(jsonResponse);

			return weatherData;
		}
	}
}
