namespace TrashMob.Controllers.V2
{
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// V2 controller for managing application secrets.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/secrets")]
    [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
    public class SecretsV2Controller(
        ISecretRepository secretRepository,
        ILogger<SecretsV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets a secret by name. Requires a valid user.
        /// </summary>
        /// <param name="name">The name of the secret.</param>
        /// <returns>The secret value.</returns>
        /// <response code="200">Returns the secret value.</response>
        [HttpGet("{name}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSecret(string name)
        {
            logger.LogInformation("V2 GetSecret requested for Name={SecretName}", name);

            var secret = await Task.FromResult(secretRepository.Get(name));
            return Ok(secret);
        }
    }
}
