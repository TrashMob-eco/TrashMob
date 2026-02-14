namespace TrashMob.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    public abstract class SecureController : BaseController
    {
        private IAuthorizationService authorizationService;

        public SecureController()
        {
        }

        public SecureController(ILogger logger) : base(logger)
        {
        }

        public SecureController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        public SecureController(ILogger logger, IAuthorizationService authorizationService) : base(logger)
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

        protected async Task<bool> IsAuthorizedAsync(object resource, string policy)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return false;
            }

            var authResult = await AuthorizationService.AuthorizeAsync(User, resource, policy);
            return authResult.Succeeded;
        }
    }
}
