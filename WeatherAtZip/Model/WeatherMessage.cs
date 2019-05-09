namespace WeatherAtZip.Model
{
    public class WeatherMessage
    {
        public string Message { get; private set; }
        public string City { get; private set; }
        public string Zipcode { get; private set; }
        public double Temperature { get; private set; }
        public string Timezone { get; private set; }
        public double Elevation { get; private set; }

        public WeatherMessage(WeatherData weatherData)
        {
            this.City = weatherData.City;
            this.Zipcode = weatherData.Zipcode;
            this.Temperature = double.Parse(weatherData.Temperature);
            this.Timezone = weatherData.Timezone;
            this.Elevation = double.Parse(weatherData.Elevation);

            this.Message = string.Format(
                    "At the location {0}, the temperature is {1:F1}F, the timezone is {2}, and the elevation is {3:F1}m",
                    weatherData.City,
                    double.Parse(weatherData.Temperature),
                    weatherData.Timezone,
                    double.Parse(weatherData.Elevation));
        }
    }
}