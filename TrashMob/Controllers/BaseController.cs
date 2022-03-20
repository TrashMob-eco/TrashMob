namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected TelemetryClient TelemetryClient { get; }

        public BaseController(TelemetryClient telemetryClient)
        {
            TelemetryClient = telemetryClient;
        }

        // Ensure the user calling in is the owner of the record
        protected virtual bool ValidateUser(string userId)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId == nameIdentifier;
        }
    }
}
