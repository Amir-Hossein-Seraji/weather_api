using System.Text.Json.Serialization;

namespace WeatherApi.Models;

public class Coordinates
{
    public double Lat { get; set; }
    public double Lon { get; set; }
}

public class Pollutants
{
    [JsonPropertyName("pm2_5")]
    public double Pm2_5 { get; set; }
    public double Co { get; set; }
    public double No2 { get; set; }
}