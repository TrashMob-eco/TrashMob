namespace TrashMob.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/secrets")]
    public class SecretsController : SecureController
    {
        private readonly ISecretRepository secretRepository;

        public SecretsController(ISecretRepository secretRepository)
        {
            this.secretRepository = secretRepository;
        }

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [Route("{name}")]
        public async Task<IActionResult> GetSecret(string name)
        {
            var secret = await Task.FromResult(secretRepository.Get(name));
            return Ok(secret);
        }

        [HttpGet]
        [Route("GetCaptcha")]
        public async Task<IActionResult> GetCaptchaSecret()
        {
            var secret = await Task.FromResult(secretRepository.Get("CaptchaSecretKey"));
            return Ok(secret);
        }
    }
}