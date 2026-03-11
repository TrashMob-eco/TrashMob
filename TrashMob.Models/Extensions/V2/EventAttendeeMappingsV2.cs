namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping EventAttendee entities to V2 DTOs.
    /// </summary>
    public static class EventAttendeeMappingsV2
    {
        /// <summary>
        /// Maps an EventAttendee entity to a V2 EventAttendeeDto, including basic user info.
        /// Requires the User navigation property to be loaded.
        /// </summary>
        /// <param name="entity">The EventAttendee entity to map.</param>
        /// <returns>An EventAttendeeDto with attendee and user info.</returns>
        public static EventAttendeeDto ToV2Dto(this EventAttendee entity)
        {
            return new EventAttendeeDto
            {
                UserId = entity.UserId,
                UserName = entity.User?.UserName ?? string.Empty,
                GivenName = entity.User?.GivenName ?? string.Empty,
                ProfilePhotoUrl = entity.User?.ProfilePhotoUrl ?? string.Empty,
                SignUpDate = entity.SignUpDate,
                IsEventLead = entity.IsEventLead,
            };
        }

        /// <summary>
        /// Maps a V2 <see cref="EventAttendeeDto"/> back to an <see cref="EventAttendee"/> entity.
        /// Note: EventId is not present in the DTO and must be set separately.
        /// </summary>
        public static EventAttendee ToEntity(this EventAttendeeDto dto)
        {
            return new EventAttendee
            {
                UserId = dto.UserId,
                SignUpDate = dto.SignUpDate,
                IsEventLead = dto.IsEventLead,
            };
        }
    }
}
