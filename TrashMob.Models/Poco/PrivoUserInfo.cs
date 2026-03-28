namespace TrashMob.Models.Poco
{
    using System.Collections.Generic;

    /// <summary>
    /// Response from PRIVO /accounts API (Section 4).
    /// Contains feature states and verified attribute data.
    /// </summary>
    public class PrivoUserInfo
    {
        /// <summary>
        /// Gets or sets the PRIVO Service Identifier (SiD).
        /// </summary>
        public string Sid { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the feature states and their approval status.
        /// </summary>
        public Dictionary<string, string> Features { get; set; } = [];

        /// <summary>
        /// Gets or sets the verified attribute values.
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = [];
    }
}
