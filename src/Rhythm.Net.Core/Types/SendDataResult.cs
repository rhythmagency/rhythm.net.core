namespace Rhythm.Net.Core.Types
{

    // Namespaces.
    using System;
    using System.Net.Http;

    /// <summary>
    /// Stores the result of an attempt to send data over a network.
    /// </summary>
    public class SendDataResult
    {

        #region Properties

        /// <summary>
        /// The HTTP response message.
        /// </summary>
        public HttpResponseMessage HttpResponseMessage { get; set; }

        /// <summary>
        /// The response text.
        /// </summary>
        public string ResponseText { get; set; }

        /// <summary>
        /// The response error, if one occurs.
        /// </summary>
        public Exception ResponseError { get; set; }

        /// <summary>
        /// Was the request a success?
        /// </summary>
        public bool Success { get; set; }

        #endregion

    }

}