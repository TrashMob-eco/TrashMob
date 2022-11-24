namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    [Route("api/docusign")]
    public class DocusignController : SecureController
    {
        private readonly IDocusignManager docusignManager;

        public DocusignController(IDocusignManager docusignManager) : base()
        {
            this.docusignManager = docusignManager;
        }

        [HttpPost()]
        public async Task<IActionResult> SendEnvelope(EnvelopeRequest envelope)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, envelope, "UserOwnsEntity");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            // Create the Envelope
            var result = docusignManager.SendEnvelope(envelope);

            return Ok(result);
        }

        [HttpGet("{userId}/{envelopeId}")]
        public async Task<IActionResult> GetEnvelopeStatus(Guid userId, string envelopeId, CancellationToken cancellationToken)
        {
            // This is a cheesy way to do this, but works for now
            var envelope = new EnvelopeRequest { CreatedByUserId = userId };
            var authResult = await AuthorizationService.AuthorizeAsync(User, envelope, "UserOwnsEntityOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            // Get the Envelope Status
            var result = await docusignManager.GetEnvelopeStatusAsync(envelopeId, cancellationToken);

            return Ok(result);
        }
    }
}
