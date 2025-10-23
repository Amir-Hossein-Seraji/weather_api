using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc.Testing; 
using Microsoft.AspNetCore.TestHost; 
using Microsoft.Extensions.DependencyInjection;
using System.Net; 
using System.Text.Json; 
using WeatherApi; 
using WeatherApi.Models;
using WeatherApi.Models.DTOs; 

namespace WeatherApi.Tests;

public class WeatherApiTests
{
    private readonly HttpClient _client;
    private readonly Mock<IWeatherService> _mockWeatherService;


    public WeatherApiTests()
    {
        _mockWeatherService = new Mock<IWeatherService>();
        var applicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IWeatherService>(provider => _mockWeatherService.Object);
                });
            });
        _client = applicationFactory.CreateClient();
    }

    [Fact]
    public async Task GetWeather_ForKnownCity_Tehran_ReturnsOkAndValidData()
    {
        var testCity = "Tehran";
        var fakeData = new EnvironmentalDataResponse
        {
            City = "tehran",
            TemperatureC = 15.0,
            Humidity = 50,
            WindSpeed = 3.5,
            Coords = new Coordination { Latitude = 35.6944, Longitude = 51.4215 },
            AirQuality = new AirQualityData
            {
                AirQualityIndex = 3,
                Pollutants = new Pollutants { Pm2_5 = 10.1, Co = 200.2, No2 = 14.3 }
            }
        };
        _mockWeatherService
            .Setup(s => s.GetEnvironmentalDataAsync(testCity))
            .ReturnsAsync(fakeData);
        
        var response = await _client.GetAsync($"/weather?city={testCity}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var jsonString = await response.Content.ReadAsStringAsync();
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var responseData = JsonSerializer.Deserialize<EnvironmentalDataResponse>(jsonString, options);
        Assert.NotNull(responseData);
        Assert.Equal(testCity, responseData.City, ignoreCase: true);
        Assert.Equal(15.0, responseData.TemperatureC);
        Assert.Equal(3, responseData.AirQuality.AirQualityIndex);
        Assert.Equal(10.1, responseData.AirQuality.Pollutants.Pm2_5);
    }
}