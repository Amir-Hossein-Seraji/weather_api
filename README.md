# Okala - Weather API Challenge

This is a .NET Web API that fulfills the Okala backend developer challenge. It provides current environmental data for a given city by fetching and combining data from the OpenWeatherMap API.

## Features

* **GET /weather** endpoint to retrieve data for a city.
* Combines data from `/weather` and `/air_pollution` endpoints.
* Clean, service-based architecture.
* Includes an integration test that mocks the external API.

## How to Run

1.  ### Prerequisites
    * .NET 9 SDK (or your current .NET SDK version)
    * A free API key from [OpenWeatherMap](https://openweathermap.org/)

2.  ### Clone the Repository
    ```sh
    git clone [https://github.com/Amir-Hossein-Seraji/weather_api.git](https://github.com/Amir-Hossein-Seraji/weather_api.git)
    cd weather_api
    ```

3.  ### Configure Your API Key
    This project uses .NET User Secrets to protect the API key. You must add your own key:

    ```sh
    # Navigate into the main API project
    cd WeatherApi

    # Initialize user secrets for the project
    dotnet user-secrets init

    # Set your API key
    dotnet user-secrets set "OpenWeather:ApiKey" "YOUR_OPENWEATHERMAP_API_KEY_GOES_HERE"
    ```

4.  ### Run the Application
    ```sh
    # Go back to the root folder
    cd ..

    # Run the API project
    dotnet run --project WeatherApi
    ```

5.  ### Test the API
    Once the app is running, open your browser and go to the Swagger UI:
    **`http://localhost:5123/swagger`** *(Your port might be different. The terminal will tell you which port it is using.)*

## How to Run the Test

To run the unit test:

```sh
dotnet test
