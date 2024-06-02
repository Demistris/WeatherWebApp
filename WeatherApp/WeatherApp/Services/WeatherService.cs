using Flurl.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class WeatherService
    {
        private readonly int _statusCode = 200;

        public async Task<ResponseModel> GetWeatherData(string location) 
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

                    string apiUrl = $"{weatherApiUrl}?access_key={weatherApiKey}&query={latitude},{longitude}";

                    var response = await apiUrl.GetAsync();

                    if (response.StatusCode == _statusCode)
                    {
                        var responseData = await response.GetJsonAsync<WeatherModel>();
                        return ResponseModel.Success(responseData);
                    }

                    return ResponseModel.Error(response.ResponseMessage.ToString());
                }

                return ResponseModel.Error(res.Result.ToString());
            }
            catch (FlurlHttpException ex)
            {
                string errorResponse = await ex.GetResponseJsonAsync<string>();
                return ResponseModel.Error(errorResponse);
            }
        }
    }
}
