using Flurl.Http;
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

            string apiUrl = $"{weatherApiUrl}?access_key={weatherApiKey}&query=Boston";

            try
            {
                var response = await apiUrl.GetAsync();

                if (response.StatusCode == _statusCode)
                {
                    var responseData = await response.GetJsonAsync<WeatherModel>();
                    return ResponseModel.Success(responseData);
                }

                return ResponseModel.Error(response.ResponseMessage.ToString());
            }
            catch (FlurlHttpException ex)
            {
                string errorResponse = await ex.GetResponseJsonAsync<string>();
                return ResponseModel.Error(errorResponse);
            }
        }
    }
}
