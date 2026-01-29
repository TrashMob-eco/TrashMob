namespace TrashMob.Shared.Poco
{
    /// <summary>
    /// Represents an Active Directory response that blocks the current operation.
    /// </summary>
    public class ActiveDirectoryBlockingResponse : ActiveDirectoryResponseBase
    {
        /// <summary>
        /// Gets or sets the message to display to the user explaining why the operation was blocked.
        /// </summary>
        public string userMessage { get; set; }
    }
}