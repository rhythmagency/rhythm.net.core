namespace Rhythm.Net.Core.Types
{

    // Namespaces.
    using System.Collections.Generic;

    /// <summary>
    /// The options that can be passed to the NetworkHelper.SendData method.
    /// </summary>
    public class SendDataOptions
    {

        #region Properties

        /// <summary>
        /// The HTTP headers to send with the request.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        #endregion

    }

}