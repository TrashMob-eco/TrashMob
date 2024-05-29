namespace TrashMobJobs
{
    //using System;
    //using System.Linq;
    //using System.Text;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    public class UpdateUserProfileValidate
    {
        private readonly IActiveDirectoryManager activeDirectoryManager;
        private readonly ILogger logger;

        public UpdateUserProfileValidate(ILoggerFactory loggerFactory, IActiveDirectoryManager activeDirectoryManager)
        {
            logger = loggerFactory.CreateLogger<UpdateUserProfile>();
            this.activeDirectoryManager = activeDirectoryManager;
        }

        [Function("UpdateUserProfileValidate")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var json = await req.ReadAsStringAsync();

            logger.LogInformation(json);

            var activeDirectoryUpdateUserProfileRequest =
                JsonSerializer.Deserialize<ActiveDirectoryUpdateUserProfileRequest>(json);
            var validateResponse =
                await activeDirectoryManager.ValidateUpdateUserProfileAsync(activeDirectoryUpdateUserProfileRequest);

            HttpResponseData response;
            switch (validateResponse.action)
            {
                case "UserNotFound":
                {
                    response = req.CreateResponse(HttpStatusCode.NotFound);
                    var validationResponse = validateResponse as ActiveDirectoryValidationFailedResponse;
                    response.WriteString(JsonSerializer.Serialize(validationResponse));
                    logger.LogError(
                        $"User with objectId {activeDirectoryUpdateUserProfileRequest.objectId} was not found.");
                    break;
                }
                case "ValidationError":
                {
                    response = req.CreateResponse(HttpStatusCode.Conflict);
                    var validationResponse = validateResponse as ActiveDirectoryValidationFailedResponse;
                    validationResponse.status = ((int)HttpStatusCode.Conflict).ToString();
                    response.WriteString(JsonSerializer.Serialize(validationResponse));
                    break;
                }
                case "Failed":
                {
                    response = req.CreateResponse(HttpStatusCode.InternalServerError);
                    var blockingResponse = validateResponse as ActiveDirectoryBlockingResponse;
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