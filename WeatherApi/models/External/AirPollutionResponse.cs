using System.Text.Json.Serialization;
using WeatherApi.Models;

namespace WeatherApi.Models.External;

public class AirPollutionResponse
{
    public List<AirPollutionList> List { get; set; } = new();
}

public class AirPollutionList
{
    [JsonPropertyName("main")]
    public AirQualityMain AqiData { get; set; } = new();
    [JsonPropertyName("components")]
    public Pollutants Pollutants { get; set; } = new();
}

public class AirQualityMain
{
    public int Aqi { get; set; }
}