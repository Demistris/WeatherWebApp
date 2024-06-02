namespace WeatherApp.Models
{
    public class WeatherModel
    {
        public Location? Location { get; set; }
        public Current? Current { get; set; }
        public List<Forecast>? Forecast { get; set; }
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
        public string[]? Weather_Icons { get; set; }
        public string[]? Weather_Descriptions { get; set; }
        public int Feelslike { get; set; }
        public int Humidity { get; set; }
        public int Wind_Speed { get; set; }
        public int UV_Index { get; set; }
    }

    public class Forecast
    {
        public string? Date { get; set; }
        public int Mintemp { get; set; }
        public int Maxtemp { get; set; }
        public int UV_Index { get; set; }
        public List<Hourly>? Hourly { get; set; }
    }

    public class Hourly
    {
        public string? Time { get; set; }
        public int Temperature { get; set; }
        public int Wind_Speed { get; set; }
        public string[]? Weather_Icons { get; set; }
        public string[]? Weather_Descriptions { get; set; }
        public int Humidity { get; set; }
        public int Visibility { get; set; }
        public int Pressure { get; set; }
        public int Feelslike { get; set; }
        public int ChanceOfRain { get; set; }
        public int UV_Index { get; set; }
    }
}
