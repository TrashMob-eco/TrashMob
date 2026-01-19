namespace TrashMob.Shared.Poco
{
    using System;

    /// <summary>
    /// Represents a request to delete a user from Active Directory.
    /// </summary>
    public class ActiveDirectoryDeleteUserRequest
    {
        /// <summary>
        /// Gets or sets the unique object identifier of the user to delete.
        /// </summary>
        public Guid objectId { get; set; }
    }
}