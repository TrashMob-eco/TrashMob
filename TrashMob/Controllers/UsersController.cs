namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Controller for managing users, including retrieval, update, and deletion operations.
    /// </summary>
    [Route("api/users")]
    public class UsersController(IUserManager userManager, IEventAttendeeMetricsManager metricsManager, IImageManager imageManager) : SecureController
    {
        /// <summary>
        /// Retrieves all users. Admin access required.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>List of all users.</remarks>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var result = await userManager.GetAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="userName">The username of the user.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>User details if found.</remarks>
        [HttpGet("getuserbyusername/{userName}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetUser(string userName, CancellationToken cancellationToken)
        {
            var user = await userManager.GetUserByUserNameAsync(userName, cancellationToken);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>User details if found.</remarks>
        [HttpGet("getuserbyemail/{email}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            var user = await userManager.GetUserByEmailAsync(email, cancellationToken);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Verifies the uniqueness of a username for a given user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="userName">The username to check.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Conflict if the username is taken by another user, otherwise OK.</remarks>
        [HttpGet("verifyunique/{userId}/{userName}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> VerifyUnique(Guid userId, string userName, CancellationToken cancellationToken)
        {
            var user = await userManager.GetUserByUserNameAsync(userName, cancellationToken);

            if (user is null)
            {
                return Ok();
            }

            if (user.Id != userId)
            {
                return Conflict();
            }

            return Ok();
        }

        /// <summary>
        /// Retrieves a user by their identity provider object ID.
        /// </summary>
        /// <param name="objectId">The object ID from the identity provider.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>User details if found.</remarks>
        [HttpGet("getbyobjectid/{objectId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetUserByObjectId(Guid objectId, CancellationToken cancellationToken)
        {
            var user = await userManager.GetUserByObjectIdAsync(objectId, cancellationToken);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Retrieves a user by their internal ID.
        /// </summary>
        /// <param name="id">The internal ID of the user.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>User details if found.</remarks>
        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetUserByInternalId(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await userManager.GetUserByInternalIdAsync(id, cancellationToken);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Updates a user's details.
        /// </summary>
        /// <param name="user">The user object with updated details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Updated user details.</remarks>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> PutUser(User user, CancellationToken cancellationToken)
        {
            // Users can only update themselves unless they are a site admin
            var currentUser = await userManager.GetUserByInternalIdAsync(UserId, cancellationToken);

            if (currentUser is null)
            {
                return NotFound();
            }

            if (user.Id != UserId && !currentUser.IsSiteAdmin)
            {
                return Forbid();
            }

            try
            {
                var updatedUser = await userManager.UpdateAsync(user, cancellationToken);
                TrackEvent("UpdateUser");
                return Ok(updatedUser);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await userManager.UserExistsAsync(user.Id, cancellationToken))
                {
                    return NotFound();
                }

                throw;
            }
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The ID of the deleted user.</remarks>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
        {
            // Users can only delete themselves unless they are a site admin
            var currentUser = await userManager.GetUserByInternalIdAsync(UserId, cancellationToken);

            if (currentUser is null)
            {
                return NotFound();
            }

            if (id != UserId && !currentUser.IsSiteAdmin)
            {
                return Forbid();
            }

            await userManager.DeleteAsync(id, cancellationToken);
            TrackEvent(nameof(DeleteUser));

            return NoContent();
        }

        /// <summary>
        /// Gets a user's personal impact statistics across all events.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>User's aggregated impact statistics from approved attendee metrics.</remarks>
        [HttpGet("{userId}/impact")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        public async Task<IActionResult> GetUserImpact(Guid userId, CancellationToken cancellationToken)
        {
            var user = await userManager.GetUserByInternalIdAsync(userId, cancellationToken);

            if (user is null)
            {
                return NotFound();
            }

            var impactStats = await metricsManager.GetUserImpactStatsAsync(userId, cancellationToken);
            return Ok(impactStats);
        }

        /// <summary>
        /// Uploads a profile photo for the current user.
        /// </summary>
        /// <param name="imageUpload">The image upload data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Updated user with new profile photo URL.</remarks>
        [HttpPost("photo")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UploadProfilePhoto(
            [FromForm] ImageUpload imageUpload,
            CancellationToken cancellationToken)
        {
            var user = await userManager.GetUserByInternalIdAsync(UserId, cancellationToken);
            if (user is null)
            {
                return NotFound();
            }

            // Delete existing profile photo if present
            if (!string.IsNullOrWhiteSpace(user.ProfilePhotoUrl))
            {
                await imageManager.DeleteImageAsync(UserId, ImageTypeEnum.UserProfilePhoto);
            }

            // Upload new photo to blob storage
            imageUpload.ParentId = UserId;
            imageUpload.ImageType = ImageTypeEnum.UserProfilePhoto;
            await imageManager.UploadImageAsync(imageUpload);

            // Get the reduced-size URL and update user
            var imageUrl = await imageManager.GetImageUrlAsync(UserId, ImageTypeEnum.UserProfilePhoto, ImageSizeEnum.Reduced, cancellationToken);
            user.ProfilePhotoUrl = imageUrl;

            var updatedUser = await userManager.UpdateAsync(user, cancellationToken);
            TrackEvent(nameof(UploadProfilePhoto));
            return Ok(updatedUser);
        }

    }
}