namespace Rhythm.Net.Core
{

    // Namespaces.
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Rhythm.Core;
    using System.Device.Location;
    using System.Linq;
    using System.Net;
    using System.Web.Configuration;

    /// <summary>
    /// Assists with operations related to geography (e.g., geocoding).
    /// </summary>
    public class GeographyHelper
    {

        #region Constants

        private const string GeocodeUrlFormat = "https://maps.googleapis.com/maps/api/geocode/json?key={0}&address={1}";

        #endregion

        #region Methods

        /// <summary>
        /// Returns the coordinate for an address.
        /// </summary>
        /// <param name="address1">
        /// The first address line.
        /// </param>
        /// <param name="address2">
        /// The second address line.
        /// </param>
        /// <param name="city">
        /// The address city.
        /// </param>
        /// <param name="state">
        /// The address state.
        /// </param>
        /// <param name="country">
        /// The address country.
        /// </param>
        /// <param name="postalCode">
        /// The address postal code.
        /// </param>
        /// <returns>
        /// The coordinate, or null if one could not be determined.
        /// </returns>
        /// <remarks>
        /// Your web.config should have an app setting called "GoogleMapsApiServerKey".
        /// This is the key that will be used to tell Google about your account.
        /// </remarks>
        public static GeoCoordinate GeocodeAddress(string address1, string address2, string city,
            string state, string country, string postalCode)
        {

            // Combine the address parts into a single address.
            var addressParts = new[]
            {
                address1, address2,
                city, state, postalCode,
                country
            }
            .WithoutNulls().ToArray();
            var address = string.Join(", ", addressParts);

            // Variables.
            var mapsKey = WebConfigurationManager.AppSettings["GoogleMapsApiServerKey"];
            var encodedKey = WebUtility.UrlEncode(mapsKey);
            var encodedAddress = WebUtility.UrlEncode(address);
            var url = string.Format(GeocodeUrlFormat, encodedKey, encodedAddress);

            // Get response from Google's geocoding service.
            var response = NetworkHelper.GetResponse(url);

            // Extract coordinates from Google's response.
            var result = JsonConvert.DeserializeObject(response) as dynamic;
            var firstResult = (result["results"] as JArray).MakeSafe().FirstOrDefault() as dynamic;
            var location = firstResult?.geometry?.location as dynamic;
            var latitude = location?.lat?.Value as double?;
            var longitude = location?.lng?.Value as double?;

            // Return the coordinate.
            return latitude.HasValue && longitude.HasValue
                ? new GeoCoordinate(latitude.Value, longitude.Value)
                : null;

        }

        #endregion

    }

}