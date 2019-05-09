using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WeatherAtZip.IntegrationTests
{
    public class RestIntegrationTest
    {
        private HttpClient _restClient;

        public RestIntegrationTest()
        {
            TestServer server = new TestServer(
                new WebHostBuilder()
                    .UseEnvironment("Development")
                    .UseStartup<Startup>());

            _restClient = server.CreateClient();
        }

        [Fact]
        public async Task InvalidZip_noZip_failsWithBadRequest()
        {
            HttpResponseMessage response = await _restClient.GetAsync("/api/weather");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task InvalidZip_tooShortZip_failsWithBadRequest()
        {
            HttpResponseMessage response = await _restClient.GetAsync("/api/weather?zipcode=9700");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// This zip is somewhere in Boston, but the OpenWeatherMap API doesn't seem to support Zip+4.
        /// </summary>
        [Fact]
        public async Task InvalidZip_tooLongZip_failsWithBadRequest()
        {
            HttpResponseMessage response = await _restClient.GetAsync("/api/weather?zipcode=02129-3011");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Post code for London without country identifier.
        /// </summary>
        [Fact]
        public async Task InvalidZip_nonUsZip_failsWithBadRequest()
        {
            HttpResponseMessage response = await _restClient.GetAsync("/api/weather?zipcode=WC2N+5DU");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// This test has a zip code that is a valid length, but doesn't exist on the weather server.
        /// </summary>
        [Fact]
        public async Task InvalidZip_zipDoesntExist_failsWithNotFound()
        {
            HttpResponseMessage response = await _restClient.GetAsync("/api/weather?zipcode=90000");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Valid_5DigitZip_succeeds_1()
        {
            HttpResponseMessage response = await _restClient.GetAsync("/api/weather?zipcode=97006");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string json = await response.Content.ReadAsStringAsync();
            JObject root = JObject.Parse(json);
            Assert.Equal("Portland", root.Value<string>("city"));
            Assert.StartsWith("Pacific", root.Value<string>("timezone"));
            Assert.Equal(67, Math.Round(root.Value<double>("elevation")));
        }

        [Fact]
        public async Task Valid_5DigitZip_succeeds_2()
        {
            //* Somewhere in Boston
            HttpResponseMessage response = await _restClient.GetAsync("/api/weather?zipcode=02129");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string json = await response.Content.ReadAsStringAsync();
            JObject root = JObject.Parse(json);
            Assert.Equal("Boston", root.Value<string>("city"));
            Assert.StartsWith("Eastern", root.Value<string>("timezone"));
            Assert.Equal(11, Math.Round(root.Value<double>("elevation")));
        }
    }
}
