using WeatherApi;
using WeatherApi.Models;
using WeatherApi.Models.DTOs;
using WeatherApi.Models.External;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("WeatherApi.Tests")]

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddScoped<IWeatherService, WeatherService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseHttpsRedirection();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/weather", async (string city, IWeatherService service) =>
{
    if (string.IsNullOrWhiteSpace(city))
    {
        return Results.BadRequest("City name is required.");
    }
    try
    {
        var data = await service.GetEnvironmentalDataAsync(city);
        
        return Results.Ok(data);
    }
    catch (Exception ex)
    {
        return Results.NotFound($"Data not found for {city}.");
    }
})
.WithName("GetWeatherByCity")
.WithDescription("Gets current environmental data for a specified city.")
.WithOpenApi();

app.Run();

public partial class Program { }