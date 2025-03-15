namespace TrashMob.Controllers
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/contactrequest")]
    public class ContactRequestController : BaseController
    {
        private readonly IKeyedManager<ContactRequest> manager;
        private readonly ISecretRepository secretRepository;

        public ContactRequestController(IKeyedManager<ContactRequest> manager, ISecretRepository secretRepository)
        {
            this.manager = manager;
            this.secretRepository = secretRepository;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Add([FromQuery] string captchaResponse, [FromBody] ContactRequest instance, CancellationToken cancellationToken)
        {
            var secret = await Task.FromResult(secretRepository.Get("CaptchaSecretKey"));

            // Call the Google reCAPTCHA API to verify the user's response
            var url = $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={captchaResponse}";

            // Make the request to the reCAPTCHA API
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(url, null, cancellationToken: cancellationToken);
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!responseString.Contains("\"success\": true"))
            {
                return BadRequest("Invalid reCAPTCHA response.");
            }

            await manager.AddAsync(instance, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent("AddContactRequest");

            return Ok();
        }
    }
}