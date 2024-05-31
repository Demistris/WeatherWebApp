namespace WeatherApp.Models
{
    public class WeatherModel
    {
        public Location? Location { get; set; }
        public Current? Current { get; set; }
    }

    public class Location
    {
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }
        public string? LocalTime { get; set; }
    }
    
    public class Current
    {
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public string[]? Weather_Descriptions { get; set; }
    }
}
