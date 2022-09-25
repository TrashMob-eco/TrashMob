namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected TelemetryClient TelemetryClient { get; }

        public IUserRepository UserRepository { get; }

        public BaseController(TelemetryClient telemetryClient, IUserRepository userRepository)
        {
            TelemetryClient = telemetryClient;
            UserRepository = userRepository;
        }

        // Ensure the user calling in is the owner of the record
        protected virtual bool ValidateUser(string userId)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId == nameIdentifier;
        }

        public async Task<User> GetUser()
        {
            return await UserRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);
        }
    }
}
