namespace TrashMob.Shared.Poco
{
    using System;

    /// <summary>
    /// Represents a request to create a new user in Active Directory.
    /// </summary>
    public class ActiveDirectoryNewUserRequest
    {
        /// <summary>
        /// Gets or sets the email address of the new user.
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// Gets or sets the unique object identifier for the new user.
        /// </summary>
        public Guid objectId { get; set; }

        /// <summary>
        /// Gets or sets the username for the new user.
        /// </summary>
        public string userName { get; set; }
    }
}