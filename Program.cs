using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WeatherApp
{
    class Program
    {
        // OpenWeatherMap API configuration
        private const string API_KEY = "d750580a25f3b36c00c4df76b15a16ac";
        private const string BASE_URL = "https://api.openweathermap.org/data/2.5/weather";

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Weather App ===\n");

            // Get city name from user
            Console.Write("Enter city name: ");
            string city = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(city))
            {
                Console.WriteLine("Error: City name cannot be empty.");
                return;
            }

            // Fetch and display weather
            await GetWeatherAsync(city);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Fetches weather data from OpenWeatherMap API
        /// </summary>
        static async Task GetWeatherAsync(string city)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Build API URL (URL encode the city name)
                    string encodedCity = Uri.EscapeDataString(city);
                    string url = $"{BASE_URL}?q={encodedCity}&appid={API_KEY}&units=metric";

                    Console.WriteLine($"\nFetching weather data for {city}...");
                    Console.WriteLine($"API URL: {url}\n");

                    // Make API request
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        // Parse JSON response
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        JObject weatherData = JObject.Parse(jsonResponse);

                        // Display weather information
                        DisplayWeather(weatherData);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($"Error: City '{city}' not found.");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        Console.WriteLine("AND ", response.Content);
                        Console.WriteLine("Error: Invalid API key. Please check your API key.");
                    }
                    else
                    {
                        Console.WriteLine($"Error: Unable to fetch weather data. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Displays formatted weather information
        /// </summary>
        static void DisplayWeather(JObject data)
        {
            // Extract data from JSON
            string cityName = data["name"]?.ToString();
            string country = data["sys"]?["country"]?.ToString();
            double temperature = data["main"]?["temp"]?.ToObject<double>() ?? 0;
            double feelsLike = data["main"]?["feels_like"]?.ToObject<double>() ?? 0;
            double tempMin = data["main"]?["temp_min"]?.ToObject<double>() ?? 0;
            double tempMax = data["main"]?["temp_max"]?.ToObject<double>() ?? 0;
            int humidity = data["main"]?["humidity"]?.ToObject<int>() ?? 0;
            int pressure = data["main"]?["pressure"]?.ToObject<int>() ?? 0;
            double windSpeed = data["wind"]?["speed"]?.ToObject<double>() ?? 0;
            string description = data["weather"]?[0]?["description"]?.ToString();
            string mainWeather = data["weather"]?[0]?["main"]?.ToString();

            // Display formatted output
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine($"║  Weather in {cityName}, {country}");
            Console.WriteLine("╠════════════════════════════════════════╣");
            Console.WriteLine($"║  Condition: {mainWeather} ({description})");
            Console.WriteLine($"║  Temperature: {temperature}°C");
            Console.WriteLine($"║  Feels Like: {feelsLike}°C");
            Console.WriteLine($"║  Min/Max: {tempMin}°C / {tempMax}°C");
            Console.WriteLine($"║  Humidity: {humidity}%");
            Console.WriteLine($"║  Pressure: {pressure} hPa");
            Console.WriteLine($"║  Wind Speed: {windSpeed} m/s");
            Console.WriteLine("╚════════════════════════════════════════╝");
        }
    }
}
