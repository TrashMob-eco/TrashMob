namespace TrashMob.Controllers.V2
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 controller for users with server-side pagination and filtering.
    /// Returns PII-safe UserDto (no email, identity provider fields, or date of birth).
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/users")]
    public class UsersV2Controller(
        IUserManager userManager,
        IEventAttendeeMetricsManager metricsManager,
        IImageManager imageManager,
        IUserDataExportManager exportManager,
        ILogger<UsersV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets a paginated list of users with optional filtering.
        /// </summary>
        /// <param name="filter">Query parameters for pagination and filtering.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of users.</returns>
        /// <response code="200">Returns the paginated user list.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsers(
            [FromQuery] UserQueryParameters filter,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetUsers requested with Page={Page}, PageSize={PageSize}",
                filter.Page, filter.PageSize);

            var query = userManager.GetFilteredUsersQueryable(filter);
            var result = await query.ToPagedAsync(filter, u => u.ToV2Dto(), cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Gets a single user by their identifier.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user details (PII-safe).</returns>
        /// <response code="200">Returns the user.</response>
        /// <response code="404">User not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUser(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetUser requested for {UserId}", id);

            var user = await userManager.GetAsync(id, cancellationToken);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user.ToV2Dto());
        }

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="userDto">The user to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">User created.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddUser(UserWriteDto userDto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddUser UserName={UserName}", userDto.UserName);

            var user = userDto.ToEntity();
            var result = await userManager.AddAsync(user, UserId, cancellationToken);
            return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result.ToV2Dto());
        }

        /// <summary>
        /// Updates a user. Users can only update themselves unless they are a site admin.
        /// </summary>
        /// <param name="userDto">The user to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated user.</response>
        /// <response code="403">User is not authorized.</response>
        /// <response code="404">User not found.</response>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(UserWriteDto userDto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateUser User={UserId}", userDto.Id);

            var currentUser = await userManager.GetUserByInternalIdAsync(UserId, cancellationToken);

            if (currentUser is null)
            {
                return NotFound();
            }

            if (userDto.Id != UserId && !currentUser.IsSiteAdmin)
            {
                return Forbid();
            }

            try
            {
                var user = userDto.ToEntity();

                // Preserve server-managed fields that clients may not have
                if (string.IsNullOrEmpty(user.Email))
                {
                    user.Email = currentUser.Email;
                }

                user.ObjectId = currentUser.ObjectId;
                user.IsSiteAdmin = currentUser.IsSiteAdmin;
                user.CreatedByUserId = currentUser.CreatedByUserId;
                user.CreatedDate = currentUser.CreatedDate;

                var updatedUser = await userManager.UpdateAsync(user, cancellationToken);
                return Ok(updatedUser.ToV2Dto());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await userManager.UserExistsAsync(userDto.Id, cancellationToken))
                {
                    return NotFound();
                }

                throw;
            }
        }

        /// <summary>
        /// Gets a user by email address.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the user.</response>
        /// <response code="404">User not found.</response>
        [HttpGet("getuserbyemail/{email}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetUserByEmail");

            var user = await userManager.GetUserByEmailAsync(email, cancellationToken);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user.ToV2Dto());
        }

        /// <summary>
        /// Gets a user by identity provider object ID.
        /// </summary>
        /// <param name="objectId">The object ID from the identity provider.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the user.</response>
        /// <response code="404">User not found.</response>
        [HttpGet("getbyobjectid/{objectId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserByObjectId(Guid objectId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetUserByObjectId ObjectId={ObjectId}", objectId);

            var user = await userManager.GetUserByObjectIdAsync(objectId, cancellationToken);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user.ToV2Dto());
        }

        /// <summary>
        /// Gets a user's personal impact statistics across all events.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns impact statistics.</response>
        /// <response code="404">User not found.</response>
        [HttpGet("{userId}/impact")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserImpact(Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetUserImpact User={UserId}", userId);

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
        /// <response code="200">Returns updated user with new profile photo URL.</response>
        /// <response code="404">User not found.</response>
        [HttpPost("photo")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadProfilePhoto(
            [FromForm] ImageUpload imageUpload,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UploadProfilePhoto");

            var user = await userManager.GetUserByInternalIdAsync(UserId, cancellationToken);
            if (user is null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(user.ProfilePhotoUrl))
            {
                await imageManager.DeleteImageAsync(UserId, ImageTypeEnum.UserProfilePhoto);
            }

            imageUpload.ParentId = UserId;
            imageUpload.ImageType = ImageTypeEnum.UserProfilePhoto;
            await imageManager.UploadImageAsync(imageUpload);

            var imageUrl = await imageManager.GetImageUrlAsync(UserId, ImageTypeEnum.UserProfilePhoto,
                ImageSizeEnum.Reduced, cancellationToken);
            user.ProfilePhotoUrl = imageUrl;

            var updatedUser = await userManager.UpdateAsync(user, cancellationToken);
            return Ok(updatedUser.ToV2Dto());
        }

        /// <summary>
        /// Verifies the uniqueness of a username for a given user ID.
        /// Returns 200 OK if the username is available or already belongs to the specified user.
        /// Returns 409 Conflict if the username is taken by a different user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="userName">The username to check.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Username is available or belongs to the specified user.</response>
        /// <response code="409">Username is taken by another user.</response>
        [HttpGet("verifyunique/{userId}/{userName}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> VerifyUnique(Guid userId, string userName, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 VerifyUnique User={UserId}, UserName={UserName}", userId, userName);

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
        /// Exports all personal data for a user as a JSON file download.
        /// Users can only export their own data unless they are a site admin.
        /// Rate limited to one export per 24 hours.
        /// </summary>
        /// <param name="id">The ID of the user whose data to export.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns a JSON file containing the user's data.</response>
        /// <response code="403">User is not authorized to export this data.</response>
        /// <response code="404">User not found.</response>
        /// <response code="429">Export rate limit exceeded (once per 24 hours).</response>
        [HttpGet("{id}/export")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ExportUserData(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ExportUserData User={UserId}", id);

            var currentUser = await userManager.GetUserByInternalIdAsync(UserId, cancellationToken);

            if (currentUser is null)
            {
                return NotFound();
            }

            if (id != UserId && !currentUser.IsSiteAdmin)
            {
                return Forbid();
            }

            // Rate limit: once per 24 hours
            if (currentUser.LastDataExportRequestedDate.HasValue &&
                currentUser.LastDataExportRequestedDate.Value > DateTimeOffset.UtcNow.AddHours(-24))
            {
                return StatusCode(StatusCodes.Status429TooManyRequests,
                    "Data export is limited to once per 24 hours.");
            }

            // Update rate limit timestamp before streaming (can't change status after body starts)
            currentUser.LastDataExportRequestedDate = DateTimeOffset.UtcNow;
            await userManager.UpdateAsync(currentUser, cancellationToken);

            var timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMdd-HHmmss");
            var ms = new MemoryStream();
            await exportManager.WriteExportToStreamAsync(id, ms, cancellationToken);
            ms.Position = 0;

            return File(ms, "application/json", $"trashmob-data-export-{timestamp}.json");
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// Users can only delete themselves unless they are a site admin.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">User successfully deleted.</response>
        /// <response code="403">User is not authorized to delete this account.</response>
        /// <response code="404">User not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeleteUser User={UserId}", id);

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

            return NoContent();
        }
    }
}
