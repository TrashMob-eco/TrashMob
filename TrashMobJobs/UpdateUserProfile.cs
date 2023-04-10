
namespace TrashMobJobs
{
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Net;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    public class UpdateUserProfile
    {
        private readonly ILogger logger;
        private readonly IActiveDirectoryManager activeDirectoryManager;

        public UpdateUserProfile(ILoggerFactory loggerFactory, IActiveDirectoryManager activeDirectoryManager)
        {
            logger = loggerFactory.CreateLogger<UpdateUserProfile>();
            this.activeDirectoryManager = activeDirectoryManager;
        }

        [Function("UpdateUserProfile")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var json = await req.ReadAsStringAsync();

            logger.LogInformation(json);

            var activeDirectoryUpdateUserProfileRequest = JsonSerializer.Deserialize<ActiveDirectoryUpdateUserProfileRequest>(json);
            var updateResponse = await activeDirectoryManager.UpdateUserProfileAsync(activeDirectoryUpdateUserProfileRequest);

            HttpResponseData response;
            switch (updateResponse.action)
            {
                case "UserNotFound":
                    {
                        response = req.CreateResponse(HttpStatusCode.NotFound);
                        var validationResponse = updateResponse as ActiveDirectoryValidationFailedResponse;
                        response.WriteString(JsonSerializer.Serialize(validationResponse));
                        logger.LogError($"User with objectId {activeDirectoryUpdateUserProfileRequest.objectId} was not found.");
                        break;
                    }
                case "ValidationError":
                    {
                        response = req.CreateResponse(HttpStatusCode.Conflict);
                        var validationResponse = updateResponse as ActiveDirectoryValidationFailedResponse;
                        validationResponse.status = ((int)HttpStatusCode.Conflict).ToString();
                        response.WriteString(JsonSerializer.Serialize(validationResponse));
                        break;
                    }
                case "Failed":
                    {
                        response = req.CreateResponse(HttpStatusCode.InternalServerError);
                        var blockingResponse = updateResponse as ActiveDirectoryBlockingResponse;
                        response.WriteString(JsonSerializer.Serialize(blockingResponse));
                        break;
                    }
                default:
                    response = req.CreateResponse(HttpStatusCode.OK);
                    break;
            }

            return response;
        }
    }
}
