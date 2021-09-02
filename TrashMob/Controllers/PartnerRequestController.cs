namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Authorize]
    [Route("api/partnerrequests")]
    public class PartnerRequestsController : ControllerBase
    {
        private readonly IPartnerRequestRepository partnerRequestRepository;
        private readonly IPartnerManager partnerManager;
        private readonly IUserRepository userRepository;
        private readonly IEmailManager emailManager;

        public PartnerRequestsController(IPartnerRequestRepository partnerRequestRepository, 
                                         IPartnerManager partnerManager,
                                         IEmailManager emailManager, 
                                         IUserRepository userRepository)
        {
            this.partnerRequestRepository = partnerRequestRepository;
            this.partnerManager = partnerManager;
            this.emailManager = emailManager;
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerRequest(PartnerRequest partnerRequest)
        {
            var user = await userRepository.GetUserByInternalId(partnerRequest.CreatedByUserId).ConfigureAwait(false);

            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            await partnerRequestRepository.AddPartnerRequest(partnerRequest);

            var message = $"From Email: {partnerRequest.PrimaryEmail}\nFrom Name:{partnerRequest.Name}\nMessage:\n{partnerRequest.Notes}";
            var subject = "Partner Request";

            var recipients = new List<EmailAddress>
            {
                new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress }
            };

            await emailManager.SendGenericSystemEmail(subject, message, recipients, CancellationToken.None).ConfigureAwait(false);
            
            return Ok();
        }

        [HttpPut("approve/{partnerRequestId}")]
        public async Task<IActionResult> ApprovePartnerRequest(Guid partnerRequestId)
        {
            var user = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (!user.IsSiteAdmin)
            {
                return Forbid();
            }

            var partnerRequest = await partnerRequestRepository.GetPartnerRequest(partnerRequestId).ConfigureAwait(false);
            partnerRequest.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Approved;

            await partnerRequestRepository.UpdatePartnerRequest(partnerRequest);

            await partnerManager.CreatePartner(partnerRequest);

            return Ok();
        }

        [HttpPut("deny/{partnerRequestId}")]
        public async Task<IActionResult> DenyPartnerRequest(Guid partnerRequestId)
        {
            var user = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (!user.IsSiteAdmin)
            {
                return Forbid();
            }

            var partnerRequest = await partnerRequestRepository.GetPartnerRequest(partnerRequestId).ConfigureAwait(false);
            
            partnerRequest.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Denied;

            await partnerRequestRepository.UpdatePartnerRequest(partnerRequest);

            // Update this to notify user when their request has been denied and what to do next
            //var email = new Email
            //{
            //    Message = $"From Email: {partnerRequest.PrimaryEmail}\nFrom Name:{partnerRequest.Name}\nMessage:\n{partnerRequest.Notes}",
            //    Subject = "Partner Request"
            //};
            //email.Addresses.Add(new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress });

            //await emailManager.SendSystemEmail(email, CancellationToken.None).ConfigureAwait(false);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetPartnerRequests()
        {
            var user = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (!user.IsSiteAdmin)
            {
                return Forbid();
            }

            return Ok(await partnerRequestRepository.GetPartnerRequests().ConfigureAwait(false));
        }

        private bool ValidateUser(string userId)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId == nameIdentifier;
        }
    }
}
