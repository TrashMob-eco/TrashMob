namespace TrashMob.Controllers
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Controller for managing application secrets.
    /// </summary>
    [Route("api/secrets")]
    public class SecretsController(ISecretRepository secretRepository)
        : SecureController
    {

        /// <summary>
        /// Gets a secret by name. Requires a valid user.
        /// </summary>
        /// <param name="name">The name of the secret.</param>
        /// <remarks>The secret value.</remarks>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [Route("{name}")]
        public async Task<IActionResult> GetSecret(string name)
        {
            var secret = await Task.FromResult(secretRepository.Get(name));
            return Ok(secret);
        }
    }
}