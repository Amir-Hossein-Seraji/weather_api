using System.Text.Json.Serialization;
using WeatherApi.Models;

namespace WeatherApi.Models.DTOs;


public class EnvironmentalDataResponse
{
    public string City { get; set; } = string.Empty;
    public double TemperatureC { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public Coordinates Coords { get; set; } = new();
    public AirQualityData AirQuality { get; set; } = new();
}
public class AirQualityData
{
    [JsonPropertyName("aqi")]
    public int Aqi { get; set; }
    public Pollutants Pollutants { get; set; } = new();
}