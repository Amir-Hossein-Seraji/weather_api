using System.Text.Json.Serialization;
using WeatherApi.Models;

namespace WeatherApi.Models.External;

public class OpenWeatherApiResponse
{
    public string Name { get; set; } = string.Empty;
    public MainData Main { get; set; } = new();
    public WindData Wind { get; set; } = new();
    [JsonPropertyName("coord")]
    public Coordination Coords { get; set; } = new();
}

public class MainData
{
    [JsonPropertyName("temp")]
    public double TemperatureByCelsius { get; set; }
    public int Humidity { get; set; }
}

public class WindData
{
    public double SpeedByMeterPerSecond { get; set; }
}