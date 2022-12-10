namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private TelemetryClient telemetryClient;

        protected TelemetryClient TelemetryClient
        {
            get
            {
                return telemetryClient ?? (telemetryClient = HttpContext.RequestServices.GetService<TelemetryClient>());
            }
            private set
            {
                telemetryClient = value;
            }
        }
        public BaseController()
        {
        }

        public BaseController(TelemetryClient telemetryClient)
        {
            TelemetryClient = telemetryClient;
        }
    }
}
