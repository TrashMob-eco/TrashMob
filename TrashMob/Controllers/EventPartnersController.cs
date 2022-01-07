
namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Poco;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Route("api/eventpartners")]
    public class EventPartnersController : ControllerBase
    {
        private readonly IEventPartnerRepository eventPartnerRepository;
        private readonly IUserRepository userRepository;
        private readonly IPartnerRepository partnerRepository;
        private readonly IPartnerLocationRepository partnerLocationRepository;
        private readonly IPartnerUserRepository partnerUserRepository;
        private readonly IEmailManager emailManager;

        public EventPartnersController(IEventPartnerRepository eventPartnerRepository, 
                                       IUserRepository userRepository, 
                                       IPartnerRepository partnerRepository, 
                                       IPartnerLocationRepository partnerLocationRepository,
                                       IPartnerUserRepository partnerUserRepository,
                                       IEmailManager emailManager)
        {
            this.eventPartnerRepository = eventPartnerRepository;
            this.userRepository = userRepository;
            this.partnerRepository = partnerRepository;
            this.partnerLocationRepository = partnerLocationRepository;
            this.partnerUserRepository = partnerUserRepository;
            this.emailManager = emailManager;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventPartners(Guid eventId, CancellationToken cancellationToken)
        {
            var displayEventPartners = new List<DisplayEventPartner>();
            var currentPartners = await eventPartnerRepository.GetEventPartners(eventId, cancellationToken).ConfigureAwait(false);
            var possiblePartners = await eventPartnerRepository.GetPotentialEventPartners(eventId, cancellationToken).ConfigureAwait(false);

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

                var partner = await partnerRepository.GetPartner(cp.PartnerId, cancellationToken).ConfigureAwait(false);
                displayEventPartner.PartnerName = partner.Name;

                var partnerLocation = partnerLocationRepository.GetPartnerLocations(cancellationToken).FirstOrDefault(pl => pl.PartnerId == cp.PartnerId && pl.Id == cp.PartnerLocationId);

                displayEventPartner.PartnerLocationName = partnerLocation.Name;
                displayEventPartner.PartnerLocationNotes = partnerLocation.Notes;
 
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
                        PartnerLocationNotes = pp.Notes,
                    };

                    var partner = await partnerRepository.GetPartner(pp.PartnerId, cancellationToken).ConfigureAwait(false);
                    displayEventPartner.PartnerName = partner.Name;

                    displayEventPartners.Add(displayEventPartner);
                }
            }

            return Ok(displayEventPartners);
        }

        [HttpPut]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PutEventPartner(EventPartner eventPartner)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (!currentUser.IsSiteAdmin)
            {
                var currentUserPartner = partnerUserRepository.GetPartnerUsers().FirstOrDefault(pu => pu.PartnerId == eventPartner.PartnerId && pu.UserId == currentUser.Id);

                if (currentUserPartner == null)
                {
                    return Forbid();
                }
            }

            eventPartner.LastUpdatedByUserId = currentUser.Id;

            var updatedEventPartner = await eventPartnerRepository.UpdateEventPartner(eventPartner).ConfigureAwait(false);

            var user = await userRepository.GetUserByInternalId(eventPartner.CreatedByUserId).ConfigureAwait(false);

            // Notify Admins that a partner request has been responded to
            var subject = "A partner request for an event has been responded to!";
            var message = $"A partner request for an event has been responded to for event {eventPartner.EventId}!";
            var htmlMessage = $"A partner request for an event has been responded to for event {eventPartner.EventId}!";

            var recipients = new List<EmailAddress>
            {
                new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress }
            };

            await emailManager.SendSystemEmail(subject, message, htmlMessage, recipients, CancellationToken.None).ConfigureAwait(false);

            var partnerMessage = emailManager.GetEmailTemplate(NotificationTypeEnum.EventPartnerResponse.ToString());
            var partnerHtmlMessage = emailManager.GetHtmlEmailTemplate(NotificationTypeEnum.EventPartnerResponse.ToString());
            var partnerSubject = "A TrashMob.eco Partner has responded to your request!";

            partnerMessage = partnerMessage.Replace("{UserName}", user.UserName);
            partnerHtmlMessage = partnerHtmlMessage.Replace("{UserName}", user.UserName);

            var dashboardLink = string.Format("https://www.trashmob.eco/manageeventdashboard/{0}", eventPartner.EventId);
            partnerMessage = partnerMessage.Replace("{PartnerResponseUrl}", dashboardLink);
            partnerHtmlMessage = partnerHtmlMessage.Replace("{PartnerResponseUrl}", dashboardLink);

            var partnerRecipients = new List<EmailAddress>
            {
                new EmailAddress { Name = user.UserName, Email = user.Email },
            };

            await emailManager.SendSystemEmail(partnerSubject, partnerMessage, partnerHtmlMessage, partnerRecipients, CancellationToken.None).ConfigureAwait(false);

            return Ok(updatedEventPartner);
        }

        [HttpPost]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PostEventPartner(EventPartner eventPartner)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);
            if (currentUser == null || !ValidateUser(currentUser.NameIdentifier))
            {
                return Forbid();
            }

            eventPartner.CreatedByUserId = currentUser.Id;
            eventPartner.LastUpdatedByUserId = currentUser.Id;
            eventPartner.CreatedDate = DateTimeOffset.UtcNow;
            eventPartner.LastUpdatedDate = DateTimeOffset.UtcNow;
            await eventPartnerRepository.AddEventPartner(eventPartner).ConfigureAwait(false);

            var partnerLocation = partnerLocationRepository.GetPartnerLocations().FirstOrDefault(pl => pl.Id == eventPartner.PartnerLocationId);

            // Notify Admins that a new partner request has been made
            var subject = "A New Partner Request for an Event has been made!";
            var message = $"A new partner request for an event has been made for event {eventPartner.EventId}!";
            var htmlMessage = $"A new partner request for an event has been made for event {eventPartner.EventId}!";

            var recipients = new List<EmailAddress>
            {
                new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress }
            };

            await emailManager.SendSystemEmail(subject, message, htmlMessage, recipients, CancellationToken.None).ConfigureAwait(false);

            // Send welcome email to new User
            var partnerMessage = emailManager.GetEmailTemplate(NotificationTypeEnum.EventPartnerRequest.ToString());
            var htmlPartnerMessage = emailManager.GetHtmlEmailTemplate(NotificationTypeEnum.EventPartnerRequest.ToString());
            var partnerSubject = "A TrashMob.eco Event would like to Partner with you!";

            partnerMessage = partnerMessage.Replace("{PartnerLocationName}", partnerLocation.Name);
            htmlPartnerMessage = htmlPartnerMessage.Replace("{PartnerLocationName}", partnerLocation.Name);

            var partnerRecipients = new List<EmailAddress>
            {
                new EmailAddress { Name = partnerLocation.Name, Email = partnerLocation.PrimaryEmail },
                new EmailAddress { Name = partnerLocation.Name, Email = partnerLocation.SecondaryEmail }
            };

            await emailManager.SendSystemEmail(partnerSubject, partnerMessage, htmlPartnerMessage, partnerRecipients, CancellationToken.None).ConfigureAwait(false);

            return Ok();
        }

        // Ensure the user calling in is the owner of the record
        private bool ValidateUser(string userId)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId == nameIdentifier;
        }
    }
}
