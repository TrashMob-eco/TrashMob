namespace TrashMob.Shared.Managers.Events
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using System;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Shared.Poco;

    public class EventPartnerManager : BaseManager<EventPartner>, IEventPartnerManager
    {
        private readonly IKeyedRepository<Event> eventRepository;
        private readonly IKeyedRepository<PartnerLocation> partnerLocationRepository;
        private readonly IEmailManager emailManager;

        public EventPartnerManager(IBaseRepository<EventPartner> repository, IKeyedRepository<Event> eventRepository, IKeyedRepository<PartnerLocation> partnerLocationRepository, IEmailManager emailManager) : base(repository)
        {
            this.eventRepository = eventRepository;
            this.partnerLocationRepository = partnerLocationRepository;
            this.emailManager = emailManager;
        }

        public override async Task<EventPartner> AddAsync(EventPartner instance, CancellationToken cancellationToken = default)
        {
            var eventPartner = await base.AddAsync(instance, cancellationToken);

            // Notify Admins that a new partner request has been made
            var subject = "A New Partner Request for an Event has been made!";
            var message = $"A new partner request for an event has been made for event {instance.EventId}!";

            var recipients = new List<EmailAddress>
            {
                new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress }
            };

            var adminDynamicTemplateData = new
            {
                username = Constants.TrashMobEmailName,
                emailCopy = message,
                subject = subject,
            };

            await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.EventRelated, adminDynamicTemplateData, recipients, CancellationToken.None).ConfigureAwait(false);

            var partnerMessage = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.EventPartnerRequest.ToString());
            var partnerSubject = "A TrashMob.eco Event would like to Partner with you!";

            partnerMessage = partnerMessage.Replace("{PartnerLocationName}", eventPartner.PartnerLocation.Name);

            var dynamicTemplateData = new
            {
                username = eventPartner.Partner.Name,
                emailCopy = partnerMessage,
                subject = subject,
            };

            var partnerRecipients = new List<EmailAddress>
            {
                new EmailAddress { Name = eventPartner.PartnerLocation.Name, Email = eventPartner.PartnerLocation.PrimaryEmail },
                new EmailAddress { Name = eventPartner.PartnerLocation.Name, Email = eventPartner.PartnerLocation.SecondaryEmail }
            };

            await emailManager.SendTemplatedEmailAsync(partnerSubject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.EventRelated, dynamicTemplateData, partnerRecipients, CancellationToken.None).ConfigureAwait(false);

            return eventPartner;
        }

        public async Task<IEnumerable<EventPartner>> GetCurrentPartnersAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var eventPartners = await Repository.Get(ea => ea.EventId == eventId).ToListAsync(cancellationToken).ConfigureAwait(false);

            return eventPartners;
        }

        public async Task<IEnumerable<PartnerLocation>> GetPotentialPartnerLocationsAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var mobEvent = await eventRepository.GetAsync(eventId, cancellationToken);

            // Simple match on postal code or city first. Radius later
            var partnerLocations = partnerLocationRepository.Get(pl => pl.IsActive && pl.Partner.PartnerStatusId == (int)PartnerStatusEnum.Active &&
                        (pl.PostalCode == mobEvent.PostalCode || pl.City == mobEvent.City));

            return partnerLocations;
        }
    }
}
