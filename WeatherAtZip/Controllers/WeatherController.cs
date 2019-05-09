using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherAtZip.Model;

namespace WeatherAtZip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private static ILogger _logger;
        private static IConfiguration _config;

        /// <summary>
        /// Cache for timezones, since they don't change between calls
        /// </summary>
        private Dictionary<string, string> _timezonesByZip = new Dictionary<string, string>();
        /// <summary>
        /// Cache for elevations, since they don't change between calls
        /// </summary>
        private readonly Dictionary<string, string> _elevationsByZip = new Dictionary<string, string>();


        public WeatherController(ILogger<WeatherController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<WeatherMessage>> Get(string zipcode)
        {
            WeatherData data = new WeatherData();
            try {
                validateZipcode(zipcode);

                data.Zipcode = zipcode;
                await getWeather(zipcode, data).ConfigureAwait(false);
                await getTimezone(zipcode, data).ConfigureAwait(false);
                await getElevation(zipcode, data).ConfigureAwait(false);
            }
            catch (WeatherRestException e) {
                return e.InternalEntity;
            }

            return new WeatherMessage(data);
        }


        private async Task getWeather(string zipcode, WeatherData data)
        {
            string weatherApiUri = _config.GetValue<string>("Api:WeatherApi:Url");
            string weatherApiAppId = _config.GetValue<string>("Api:WeatherApi:AppId");
            ActionResult response = await getFromExternalService(string.Format(weatherApiUri, zipcode, weatherApiAppId)).ConfigureAwait(false);

            try {

                JObject root = JObject.Parse((string)(response as JsonResult).Value);

                data.Temperature = root["main"].Value<string>("temp");
                data.City = root.Value<string>("name");
                data.Latitude = root["coord"].Value<string>("lat");
                data.Longitude = root["coord"].Value<string>("lon");

            }
            catch (JsonReaderException e) {
                throw new WeatherRestException(
                    StatusCode(500, "Error parsing data from temperature server"));
            }
        }


        private async Task getTimezone(string zipcode, WeatherData data)
        {
            string timezoneApiUri = _config.GetValue<string>("Api:TimezoneApi:Url");
            string timezoneApiAppId = _config.GetValue<string>("Api:TimezoneApi:AppId");

            if (_timezonesByZip.ContainsKey(zipcode)) {
                data.Timezone = _timezonesByZip[zipcode];
            }
            else {
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                ActionResult response = await getFromExternalService(
                   string.Format(timezoneApiUri,
                            data.Latitude, data.Longitude, (long)t.TotalSeconds, timezoneApiAppId)).ConfigureAwait(false);

                try {
                    JObject root = JObject.Parse((string)(response as JsonResult).Value);
                    data.Timezone = root.Value<string>("timeZoneName");
                    _timezonesByZip[zipcode] = data.Timezone;
                }
                catch (JsonReaderException e) {
                    throw new WeatherRestException(
                        StatusCode(500, "Error parsing data from timezone server"));
                }
            }
        }


        private async Task getElevation(string zipcode, WeatherData data)
        {
            string elevationApiUri = _config.GetValue<string>("Api:ElevationApi:Url");
            string elevationApiAppId = _config.GetValue<string>("Api:ElevationApi:AppId");

            if (_elevationsByZip.ContainsKey(zipcode)) {
                data.Elevation = _elevationsByZip[zipcode];
            }
            else {
                ActionResult response = await getFromExternalService(
                    string.Format(elevationApiUri, data.Latitude, data.Longitude, elevationApiAppId)).ConfigureAwait(false);
                try {
                    JObject root = JObject.Parse((string)(response as JsonResult).Value);
                    data.Elevation = root["results"][0].Value<string>("elevation");
                    _elevationsByZip[zipcode] = data.Elevation;
                }
                catch (JsonReaderException e) {
                    throw new WeatherRestException(
                        StatusCode(500, "Error parsing data from elevation server"));
                }
            }
        }


        private async Task<ActionResult> getFromExternalService(string uri)
        {
            string json = string.Empty;
            
            try {
                using (HttpClient client = new HttpClient()) {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));

                    HttpResponseMessage response = await client.GetAsync(uri).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode) {
                        json = await response.Content.ReadAsStringAsync();
                    }
                    else {
                        _logger.LogError("Can't get values from external service at {0}", uri);
                        throw new WeatherRestException(
                            StatusCode((int)response.StatusCode, "Error getting data from outside service"));
                    }
                }
            }
            catch (HttpRequestException e) {
                _logger.LogError("Can't get values from external service at {0}", uri);
                throw new WeatherRestException(
                    StatusCode(500, "Error getting data from outside service"));
            }

            return new JsonResult(json);
        }


        /// <summary>
        /// Basic checks for valid zip codes
        /// </summary>
        private void validateZipcode(String zipcode)
        {
            if (zipcode == null || zipcode.Length == 0) {
                throw new WeatherRestException(BadRequest("Missing Zip Code"));
            }

            //* check for out-of-country zips
            foreach (char c in zipcode) {
                if (!char.IsDigit(c)) {
                    throw new WeatherRestException(BadRequest("Zip Code has invalid characters"));
                }
            }

            if (zipcode.Length < 5) {
                throw new WeatherRestException(BadRequest("Zip Code is too short"));
            }

            if (zipcode.Length > 5) {
                throw new WeatherRestException(BadRequest("Zip Code is too long"));
            }
        }
    }
}
