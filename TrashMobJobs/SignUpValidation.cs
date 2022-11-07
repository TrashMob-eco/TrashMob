
namespace TrashMobJobs
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Net;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.Extensions.Logging;
    using TrashMob.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using System.Runtime.InteropServices;

    public class SignUpValidation
    {
        private readonly ILogger logger;
        private readonly IActiveDirectoryManager activeDirectoryManager;

        public SignUpValidation(ILoggerFactory loggerFactory, IActiveDirectoryManager activeDirectoryManager)
        {
            logger = loggerFactory.CreateLogger<SignUpValidation>();
            this.activeDirectoryManager = activeDirectoryManager;
        }

        [Function("SignUpValidation")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            // Check HTTP basic authorization
            if (!Authorize(req, logger))
            {
                logger.LogWarning("HTTP basic authentication validation failed.");
                var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                return unauthorizedResponse;
            }

            var json = await req.ReadAsStringAsync();

            logger.LogInformation(json);

            var activeDirectoryNewUserRequest = JsonSerializer.Deserialize<ActiveDirectoryNewUserRequest>(json);
            var createResponse = await activeDirectoryManager.CreateUserAsync(activeDirectoryNewUserRequest);

            var responseCode = HttpStatusCode.OK;
            switch (createResponse.action)
            {
                case "ValidationError":
                    responseCode = HttpStatusCode.BadRequest;
                    createResponse.status = ((int)HttpStatusCode.BadRequest).ToString();
                    break;
                case "Failed":
                    // Yes, really. It needs an Ok when failed...
                    responseCode = HttpStatusCode.OK;
                    break;
                case "Continue":
                    responseCode = HttpStatusCode.OK;
                    break;
            }

            var response = req.CreateResponse(responseCode);
            response.WriteString(JsonSerializer.Serialize(createResponse));
            return response;
        }

        private static bool Authorize(HttpRequestData req, ILogger log)
        {
            // Get the environment's credentials 
            string username = Environment.GetEnvironmentVariable("BASIC_AUTH_USERNAME", EnvironmentVariableTarget.Process);
            string password = Environment.GetEnvironmentVariable("BASIC_AUTH_PASSWORD", EnvironmentVariableTarget.Process);

            // Returns authorized if the username is empty or not exists.
            if (string.IsNullOrEmpty(username))
            {
                log.LogInformation("HTTP basic authentication is not set.");
                return true;
            }

            // Check if the HTTP Authorization header exist
            if (!req.Headers.TryGetValues("Authorization", out var authHeaderValues))
            {
                log.LogWarning("Missing HTTP basic authentication header.");
                return false;
            }

            // Read the authorization header
            // Ensure the type of the authorization header id `Basic`

            var basicHeader = authHeaderValues.FirstOrDefault(v => v.StartsWith("Basic "));

            if (basicHeader == null)
            {
                log.LogWarning("HTTP basic authentication header must start with 'Basic '.");
                return false;
            }

            // Get the the HTTP basic authorization credentials

            var cred = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(basicHeader.Substring(6))).Split(':');

            // Evaluate the credentials and return the result
            return cred[0] == username && cred[1] == password;
        }
    }
}
