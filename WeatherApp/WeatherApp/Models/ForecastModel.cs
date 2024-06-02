using static WeatherApp.Models.CurrentWeatherModel;

namespace WeatherApp.Models
{
    public class ForecastModel
    {
        public class ForecastResponse
        {
            public List<ForecastItem> List { get; set; }
            public City City { get; set; }
        }

        public class ForecastItem
        {
            public long Dt { get; set; }
            public Main Main { get; set; }
            public Weather[] Weather { get; set; }
            public Clouds Clouds { get; set; }
            public Wind Wind { get; set; }
            public int Visibility { get; set; }
            public string Dt_Txt { get; set; }
        }

        public class City
        {
            public string Name { get; set; }
            public Coord Coord { get; set; }
            public string Country { get; set; }
        }
    }
}
