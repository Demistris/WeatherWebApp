using Flurl.Http;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class WeatherService
    {
        private readonly int _statusCode = 200;

        public async Task<ResponseModel> GetWeatherData() 
        {
            string apiUrl = "http://api.weatherstack.com/current?access_key=a5bdb300635ee5e45fd0cdfe1a71e924&query=NewYork";

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
                var errorResponse = await ex.GetResponseJsonAsync();
                return ResponseModel.Error(errorResponse);
            }
        }
    }
}
