namespace TrashMob.Models.Poco
{
    using System;

    /// <summary>
    /// Represents user information for display purposes.
    /// </summary>
    public class DisplayUser
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user's display name.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's city.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's region or state.
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's country.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the user became a member.
        /// </summary>
        public DateTimeOffset? MemberSince { get; set; }
    }
}