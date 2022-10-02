namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    [ApiController]
    public abstract class SecureController : BaseController
    {
        private IAuthorizationService authorizationService;

        protected IAuthorizationService AuthorizationService
        {
            get
            {
                return authorizationService ?? (authorizationService = HttpContext.RequestServices.GetService<IAuthorizationService>());
            }
            private set
            {
                authorizationService = value;
            }
        }
        public SecureController()
        {
        }

        public SecureController(TelemetryClient telemetryClient) : base(telemetryClient)
        {
        }

        public SecureController(IAuthorizationService authorizationService) : base()
        {
            this.authorizationService = authorizationService;
        }

        public SecureController(TelemetryClient telemetryClient, IAuthorizationService authorizationService) : base(telemetryClient)
        {
            this.authorizationService = authorizationService;
        }

        protected Guid UserId => new(HttpContext.Items["Userid"].ToString());
    }
}
