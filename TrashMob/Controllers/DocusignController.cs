namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Common;
    using TrashMob.Docusign;
    using TrashMob.Shared.Persistence;

    [Route("api/docusign")]
    public class DocusignController : BaseController
    {
        private readonly IUserRepository userRepository;

        public DocusignController(TelemetryClient telemetryClient, IUserRepository userRepository)
            : base(telemetryClient)
        {
            this.userRepository = userRepository;
        }

        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> SendEnvelope(EnvelopeRequest envelope)
        {
            var user = await userRepository.GetUserByInternalId(envelope.SignerClientId).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            // Create the Envelope
            var result = CreateEnvelopeFromTemplate.SendEnvelopeFromTemplate(envelope);

            return Ok(result);
        }
    }
}
