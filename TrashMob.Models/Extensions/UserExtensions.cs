namespace TrashMob.Models.Extensions
{
    using System;

    /// <summary>
    /// Extension methods for the <see cref="User"/> entity that produce
    /// display-safe representations of user identity.
    /// </summary>
    /// <remarks>
    /// Minors (users with <c>IsMinor == true</c>) must not have their full
    /// name or contact information rendered to other users. These helpers
    /// centralise the masking rules so every V2 DTO mapping applies the
    /// same policy. See Project 23 (Parental Consent).
    /// </remarks>
    public static class UserExtensions
    {
        /// <summary>
        /// Returns a display-safe user name for the given user. For minors,
        /// this is "FirstName L." (first name + last initial). For adults
        /// and anonymous callers, this is the raw <see cref="User.UserName"/>.
        /// </summary>
        /// <param name="user">The user to render. May be <c>null</c>.</param>
        /// <returns>
        /// The masked display name for a minor, or the raw user name for an
        /// adult. Returns <see cref="string.Empty"/> when <paramref name="user"/>
        /// is <c>null</c>.
        /// </returns>
        public static string DisplayUserName(this User user)
        {
            if (user == null)
            {
                return string.Empty;
            }

            if (!user.IsMinor)
            {
                return user.UserName ?? string.Empty;
            }

            // Prefer GivenName + Surname initial when we have the structured fields.
            if (!string.IsNullOrWhiteSpace(user.GivenName))
            {
                var lastInitial = !string.IsNullOrWhiteSpace(user.Surname)
                    ? $" {user.Surname[0]}."
                    : string.Empty;
                return $"{user.GivenName}{lastInitial}";
            }

            // Fall back to splitting the raw UserName on the first space so we can
            // still mask legacy accounts that never populated GivenName/Surname.
            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                var parts = user.UserName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    return $"{parts[0]} {parts[1][0]}.";
                }

                return parts[0];
            }

            return "TrashMob User";
        }
    }
}
