namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;

    [ApiController]
    public abstract class SecureController : BaseController
    {
        protected IAuthorizationService AuthorizationService { get; }

        protected Guid UserId => new(HttpContext.Items["Userid"].ToString());

        public SecureController(TelemetryClient telemetryClient, IAuthorizationService authorizationService)
            : base(telemetryClient)
        {
            AuthorizationService = authorizationService;
        }
    }
}
