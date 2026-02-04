namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using TrashMob.Models;

    /// <summary>
    /// Builder for creating EventAttendee test data with sensible defaults.
    /// </summary>
    public class EventAttendeeBuilder
    {
        private readonly EventAttendee _eventAttendee;

        public EventAttendeeBuilder()
        {
            var userId = Guid.NewGuid();
            _eventAttendee = new EventAttendee
            {
                EventId = Guid.NewGuid(),
                UserId = userId,
                SignUpDate = DateTimeOffset.UtcNow,
                CanceledDate = null,
                IsEventLead = false,
                CreatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = userId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };
        }

        public EventAttendeeBuilder ForEvent(Guid eventId)
        {
            _eventAttendee.EventId = eventId;
            return this;
        }

        public EventAttendeeBuilder ForEvent(Event evt)
        {
            _eventAttendee.EventId = evt.Id;
            _eventAttendee.Event = evt;
            return this;
        }

        public EventAttendeeBuilder ForUser(Guid userId)
        {
            _eventAttendee.UserId = userId;
            _eventAttendee.CreatedByUserId = userId;
            _eventAttendee.LastUpdatedByUserId = userId;
            return this;
        }

        public EventAttendeeBuilder ForUser(User user)
        {
            _eventAttendee.UserId = user.Id;
            _eventAttendee.User = user;
            _eventAttendee.CreatedByUserId = user.Id;
            _eventAttendee.LastUpdatedByUserId = user.Id;
            return this;
        }

        public EventAttendeeBuilder WithSignUpDate(DateTimeOffset signUpDate)
        {
            _eventAttendee.SignUpDate = signUpDate;
            return this;
        }

        public EventAttendeeBuilder AsCancelled()
        {
            _eventAttendee.CanceledDate = DateTimeOffset.UtcNow;
            return this;
        }

        public EventAttendeeBuilder AsEventLead()
        {
            _eventAttendee.IsEventLead = true;
            return this;
        }

        public EventAttendeeBuilder AsRegularAttendee()
        {
            _eventAttendee.IsEventLead = false;
            return this;
        }

        public EventAttendee Build() => _eventAttendee;
    }
}
