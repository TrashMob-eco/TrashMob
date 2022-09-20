namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;
    using TrashMob.Shared;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/docusign")]
    public class DocusignController : BaseController
    {
        private readonly IUserRepository userRepository;
        private readonly IDocusignManager docusignManager;

        public DocusignController(TelemetryClient telemetryClient, IUserRepository userRepository, IDocusignManager docusignManager)
            : base(telemetryClient)
        {
            this.userRepository = userRepository;
            this.docusignManager = docusignManager;
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
            var result = docusignManager.SendEnvelope(envelope);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{userId}/{envelopeId}")]
        public async Task<IActionResult> GetEnvelopeStatus(Guid userId, string envelopeId)
        {
            var user = await userRepository.GetUserByInternalId(userId).ConfigureAwait(false);
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            // Get the Envelope Status
            var result = await docusignManager.GetEnvelopeStatus(envelopeId);

            return Ok(result);
        }
    }
}
