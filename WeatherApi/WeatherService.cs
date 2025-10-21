using WeatherApi.Models;
using WeatherApi.Models.DTOs;
using WeatherApi.Models.External;


namespace WeatherApi;


public interface IWeatherService
{
    Task<EnvironmentalDataResponse> GetEnvironmentalDataAsync(string city);
}

public class WeatherService : IWeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;


    public WeatherService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;

        _apiKey = configuration["OpenWeather:ApiKey"]
                  ?? throw new ArgumentNullException(nameof(configuration), "OpenWeather:ApiKey is not configured.");
    }

    public async Task<EnvironmentalDataResponse> GetEnvironmentalDataAsync(string city)
    {

        var httpClient = _httpClientFactory.CreateClient();

        var weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
        var weatherResponse = await httpClient.GetAsync(weatherUrl);
        if (!weatherResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Could not retrieve weather for {city}.");
        }
        var weatherData = await weatherResponse.Content.ReadFromJsonAsync<WeatherApiResponse>();
        if (weatherData == null)
        {
            throw new Exception("Failed to deserialize weather data.");
        }
        var lat = weatherData.Coords.Lat;
        var lon = weatherData.Coords.Lon;
        var airPollutionUrl = $"https://api.openweathermap.org/data/2.5/air_pollution?lat={lat}&lon={lon}&appid={_apiKey}";
        var airPollutionResponse = await httpClient.GetAsync(airPollutionUrl);
        if (!airPollutionResponse.IsSuccessStatusCode)
        {
            throw new Exception("Could not retrieve air pollution data.");
        }
        var airPollutionData = await airPollutionResponse.Content.ReadFromJsonAsync<AirPollutionResponse>();
        if (airPollutionData == null || airPollutionData.List.Count == 0)
        {
            throw new Exception("Failed to deserialize air pollution data.");
        }
        var airData = airPollutionData.List[0];
        // MAP TO OUR DTO (Data Transfer Object)
        var finalResponse = new EnvironmentalDataResponse
        {
            City = weatherData.Name,
            TemperatureC = weatherData.Main.Temperature,
            Humidity = weatherData.Main.Humidity,
            WindSpeed = weatherData.Wind.Speed,
            Coords = weatherData.Coords, 
            AirQuality = new AirQualityData
            {
                Aqi = airData.AqiData.Aqi,
                Pollutants = airData.Pollutants 
            }
        };

        return finalResponse;
    }
}