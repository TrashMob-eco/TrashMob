namespace TrashMob.Shared.Poco
{
    /// <summary>
    /// Represents the base class for Active Directory API responses.
    /// </summary>
    public abstract class ActiveDirectoryResponseBase
    {
        /// <summary>
        /// Gets or sets the API version of the response.
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// Gets or sets the action to be taken based on the response.
        /// </summary>
        public string action { get; set; }
    }
}