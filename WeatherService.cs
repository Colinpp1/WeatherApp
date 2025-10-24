using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WeatherApp
{
    /// <summary>
    /// Service class for fetching and parsing weather data
    /// </summary>
    public class WeatherService
    {
        private readonly string apiKey;
        private readonly string baseUrl;
        private readonly HttpClient httpClient;

        public WeatherService(string apiKey, string baseUrl = "https://api.openweathermap.org/data/2.5/weather")
        {
            this.apiKey = apiKey;
            this.baseUrl = baseUrl;
            this.httpClient = new HttpClient();
        }

        /// <summary>
        /// Fetches weather data for a given city
        /// </summary>
        public async Task<WeatherData> GetWeatherAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                throw new ArgumentException("City name cannot be empty.", nameof(city));
            }

            string encodedCity = Uri.EscapeDataString(city);
            string url = $"{baseUrl}?q={encodedCity}&appid={apiKey}&units=metric";

            HttpResponseMessage response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return ParseWeatherData(jsonResponse);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception($"City '{city}' not found.");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new Exception("Invalid API key.");
            }
            else
            {
                throw new Exception($"Unable to fetch weather data. Status code: {response.StatusCode}");
            }
        }

        /// <summary>
        /// Parses JSON response into WeatherData object
        /// </summary>
        public WeatherData ParseWeatherData(string jsonResponse)
        {
            JObject data = JObject.Parse(jsonResponse);

            return new WeatherData
            {
                CityName = data["name"]?.ToString(),
                Country = data["sys"]?["country"]?.ToString(),
                Temperature = data["main"]?["temp"]?.ToObject<double>() ?? 0,
                FeelsLike = data["main"]?["feels_like"]?.ToObject<double>() ?? 0,
                TempMin = data["main"]?["temp_min"]?.ToObject<double>() ?? 0,
                TempMax = data["main"]?["temp_max"]?.ToObject<double>() ?? 0,
                Humidity = data["main"]?["humidity"]?.ToObject<int>() ?? 0,
                Pressure = data["main"]?["pressure"]?.ToObject<int>() ?? 0,
                WindSpeed = data["wind"]?["speed"]?.ToObject<double>() ?? 0,
                Description = data["weather"]?[0]?["description"]?.ToString(),
                MainWeather = data["weather"]?[0]?["main"]?.ToString()
            };
        }
    }

    /// <summary>
    /// Weather data model
    /// </summary>
    public class WeatherData
    {
        public string CityName { get; set; }
        public string Country { get; set; }
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public double TempMin { get; set; }
        public double TempMax { get; set; }
        public int Humidity { get; set; }
        public int Pressure { get; set; }
        public double WindSpeed { get; set; }
        public string Description { get; set; }
        public string MainWeather { get; set; }
    }
}
