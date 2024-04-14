namespace TrashMob.Shared.Poco
{
    using TrashMob.Models;

    public static class PocoExtensions
    {
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
                Region = user.Region
            };
        }

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
                IsEventPublic = mobEvent.IsEventPublic,
                CreatedByUserId = mobEvent.CreatedByUserId,
                CreatedDate = mobEvent.CreatedDate,
                LastUpdatedByUserId = mobEvent.LastUpdatedByUserId,
                LastUpdatedDate = mobEvent.LastUpdatedDate,
                CreatedByUserName = userName,
            };
        }
    }
}
