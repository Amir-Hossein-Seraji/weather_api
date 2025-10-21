using System.Text.Json.Serialization ;
namespace WeatherApi;
// This is the FINAL response your API will send
public class EnvironmentalDataResponse
{
    public string City { get; set; }
    public double TemperatureC { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public Coordinates Coords { get; set; }
    public AirQualityData AirQuality { get; set; }
    public class Coordinates
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
    public class AirQualityData
    {
        [JsonPropertyName("aqi")]
        public int Aqi { get; set; }
        public Pollutants Pollutants { get; set; }
    }
    public class Pollutants
    {
        [JsonPropertyName("pm2_5")]
        public double Pm2_5 { get; set; }
        public double Co { get; set; }
        public double No2 { get; set; }
    }
    // Model for the /weather endpoint
    public class WeatherApiResponse
    {
        public string Name { get; set; }
        public MainData Main { get; set; }
        public WindData Wind { get; set; }
        [JsonPropertyName("coord")]
        public Coordinates Coords { get; set; }
    }
    public class MainData
    {
        [JsonPropertyName("temp")]
        public double Temperature { get; set; }
        public int Humidity { get; set; }
    }

    public class WindData
    {
        public double Speed { get; set; }
    }
    // Model for the /air_pollution endpoint
    public class AirPollutionResponse
    {
        public List<AirPollutionList> List { get; set; }
    }
    public class AirPollutionList
    {
        [JsonPropertyName("main")]
        public AirQualityMain AqiData { get; set; }
        [JsonPropertyName("components")]
        public Pollutants Pollutants { get; set; }
    }
    public class AirQualityMain
    {
        public int Aqi { get; set; }
    }
}
