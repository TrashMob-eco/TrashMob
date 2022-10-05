
namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Poco;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    [Route("api/eventpartners")]
    public class EventPartnersController : SecureController
    {
        private readonly IEventPartnerManager eventPartnerManager;
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IKeyedManager<User> userManager;
        private readonly IKeyedManager<PartnerLocation> partnerLocationManager;
        private readonly IBaseManager<PartnerUser> partnerUserManager;
        private readonly IEmailManager emailManager;

        public EventPartnersController(IKeyedManager<Partner> partnerManager,
                                       IKeyedManager<User> userManager,
                                       IEventPartnerManager eventPartnerManager,
                                       IKeyedManager<PartnerLocation> partnerLocationManager,
                                       IBaseManager<PartnerUser> partnerUserManager,
                                       IEmailManager emailManager) 
            : base()
        {
            this.eventPartnerManager = eventPartnerManager;
            this.partnerManager = partnerManager;
            this.userManager = userManager;
            this.partnerLocationManager = partnerLocationManager;
            this.partnerUserManager = partnerUserManager;
            this.emailManager = emailManager;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventPartners(Guid eventId, CancellationToken cancellationToken)
        {
            var displayEventPartners = new List<DisplayEventPartner>();
            var currentPartners = await eventPartnerManager.GetCurrentPartnersAsync(eventId, cancellationToken).ConfigureAwait(false);
            var possiblePartners = await eventPartnerManager.GetPotentialPartnerLocationsAsync(eventId, cancellationToken).ConfigureAwait(false);

            // Convert the current list of partners for the event to a display partner (reduces round trips)
            foreach (var cp in currentPartners.ToList())
            {
                var displayEventPartner = new DisplayEventPartner
                {
                    EventId = eventId,
                    PartnerId = cp.PartnerId,
                    PartnerLocationId = cp.PartnerLocationId,
                    EventPartnerStatusId = cp.EventPartnerStatusId,
                };

                var partner = await partnerManager.GetAsync(cp.PartnerId, cancellationToken).ConfigureAwait(false);
                displayEventPartner.PartnerName = partner.Name;

                var partnerLocation = (await partnerLocationManager.GetAsync(pl => pl.PartnerId == cp.PartnerId && pl.Id == cp.PartnerLocationId, cancellationToken)).FirstOrDefault();

                displayEventPartner.PartnerLocationName = partnerLocation.Name;
                displayEventPartner.PartnerLocationNotes = partnerLocation.PublicNotes;
 
                displayEventPartners.Add(displayEventPartner);
            }

            // Convert the current list of possible partners for the event to a display partner unless the partner location is already included (reduces round trips)
            foreach (var pp in possiblePartners.ToList())
            {
                if (!displayEventPartners.Any(ep => ep.PartnerLocationId == pp.Id))
                {
                    var displayEventPartner = new DisplayEventPartner
                    {
                        EventId = eventId,
                        PartnerId = pp.PartnerId,
                        PartnerLocationId = pp.Id,
                        EventPartnerStatusId = (int)EventPartnerStatusEnum.None,
                        PartnerLocationName = pp.Name,
                        PartnerLocationNotes = pp.PublicNotes,
                    };

                    var partner = await partnerManager.GetAsync(pp.PartnerId, cancellationToken).ConfigureAwait(false);
                    displayEventPartner.PartnerName = partner.Name;

                    displayEventPartners.Add(displayEventPartner);
                }
            }

            return Ok(displayEventPartners);
        }

        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEventPartner(EventPartner eventPartner, CancellationToken cancellationToken = default)
        {
            var partner = partnerManager.GetAsync(eventPartner.PartnerId);

            if (partner == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            eventPartner.LastUpdatedByUserId = UserId;

            var updatedEventPartner = await eventPartnerManager.UpdateAsync(eventPartner, cancellationToken).ConfigureAwait(false);

            var user = await userManager.GetAsync(eventPartner.CreatedByUserId).ConfigureAwait(false);

            // Notify Admins that a partner request has been responded to
            var subject = "A partner request for an event has been responded to!";
            var message = $"A partner request for an event has been responded to for event {eventPartner.EventId}!";

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

            var dashboardLink = string.Format("https://www.trashmob.eco/manageeventdashboard/{0}", eventPartner.EventId);
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
            TelemetryClient.TrackEvent(nameof(UpdateEventPartner));

            return Ok(updatedEventPartner);
        }

        [HttpPost]
        [Authorize(Policy = "ValidUser")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddEventPartner(EventPartner eventPartner, CancellationToken cancellationToken)
        {
            await eventPartnerManager.AddAsync(eventPartner, cancellationToken).ConfigureAwait(false);

            var partnerLocation = partnerLocationManager.GetAsync(eventPartner.PartnerLocationId, cancellationToken);

            TelemetryClient.TrackEvent(nameof(AddEventPartner));

            return Ok(partnerLocation);
        }
    }
}
