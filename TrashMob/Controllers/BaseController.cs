namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

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

        public async Task<User> GetUser(IUserRepository userRepository)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);
            return currentUser;
        }
    }
}
