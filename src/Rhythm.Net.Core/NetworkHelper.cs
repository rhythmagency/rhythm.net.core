namespace Rhythm.Net.Core
{
    // Namespaces.
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.WebUtilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using Types;

    /// <summary>
    /// Assists with operations related to networking.
    /// </summary>
    public class NetworkHelper
    {

        #region Constants

        private const string DefaultUserAgent = ".Net Server-Side Client";

        private static readonly HttpClient _client;

        #endregion

        #region Constructors

        static NetworkHelper()
        {
            var socketsHandler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
            };

            _client = new HttpClient(socketsHandler);

            _client.DefaultRequestHeaders.Add("User-Agent", DefaultUserAgent);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the response string downloaded from the request to the specified URL.
        /// </summary>
        /// <param name="url">
        /// The URL to make the request to.
        /// </param>
        /// <returns>
        /// The response string.
        /// </returns>
        public static string GetResponse(string url)
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                var result = _client.SendAsync(requestMessage).GetAwaiter().GetResult();
                var resultContentString = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return resultContentString;
            }
        }

        /// <summary>
        /// Sends a web request with the data either in the query string or in the body.
        /// </summary>
        /// <param name="url">
        /// The URL to send the request to.
        /// </param>
        /// <param name="data">
        /// The data to send.
        /// </param>
        /// <param name="method">
        /// The HTTP method (e.g., GET, POST) to use when sending the request.
        /// </param>
        /// <param name="sendInBody">
        /// Send the data as part of the body (or in the query string)?
        /// </param>
        /// <param name="options">
        /// Optional. Additional options.
        /// </param>
        /// <returns>
        /// An object containing details about the result of the attempt to send the data.
        /// </returns>
        /// <remarks>
        /// Parts of this function are from: http://stackoverflow.com/a/9772003/2052963
        /// and http://stackoverflow.com/questions/14702902
        /// </remarks>
        public static SendDataResult SendData(string url, IDictionary<string, string> data,
            HttpMethod method, bool sendInBody, SendDataOptions options = null)
        {

            // Construct a URL, possibly containing the data as query string parameters.
            var sendInUrl = !sendInBody;
            var sendDataResult = new SendDataResult();
            var uri = new Uri(url);
            var bareUrl = uri.GetLeftPart(UriPartial.Path);
            var strQueryString = ConstructQueryString(uri, data);
            var hasQueryString = !string.IsNullOrWhiteSpace(strQueryString);
            var requestUrl = hasQueryString && sendInUrl
                ? $"{bareUrl}{strQueryString}"
                : url;

            // Attempt to send the web request.
            try
            {
                // Construct web request.
                using (var requestMessage = new HttpRequestMessage(method, requestUrl))
                {
                    // Add headers?
                    if (options?.Headers != null)
                    {
                        foreach (var header in options.Headers)
                        {
                            requestMessage.Headers.Add(header.Key, header.Value);
                        }
                    }

                    // Send the data in the body (rather than the query string)?
                    if (sendInBody)
                    {
                        requestMessage.Content = new FormUrlEncodedContent(data);
                    }

                    // Get and retain response.
                    var result = _client.SendAsync(requestMessage).GetAwaiter().GetResult();
                    var resultContentString = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    sendDataResult.HttpResponseMessage = result;
                    sendDataResult.ResponseText = resultContentString;
                    sendDataResult.Success = true;
                }
            }
            catch (Exception ex)
            {
                sendDataResult.ResponseError = ex;
                sendDataResult.Success = false;
            }

            // Return the result of the request.
            return sendDataResult;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Constructs a query string from the specified URL and data.
        /// </summary>
        /// <param name="uri">
        /// The URL (potentially containing a query string).
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The query string.
        /// </returns>
        /// <remarks>
        /// Parts are based on this: https://stackoverflow.com/a/43407008
        /// </remarks>
        private static string ConstructQueryString(Uri uri, IDictionary<string, string> data)
        {
            // Parse the existing querystring of the input URI, if any.
            var queryString = QueryHelpers.ParseQuery(uri.Query);
            
            // Convert to a list.
            var items = queryString
                .SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value))
                .ToList();

            // Remove any that are present in the input dictionary.
            items.RemoveAll(x => data.ContainsKey(x.Key));

            // Construct the new querystring.
            var qb = new QueryBuilder(items);
            foreach (var pair in data)
            {
                qb.Add(pair.Key, pair.Value);
            }

            return qb.ToString();
        }

        #endregion

    }

}