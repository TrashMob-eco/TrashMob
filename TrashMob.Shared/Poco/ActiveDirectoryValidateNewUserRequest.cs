namespace TrashMob.Shared.Poco
{
    /// <summary>
    /// Represents a request to validate a new user before creation in Active Directory.
    /// </summary>
    public class ActiveDirectoryValidateNewUserRequest
    {
        /// <summary>
        /// Gets or sets the email address to validate.
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// Gets or sets the username to validate.
        /// </summary>
        public string userName { get; set; }
    }
}