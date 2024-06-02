namespace WeatherApp.Models
{
    public class WeatherModel
    {
        public Location Location { get; set; }
        public Current Current { get; set; }
        public List<Hourly> TodayHourly { get; set; }
        public List<ForecastDay> Forecast { get; set; }
    }

    public class Location
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string LocalTime { get; set; }
    }

    public class Current
    {
        public double Temperature { get; set; }
        public string[] Weather_Icons { get; set; }
        public string[] Weather_Descriptions { get; set; }
        public int Humidity { get; set; }
        public double Wind_Speed { get; set; }
        public string Wind_Dir { get; set; }
        public int Pressure { get; set; }
        public double Precip { get; set; }
        public int Cloudcover { get; set; }
        public double Feelslike { get; set; }
        public int UV_Index { get; set; }
        public double Visibility { get; set; }
    }

    public class Hourly
    {
        public string Time { get; set; }
        public double Temperature { get; set; }
        public string[] Weather_Icons { get; set; }
        public string[] Weather_Descriptions { get; set; }
    }

    public class ForecastDay
    {
        public string Date { get; set; }
        public double Mintemp { get; set; }
        public double Maxtemp { get; set; }
        public double Avgtemp { get; set; }
        public string[] Weather_Icons { get; set; }
        public string[] Weather_Descriptions { get; set; }
    }
}
