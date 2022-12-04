namespace TrashMob.Shared.Managers.Partners
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    public class PickupLocationManager : KeyedManager<PickupLocation>, IPickupLocationManager
    {
        private readonly IEventManager eventManager;
        private readonly IEventPartnerLocationServiceManager eventPartnerLocationServiceManager;
        private readonly IPartnerLocationContactManager partnerLocationContactManager;
        private readonly IEmailManager emailManager;

        public PickupLocationManager(IKeyedRepository<PickupLocation> pickupLocationRepository,
                                     IEventManager eventManager,
                                     IEventPartnerLocationServiceManager eventPartnerLocationServiceManager,
                                     IPartnerLocationContactManager partnerLocationContactManager,
                                     IEmailManager emailManager)
            : base(pickupLocationRepository)
        {
            this.eventManager = eventManager;
            this.eventPartnerLocationServiceManager = eventPartnerLocationServiceManager;
            this.partnerLocationContactManager = partnerLocationContactManager;
            this.emailManager = emailManager;
        }

        public override async Task<IEnumerable<PickupLocation>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.EventId == parentId)
                                          .ToListAsync(cancellationToken))
                                          .AsEnumerable();
        }

        public async Task SubmitPickupLocations(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            // Get the Event
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);

            var partnerLocation = await eventPartnerLocationServiceManager.GetHaulingPartnerLocationForEvent(eventId, cancellationToken);

            if (partnerLocation == null) 
            { 
                // Todo add error handling for this
                return;
            }

            var contacts = await partnerLocationContactManager.GetByParentIdAsync(partnerLocation.Id, cancellationToken);

            // Get all pickup locations for the event that haven't been submitted or picked up
            var pickupLocations = await Repository.Get(p => p.EventId == eventId && !p.HasBeenSubmitted && !p.HasBeenPickedUp).ToListAsync(cancellationToken);

            // Todo: Create email
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
                emailCopy = emailCopy,
                subject = emailSubject,
                eventDetailsUrl = mobEvent.EventDetailsUrl(),
                eventSummaryUrl = mobEvent.EventSummaryUrl(),
                pickupSpots = new List<PickupSpot>()
            };

            foreach (var pickupLocation in pickupLocations)
            {
                var pickSpot = new PickupSpot
                {
                    StreetAddress = pickupLocation.StreetAddress,
                    GoogleMapsUrl = pickupLocation.GoogleMapsUrl(),
                    Notes = pickupLocation.Notes
                };

                dynamicTemplateData.pickupSpots.Add(pickSpot);
            }

            await emailManager.SendTemplatedEmailAsync(emailSubject, SendGridEmailTemplateId.PickupEmail, SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None).ConfigureAwait(false);

            // Update the submitted status
            foreach (var pickupLocation in pickupLocations)
            {
                pickupLocation.HasBeenSubmitted = true;
                await base.UpdateAsync(pickupLocation, userId, cancellationToken);
            }
        }
    }
}
