namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using TrashMob.Models;

    /// <summary>
    /// Builder for creating Event test data with sensible defaults.
    /// </summary>
    public class EventBuilder
    {
        private readonly Event _event;

        public EventBuilder()
        {
            var creatorId = Guid.NewGuid();
            _event = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Test Cleanup Event",
                Description = "A test cleanup event for unit testing",
                EventDate = DateTimeOffset.UtcNow.AddDays(7),
                DurationHours = 2,
                DurationMinutes = 0,
                EventTypeId = 1, // Cleanup
                EventStatusId = 1, // Active
                StreetAddress = "123 Test Street",
                City = "Seattle",
                Region = "WA",
                Country = "United States",
                PostalCode = "98101",
                Latitude = 47.6062,
                Longitude = -122.3321,
                MaxNumberOfParticipants = 20,
                EventVisibilityId = (int)EventVisibilityEnum.Public,
                CreatedByUserId = creatorId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = creatorId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };
        }

        public EventBuilder WithId(Guid id)
        {
            _event.Id = id;
            return this;
        }

        public EventBuilder WithName(string name)
        {
            _event.Name = name;
            return this;
        }

        public EventBuilder WithDescription(string description)
        {
            _event.Description = description;
            return this;
        }

        public EventBuilder WithEventDate(DateTimeOffset eventDate)
        {
            _event.EventDate = eventDate;
            return this;
        }

        public EventBuilder InThePast()
        {
            _event.EventDate = DateTimeOffset.UtcNow.AddDays(-7);
            return this;
        }

        public EventBuilder InTheFuture(int days = 7)
        {
            _event.EventDate = DateTimeOffset.UtcNow.AddDays(days);
            return this;
        }

        public EventBuilder WithDuration(int hours, int minutes = 0)
        {
            _event.DurationHours = hours;
            _event.DurationMinutes = minutes;
            return this;
        }

        public EventBuilder WithEventType(int eventTypeId)
        {
            _event.EventTypeId = eventTypeId;
            return this;
        }

        public EventBuilder WithStatus(int statusId)
        {
            _event.EventStatusId = statusId;
            return this;
        }

        public EventBuilder AsActive()
        {
            _event.EventStatusId = 1;
            return this;
        }

        public EventBuilder AsCancelled(string reason = "Test cancellation")
        {
            _event.EventStatusId = 3;
            _event.CancellationReason = reason;
            return this;
        }

        public EventBuilder AsCompleted()
        {
            _event.EventStatusId = 4; // Complete status
            _event.EventDate = DateTimeOffset.UtcNow.AddDays(-1);
            return this;
        }

        public EventBuilder AsFull()
        {
            _event.EventStatusId = 2; // Full status
            return this;
        }

        public EventBuilder WithLocation(string city, string region, string country)
        {
            _event.City = city;
            _event.Region = region;
            _event.Country = country;
            return this;
        }

        public EventBuilder InCity(string city)
        {
            _event.City = city;
            return this;
        }

        public EventBuilder InRegion(string region)
        {
            _event.Region = region;
            return this;
        }

        public EventBuilder OnDate(DateTimeOffset date)
        {
            _event.EventDate = date;
            return this;
        }

        public EventBuilder WithCoordinates(double latitude, double longitude)
        {
            _event.Latitude = latitude;
            _event.Longitude = longitude;
            return this;
        }

        public EventBuilder WithAddress(string streetAddress, string postalCode)
        {
            _event.StreetAddress = streetAddress;
            _event.PostalCode = postalCode;
            return this;
        }

        public EventBuilder WithMaxParticipants(int max)
        {
            _event.MaxNumberOfParticipants = max;
            return this;
        }

        public EventBuilder AsPrivate()
        {
            _event.EventVisibilityId = (int)EventVisibilityEnum.Private;
            _event.TeamId = null;
            return this;
        }

        public EventBuilder AsPublic()
        {
            _event.EventVisibilityId = (int)EventVisibilityEnum.Public;
            _event.TeamId = null;
            return this;
        }

        public EventBuilder AsTeamOnly(Guid teamId)
        {
            _event.EventVisibilityId = (int)EventVisibilityEnum.TeamOnly;
            _event.TeamId = teamId;
            return this;
        }

        public EventBuilder CreatedBy(Guid userId)
        {
            _event.CreatedByUserId = userId;
            _event.LastUpdatedByUserId = userId;
            return this;
        }

        public Event Build() => _event;
    }
}
