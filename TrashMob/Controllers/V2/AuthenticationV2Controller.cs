namespace TrashMob.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 controller for Active Directory authentication operations.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/authentication")]
    public class AuthenticationV2Controller(
        IActiveDirectoryManager activeDirectoryManager,
        ILogger<AuthenticationV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Validates whether a new user can be created in Active Directory.
        /// </summary>
        /// <param name="request">The new user details to validate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>OK if valid, 409 if validation error, 500 if failed.</returns>
        [HttpPost("validateuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActiveDirectoryValidationFailedResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ActiveDirectoryBlockingResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidateUser([FromBody] ActiveDirectoryValidateNewUserRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ValidateUser request received for email: {Email}", request?.email);

            var createResponse = await activeDirectoryManager.ValidateNewUserAsync(request, cancellationToken);

            return createResponse.action switch
            {
                "ValidationError" => Conflict(new ActiveDirectoryValidationFailedResponse
                {
                    action = createResponse.action,
                    version = createResponse.version,
                    userMessage = (createResponse as ActiveDirectoryValidationFailedResponse)?.userMessage,
                    status = ((int)StatusCodes.Status409Conflict).ToString()
                }),
                "Failed" => StatusCode(StatusCodes.Status500InternalServerError, createResponse as ActiveDirectoryBlockingResponse),
                _ => Ok()
            };
        }

        /// <summary>
        /// Creates a new user in Active Directory.
        /// </summary>
        /// <param name="request">The new user details used to create the account.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>OK if created, 409 if validation error, 500 if failed.</returns>
        [HttpPost("signupuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActiveDirectoryValidationFailedResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ActiveDirectoryBlockingResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SignUpUser([FromBody] ActiveDirectoryNewUserRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 SignUpUser request received for email: {Email}", request?.email);

            var createResponse = await activeDirectoryManager.CreateUserAsync(request, cancellationToken);

            return createResponse.action switch
            {
                "ValidationError" => Conflict(new ActiveDirectoryValidationFailedResponse
                {
                    action = createResponse.action,
                    version = createResponse.version,
                    userMessage = (createResponse as ActiveDirectoryValidationFailedResponse)?.userMessage,
                    status = ((int)StatusCodes.Status409Conflict).ToString()
                }),
                "Failed" => StatusCode(StatusCodes.Status500InternalServerError, createResponse as ActiveDirectoryBlockingResponse),
                _ => Ok()
            };
        }

        /// <summary>
        /// Validates updates to an existing user's profile in Active Directory.
        /// </summary>
        /// <param name="request">The updated user profile details to validate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>OK if valid, 404 if user not found, 409 if validation error, 500 if failed.</returns>
        [HttpPost("validateuserprofile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActiveDirectoryValidationFailedResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActiveDirectoryValidationFailedResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ActiveDirectoryBlockingResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidateUserProfile([FromBody] ActiveDirectoryUpdateUserProfileRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ValidateUserProfile request received for objectId: {ObjectId}", request?.objectId);

            var validateResponse = await activeDirectoryManager.ValidateUpdateUserProfileAsync(request, cancellationToken);

            return validateResponse.action switch
            {
                "UserNotFound" => NotFound(validateResponse as ActiveDirectoryValidationFailedResponse),
                "ValidationError" => Conflict(new ActiveDirectoryValidationFailedResponse
                {
                    action = validateResponse.action,
                    version = validateResponse.version,
                    userMessage = (validateResponse as ActiveDirectoryValidationFailedResponse)?.userMessage,
                    status = ((int)StatusCodes.Status409Conflict).ToString()
                }),
                "Failed" => StatusCode(StatusCodes.Status500InternalServerError, validateResponse as ActiveDirectoryBlockingResponse),
                _ => Ok()
            };
        }

        /// <summary>
        /// Updates an existing user's profile in Active Directory.
        /// </summary>
        /// <param name="request">The updated user profile details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>OK if updated, 404 if user not found, 409 if validation error, 500 if failed.</returns>
        [HttpPost("updateuserprofile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActiveDirectoryValidationFailedResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActiveDirectoryValidationFailedResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ActiveDirectoryBlockingResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserProfile([FromBody] ActiveDirectoryUpdateUserProfileRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateUserProfile request received for objectId: {ObjectId}", request?.objectId);

            var updateResponse = await activeDirectoryManager.UpdateUserProfileAsync(request, cancellationToken);

            return updateResponse.action switch
            {
                "UserNotFound" => NotFound(updateResponse as ActiveDirectoryValidationFailedResponse),
                "ValidationError" => Conflict(new ActiveDirectoryValidationFailedResponse
                {
                    action = updateResponse.action,
                    version = updateResponse.version,
                    userMessage = (updateResponse as ActiveDirectoryValidationFailedResponse)?.userMessage,
                    status = ((int)StatusCodes.Status409Conflict).ToString()
                }),
                "Failed" => StatusCode(StatusCodes.Status500InternalServerError, updateResponse as ActiveDirectoryBlockingResponse),
                _ => Ok()
            };
        }

        /// <summary>
        /// Deletes a user from Active Directory.
        /// </summary>
        /// <param name="request">The request containing the object ID of the user to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>OK if deleted or user not found, 500 if failed.</returns>
        [HttpPost("deleteuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActiveDirectoryValidationFailedResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActiveDirectoryBlockingResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser([FromBody] ActiveDirectoryDeleteUserRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Deleting User with objectId {ObjectId}", request?.objectId);

            try
            {
                var deleteResponse = await activeDirectoryManager.DeleteUserAsync(request.objectId, cancellationToken);

                return deleteResponse.action switch
                {
                    "UserNotFound" => Ok(deleteResponse as ActiveDirectoryValidationFailedResponse),
                    _ => Ok(deleteResponse)
                };
            }
            catch (Exception ex)
            {
                var blockingResponse = new ActiveDirectoryBlockingResponse
                {
                    action = "Failed",
                    version = "1.0.0",
                    userMessage = "User failed to delete.",
                };

                logger.LogError(ex, "V2 User with objectId {ObjectId} failed to delete. Message: {Message}, InnerException: {InnerException}",
                    request?.objectId, ex.Message, ex.InnerException);

                return StatusCode(StatusCodes.Status500InternalServerError, blockingResponse);
            }
        }
    }
}
