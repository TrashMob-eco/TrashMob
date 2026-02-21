namespace TrashMob.Security
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Authorization handler that checks if the user is an event lead.
    /// An event lead is either the event creator or has been promoted to co-lead status.
    /// </summary>
    public class UserIsEventLeadAuthHandler : AuthorizationHandler<UserIsEventLeadRequirement, BaseModel>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly ILogger<UserIsEventLeadAuthHandler> logger;
        private readonly IUserManager userManager;
        private readonly IEventAttendeeManager eventAttendeeManager;
        private readonly IKeyedManager<Event> eventManager;

        public UserIsEventLeadAuthHandler(
            IHttpContextAccessor httpContext,
            IUserManager userManager,
            IEventAttendeeManager eventAttendeeManager,
            IKeyedManager<Event> eventManager,
            ILogger<UserIsEventLeadAuthHandler> logger)
        {
            this.httpContext = httpContext;
            this.userManager = userManager;
            this.eventAttendeeManager = eventAttendeeManager;
            this.eventManager = eventManager;
            this.logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UserIsEventLeadRequirement requirement,
            BaseModel resource)
        {
            try
            {
                var emailAddressClaim = context.User.FindFirst(ClaimTypes.Email);
                var emailClaim = context.User.FindFirst("email");

                var email = emailAddressClaim is null ? emailClaim?.Value : emailAddressClaim?.Value;

                var user = await userManager.GetUserByEmailAsync(email, CancellationToken.None);

                if (user is null)
                {
                    AuthorizationFailure.Failed(new List<AuthorizationFailureReason>
                        { new(this, $"User with email '{email}' not found.") });
                    return;
                }

                if (!httpContext.HttpContext.Items.ContainsKey("UserId"))
                {
                    httpContext.HttpContext.Items.Add("UserId", user.Id);
                }

                // Get the EventId based on resource type
                Guid? eventId = GetEventId(resource);

                if (!eventId.HasValue)
                {
                    AuthorizationFailure.Failed(new List<AuthorizationFailureReason>
                        { new(this, "Could not determine EventId from resource.") });
                    return;
                }

                // Look up the actual event from the database to verify ownership
                // SECURITY: Do not trust CreatedByUserId from the request body
                // Use no-tracking to avoid EF Core conflicts when the entity is later updated
                var actualEvent = await eventManager.GetWithNoTrackingAsync(eventId.Value, CancellationToken.None);

                if (actualEvent is null)
                {
                    AuthorizationFailure.Failed(new List<AuthorizationFailureReason>
                        { new(this, $"Event with ID '{eventId.Value}' not found.") });
                    return;
                }

                // Check if user is the event creator (verified against database)
                if (user.Id == actualEvent.CreatedByUserId)
                {
                    context.Succeed(requirement);
                    return;
                }

                // Check if user is an event lead (co-lead)
                var isEventLead = await eventAttendeeManager.IsEventLeadAsync(eventId.Value, user.Id, CancellationToken.None);

                if (isEventLead)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    AuthorizationFailure.Failed(new List<AuthorizationFailureReason>
                        { new(this, "User is not an event lead.") });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while authenticating user for event lead check.");
            }
        }

        /// <summary>
        /// Extracts the EventId from the resource based on its type.
        /// </summary>
        private static Guid? GetEventId(BaseModel resource)
        {
            // If the resource is an Event, return its Id
            if (resource is Event evt)
            {
                return evt.Id;
            }

            // Try to get EventId from known types
            return resource switch
            {
                EventSummary es => es.EventId,
                EventAttendee ea => ea.EventId,
                EventAttendeeRoute ear => ear.EventId,
                EventAttendeeMetrics eam => eam.EventId,
                EventLitterReport elr => elr.EventId,
                EventPartnerLocationService epls => epls.EventId,
                PickupLocation pl => pl.EventId,
                UserNotification un => un.EventId,
                _ => TryGetEventIdViaReflection(resource)
            };
        }

        /// <summary>
        /// Fallback method to get EventId via reflection for unknown types.
        /// </summary>
        private static Guid? TryGetEventIdViaReflection(BaseModel resource)
        {
            var eventIdProperty = resource.GetType().GetProperty("EventId");
            if (eventIdProperty is not null && eventIdProperty.PropertyType == typeof(Guid))
            {
                return (Guid)eventIdProperty.GetValue(resource);
            }

            return null;
        }
    }
}
