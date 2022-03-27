
namespace TrashMob.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.ApplicationInsights;

    [Route("api/usernotificationpreferences")]
    public class UserNotificationPreferencesController : BaseController
    {
        private readonly IUserRepository userRepository;
        private readonly IUserNotificationPreferenceRepository userNotificationPreferenceRepository;

        public UserNotificationPreferencesController(IUserRepository userRepository,
                                                     IUserNotificationPreferenceRepository userNotificationPreferenceRepository,
                                                     TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.userRepository = userRepository;
            this.userNotificationPreferenceRepository = userNotificationPreferenceRepository;
        }

        [HttpGet("{userId}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> GetUserNotificationPreferences(Guid userId, CancellationToken cancellationToken)
        {
            var result = await userNotificationPreferenceRepository.GetUserNotificationPreferences(userId, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{userId}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateUserNotificationPreferences(Guid userId, IList<UserNotificationPreference> userNotificationPreferences)
        {
            var user = await userRepository.GetUserByInternalId(userId).ConfigureAwait(false);

            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            foreach (var notificationPreference in userNotificationPreferences)
            {
                await userNotificationPreferenceRepository.AddUpdateUserNotificationPreference(notificationPreference).ConfigureAwait(false);
            }

            TelemetryClient.TrackEvent(nameof(UpdateUserNotificationPreferences));

            return Ok();
        }
    }
}
