using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace WeatherApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string apiKey = configuration["WeatherApi:ApiKey"];
            string baseUrl = configuration["WeatherApi:BaseUrl"];

            Console.WriteLine("=== Weather App ===\n");

            // Get city name from user
            Console.Write("Enter city name: ");
            string city = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(city))
            {
                Console.WriteLine("Error: City name cannot be empty.");
                return;
            }

            // Create weather service and fetch data
            var weatherService = new WeatherService(apiKey, baseUrl);

            try
            {
                Console.WriteLine($"\nFetching weather data for {city}...\n");
                WeatherData weather = await weatherService.GetWeatherAsync(city);
                DisplayWeather(weather);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Displays formatted weather information
        /// </summary>
        static void DisplayWeather(WeatherData weather)
        {
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine($"║  Weather in {weather.CityName}, {weather.Country}");
            Console.WriteLine("╠════════════════════════════════════════╣");
            Console.WriteLine($"║  Condition: {weather.MainWeather} ({weather.Description})");
            Console.WriteLine($"║  Temperature: {weather.Temperature}°C");
            Console.WriteLine($"║  Feels Like: {weather.FeelsLike}°C");
            Console.WriteLine($"║  Min/Max: {weather.TempMin}°C / {weather.TempMax}°C");
            Console.WriteLine($"║  Humidity: {weather.Humidity}%");
            Console.WriteLine($"║  Pressure: {weather.Pressure} hPa");
            Console.WriteLine($"║  Wind Speed: {weather.WindSpeed} m/s");
            Console.WriteLine("╚════════════════════════════════════════╝");
        }
    }

    public class WeatherService
    {
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public WeatherService(string apiKey, string baseUrl)
        {
            _apiKey = apiKey;
            _baseUrl = baseUrl;
        }

        public async Task<WeatherData> GetWeatherAsync(string city)
        {
            using (HttpClient client = new HttpClient())
            {
                // Build API URL (URL encode the city name)
                string encodedCity = Uri.EscapeDataString(city);
                string url = $"{_baseUrl}?q={encodedCity}&appid={_apiKey}&units=metric";

                // Make API request
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // Parse JSON response
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject weatherData = JObject.Parse(jsonResponse);

                    // Extract data from JSON
                    string cityName = weatherData["name"]?.ToString();
                    string country = weatherData["sys"]?["country"]?.ToString();
                    double temperature = weatherData["main"]?["temp"]?.ToObject<double>() ?? 0;
                    double feelsLike = weatherData["main"]?["feels_like"]?.ToObject<double>() ?? 0;
                    double tempMin = weatherData["main"]?["temp_min"]?.ToObject<double>() ?? 0;
                    double tempMax = weatherData["main"]?["temp_max"]?.ToObject<double>() ?? 0;
                    int humidity = weatherData["main"]?["humidity"]?.ToObject<int>() ?? 0;
                    int pressure = weatherData["main"]?["pressure"]?.ToObject<int>() ?? 0;
                    double windSpeed = weatherData["wind"]?["speed"]?.ToObject<double>() ?? 0;
                    string description = weatherData["weather"]?[0]?["description"]?.ToString();
                    string mainWeather = weatherData["weather"]?[0]?["main"]?.ToString();

                    return new WeatherData
                    {
                        CityName = cityName,
                        Country = country,
                        Temperature = temperature,
                        FeelsLike = feelsLike,
                        TempMin = tempMin,
                        TempMax = tempMax,
                        Humidity = humidity,
                        Pressure = pressure,
                        WindSpeed = windSpeed,
                        Description = description,
                        MainWeather = mainWeather
                    };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception($"City '{city}' not found.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Invalid API key. Please check your API key.");
                }
                else
                {
                    throw new Exception($"Unable to fetch weather data. Status code: {response.StatusCode}");
                }
            }
        }
    }

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
