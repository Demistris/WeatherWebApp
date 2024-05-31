namespace WeatherApp.Models
{
    public class ResponseModel
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public dynamic? JsonData { get; set; }

        public static ResponseModel Success(dynamic jsonData)
        {
            return new ResponseModel 
            { 
                JsonData = jsonData,
                Message = null,
                IsSuccess = true
            };
        }
        
        public static ResponseModel Error(dynamic errorMessage)
        {
            return new ResponseModel 
            { 
                JsonData = null,
                Message = errorMessage, 
                IsSuccess = false 
            };
        }
    }
}
