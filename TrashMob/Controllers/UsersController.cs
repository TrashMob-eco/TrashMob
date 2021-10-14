
namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared;
    using System.Collections.Generic;
    using TrashMob.Shared.Engine;

    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IEmailManager emailManager;

        public UsersController(IUserRepository userRepository, IEmailManager emailManager)
        {
            this.userRepository = userRepository;
            this.emailManager = emailManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await userRepository.GetAllUsers().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("getUserByUserName/{userName}")]
        public async Task<IActionResult> GetUser(string userName)
        {
            var user = await userRepository.GetUserByUserName(userName).ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("verifyunique/{userId}/{userName}")]
        public async Task<IActionResult> VerifyUnique(Guid userId, string userName)
        {
            var user = await userRepository.GetUserByUserName(userName).ConfigureAwait(false);

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
        public async Task<IActionResult> GetUserByInternalId(Guid id)
        {
            var user = await userRepository.GetUserByInternalId(id).ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut()]
        public async Task<IActionResult> PutUser(User user)
        {
            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            try
            {
                // ToDo: Verify can't have duplicate usernames
                var updatedUser = await userRepository.UpdateUser(user).ConfigureAwait(false);
                var returnedUser = await userRepository.GetUserByNameIdentifier(user.NameIdentifier).ConfigureAwait(false);
                return Ok(returnedUser);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(user.Id).ConfigureAwait(false))
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

            if ((originalUser = await UserExists(user.NameIdentifier).ConfigureAwait(false)) != null)
            {
                if (!ValidateUser(originalUser.NameIdentifier))
                {
                    return Forbid();
                }

                originalUser.Email = user.Email;
                originalUser.SourceSystemUserName = user.SourceSystemUserName;

                await userRepository.UpdateUser(originalUser).ConfigureAwait(false);

                var returnedUser = await userRepository.GetUserByNameIdentifier(user.NameIdentifier).ConfigureAwait(false);
                return Ok(returnedUser);
            }

            if (string.IsNullOrEmpty(user.UserName))
            {
                // On insert we need a random user name to avoid duplicates, but we don't want to show the full email address ever, so take a subset
                // of their email and then add a random number to the end.
                Random rnd = new Random();
                var userNum = rnd.Next(1000000, 9999999).ToString();
                user.UserName = user.Email.Split("@")[0].Substring(0, 8) + userNum;
            }

            var newUser = await userRepository.AddUser(user).ConfigureAwait(false);

            // Notify Admins that a new user has joined
            var message = $"A new user: {user.Email} has joined TrashMob.eco!";
            var htmlMessage = $"A new user: {user.Email} has joined TrashMob.eco!";
            var subject = "New User Alert";

            var recipients = new List<EmailAddress>
            {
                new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress }
            };

            await emailManager.SendSystemEmail(subject, message, htmlMessage, recipients, CancellationToken.None).ConfigureAwait(false);

            // Send welcome email to new User
            var welcomeMessage = emailManager.GetEmailTemplate(NotificationTypeEnum.WelcomeToTrashMob.ToString());
            var welcomeSubject = "Welcome to TrashMob.eco!";
            welcomeMessage = welcomeMessage.Replace("{UserName}", user.UserName);

            var welcomeHtmlMessage = emailManager.GetHtmlEmailTemplate(NotificationTypeEnum.WelcomeToTrashMob.ToString());
            welcomeHtmlMessage = welcomeHtmlMessage.Replace("{UserName}", user.UserName);

            var welcomeRecipients = new List<EmailAddress>
            {
                new EmailAddress { Name = user.UserName, Email = user.Email }
            };

            await emailManager.SendSystemEmail(welcomeSubject, welcomeMessage, welcomeHtmlMessage, welcomeRecipients, CancellationToken.None).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetUserByInternalId), new { id = newUser.Id }, newUser);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await userRepository.GetUserByInternalId(id).ConfigureAwait(false);

            if (user == null || !ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            await userRepository.DeleteUserByInternalId(id).ConfigureAwait(false);
            return Ok(id);
        }

        private async Task<bool> UserExists(Guid id)
        {
            return (await userRepository.GetAllUsers().ConfigureAwait(false)).Any(e => e.Id == id);
        }

        private async Task<User> UserExists(string nameIdentifier)
        {
            return (await userRepository.GetAllUsers().ConfigureAwait(false)).FirstOrDefault(u => u.NameIdentifier == nameIdentifier);
        }

        private bool ValidateUser(string userId)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId == nameIdentifier;
        }
    }
}
