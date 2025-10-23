using WeatherApi.Models;
using WeatherApi.Models.DTOs;
using WeatherApi.Models.External;
using Microsoft.Extensions.Caching.Memory;


namespace WeatherApi;


public interface IWeatherService
{
    Task<EnvironmentalDataResponse> GetEnvironmentalDataAsync(string city);
}

public class WeatherService : IWeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;
    private readonly IMemoryCache _cache;

    public WeatherService(IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IMemoryCache memoryCache) 
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = configuration["OpenWeather:ApiKey"] 
                  ?? throw new ArgumentNullException(nameof(configuration), "OpenWeather:ApiKey is not configured.");
        _cache = memoryCache; 
    }
    
    public async Task<EnvironmentalDataResponse> GetEnvironmentalDataAsync(string city)
    {
        var cacheKey = $"weather_{city.ToLowerInvariant()}";

        if (_cache.TryGetValue(cacheKey, out EnvironmentalDataResponse cachedData))
        {
            return cachedData;
        }
        
        var httpClient = _httpClientFactory.CreateClient();

        var weatherData = await OpenWeatherApiResponse(city, httpClient);
        var airPollutionData = await AirPollutionResponse(weatherData, httpClient);
        var airData = airPollutionData.List[0];
        // MAP TO OUR DTO (Data Transfer Object)
        var finalResponse = new EnvironmentalDataResponse
        {
            City = weatherData.Name,
            TemperatureC = weatherData.Main.TemperatureByCelsius,
            Humidity = weatherData.Main.Humidity,
            WindSpeed = weatherData.Wind.SpeedByMeterPerSecond,
            Coords = weatherData.Coords, 
            AirQuality = new AirQualityData
            {
                AirQualityIndex = airData.AqiData.AirQualityIndex,
                Pollutants = airData.Pollutants 
            }
        };
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(15)); // Cache for 15 mins

        _cache.Set(cacheKey, finalResponse, cacheOptions);
        
        return finalResponse;
    }

    private async Task<AirPollutionResponse?> AirPollutionResponse(OpenWeatherApiResponse? weatherData, HttpClient httpClient)
    {
        var lat = weatherData.Coords.Latitude;
        var lon = weatherData.Coords.Longitude;
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

        return airPollutionData;
    }

    private async Task<OpenWeatherApiResponse?> OpenWeatherApiResponse(string city, HttpClient httpClient)
    {
        var weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
        var weatherResponse = await httpClient.GetAsync(weatherUrl);
        if (!weatherResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Could not retrieve weather for {city}.");
        }
        var weatherData = await weatherResponse.Content.ReadFromJsonAsync<OpenWeatherApiResponse>();
        if (weatherData == null)
        {
            throw new Exception("Failed to deserialize weather data.");
        }

        return weatherData;
    }
}