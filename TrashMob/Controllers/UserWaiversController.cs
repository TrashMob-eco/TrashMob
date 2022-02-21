namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Poco;
    using TrashMob.Shared;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Authorize]
    [Route("api/userwaivers")]
    public class UserWaiversController : ControllerBase
    {
        private readonly IUserWaiverRepository userWaiverRepository;
        private readonly IUserRepository userRepository;
        private readonly IWaiverRepository waiverRepository;

        public UserWaiversController(IUserWaiverRepository userWaiverRepository, IUserRepository userRepository, IWaiverRepository waiverRepository)
        {
            this.userWaiverRepository = userWaiverRepository;
            this.userRepository = userRepository;
            this.waiverRepository = waiverRepository;
        }

        [HttpGet("{userId}")]
        public IActionResult GetUserWaivers(Guid userId, CancellationToken cancellationToken)
        {
            return Ok(userWaiverRepository.GetUserWaivers(cancellationToken).Where(uw => uw.UserId == userId).ToList());
        }

        [HttpGet("{userId}/{waiverId}")]
        public IActionResult GetUserWaiver(Guid userId, Guid waiverId, CancellationToken cancellationToken)
        {
            var userWaiver = userWaiverRepository.GetUserWaivers(cancellationToken).FirstOrDefault(uw => uw.UserId == userId && uw.WaiverId == waiverId);

            if (userWaiver == null)
            {
                return NotFound();
            }

            return Ok(userWaiver);
        }

        [HttpPost("{userId}/{waiverId}")]
        public async Task<IActionResult> AddUserWaiver(Guid userId, Guid waiverId)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            // Get the waiver details so we can calculate the expiry date.
            var waiver = await waiverRepository.GetWaiver(waiverId);

            var expiryDate = GetExpiryDate(waiver.WaiverDurationTypeId);

            var userWaiver = new UserWaiver()
            {
                UserId = userId,
                WaiverId = waiverId,
                EffectiveDate = DateTimeOffset.UtcNow,
                ExpiryDate = expiryDate,
                CreatedByUserId = currentUser.Id,
                LastUpdatedByUserId = currentUser.Id
            };

            await userWaiverRepository.AddUserWaiver(userWaiver).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetUserWaiver), new { userId, waiverId });
        }

        private DateTimeOffset GetExpiryDate(int waiverDurationId)
        {
            var currentDay = DateTimeOffset.Now;
            switch ((WaiverDurationTypeEnum)waiverDurationId)
            {
                case WaiverDurationTypeEnum.CalendarYear:
                    return new DateTimeOffset(currentDay.Year + 1, 1, 1, 0, 0, 0, TimeSpan.Zero);
                case WaiverDurationTypeEnum.YearFromSigning:
                    return currentDay.AddYears(1);
                case WaiverDurationTypeEnum.MonthFromSigning:
                    return currentDay.AddMonths(1);
                case WaiverDurationTypeEnum.SingleDay:
                    return currentDay.AddDays(1);
                default:
                    return DateTimeOffset.MinValue;
            }
        }
    }
}
