
namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Shared;
    using System.Collections.Generic;
    using TrashMob.Shared.Engine;
    using Microsoft.ApplicationInsights;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/users")]
    public class UsersController : SecureController
    {
        private readonly IUserManager userManager;

        public UsersController(IUserManager userManager)
            : base()
        {
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = "UserIsAdmin")]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var result = await userManager.Get(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("getUserByUserName/{userName}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> GetUser(string userName, CancellationToken cancellationToken)
        {
            var user = await userManager.GetUserByUserName(userName, cancellationToken).ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("verifyunique/{userId}/{userName}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> VerifyUnique(Guid userId, string userName, CancellationToken cancellationToken)
        {
            var user = await userManager.GetUserByUserName(userName, cancellationToken).ConfigureAwait(false);

            if (user == null)
            {
                return Ok();
            }

            if (user.Id != userId)
            {
                return Conflict();
            }

            return Ok();
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> GetUserByInternalId(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await userManager.GetUserByInternalId(id, cancellationToken).ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut()]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> PutUser(User user)
        {
            try
            {
                var updatedUser = await userManager.Update(user).ConfigureAwait(false);
                TelemetryClient.TrackEvent("UpdateUser");
                return Ok(updatedUser);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await userManager.UserExists(user.Id).ConfigureAwait(false))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PostUser(User user)
        {
            User originalUser;

            if ((originalUser = await userManager.UserExists(user.NameIdentifier).ConfigureAwait(false)) != null)
            {
                // TODO: Fix this
                //if (!ValidateUser(originalUser.NameIdentifier))
                //{
                //    return Forbid();
                //}

                originalUser.Email = user.Email;
                originalUser.SourceSystemUserName = user.SourceSystemUserName;

                var updatedUser = await userManager.Update(originalUser).ConfigureAwait(false);
                TelemetryClient.TrackEvent("UpdateUser");

                return Ok(updatedUser);
            }

            if (string.IsNullOrEmpty(user.UserName))
            {
                // On insert we need a random user name to avoid duplicates, but we don't want to show the full email address ever, so take a subset
                // of their email and then add a random number to the end.
                Random rnd = new();
                var userNum = rnd.Next(100, 999).ToString();
                var first = user.Email.Split("@")[0];
                user.UserName = first.Substring(0, Math.Min(first.Length - 1, 8)) + userNum;
            }

            var newUser = await userManager.Add(user).ConfigureAwait(false);
            TelemetryClient.TrackEvent("AddUser");

            return CreatedAtAction(nameof(GetUserByInternalId), new { id = newUser.Id }, newUser);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ValidUser")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await userManager.Delete(id).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeleteUser));

            return Ok(id);
        }
    }
}
