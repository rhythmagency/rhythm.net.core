namespace Rhythm.Net.Core
{

    // Namespaces.
    using System.IO;
    using System.Net;

    /// <summary>
    /// Assists with operations related to networking.
    /// </summary>
    public class NetworkHelper
    {

        #region Constants

        private const string DEFAULT_USER_AGENT = ".Net Server-Side Client";

        #endregion

        #region Methods

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
            request.UserAgent = DEFAULT_USER_AGENT;
            var response = (HttpWebResponse)request.GetResponse();
            var responseStream = response.GetResponseStream();
            var reader = new StreamReader(responseStream);
            return reader.ReadToEnd();
        }

        #endregion

    }

}