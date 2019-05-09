namespace WeatherAtZip.Model
{
    public class WeatherMessage
    {
        public string message { get; private set; }
        public string city { get; private set; }
        public string zipcode { get; private set; }
        public double temperature { get; private set; }
        public string timezone { get; private set; }
        public double elevation { get; private set; }

        public WeatherMessage(WeatherData weatherData)
        {
            this.city = weatherData.city;
            this.zipcode = weatherData.zipcode;
            this.temperature = double.Parse(weatherData.temperature);
            this.timezone = weatherData.timezone;
            this.elevation = double.Parse(weatherData.elevation);

            this.message = string.Format(
                    "At the location {0}, the temperature is {1:F1}F, the timezone is {2}, and the elevation is {3:F1}m",
                    weatherData.city,
                    double.Parse(weatherData.temperature),
                    weatherData.timezone,
                    double.Parse(weatherData.elevation));
        }
    }
}