namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected TelemetryClient TelemetryClient { get; }

        public BaseController(TelemetryClient telemetryClient)
        {
            TelemetryClient = telemetryClient;
        }
    }
}
