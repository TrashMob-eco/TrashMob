namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IActiveDirectoryManager activeDirectoryManager;
        private readonly ILogger<AuthenticationController> logger;

        public AuthenticationController(IActiveDirectoryManager activeDirectoryManager, ILogger<AuthenticationController> logger)
        {
            this.activeDirectoryManager = activeDirectoryManager;
            this.logger = logger;
        }

        [HttpPost("validateuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActiveDirectoryValidationFailedResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ActiveDirectoryBlockingResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidateUser([FromBody] ActiveDirectoryValidateNewUserRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("ValidateUser request received for email: {Email}", request?.email);

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

        [HttpPost("signupuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActiveDirectoryValidationFailedResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ActiveDirectoryBlockingResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SignUpUser([FromBody] ActiveDirectoryNewUserRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("SignUpUser request received for email: {Email}", request?.email);

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

        [HttpPost("validateuserprofile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActiveDirectoryValidationFailedResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActiveDirectoryValidationFailedResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ActiveDirectoryBlockingResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValidateUserProfile([FromBody] ActiveDirectoryUpdateUserProfileRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("ValidateUserProfile request received for objectId: {ObjectId}", request?.objectId);

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

        [HttpPost("updateuserprofile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActiveDirectoryValidationFailedResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActiveDirectoryValidationFailedResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ActiveDirectoryBlockingResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserProfile([FromBody] ActiveDirectoryUpdateUserProfileRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("UpdateUserProfile request received for objectId: {ObjectId}", request?.objectId);

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

        [HttpPost("deleteuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActiveDirectoryValidationFailedResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActiveDirectoryBlockingResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser([FromBody] ActiveDirectoryDeleteUserRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting User with objectId {ObjectId}", request?.objectId);

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

                logger.LogError(ex, "User with objectId {ObjectId} failed to delete. Message: {Message}, InnerException: {InnerException}", 
                    request?.objectId, ex.Message, ex.InnerException);

                return StatusCode(StatusCodes.Status500InternalServerError, blockingResponse);
            }
        }
    }
}