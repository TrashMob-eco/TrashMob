
namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
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

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, User user)
        {
            try
            {
                var updatedUser = await userRepository.UpdateUser(user).ConfigureAwait(false);
                return Ok(updatedUser);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(id).ConfigureAwait(false))
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
        public async Task<IActionResult> PostUser(User user)
        {
            if (await UserExists(user.TenantId, user.UniqueId).ConfigureAwait(false))
            {
                var userId = await GetUserId(user.TenantId, user.UniqueId).ConfigureAwait(false);
                return Ok(userId);
            }

            var newUserId = await userRepository.AddUser(user).ConfigureAwait(false);

            return Ok(newUserId);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await userRepository.DeleteUserByInternalId(id).ConfigureAwait(false);
            return NoContent();
        }

        private async Task<bool> UserExists(Guid id)
        {
            return (await userRepository.GetAllUsers().ConfigureAwait(false)).Any(e => e.Id == id);
        }

        private async Task<bool> UserExists(string tenantId, string uniqueId)
        {
            return (await userRepository.GetAllUsers().ConfigureAwait(false)).Any(e => e.TenantId == tenantId && e.UniqueId == uniqueId);
        }

        private async Task<Guid> GetUserId(string tenantId, string uniqueId)
        {
            var user = await userRepository.GetUserByExternalId(tenantId, uniqueId).ConfigureAwait(false);
            return user.Id;
        }
    }
}
