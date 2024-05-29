namespace TrashMob.Controllers
{
    using System;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    [ApiController]
    public abstract class SecureController : BaseController
    {
        private IAuthorizationService authorizationService;

        public SecureController()
        {
        }

        public SecureController(TelemetryClient telemetryClient) : base(telemetryClient)
        {
        }

        public SecureController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        public SecureController(TelemetryClient telemetryClient, IAuthorizationService authorizationService) : base(
            telemetryClient)
        {
            this.authorizationService = authorizationService;
        }

        protected IAuthorizationService AuthorizationService
        {
            get => authorizationService ??
                   (authorizationService = HttpContext.RequestServices.GetService<IAuthorizationService>());
            private set => authorizationService = value;
        }

        protected Guid UserId => new(HttpContext.Items["UserId"].ToString());
    }
}