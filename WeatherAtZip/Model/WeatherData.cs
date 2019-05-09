namespace WeatherAtZip.Model
{
    /// <Summary>
    /// Store all of the data that is used to generate the final message.
    /// </Summary>
    public class WeatherData
    {
        public string zipcode { get; set; }
        public string city { get; set; }
        public string temperature { get; set; }
        public string timezone { get; set; }
        public string elevation { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }
}
