namespace TrashMob.Controllers
{
    using System.Net.Http;
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
        [Route("CheckCaptcha")]
        public async Task<IActionResult> CheckCaptcha([FromQuery] string captchaResponse)
        {
            var secret = await Task.FromResult(secretRepository.Get("CaptchaSecretKey"));

            // Call the Google reCAPTCHA API to verify the user's response
            var url = $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={captchaResponse}";

            // Make the request to the reCAPTCHA API
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(url, null);
            var responseString = await response.Content.ReadAsStringAsync();

            // Return the response from the reCAPTCHA API
            return Ok(responseString);
        }
    }
}