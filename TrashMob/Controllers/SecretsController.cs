namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/secrets")]
    public class SecretsController : SecureController
    {
        private readonly ISecretRepository secretRepository;

        public SecretsController(ISecretRepository secretRepository) : base()
        {
            this.secretRepository = secretRepository;
        }

        [HttpGet]
        [Authorize(Policy = "ValidUser")]
        [Route("{name}")]
        public async Task<IActionResult> GetSecret(string name)
        {
            var secret = await Task.FromResult(secretRepository.Get(name));
            return Ok(secret);
        }
    }
}
