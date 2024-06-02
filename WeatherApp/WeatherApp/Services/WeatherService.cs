using Flurl.Http;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Diagnostics;
using WeatherApp.Models;
using static WeatherApp.Models.CurrentWeatherModel;
using static WeatherApp.Models.ForecastModel;

namespace WeatherApp.Services
{
    public class WeatherService
    {
        private readonly int _statusCode = 200;

        public async Task<WeatherModel> GetWeatherData(string location) 
        {
            string weatherApiUrl = AppConstants.WeatherApiUrl;
            string weatherApiKey = AppConstants.WeatherApiKey;

            try
            {
                LocationService locationService = new LocationService();
                var res = locationService.GetLocationData(location);
                dynamic coordinates;

                if (res.Result.IsSuccess)
                {
                    coordinates = res.Result.JsonData;
                    var longitude = coordinates[0];
                    var latitude = coordinates[1];

                    // Construct the API URLs for current weather and forecast
                    string currentWeatherUrl = $"{weatherApiUrl}/weather?lat={latitude}&lon={longitude}&appid={weatherApiKey}&units=metric";
                    string forecastUrl = $"{weatherApiUrl}/forecast?lat={latitude}&lon={longitude}&appid={weatherApiKey}&units=metric";

                    // Fetch current weather data
                    var currentWeatherResponse = await currentWeatherUrl.GetAsync();
                    var currentWeatherData = await currentWeatherResponse.GetJsonAsync<CurrentWeatherResponse>();

                    // Fetch forecast data
                    var forecastResponse = await forecastUrl.GetAsync();
                    var forecastData = await forecastResponse.GetJsonAsync<ForecastResponse>();

                    // Map the API response to WeatherModel
                    var weatherModel = new WeatherModel
                    {
                        Location = new Location
                        {
                            Name = currentWeatherData.Name,
                            Country = currentWeatherData.Sys.Country,
                            LocalTime = DateTimeOffset.FromUnixTimeSeconds(currentWeatherData.Dt).ToString("yyyy-MM-dd HH:mm")
                        },
                        Current = new Current
                        {
                            Temperature = Math.Round(currentWeatherData.Main.Temp),
                            Weather_Descriptions = new[] { currentWeatherData.Weather[0].Description },
                            Weather_Icons = new[] { $"http://openweathermap.org/img/wn/{currentWeatherData.Weather[0].Icon}.png" },
                        },
                        TodayHourly = new List<Hourly>(),
                        Forecast = new List<ForecastDay>()
                    };

                    // Define the target hours
                    var targetHours = new[] { 0, 3, 6, 9, 12, 15, 18, 21 };
                    var now = DateTimeOffset.UtcNow;

                    // Filter today's hourly data from 6AM to 9PM
                    foreach (var forecast in forecastData.List)
                    {
                        var forecastDateTime = DateTimeOffset.FromUnixTimeSeconds(forecast.Dt);
                        if ((forecastDateTime.Date == now.Date && targetHours.Contains(forecastDateTime.Hour) && forecastDateTime.Hour >= now.Hour) ||
                            (forecastDateTime.Date == now.Date.AddDays(1) && targetHours.Contains(forecastDateTime.Hour)))
                        {
                            weatherModel.TodayHourly.Add(new Hourly
                            {
                                Time = forecastDateTime.ToString("HH:mm"),
                                Temperature = Math.Round(forecast.Main.Temp),
                                Weather_Descriptions = new[] { forecast.Weather[0].Description },
                                Weather_Icons = new[] { $"http://openweathermap.org/img/wn/{forecast.Weather[0].Icon}.png" }
                            });

                            // Break if we have reached 6 entries
                            if (weatherModel.TodayHourly.Count == 6)
                            {
                                break;
                            }
                        }
                    }

                    // Ensure we have exactly 6 entries, filling in from the next day if necessary
                    if (weatherModel.TodayHourly.Count < 6)
                    {
                        foreach (var forecast in forecastData.List)
                        {
                            var forecastDateTime = DateTimeOffset.FromUnixTimeSeconds(forecast.Dt);
                            if (forecastDateTime.Date == now.Date.AddDays(1) && targetHours.Contains(forecastDateTime.Hour))
                            {
                                weatherModel.TodayHourly.Add(new Hourly
                                {
                                    Time = forecastDateTime.ToString("HH:mm"),
                                    Temperature = Math.Round(forecast.Main.Temp),
                                    Weather_Descriptions = new[] { forecast.Weather[0].Description },
                                    Weather_Icons = new[] { $"http://openweathermap.org/img/wn/{forecast.Weather[0].Icon}.png" }
                                });

                                // Break if we have reached 6 entries
                                if (weatherModel.TodayHourly.Count == 6)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    // Process 5-day forecast
                    var groupedForecasts = forecastData.List
                        .GroupBy(f => DateTimeOffset.FromUnixTimeSeconds(f.Dt).Date)
                        .Take(5);

                    foreach (var group in groupedForecasts)
                    {
                        var date = group.Key;
                        var dailyForecasts = group.ToList();

                        var minTemp = dailyForecasts.Min(f => f.Main.Temp);
                        var maxTemp = dailyForecasts.Max(f => f.Main.Temp);
                        var avgTemp = dailyForecasts.Average(f => f.Main.Temp);
                        var weatherDescriptions = dailyForecasts
                            .Select(f => f.Weather[0].Description)
                            .GroupBy(d => d)
                            .OrderByDescending(g => g.Count())
                            .First().Key;
                        var weatherIcons = dailyForecasts
                            .Select(f => f.Weather[0].Icon)
                            .GroupBy(i => i)
                            .OrderByDescending(g => g.Count())
                            .First().Key;

                        weatherModel.Forecast.Add(new ForecastDay
                        {
                            Date = date.ToString("ddd"),
                            Avgtemp = Math.Round(avgTemp),
                            Mintemp = Math.Round(minTemp),
                            Maxtemp = Math.Round(maxTemp),
                            Weather_Descriptions = new[] { weatherDescriptions },
                            Weather_Icons = new[] { $"http://openweathermap.org/img/wn/{weatherIcons}.png" }
                        });
                    }

                    return weatherModel;
                }

                return null;
            }
            catch (FlurlHttpException ex)
            {
                string errorResponse = await ex.GetResponseJsonAsync<string>();
                Debug.WriteLine($"Error fetching weather data: {errorResponse}");
                return null;
            }
        }
    }
}
