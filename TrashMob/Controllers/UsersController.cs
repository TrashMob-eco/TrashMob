namespace TrashMob.Controllers
{
    using System;
    using System.Security.Claims;
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
            var result = await userManager.GetAsync(cancellationToken).ConfigureAwait(false);
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
            var user = await userManager.GetUserByUserNameAsync(userName, cancellationToken).ConfigureAwait(false);

            if (user == null)
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
            var user = await userManager.GetUserByEmailAsync(email, cancellationToken).ConfigureAwait(false);

            if (user == null)
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
            var user = await userManager.GetUserByUserNameAsync(userName, cancellationToken).ConfigureAwait(false);

            if (user == null)
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
        /// Retrieves a user by their internal ID.
        /// </summary>
        /// <param name="id">The internal ID of the user.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>User details if found.</remarks>
        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetUserByInternalId(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await userManager.GetUserByInternalIdAsync(id, cancellationToken).ConfigureAwait(false);

            if (user == null)
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
            try
            {
                var updatedUser = await userManager.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
                TrackEvent("UpdateUser");
                return Ok(updatedUser);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await userManager.UserExistsAsync(user.Id, cancellationToken).ConfigureAwait(false))
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
            await userManager.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
            TrackEvent(nameof(DeleteUser));

            return Ok(id);
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
            var user = await userManager.GetUserByInternalIdAsync(userId, cancellationToken).ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            var impactStats = await metricsManager.GetUserImpactStatsAsync(userId, cancellationToken).ConfigureAwait(false);
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
            var user = await userManager.GetUserByInternalIdAsync(UserId, cancellationToken).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound();
            }

            // Delete existing profile photo if present
            if (!string.IsNullOrEmpty(user.ProfilePhotoUrl))
            {
                await imageManager.DeleteImage(UserId, ImageTypeEnum.UserProfilePhoto);
            }

            // Upload new photo to blob storage
            imageUpload.ParentId = UserId;
            imageUpload.ImageType = ImageTypeEnum.UserProfilePhoto;
            await imageManager.UploadImage(imageUpload);

            // Get the reduced-size URL and update user
            var imageUrl = await imageManager.GetImageUrlAsync(UserId, ImageTypeEnum.UserProfilePhoto, ImageSizeEnum.Reduced, cancellationToken);
            user.ProfilePhotoUrl = imageUrl;

            var updatedUser = await userManager.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
            TrackEvent(nameof(UploadProfilePhoto));
            return Ok(updatedUser);
        }

        private bool ValidateUser(string userId)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId == nameIdentifier;
        }
    }
}