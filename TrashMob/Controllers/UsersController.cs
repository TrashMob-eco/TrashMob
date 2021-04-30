
namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Common;
    using TrashMob.Models;
    using TrashMob.Persistence;

    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;

        public UsersController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
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

        [HttpGet("{nameIdentifier}")]
        public async Task<IActionResult> GetUserByNamerIdentifier(string id)
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(User user)
        {
            if (!ValidateUser(user.NameIdentifier))
            {
                return Forbid();
            }

            try
            {
                var updatedUser = await userRepository.UpdateUser(user).ConfigureAwait(false);
                return Ok(updatedUser);
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

                originalUser.City = user.City;
                originalUser.Country = user.Country;
                originalUser.Email = user.Email;
                originalUser.GivenName = user.GivenName;
                originalUser.PostalCode = user.PostalCode;
                originalUser.Region = user.Region;
                originalUser.SurName = user.SurName;
                originalUser.UserName = user.UserName;
                await userRepository.UpdateUser(originalUser);
                var returnedUser = await userRepository.GetUserByNameIdentifier(user.NameIdentifier).ConfigureAwait(false);
                return Ok(returnedUser);
            }

            var newUser = await userRepository.AddUser(user).ConfigureAwait(false);

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
