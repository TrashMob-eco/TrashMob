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

        public SecretsController(TelemetryClient telemetryClient,
                                 IAuthorizationService authorizationService,
                                 ISecretRepository secretRepository)
            : base(telemetryClient, authorizationService)
        {
            this.secretRepository = secretRepository;
        }

        [HttpGet]
        [Authorize(Policy = "ValidUser")]
        [Route("{name}")]
        public async Task<IActionResult> GetSecret(string name)
        {
            var secret = await Task.FromResult(secretRepository.GetSecret(name));
            return Ok(secret);
        }
    }
}
