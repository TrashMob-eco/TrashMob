namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    [Route("api/pickuplocations")]
    public class PickupLocationsController(
        IPickupLocationManager pickupLocationManager,
        IEventManager eventManager,
        IImageManager imageManager)
        : KeyedController<PickupLocation>(pickupLocationManager)
    {
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
            var localPickupLocation = await Manager.GetAsync(pickupLocation.Id, cancellationToken);

            // Does the user own the pickup location?
            var authResult =
                await AuthorizationService.AuthorizeAsync(User, localPickupLocation,
                    AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                // Does the user own the event?
                var mobEvent = await eventManager.GetAsync(pickupLocation.EventId, cancellationToken);

                authResult =
                    await AuthorizationService.AuthorizeAsync(User, mobEvent,
                        AuthorizationPolicyConstants.UserOwnsEntity);

                if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
                {
                    return Forbid();
                }
            }

            localPickupLocation.County = pickupLocation.County;
            localPickupLocation.Latitude = pickupLocation.Latitude;
            localPickupLocation.Longitude = pickupLocation.Longitude;
            localPickupLocation.Name = pickupLocation.Name;
            localPickupLocation.Notes = pickupLocation.Notes;
            localPickupLocation.PostalCode = pickupLocation.PostalCode;
            localPickupLocation.Region = pickupLocation.Region;
            localPickupLocation.StreetAddress = pickupLocation.StreetAddress;
            localPickupLocation.Country = pickupLocation.Country;
            localPickupLocation.City = pickupLocation.City;
            localPickupLocation.HasBeenPickedUp = pickupLocation.HasBeenPickedUp;
            localPickupLocation.HasBeenSubmitted = pickupLocation.HasBeenSubmitted; 

            var result = await Manager.UpdateAsync(localPickupLocation, UserId, cancellationToken).ConfigureAwait(false);
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

            await pickupLocationManager.MarkAsPickedUpAsync(pickupLocationId, UserId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent("MarkAsPickedUp");

            return Ok();
        }

        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public override async Task<IActionResult> Add(PickupLocation instance, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(instance.EventId, cancellationToken);

            var authResult =
                await AuthorizationService.AuthorizeAsync(User, mobEvent, AuthorizationPolicyConstants.UserOwnsEntity);

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

            var authResult =
                await AuthorizationService.AuthorizeAsync(User, mobEvent, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await pickupLocationManager.SubmitPickupLocations(eventId, UserId, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent("SubmitPickupLocations");

            return Ok();
        }

        [HttpPost("image/{eventId}")]
        public async Task<IActionResult> UploadImage([FromForm] ImageUpload imageUpload, Guid eventId,
            CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            var authResult =
                await AuthorizationService.AuthorizeAsync(User, mobEvent, AuthorizationPolicyConstants.UserOwnsEntity);

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
            var url = await imageManager.GetImageUrlAsync(pickupLocationId, ImageTypeEnum.Pickup, ImageSizeEnum.Raw, cancellationToken);

            if (string.IsNullOrEmpty(url))
            {
                return NoContent();
            }

            return Ok(url);
        }

        [HttpGet("image/{pickupLocationId}/{imageSize}")]
        public async Task<IActionResult> GetImage(Guid pickupLocationId, ImageSizeEnum imageSize,
            CancellationToken cancellationToken)
        {
            var url = await imageManager.GetImageUrlAsync(pickupLocationId, ImageTypeEnum.Pickup, imageSize, cancellationToken);

            if (string.IsNullOrEmpty(url))
            {
                return NoContent();
            }

            return Ok(url);
        }

        [HttpDelete("{id}")]
        public override async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            // Is the user the owner of the pickup location?
            var entity = await Manager.GetAsync(id, cancellationToken);

            var authResult =
                await AuthorizationService.AuthorizeAsync(User, entity, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                // Does the user own the event?
                var mobEvent = await eventManager.GetAsync(entity.EventId, cancellationToken);

                authResult =
                    await AuthorizationService.AuthorizeAsync(User, mobEvent,
                        AuthorizationPolicyConstants.UserOwnsEntity);

                if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
                {
                    return Forbid();
                }
            }

            var results = await Manager.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent("Delete" + nameof(PickupLocation));

            return Ok(results);
        }
    }
}