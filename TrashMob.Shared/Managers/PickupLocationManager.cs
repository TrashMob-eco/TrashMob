namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manages pickup locations for events, including submission to hauling partners and pickup tracking.
    /// </summary>
    /// <param name="pickupLocationRepository">The repository for pickup location data access.</param>
    /// <param name="eventManager">The event manager for event operations.</param>
    /// <param name="eventPartnerLocationServiceManager">The manager for event partner location services.</param>
    /// <param name="partnerLocationContactManager">The manager for partner location contacts.</param>
    /// <param name="partnerAdminManager">The manager for partner administrators.</param>
    /// <param name="emailManager">The email manager for sending notifications.</param>
    /// <param name="imageManager">The image manager for pickup location images.</param>
    public class PickupLocationManager(
        IKeyedRepository<PickupLocation> pickupLocationRepository,
        IEventManager eventManager,
        IEventPartnerLocationServiceManager eventPartnerLocationServiceManager,
        IPartnerLocationContactManager partnerLocationContactManager,
        IPartnerAdminManager partnerAdminManager,
        IEmailManager emailManager,
        IImageManager imageManager)
        : KeyedManager<PickupLocation>(pickupLocationRepository), IPickupLocationManager
    {
        /// <inheritdoc />
        public override async Task<IEnumerable<PickupLocation>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return await Repository.Get().Where(p => p.EventId == parentId)
                    .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PickupLocation>> GetByUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            // Get list of Partner Locations for this user that have Hauling set up
            var partnerLocations =
                await partnerAdminManager.GetHaulingPartnerLocationsByUserIdAsync(userId, cancellationToken);

            List<PickupLocation> pickupLocations = [];

            // For each partner location
            foreach (var partnerLocation in partnerLocations)
            {
                // Get the services offered
                var services =
                    await eventPartnerLocationServiceManager.GetByPartnerLocationAsync(partnerLocation.Id,
                        cancellationToken);

                foreach (var service in services.Where(s => s.ServiceTypeId == (int)ServiceTypeEnum.Hauling))
                {
                    // Get the pickup locations for this event which have not been picked up and have been submitted
                    var eventPickupLocations = await Repository
                        .Get(pl => pl.EventId == service.EventId && !pl.HasBeenPickedUp && pl.HasBeenSubmitted)
                        .ToListAsync(cancellationToken);
                    pickupLocations.AddRange(eventPickupLocations);
                }
            }

            return pickupLocations;
        }

        /// <inheritdoc />
        public async Task MarkAsPickedUpAsync(Guid pickupLocationId, Guid userId, CancellationToken cancellationToken)
        {
            var pickupLocation = await base.GetAsync(pickupLocationId, cancellationToken);

            pickupLocation.HasBeenPickedUp = true;

            await base.UpdateAsync(pickupLocation, userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SubmitPickupLocationsAsync(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            // Get the Event
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);

            var partnerLocation =
                await eventPartnerLocationServiceManager.GetHaulingPartnerLocationForEvent(eventId, cancellationToken);

            if (partnerLocation is null)
            {
                // Todo add error handling for this
                return;
            }

            var contacts =
                await partnerLocationContactManager.GetByParentIdAsync(partnerLocation.Id, cancellationToken);

            // Get all pickup locations for the event that haven't been submitted or picked up
            var pickupLocations = await Repository
                .Get(p => p.EventId == eventId && !p.HasBeenSubmitted && !p.HasBeenPickedUp)
                .ToListAsync(cancellationToken);

            var emailCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.EventPartnerPickupRequest.ToString());

            var emailSubject = "A Trashmob.eco Event has pickups ready!";

            List<EmailAddress> recipients = [];

            foreach (var contact in contacts)
            {
                recipients.Add(new EmailAddress { Name = contact.Name, Email = contact.Email });
            }

            var dynamicTemplateData = new
            {
                username = partnerLocation.Name,
                eventName = mobEvent.Name,
                emailCopy,
                subject = emailSubject,
                eventDetailsUrl = mobEvent.EventDetailsUrl(),
                eventSummaryUrl = mobEvent.EventSummaryUrl(),
                pickupSpots = new List<PickupSpot>(),
            };

            foreach (var pickupLocation in pickupLocations)
            {
                var imageUrl = await imageManager.GetImageUrlAsync(eventId, ImageTypeEnum.Pickup, ImageSizeEnum.Thumb, cancellationToken);

                var pickSpot = new PickupSpot
                {
                    StreetAddress = pickupLocation.StreetAddress,
                    GoogleMapsUrl = pickupLocation.GoogleMapsUrl(),
                    Notes = pickupLocation.Notes,
                    Name = pickupLocation.Name,
                    ImageUrl = imageUrl,
                };

                dynamicTemplateData.pickupSpots.Add(pickSpot);
            }

            await emailManager.SendTemplatedEmailAsync(emailSubject, SendGridEmailTemplateId.PickupEmail,
                    SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None);

            // Update the submitted status
            foreach (var pickupLocation in pickupLocations)
            {
                pickupLocation.HasBeenSubmitted = true;
                await base.UpdateAsync(pickupLocation, userId, cancellationToken);
            }
        }
    }
}