namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    [Route("api/pickuplocations")]
    public class PickupLocationsController : KeyedController<PickupLocation>
    {
        private readonly IPickupLocationManager pickupLocationManager;
        private readonly IEventManager eventManager;
        private readonly IImageManager imageManager;

        public PickupLocationsController(IPickupLocationManager pickupLocationManager, IEventManager eventManager, IImageManager imageManager) 
            : base(pickupLocationManager)
        {
            this.pickupLocationManager = pickupLocationManager;
            this.eventManager = eventManager;
            this.imageManager = imageManager;
        }

        [HttpGet("{pickupLocationId}")]
        public async Task<IActionResult> Get(Guid pickupLocationId, CancellationToken cancellationToken)
        {
            return Ok(await Manager.GetAsync(pickupLocationId, cancellationToken).ConfigureAwait(false));
        }

        [HttpGet("getbyevent/{eventId}")]
        public async Task<IActionResult> GetByEvent(Guid eventId, CancellationToken cancellationToken)
        {
            return Ok(await Manager.GetByParentIdAsync(eventId, cancellationToken).ConfigureAwait(false));
        }

        [HttpGet("getbyuser/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId, CancellationToken cancellationToken)
        {
            return Ok(await pickupLocationManager.GetByUserAsync(userId, cancellationToken).ConfigureAwait(false));
        }

        [HttpPut]
        public async Task<IActionResult> Update(PickupLocation pickupLocation, CancellationToken cancellationToken)
        {
            // Todo: Add security
            var authResult = await AuthorizationService.AuthorizeAsync(User, pickupLocation, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await Manager.UpdateAsync(pickupLocation, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(Update) + typeof(PickupLocation));

            return Ok(result);
        }

        [HttpPost("markpickedup/{pickupLocationId}")]
        [Authorize(AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> MarkAsPickedUp(Guid pickupLocationId, CancellationToken cancellationToken)
        {            
            // Todo: Add security
            //var authResult = await AuthorizationService.AuthorizeAsync(User, pickupLocation, AuthorizationPolicyConstants.UserOwnsEntity);

            //if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            //{
            //    return Forbid();
            //}

            await pickupLocationManager.MarkAsPickedUpAsync(pickupLocationId, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent("MarkAsPickedUp");

            return Ok();
        }

        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public override async Task<IActionResult> Add(PickupLocation instance, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(instance.EventId, cancellationToken);

            var authResult = await AuthorizationService.AuthorizeAsync(User, mobEvent, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await Manager.AddAsync(instance, UserId, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent("AddPickupLocation");

            return Ok(result);
        }

        [HttpPost("submit/{eventId}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> SubmitPickupLocations(Guid eventId, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);

            var authResult = await AuthorizationService.AuthorizeAsync(User, mobEvent, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await pickupLocationManager.SubmitPickupLocations(eventId, UserId, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent("SubmitPickupLocations");

            return Ok();
        }

        [HttpPost("image/{eventId}")]
        public async Task<IActionResult> UploadImage([FromForm] ImageUpload imageUpload, Guid eventId, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, mobEvent, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await imageManager.UploadImage(imageUpload);

            return Ok();
        }

        [HttpGet("image/{pickupLocationId}")]
        public async Task<IActionResult> GetImage(Guid pickupLocationId, CancellationToken cancellationToken)
        {
            var url = await imageManager.GetImageUrlAsync(pickupLocationId, ImageTypeEnum.Pickup, ImageSizeEnum.Raw);

            if (string.IsNullOrEmpty(url))
            {
                return NoContent();
            }

            return Ok(url);
        }

        [HttpGet("image/{pickupLocationId}/{imageSize}")]
        public async Task<IActionResult> GetImage(Guid pickupLocationId, ImageSizeEnum imageSize, CancellationToken cancellationToken)
        {
            var url = await imageManager.GetImageUrlAsync(pickupLocationId, ImageTypeEnum.Pickup, imageSize);

            if (string.IsNullOrEmpty(url))
            {
                return NoContent();
            }

            return Ok(url);
        }
    }
}
