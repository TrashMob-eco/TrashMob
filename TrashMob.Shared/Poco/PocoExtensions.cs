namespace TrashMob.Shared.Poco
{
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
            return new DisplayUser
            {
                City = user.City,
                Country = user.Country,
                UserName = user.UserName,
                Email = user.Email,
                Id = user.Id,
                MemberSince = user.MemberSince,
                Region = user.Region,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
            };
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