
namespace TrashMob.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Shared;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Route("api/eventpartners")]
    public class EventPartnersController : ControllerBase
    {
        private readonly IEventPartnerRepository eventPartnerRepository;
        private readonly IUserRepository userRepository;

        public EventPartnersController(IEventPartnerRepository eventPartnerRepository, IUserRepository userRepository)
        {
            this.eventPartnerRepository = eventPartnerRepository;
            this.userRepository = userRepository;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventPartners(Guid eventId)
        {
            var result = await eventPartnerRepository.GetEventPartners(eventId).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PutEventPartner(EventPartner eventPartner)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);
            if (currentUser == null || !ValidateUser(currentUser.NameIdentifier))
            {
                return Forbid();
            }

            var updatedEventPartner = await eventPartnerRepository.UpdateEventPartner(eventPartner).ConfigureAwait(false);
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

            await eventPartnerRepository.AddEventPartner(eventPartner).ConfigureAwait(false);

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
