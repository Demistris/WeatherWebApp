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
                            Temperature = currentWeatherData.Main.Temp,
                            Weather_Descriptions = new[] { currentWeatherData.Weather[0].Description },
                            Weather_Icons = new[] { $"http://openweathermap.org/img/wn/{currentWeatherData.Weather[0].Icon}.png" }
                        },
                        TodayHourly = new List<Hourly>()
                    };

                    // Define the target hours
                    var targetHours = new[] { 6, 9, 12, 15, 18, 21 };
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
                                Temperature = forecast.Main.Temp,
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
                                    Temperature = forecast.Main.Temp,
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
