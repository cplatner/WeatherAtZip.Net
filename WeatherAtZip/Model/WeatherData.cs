namespace WeatherAtZip.Model
{
    /// <Summary>
    /// Store all of the data that is used to generate the final message.
    /// </Summary>
    public class WeatherData
    {
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string Temperature { get; set; }
        public string Timezone { get; set; }
        public string Elevation { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
