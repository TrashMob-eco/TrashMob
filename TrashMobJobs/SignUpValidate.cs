
namespace TrashMobJobs
{
    //using System;
    //using System.Linq;
    //using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Net;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    public class SignUpValidate
    {
        private readonly ILogger logger;
        private readonly IActiveDirectoryManager activeDirectoryManager;

        public SignUpValidate(ILoggerFactory loggerFactory, IActiveDirectoryManager activeDirectoryManager)
        {
            logger = loggerFactory.CreateLogger<SignUpValidate>();
            this.activeDirectoryManager = activeDirectoryManager;
        }

        [Function("SignUpValidate")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var json = await req.ReadAsStringAsync();

            logger.LogInformation(json);

            var activeDirectoryNewUserRequest = JsonSerializer.Deserialize<ActiveDirectoryValidateNewUserRequest>(json);
            var createResponse = await activeDirectoryManager.ValidateNewUserAsync(activeDirectoryNewUserRequest);

            HttpResponseData response;
            switch (createResponse.action)
            {
                case "ValidationError":
                    response = req.CreateResponse(HttpStatusCode.Conflict);
                    var validationResponse = createResponse as ActiveDirectoryValidationFailedResponse;
                    validationResponse.status = ((int)HttpStatusCode.Conflict).ToString();
                    response.WriteString(JsonSerializer.Serialize(validationResponse));
                    break;
                case "Failed":
                    response = req.CreateResponse(HttpStatusCode.InternalServerError);
                    var blockingResponse = createResponse as ActiveDirectoryBlockingResponse;
                    response.WriteString(JsonSerializer.Serialize(blockingResponse));
                    break;
                default:
                    response = req.CreateResponse(HttpStatusCode.OK);
                    break;
            }

            return response;
        }
    }
}
