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
    using System.Linq;
    using EmailAddress = Poco.EmailAddress;
    using TrashMob.Poco;

    public class EventPartnerLocationServiceManager : BaseManager<EventPartnerLocationService>, IEventPartnerLocationServiceManager
    {
        private readonly IKeyedRepository<Event> eventRepository;
        private readonly IKeyedRepository<Partner> partnerRepository;
        private readonly IKeyedRepository<PartnerLocation> partnerLocationRepository;
        private readonly IBaseRepository<PartnerLocationService> partnerLocationServiceRepository;
        private readonly IKeyedRepository<User> userRepository;
        private readonly IBaseRepository<PartnerAdmin> partnerAdminRepository;
        private readonly IEmailManager emailManager;

        public EventPartnerLocationServiceManager(IBaseRepository<EventPartnerLocationService> repository,
                                           IKeyedRepository<Event> eventRepository,
                                           IKeyedRepository<Partner> partnerRepository,
                                           IKeyedRepository<PartnerLocation> partnerLocationRepository,
                                           IBaseRepository<PartnerLocationService> partnerLocationServiceRepository,
                                           IKeyedRepository<User> userRepository,
                                           IBaseRepository<PartnerAdmin> partnerAdminRepository,
                                           IEmailManager emailManager)
            : base(repository)
        {
            this.eventRepository = eventRepository;
            this.partnerRepository = partnerRepository;
            this.partnerLocationRepository = partnerLocationRepository;
            this.partnerLocationServiceRepository = partnerLocationServiceRepository;
            this.userRepository = userRepository;
            this.partnerAdminRepository = partnerAdminRepository;
            this.emailManager = emailManager;
        }

        public override async Task<EventPartnerLocationService> AddAsync(EventPartnerLocationService instance, Guid userId, CancellationToken cancellationToken = default)
        {
            await base.AddAsync(instance, userId, cancellationToken);

            var existingService = await Repository.Get(epls => epls.EventId == instance.EventId && epls.PartnerLocationId == instance.PartnerLocationId && epls.ServiceTypeId == instance.ServiceTypeId)
                                                  .Include(ep => ep.Event)
                                                  .Include(ep => ep.PartnerLocation)
                                                  .Include(ep => ep.PartnerLocation.PartnerLocationContacts)
                                                  .Include(ep => ep.PartnerLocation.Partner)
                                                  .FirstOrDefaultAsync(cancellationToken);

            // Notify Admins that a new partner request has been made
            var subject = "A New Partner Request for an Event has been made!";
            var message = $"A new partner request for an event has been made for event {existingService.Event.Name}!";

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

            partnerMessage = partnerMessage.Replace("{PartnerLocationName}", existingService.PartnerLocation.Name);

            var dynamicTemplateData = new
            {
                username = existingService.PartnerLocation.Partner.Name,
                emailCopy = partnerMessage,
                subject = subject,
            };

            var partnerRecipients = new List<EmailAddress>();

            foreach (var contact in existingService.PartnerLocation.PartnerLocationContacts)
            {
                var newEmailAddress = new EmailAddress { Name = contact.Name, Email = contact.Email };
                partnerRecipients.Add(newEmailAddress);
            }

            await emailManager.SendTemplatedEmailAsync(partnerSubject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.EventRelated, dynamicTemplateData, partnerRecipients, CancellationToken.None).ConfigureAwait(false);

            return existingService;
        }

        public override async Task<EventPartnerLocationService> UpdateAsync(EventPartnerLocationService instance, Guid userId, CancellationToken cancellationToken = default)
        {
            await base.UpdateAsync(instance, userId, cancellationToken);

            var existingService = await Repository.Get(epls => epls.EventId == instance.EventId && epls.PartnerLocationId == instance.PartnerLocationId && epls.ServiceTypeId == instance.ServiceTypeId)
                                                  .Include(ep => ep.Event)
                                                  .Include(ep => ep.PartnerLocation)
                                                  .Include(ep => ep.PartnerLocation.PartnerLocationContacts)
                                                  .Include(ep => ep.PartnerLocation.Partner)
                                                  .FirstOrDefaultAsync(cancellationToken);

            var user = await userRepository.GetAsync(existingService.CreatedByUserId, cancellationToken).ConfigureAwait(false);

            // Notify Admins that a partner request has been responded to
            var subject = "A partner request for an event has been responded to!";
            var message = $"A partner request for an event has been responded to for event {existingService.Event.Name}!";

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

            var partnerMessage = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.EventPartnerResponse.ToString());
            var partnerSubject = "A TrashMob.eco Partner has responded to your request!";

            partnerMessage = partnerMessage.Replace("{UserName}", user.UserName);

            var dashboardLink = string.Format("https://www.trashmob.eco/manageeventdashboard/{0}", existingService.EventId);
            partnerMessage = partnerMessage.Replace("{PartnerResponseUrl}", dashboardLink);

            var partnerRecipients = new List<EmailAddress>
            {
                new EmailAddress { Name = user.UserName, Email = user.Email },
            };

            var dynamicTemplateData = new
            {
                username = user.UserName,
                emailCopy = partnerMessage,
                subject = subject,
            };

            await emailManager.SendTemplatedEmailAsync(partnerSubject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.EventRelated, dynamicTemplateData, partnerRecipients, CancellationToken.None).ConfigureAwait(false);

            return existingService;
        }

        public async Task<IEnumerable<DisplayPartnerLocationServiceEvent>> GetByPartnerLocationAsync(Guid partnerLocationId, CancellationToken cancellationToken = default)
        {
            var displayEventPartners = new List<DisplayPartnerLocationServiceEvent>();

            var currentPartnerLocations = await Repository.Get(p => p.PartnerLocation.Id == partnerLocationId)
                                                          .Include(p => p.PartnerLocation)
                                                          .Include(p => p.PartnerLocation.Partner)
                                                          .Include(p => p.Event)
                                                          .ToListAsync(cancellationToken: cancellationToken);

            if (currentPartnerLocations.Any())
            {
                // Convert the current list of partner events for the event to a display partner (reduces round trips)
                foreach (var cpl in currentPartnerLocations)
                {
                    var displayPartnerLocationEvent = new DisplayPartnerLocationServiceEvent
                    {
                        EventId = cpl.EventId,
                        PartnerLocationId = partnerLocationId,
                        EventPartnerLocationStatusId = cpl.EventPartnerLocationServiceStatusId,
                        PartnerName = cpl.PartnerLocation.Partner.Name,
                        PartnerLocationName = cpl.PartnerLocation.Name,
                        EventName = cpl.Event.Name,
                        EventStreetAddress = cpl.Event.StreetAddress,
                        EventCity = cpl.Event.City,
                        EventRegion = cpl.Event.Region,
                        EventCountry = cpl.Event.Country,
                        EventPostalCode = cpl.Event.PostalCode,
                        EventDescription = cpl.Event.Description,
                        EventDate = cpl.Event.EventDate,
                        ServiceTypeId = cpl.ServiceTypeId,
                    };

                    displayEventPartners.Add(displayPartnerLocationEvent);
                }
            }

            return displayEventPartners;
        }

        public async Task<IEnumerable<DisplayPartnerLocationServiceEvent>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var displayEventPartners = new List<DisplayPartnerLocationServiceEvent>();

            // Get list of partners for a user
            var partners = await partnerAdminRepository.Get(p => p.UserId == userId)
                                                       .Include(p => p.Partner)
                                                       .Select(p => p.Partner)
                                                       .ToListAsync(cancellationToken);

            foreach (var partner in partners)
            {
                var currentPartnerLocations = await Repository.Get(p => p.PartnerLocation.Partner.Id == partner.Id )
                                                              .Include(p => p.PartnerLocation)
                                                              .Include(p => p.Event)
                                                              .ToListAsync(cancellationToken: cancellationToken);

                if (currentPartnerLocations.Any())
                {
                    // Convert the current list of partner events for the event to a display partner (reduces round trips)
                    foreach (var cpl in currentPartnerLocations)
                    {
                        var displayPartnerLocationEvent = new DisplayPartnerLocationServiceEvent
                        {
                            EventId = cpl.EventId,
                            PartnerLocationId = cpl.PartnerLocationId,
                            EventPartnerLocationStatusId = cpl.EventPartnerLocationServiceStatusId,
                            PartnerName = partner.Name,
                            PartnerLocationName = cpl.PartnerLocation.Name,
                            EventName = cpl.Event.Name,
                            EventStreetAddress = cpl.Event.StreetAddress,
                            EventCity = cpl.Event.City,
                            EventRegion = cpl.Event.Region,
                            EventCountry = cpl.Event.Country,
                            EventPostalCode = cpl.Event.PostalCode,
                            EventDescription = cpl.Event.Description,
                            EventDate = cpl.Event.EventDate,
                            ServiceTypeId = cpl.ServiceTypeId,
                        };

                        displayEventPartners.Add(displayPartnerLocationEvent);
                    }
                }
            }

            return displayEventPartners;
        }

        public async Task<IEnumerable<DisplayEventPartnerLocationService>> GetByEventAndPartnerLocationAsync(Guid eventId, Guid partnerLocationId, CancellationToken cancellationToken = default)
        {
            var existingServices = await Repository.Get(epls => epls.EventId == eventId && epls.PartnerLocationId == partnerLocationId)
                                               .Include(ep => ep.PartnerLocation)
                                               .Include(ep => ep.PartnerLocation.Partner)
                                               .ToListAsync(cancellationToken);

            var possibleServices = await partnerLocationServiceRepository.Get(p => p.PartnerLocationId == partnerLocationId)
                                                                         .Include(p => p.PartnerLocation)
                                                                         .Include(p => p.PartnerLocation.Partner)
                                                                         .ToListAsync(cancellationToken);

            var displayEventPartnerLocationServices = new List<DisplayEventPartnerLocationService>();

            foreach (var service in existingServices)
            {
                var matchService = possibleServices.FirstOrDefault(s => s.ServiceTypeId == service.ServiceTypeId);
                var displayService = new DisplayEventPartnerLocationService()
                {
                    EventId = eventId,
                    PartnerLocationId = partnerLocationId,
                    PartnerLocationName = service.PartnerLocation.Name,
                    EventPartnerLocationServiceStatusId = service.EventPartnerLocationServiceStatusId,
                    PartnerName = service.PartnerLocation.Partner.Name,
                    ServiceTypeId = service.ServiceTypeId,
                    PartnerLocationServicePublicNotes = matchService.Notes
                };

                displayEventPartnerLocationServices.Add(displayService);
            }

            foreach (var service in possibleServices)
            {
                if (!displayEventPartnerLocationServices.Any(d => d.ServiceTypeId == service.ServiceTypeId))
                {
                    var displayService = new DisplayEventPartnerLocationService()
                    {
                        EventId = eventId,
                        PartnerLocationId = partnerLocationId,
                        PartnerLocationName = service.PartnerLocation.Name,
                        EventPartnerLocationServiceStatusId = (int)EventPartnerLocationServiceStatusEnum.None,
                        PartnerName = service.PartnerLocation.Partner.Name,
                        ServiceTypeId = service.ServiceTypeId,
                        PartnerLocationServicePublicNotes = service.Notes
                    };

                    displayEventPartnerLocationServices.Add(displayService);
                }
            }

            return displayEventPartnerLocationServices;
        }

        public async Task<PartnerLocation> GetHaulingPartnerLocationForEvent(Guid eventId, CancellationToken cancellationToken = default)
        {
            var partnerLocation = await Repository.Get(ea => ea.EventId == eventId && ea.ServiceTypeId == (int)ServiceTypeEnum.Hauling && ea.EventPartnerLocationServiceStatusId == (int)EventPartnerLocationServiceStatusEnum.Accepted)
                                                  .Include(p => p.PartnerLocation)
                                                  .Include(p => p.PartnerLocation.PartnerLocationContacts)
                                                  .Select(p => p.PartnerLocation)
                                                  .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            return partnerLocation;
        }

        public async Task<IEnumerable<DisplayEventPartnerLocation>> GetByEventAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var displayEventPartners = new List<DisplayEventPartnerLocation>();
            var currentPartners = (await GetCurrentPartnersAsync(eventId, cancellationToken).ConfigureAwait(false)).DistinctBy(p => new { p.EventId, p.PartnerLocationId });
            var possiblePartners = await GetPotentialPartnerLocationsAsync(eventId, cancellationToken).ConfigureAwait(false);

            // Convert the current list of partners for the event to a display partner (reduces round trips)
            foreach (var cp in currentPartners.ToList())
            {
                var existingServices = await Repository.Get(epls => epls.EventId == eventId && epls.PartnerLocationId == cp.PartnerLocationId)
                                                   .Include(p => p.ServiceType)
                                                   .Include(p => p.EventPartnerLocationServiceStatus)
                                                   .ToListAsync(cancellationToken);

                var partnerServicesEngaged = "None";
                var isFirst = true;
                foreach (var service in existingServices)
                {
                    if (isFirst)
                    {
                        partnerServicesEngaged = service.ServiceType.Name + " (" + service.EventPartnerLocationServiceStatus.Name + ")";
                        isFirst = false;
                    }
                    else
                    {
                        partnerServicesEngaged += ", " + service.ServiceType.Name + " (" + service.EventPartnerLocationServiceStatus.Name + ")";
                    }
                }

                var displayEventPartner = new DisplayEventPartnerLocation
                {
                    EventId = eventId,
                    PartnerId = cp.PartnerLocation.PartnerId,
                    PartnerLocationId = cp.PartnerLocationId,
                    EventPartnerStatusId = cp.EventPartnerLocationServiceStatusId,
                    PartnerName = cp.PartnerLocation.Partner.Name,
                    PartnerLocationName = cp.PartnerLocation.Name,
                    PartnerLocationNotes = cp.PartnerLocation.PublicNotes,
                    PartnerServicesEngaged = partnerServicesEngaged
                };

                displayEventPartners.Add(displayEventPartner);
            }

            // Convert the current list of possible partners for the event to a display partner unless the partner location is already included (reduces round trips)
            foreach (var pp in possiblePartners.ToList())
            {
                if (!displayEventPartners.Any(ep => ep.PartnerLocationId == pp.Id))
                {
                    var displayEventPartner = new DisplayEventPartnerLocation
                    {
                        EventId = eventId,
                        PartnerId = pp.PartnerId,
                        PartnerLocationId = pp.Id,
                        EventPartnerStatusId = (int)EventPartnerLocationServiceStatusEnum.None,
                        PartnerLocationName = pp.Name,
                        PartnerLocationNotes = pp.PublicNotes,
                        PartnerName = pp.Partner.Name,
                        PartnerServicesEngaged = "None"
                    };

                    displayEventPartners.Add(displayEventPartner);
                }
            }

            return displayEventPartners;
        }

        public async Task<IEnumerable<EventPartnerLocationService>> GetCurrentPartnersAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var eventPartners = await Repository.Get(ea => ea.EventId == eventId)
                                                .Include(p => p.PartnerLocation)
                                                .Include(p => p.PartnerLocation.Partner)
                                                .ToListAsync(cancellationToken).ConfigureAwait(false);

            return eventPartners;
        }

        public async Task<IEnumerable<PartnerLocation>> GetPotentialPartnerLocationsAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var mobEvent = await eventRepository.GetAsync(eventId, cancellationToken);

            // Simple match on postal code or city first. Radius later
            var partnerLocations = partnerLocationRepository.Get(pl => pl.IsActive && pl.Partner.PartnerStatusId == (int)PartnerStatusEnum.Active &&
                        (pl.PostalCode == mobEvent.PostalCode || pl.City == mobEvent.City))
                                .Include(p => p.Partner);

            return partnerLocations;
        }

        public async Task<int> DeleteAsync(Guid eventId, Guid partnerLocationId, int serviceTypeId, CancellationToken cancellationToken = default)
        {
            var eventPartnerLocationService = await Repository.Get(epls => epls.EventId == eventId && epls.PartnerLocationId == partnerLocationId && epls.ServiceTypeId == serviceTypeId).FirstOrDefaultAsync(cancellationToken);

            return await Repository.DeleteAsync(eventPartnerLocationService);
        }
    }
}
