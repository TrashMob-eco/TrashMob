
namespace TrashMob.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.ApplicationInsights;

    [Route("api/eventmedias")]
    public class EventMediasController : BaseController
    {
        private readonly IUserRepository userRepository;
        private readonly IEventMediaRepository eventMediaRepository;

        public EventMediasController(IUserRepository userRepository,
                                     IEventMediaRepository eventMediaRepository, 
                                     TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.userRepository = userRepository;
            this.eventMediaRepository = eventMediaRepository;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventMedias(Guid eventId, CancellationToken cancellationToken)
        {
            var result = await eventMediaRepository.GetEventMediasByEvent(eventId, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("byUserId/{userId}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobReadScope)]
        public async Task<IActionResult> GetEventMediasByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByInternalId(userId, cancellationToken).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            var result = await eventMediaRepository.GetEventMediasByUser(userId, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet()]
        public async Task<IActionResult> GetEventMedias(CancellationToken cancellationToken)
        {
            var result = await eventMediaRepository.GetEventMedias(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("bymediaid/{eventMediaId}")]
        public async Task<IActionResult> GetEventMediaById(Guid eventMediaId, CancellationToken cancellationToken)
        {
            var result = await eventMediaRepository.GetEventMediaById(eventMediaId, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPut("{userId}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateEventMedia(Guid userId, IList<EventMedia> eventMedias)
        {
            var user = await userRepository.GetUserByInternalId(userId).ConfigureAwait(false);

            if (!ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            foreach (var eventMedia in eventMedias)
            {
                await eventMediaRepository.AddUpdateEventMedia(eventMedia).ConfigureAwait(false);
            }

            TelemetryClient.TrackEvent(nameof(UpdateEventMedia));

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteEventMedia(Guid id)
        {
            var eventMedia = await eventMediaRepository.GetEventMediaById(id).ConfigureAwait(false);
            var user = await userRepository.GetUserByInternalId(eventMedia.CreatedByUserId).ConfigureAwait(false);

            if (!ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            await eventMediaRepository.DeleteEventMedia(id).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeleteEventMedia));

            return Ok(id);
        }
    }
}
