namespace TrashMob.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Extensions;
    using TrashMob.Models;

    public class UserRepository : IUserRepository
    {
        private readonly MobDbContext mobDbContext;

        public UserRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await mobDbContext.Users
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }

        // Add new User record     
        public async Task<User> AddUser(User user)
        {
            user.Id = Guid.NewGuid();
            user.MemberSince = DateTimeOffset.UtcNow;
            user.DateAgreedToPrivacyPolicy = DateTimeOffset.MinValue;
            user.DateAgreedToTermsOfService = DateTimeOffset.MinValue;
            user.PrivacyPolicyVersion = string.Empty;
            user.TermsOfServiceVersion = string.Empty;
            mobDbContext.Users.Add(user);
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return user;
        }

        // Update the records of a particluar user
        public Task<int> UpdateUser(User user)
        {
            mobDbContext.Entry(user).State = EntityState.Modified;
            return mobDbContext.SaveChangesAsync();
        }

        // Get the details of a particular User
        public async Task<User> GetUserByInternalId(Guid id)
        {
            return await mobDbContext.Users.FindAsync(id).ConfigureAwait(false);
        }

        // Delete the record of a particular User
        public async Task<int> DeleteUserByInternalId(Guid id)
        {
            var user = await mobDbContext.Users.FindAsync(id).ConfigureAwait(false);
            mobDbContext.Users.Remove(user);
            return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<User> GetUserByNameIdentifier(string nameIdentifier)
        {
            return mobDbContext.Users.FirstOrDefaultAsync(u => u.NameIdentifier == nameIdentifier);
        }
    }
}
