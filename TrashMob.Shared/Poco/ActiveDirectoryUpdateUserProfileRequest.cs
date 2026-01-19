namespace TrashMob.Shared.Poco
{
    using System;

    /// <summary>
    /// Represents a request to update a user's profile in Active Directory.
    /// </summary>
    public class ActiveDirectoryUpdateUserProfileRequest
    {
        /// <summary>
        /// Gets or sets the unique object identifier of the user to update.
        /// </summary>
        public Guid objectId { get; set; }

        /// <summary>
        /// Gets or sets the new username for the user.
        /// </summary>
        public string userName { get; set; }
    }
}