namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Route("api/partners")]
    public class PartnersController : ControllerBase
    {
        private readonly IPartnerRepository partnerRepository;

        public PartnersController(IPartnerRepository partnerRepository)
        {
            this.partnerRepository = partnerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPartners()
        {
            return Ok(await partnerRepository.GetPartners().ConfigureAwait(false));
        }

        [HttpGet("{partnerId}")]
        public async Task<IActionResult> GetPartner(Guid partnerId)
        {
            return Ok(await partnerRepository.GetPartner(partnerId).ConfigureAwait(false));
        }
    }
}
