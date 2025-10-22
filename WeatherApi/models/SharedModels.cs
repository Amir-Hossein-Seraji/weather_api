using System.Text.Json.Serialization;

namespace WeatherApi.Models;

public class Coordination
{
    public double Latitude { get; set; }
    public double Longtitude { get; set; }
}

public class Pollutants
{
    [JsonPropertyName("pm2_5")]
    public double Pm2_5 { get; set; }
    public double Co { get; set; }
    public double No2 { get; set; }
}