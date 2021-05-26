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
            // Remove the records where the user is attending an event
            var attendees = mobDbContext.EventAttendees.Where(ea => ea.UserId == id);

            foreach (var attendee in attendees)
            {
                mobDbContext.EventAttendees.Remove(attendee);
            }

            // Remove the history records where the user created or updated the event
            var eventHistories = mobDbContext.EventHistories.Where(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id);

            foreach (var mobEvent in eventHistories)
            {
                mobDbContext.EventHistories.Remove(mobEvent);
            }

            // Remove the first set of records
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            // Remove the records where the user created or updated the event
            var events = mobDbContext.Events.Where(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id).ToList();

            foreach (var mobEvent in events)
            {
                // Remove the records where other users are attending an event created by this user
                var eventAttendees = mobDbContext.EventAttendees.Where(ea => ea.EventId == mobEvent.Id && ea.UserId != id);

                foreach (var attendee in eventAttendees)
                {
                    mobDbContext.EventAttendees.Remove(attendee);
                }

                mobDbContext.Events.Remove(mobEvent);
            }

            // Remove the user's profile
            var user = await mobDbContext.Users.FindAsync(id).ConfigureAwait(false);
            mobDbContext.Users.Remove(user);

            // Save the changes
            return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<User> GetUserByNameIdentifier(string nameIdentifier)
        {
            return mobDbContext.Users.FirstOrDefaultAsync(u => u.NameIdentifier == nameIdentifier);
        }
    }
}
