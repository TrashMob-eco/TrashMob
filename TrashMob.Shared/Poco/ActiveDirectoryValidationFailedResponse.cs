namespace TrashMob.Shared.Poco
{
    /// <summary>
    /// Represents an Active Directory response indicating that validation has failed.
    /// </summary>
    public class ActiveDirectoryValidationFailedResponse : ActiveDirectoryResponseBase
    {
        /// <summary>
        /// Gets or sets the message to display to the user explaining the validation failure.
        /// </summary>
        public string userMessage { get; set; }

        /// <summary>
        /// Gets or sets the status code or description of the validation failure.
        /// </summary>
        public string status { get; set; }
    }
}