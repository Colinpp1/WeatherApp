# Weather App

A C# console application that fetches and displays current weather information using the OpenWeatherMap API.

## What This Program Does

- Prompts user to enter a city name
- Fetches current weather data from OpenWeatherMap API
- Displays temperature, humidity, wind speed, and weather conditions
- Uses async/await for non-blocking API calls
- Handles errors gracefully (invalid city, network issues, invalid API key)

## Setup

### 1. Get an API Key

1. Go to [OpenWeatherMap](https://openweathermap.org/api)
2. Sign up for a free account
3. Get your API key from the dashboard

### 2. Add Your API Key

Open `Program.cs` and replace the weather api key with your own:


## How to Run

git clone repo

### Command Line
```bash
cd location_project
dotnet run
```

### Visual Studio
1. Open `WeatherApp.csproj`
2. Press `F5` to run

## Sample Output

```
=== Weather App ===

Enter city name: London

Fetching weather data for London...

╔════════════════════════════════════════╗
║  Weather in London, GB
╠════════════════════════════════════════╣
║  Condition: Clouds (overcast clouds)
║  Temperature: 15.5°C
║  Feels Like: 14.8°C
║  Min/Max: 14.2°C / 16.7°C
║  Humidity: 72%
║  Pressure: 1013 hPa
║  Wind Speed: 3.5 m/s
╚════════════════════════════════════════╝

Press any key to exit...
```

## Features

✅ Async/await for API calls  
✅ JSON parsing with Newtonsoft.Json  
✅ Error handling (city not found, invalid API key, network errors)  
✅ Formatted console output  
✅ Temperature in Celsius  
