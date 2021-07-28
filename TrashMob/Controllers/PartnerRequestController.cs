namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Route("api/partnerrequests")]
    public class PartnerRequestsController : ControllerBase
    {
        private readonly IPartnerRequestRepository partnerRequestRepository;
        private readonly IEmailManager emailManager;

        public PartnerRequestsController(IPartnerRequestRepository partnerRequestRepository, IEmailManager emailManager)
        {
            this.partnerRequestRepository = partnerRequestRepository;
            this.emailManager = emailManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerRequest(PartnerRequest partnerRequest)
        {
            await partnerRequestRepository.AddPartnerRequest(partnerRequest);
            var email = new Email
            {
                Message = $"From Email: {partnerRequest.PrimaryEmail}\nFrom Name:{partnerRequest.Name}\nMessage:\n{partnerRequest.Notes}",
                Subject = "Partner Request"
            };
            email.Addresses.Add(new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress });
           
            await emailManager.SendSystemEmail(email, CancellationToken.None).ConfigureAwait(false);
            
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetPartnerRequests()
        {
            return Ok(await partnerRequestRepository.GetPartnerRequests().ConfigureAwait(false));
        }
    }
}
