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
                    "At the location %s, the temperature is %.1fF, the timezone is %s, and the elevation is %.1fm",
                    weatherData.city,
                    double.Parse(weatherData.temperature),
                    weatherData.timezone,
                    double.Parse(weatherData.elevation));
        }
    }
}