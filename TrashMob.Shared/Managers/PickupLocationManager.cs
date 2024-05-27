﻿namespace TrashMob.Shared.Managers
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

    public class PickupLocationManager : KeyedManager<PickupLocation>, IPickupLocationManager
    {
        private readonly IEmailManager emailManager;
        private readonly IEventManager eventManager;
        private readonly IEventPartnerLocationServiceManager eventPartnerLocationServiceManager;
        private readonly IImageManager imageManager;
        private readonly IPartnerAdminManager partnerAdminManager;
        private readonly IPartnerLocationContactManager partnerLocationContactManager;

        public PickupLocationManager(IKeyedRepository<PickupLocation> pickupLocationRepository,
            IEventManager eventManager,
            IEventPartnerLocationServiceManager eventPartnerLocationServiceManager,
            IPartnerLocationContactManager partnerLocationContactManager,
            IPartnerAdminManager partnerAdminManager,
            IEmailManager emailManager,
            IImageManager imageManager)
            : base(pickupLocationRepository)
        {
            this.eventManager = eventManager;
            this.eventPartnerLocationServiceManager = eventPartnerLocationServiceManager;
            this.partnerLocationContactManager = partnerLocationContactManager;
            this.partnerAdminManager = partnerAdminManager;
            this.emailManager = emailManager;
            this.imageManager = imageManager;
        }

        public override async Task<IEnumerable<PickupLocation>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.EventId == parentId)
                    .ToListAsync(cancellationToken))
                .AsEnumerable();
        }

        public async Task<IEnumerable<PickupLocation>> GetByUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            // Get list of Partner Locations for this user that have Hauling set up
            var partnerLocations =
                await partnerAdminManager.GetHaulingPartnerLocationsByUserIdAsync(userId, cancellationToken);

            var pickupLocations = new List<PickupLocation>();

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

        public async Task MarkAsPickedUpAsync(Guid pickupLocationId, Guid userId, CancellationToken cancellationToken)
        {
            var pickupLocation = await base.GetAsync(pickupLocationId, cancellationToken);

            pickupLocation.HasBeenPickedUp = true;

            await base.UpdateAsync(pickupLocation, userId, cancellationToken);
        }

        public async Task SubmitPickupLocations(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            // Get the Event
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);

            var partnerLocation =
                await eventPartnerLocationServiceManager.GetHaulingPartnerLocationForEvent(eventId, cancellationToken);

            if (partnerLocation == null)
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

            var recipients = new List<EmailAddress>();

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
                var imageUrl = await imageManager.GetImageUrlAsync(eventId, ImageTypeEnum.Pickup, ImageSizeEnum.Thumb);

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
                    SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None)
                .ConfigureAwait(false);

            // Update the submitted status
            foreach (var pickupLocation in pickupLocations)
            {
                pickupLocation.HasBeenSubmitted = true;
                await base.UpdateAsync(pickupLocation, userId, cancellationToken);
            }
        }
    }
}