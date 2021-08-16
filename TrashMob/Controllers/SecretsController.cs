namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Route("api/secrets")]
    public class SecretsController : ControllerBase
    {
        private readonly ISecretRepository secretRepository;

        public SecretsController(ISecretRepository secretRepository)
        {
            this.secretRepository = secretRepository;
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> GetSecret(string name)
        {
            var secret = await Task.FromResult(secretRepository.GetSecret(name));
            return Ok(secret);
        }
    }
}
