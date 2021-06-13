
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
    using TrashMob.Models;
    using TrashMob.Persistence;
    using TrashMob.Shared;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
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
            if (!ValidateUser(user.NameIdentifier))
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
                await userRepository.UpdateUser(originalUser);
                var returnedUser = await userRepository.GetUserByNameIdentifier(user.NameIdentifier).ConfigureAwait(false);
                return Ok(returnedUser);
            }

            var newUser = await userRepository.AddUser(user).ConfigureAwait(false);

            var email = new Email
            {
                Message = $"A new user: {user.Email} has joined TrashMob.eco!",
                Subject = "New User Alert"
            };

            email.Addresses.Add(new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress });

            await emailManager.SendSystemEmail(email, CancellationToken.None).ConfigureAwait(false);

            return Ok(newUser);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await userRepository.GetUserByInternalId(id).ConfigureAwait(false);

            if (!ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            await userRepository.DeleteUserByInternalId(id).ConfigureAwait(false);
            return NoContent();
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
