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
    /// Authorization handler that checks if the user is an event lead or a site admin.
    /// </summary>
    public class UserIsEventLeadOrIsAdminAuthHandler : AuthorizationHandler<UserIsEventLeadOrIsAdminRequirement, BaseModel>
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly ILogger<UserIsEventLeadOrIsAdminAuthHandler> logger;
        private readonly IUserManager userManager;
        private readonly IEventAttendeeManager eventAttendeeManager;

        public UserIsEventLeadOrIsAdminAuthHandler(
            IHttpContextAccessor httpContext,
            IUserManager userManager,
            IEventAttendeeManager eventAttendeeManager,
            ILogger<UserIsEventLeadOrIsAdminAuthHandler> logger)
        {
            this.httpContext = httpContext;
            this.userManager = userManager;
            this.eventAttendeeManager = eventAttendeeManager;
            this.logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            UserIsEventLeadOrIsAdminRequirement requirement,
            BaseModel resource)
        {
            try
            {
                var emailAddressClaim = context.User.FindFirst(ClaimTypes.Email);
                var emailClaim = context.User.FindFirst("email");

                var email = emailAddressClaim == null ? emailClaim?.Value : emailAddressClaim?.Value;

                var user = await userManager.GetUserByEmailAsync(email, CancellationToken.None);

                if (user == null)
                {
                    AuthorizationFailure.Failed(new List<AuthorizationFailureReason>
                        { new(this, $"User with email '{email}' not found.") });
                    return;
                }

                if (!httpContext.HttpContext.Items.ContainsKey("UserId"))
                {
                    httpContext.HttpContext.Items.Add("UserId", user.Id);
                }

                // Site admin can do anything
                if (user.IsSiteAdmin)
                {
                    context.Succeed(requirement);
                    return;
                }

                // Check if user is the event creator
                if (user.Id == resource.CreatedByUserId)
                {
                    context.Succeed(requirement);
                    return;
                }

                // Get the EventId based on resource type
                Guid? eventId = GetEventId(resource);

                if (!eventId.HasValue)
                {
                    AuthorizationFailure.Failed(new List<AuthorizationFailureReason>
                        { new(this, "Could not determine EventId from resource.") });
                    return;
                }

                // Check if user is an event lead
                var isEventLead = await eventAttendeeManager.IsEventLeadAsync(eventId.Value, user.Id, CancellationToken.None);

                if (isEventLead)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    AuthorizationFailure.Failed(new List<AuthorizationFailureReason>
                        { new(this, "User is not an event lead and is not a site admin.") });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while authenticating user for event lead or admin check.");
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
            if (eventIdProperty != null && eventIdProperty.PropertyType == typeof(Guid))
            {
                return (Guid)eventIdProperty.GetValue(resource);
            }

            return null;
        }
    }
}
