namespace Rhythm.Net.Core
{

    // Namespaces.
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;
    using Types;

    /// <summary>
    /// Assists with operations related to networking.
    /// </summary>
    public class NetworkHelper
    {

        #region Constants

        private const string DefaultUserAgent = ".Net Server-Side Client";

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
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.UserAgent = DefaultUserAgent;
            var response = (HttpWebResponse)request.GetResponse();
            var responseStream = response.GetResponseStream();
            var reader = new StreamReader(responseStream);
            return reader.ReadToEnd();
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
            string method, bool sendInBody, SendDataOptions options = null)
        {

            // Construct a URL, possibly containing the data as query string parameters.
            var sendInUrl = !sendInBody;
            var sendDataResult = new SendDataResult();
            var uri = new Uri(url);
            var bareUrl = uri.GetLeftPart(UriPartial.Path);
            var strQueryString = ConstructQueryString(uri, data);
            var hasQueryString = !string.IsNullOrWhiteSpace(strQueryString);
            var requestUrl = hasQueryString && sendInUrl
                ? $"{bareUrl}?{strQueryString}"
                : url;

            // Attempt to send the web request.
            try
            {

                // Construct web request.
                var request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.AllowAutoRedirect = false;
                request.UserAgent = DefaultUserAgent;
                request.Method = method;

                // Add headers?
                if (options?.Headers != null)
                {
                    foreach (var header in options.Headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                // Send the data in the body (rather than the query string)?
                if (sendInBody)
                {
                    var postBytes = Encoding.UTF8.GetBytes(strQueryString);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = postBytes.Length;
                    var postStream = request.GetRequestStream();
                    postStream.Write(postBytes, 0, postBytes.Length);
                }

                // Get and retain response.
                var response = (HttpWebResponse)request.GetResponse();
                sendDataResult.HttpWebResponse = response;
                var responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream);
                var resultText = reader.ReadToEnd();
                sendDataResult.ResponseText = resultText;
                sendDataResult.Success = true;

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
        private static string ConstructQueryString(Uri uri, IDictionary<string, string> data)
        {
            var queryString = HttpUtility.ParseQueryString(uri.Query);
            foreach (var pair in data)
            {
                queryString.Set(pair.Key, pair.Value);
            }
            var strQueryString = queryString.ToString();
            return strQueryString;
        }

        #endregion

    }

}