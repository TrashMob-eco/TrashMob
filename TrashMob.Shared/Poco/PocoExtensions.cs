namespace TrashMob.Shared.Poco
{
    using System;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Contains extension methods for converting domain models to display POCOs.
    /// </summary>
    public static class PocoExtensions
    {
        /// <summary>
        /// Converts a <see cref="User"/> entity to a <see cref="DisplayUser"/> POCO.
        /// </summary>
        /// <param name="user">The user entity to convert.</param>
        /// <returns>A <see cref="DisplayUser"/> containing the user's display information.</returns>
        public static DisplayUser ToDisplayUser(this User user)
        {
            var displayUser = new DisplayUser
            {
                City = user.City,
                Country = user.Country,
                UserName = user.UserName,
                Email = user.Email,
                Id = user.Id,
                MemberSince = user.MemberSince,
                Region = user.Region,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                IsMinor = user.IsMinor,
            };

            if (user.IsMinor)
            {
                displayUser.UserName = MaskMinorName(user);
                displayUser.Email = string.Empty;
            }

            return displayUser;
        }

        /// <summary>
        /// Masks a minor's name to "FirstName L." format for privacy.
        /// </summary>
        private static string MaskMinorName(User user)
        {
            // Prefer GivenName/Surname if available
            if (!string.IsNullOrWhiteSpace(user.GivenName))
            {
                var lastInitial = !string.IsNullOrWhiteSpace(user.Surname) ? $" {user.Surname[0]}." : string.Empty;
                return $"{user.GivenName}{lastInitial}";
            }

            // Fall back to splitting UserName
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

        /// <summary>
        /// Converts an <see cref="Event"/> entity to a <see cref="DisplayEvent"/> POCO.
        /// </summary>
        /// <param name="mobEvent">The event entity to convert.</param>
        /// <param name="userName">The username of the event creator to include in the display object.</param>
        /// <returns>A <see cref="DisplayEvent"/> containing the event's display information.</returns>
        public static DisplayEvent ToDisplayEvent(this Event mobEvent, string userName)
        {
            return new DisplayEvent
            {
                Id = mobEvent.Id,
                Name = mobEvent.Name,
                Description = mobEvent.Description,
                EventDate = mobEvent.EventDate,
                DurationHours = mobEvent.DurationHours,
                DurationMinutes = mobEvent.DurationMinutes,
                EventTypeId = mobEvent.EventTypeId,
                EventStatusId = mobEvent.EventStatusId,
                StreetAddress = mobEvent.StreetAddress,
                City = mobEvent.City,
                Region = mobEvent.Region,
                Country = mobEvent.Country,
                PostalCode = mobEvent.PostalCode,
                Latitude = mobEvent.Latitude,
                Longitude = mobEvent.Longitude,
                MaxNumberOfParticipants = mobEvent.MaxNumberOfParticipants,
                EventVisibilityId = mobEvent.EventVisibilityId,
                TeamId = mobEvent.TeamId,
                CreatedByUserId = mobEvent.CreatedByUserId,
                CreatedDate = mobEvent.CreatedDate,
                LastUpdatedByUserId = mobEvent.LastUpdatedByUserId,
                LastUpdatedDate = mobEvent.LastUpdatedDate,
                CreatedByUserName = userName,
            };
        }
    }
}