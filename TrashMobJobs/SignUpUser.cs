
namespace TrashMobJobs
{
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Net;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.Extensions.Logging;
    using TrashMob.Poco;
    using TrashMob.Shared.Managers.Interfaces;

    public class SignUpUser
    {
        private readonly ILogger logger;
        private readonly IActiveDirectoryManager activeDirectoryManager;

        public SignUpUser(ILoggerFactory loggerFactory, IActiveDirectoryManager activeDirectoryManager)
        {
            logger = loggerFactory.CreateLogger<SignUpValidate>();
            this.activeDirectoryManager = activeDirectoryManager;
        }

        [Function("SignUpUser")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var json = await req.ReadAsStringAsync();

            logger.LogInformation(json);

            var activeDirectoryNewUserRequest = JsonSerializer.Deserialize<ActiveDirectoryNewUserRequest>(json);
            var createResponse = await activeDirectoryManager.CreateUserAsync(activeDirectoryNewUserRequest);

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
                    // Yes, really. It needs an Ok when failed...
                    response = req.CreateResponse(HttpStatusCode.InternalServerError);
                    var blockingResponse = createResponse as ActiveDirectoryBlockingResponse;
                    response.WriteString(JsonSerializer.Serialize(blockingResponse));
                    break;
                default:
                    response = req.CreateResponse(HttpStatusCode.OK);
                    response.WriteString(JsonSerializer.Serialize(createResponse));
                    break;
            }

            return response;
        }
    }
}
