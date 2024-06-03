namespace WeatherApp.Models
{
    public class CurrentWeatherModel
    {
        public class CurrentWeatherResponse
        {
            public Coord Coord { get; set; }
            public Weather[] Weather { get; set; }
            public Main Main { get; set; }
            public Wind Wind { get; set; }
            public Clouds Clouds { get; set; }
            public Sys Sys { get; set; }
            public long Dt { get; set; }
            public string Name { get; set; }
            public int Visibility { get; set; }
        }

        public class Coord
        {
            public double Lon { get; set; }
            public double Lat { get; set; }
        }

        public class Weather
        {
            public string Description { get; set; }
            public string Icon { get; set; }
        }

        public class Main
        {
            public double Temp { get; set; }
            public double Feels_Like { get; set; }
            public int Pressure { get; set; }
            public int Humidity { get; set; }
            public double Temp_Min { get; set; }
            public double Temp_Max { get; set; }
        }

        public class Wind
        {
            public double Speed { get; set; }
            public int Deg { get; set; }
        }

        public class Clouds
        {
            public int All { get; set; }
        }

        public class Sys
        {
            public string Country { get; set; }
        }
    }
}
