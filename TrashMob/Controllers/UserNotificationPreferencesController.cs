
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

    [ApiController]
    [Route("api/users")]
    public class UserNotificationPreferencesController : ControllerBase
    {
        private readonly IUserNotificationPreferenceRepository userNotificationPreferenceRepository;

        public UserNotificationPreferencesController(IUserNotificationPreferenceRepository userNotificationPreferenceRepository)
        {
            this.userNotificationPreferenceRepository = userNotificationPreferenceRepository;
        }

        [HttpGet("{userId}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> GetUserNotificationPreferences(Guid userId)
        {
            var result = await userNotificationPreferenceRepository.GetUserNotificationPreferences(userId).ConfigureAwait(false);
            return Ok(result);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PutUserNotificationPreferences(User user, IList<UserNotificationPreference> userNotificationPreferences)
        {
            if (!ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

                foreach(var notificationPreference in userNotificationPreferences)
                {
                    await userNotificationPreferenceRepository.AddUpdateUserNotificationPreference(notificationPreference).ConfigureAwait(false);
                }

                return Ok();
        }

        private bool ValidateUser(string userId)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId == nameIdentifier;
        }
    }
}
