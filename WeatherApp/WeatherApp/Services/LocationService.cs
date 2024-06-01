using Flurl.Http;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class LocationService
    {
        private readonly int _statusCode = 200;

        public async Task<ResponseModel> GetLocationData(string location)
        {
            string mapApiUrl = AppConstants.MapApiUrl;
            string mapApiKey = AppConstants.MapApiKey;

            string apiUrl = $"{mapApiUrl}/{location}.json?access_token={mapApiKey}&limit=1";

            try
            {
                var response = await apiUrl.GetAsync();

                if (response.StatusCode == _statusCode)
                {
                    var responseData = await response.GetJsonAsync<Root>();
                    return ResponseModel.Success(responseData.Features[0].Center);
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
