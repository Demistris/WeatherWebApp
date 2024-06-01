namespace WeatherApp.Models
{
    public class Root
    {
        public List<Feature>? Features { get; set; }
    }

    public class Feature
    {
        public List<Double>? Center { get; set; }
    }
}
