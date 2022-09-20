namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/secrets")]
    public class SecretsController : BaseController
    {
        private readonly ISecretRepository secretRepository;

        public SecretsController(ISecretRepository secretRepository,
                                 TelemetryClient telemetryClient)
            : base(telemetryClient)
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
