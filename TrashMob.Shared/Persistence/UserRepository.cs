namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UserRepository : IUserRepository
    {
        private readonly MobDbContext mobDbContext;
        private readonly Guid TrashMobUserId = Guid.Empty;

        public UserRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<User>> GetAllUsers(CancellationToken cancellationToken = default)
        {
            return await mobDbContext.Users
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        // Add new User record     
        public async Task<User> AddUser(User user)
        {
            user.Id = Guid.NewGuid();
            user.MemberSince = DateTimeOffset.UtcNow;
            user.DateAgreedToPrivacyPolicy = DateTimeOffset.MinValue;
            user.DateAgreedToTermsOfService = DateTimeOffset.MinValue;
            user.DateAgreedToTrashMobWaiver = DateTimeOffset.MinValue;
            user.PrivacyPolicyVersion = string.Empty;
            user.TermsOfServiceVersion = string.Empty;
            user.TrashMobWaiverVersion = string.Empty;
            user.IsSiteAdmin = false;
            mobDbContext.Users.Add(user);
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await mobDbContext.Users.FindAsync(user.Id).ConfigureAwait(false);
        }

        // Update the records of a particular user
        public async Task<User> UpdateUser(User user)
        {
            mobDbContext.Entry(user).State = EntityState.Modified;

            // The IsSiteAdmin flag can only be changed directly in the database, so once set, we need to preserve that, no matter what the user passes in
            var matchedUser = await GetUserByInternalId(user.Id).ConfigureAwait(false);
            user.IsSiteAdmin = matchedUser.IsSiteAdmin;

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await mobDbContext.Users.FindAsync(user.Id).ConfigureAwait(false);
        }

        // Get the details of a particular User
        public async Task<User> GetUserByInternalId(Guid id, CancellationToken cancellationToken = default)
        {
            return await mobDbContext.Users.FindAsync(new object[] { id }, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        // Get the details of a particular User
        public async Task<User> GetUserByUserName(string userName, CancellationToken cancellationToken = default)
        {
            return await mobDbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken: cancellationToken).ConfigureAwait(false);
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

            // Remove the userNotification records where the user created or updated the event
            var userNotifications = mobDbContext.UserNotifications.Where(e => e.UserId == id);

            foreach (var userNotification in userNotifications)
            {
                mobDbContext.UserNotifications.Remove(userNotification);
            }

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            // Remove the Partner Requests
            var partnerRequests = mobDbContext.PartnerRequests.Where(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id);

            foreach (var partnerRequest in partnerRequests)
            {
                if (partnerRequest.CreatedByUserId == id)
                {
                    partnerRequest.CreatedByUserId = TrashMobUserId;
                }

                if (partnerRequest.LastUpdatedByUserId == id)
                {
                    partnerRequest.LastUpdatedByUserId = TrashMobUserId;
                }
            }

            // Remove the first set of records
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            // Remove the records where the user created or updated the event
            var eventSummaries = mobDbContext.EventSummaries.Where(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id).ToList();

            foreach (var eventSummary in eventSummaries)
            {
                if (eventSummary.CreatedByUserId == id)
                {
                    eventSummary.CreatedByUserId = TrashMobUserId;
                }

                if (eventSummary.LastUpdatedByUserId == id)
                {
                    eventSummary.LastUpdatedByUserId = TrashMobUserId;
                }
            }

            // Remove the event partner records for this event
            var eventPartners = mobDbContext.EventPartners.Where(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id);

            foreach (var eventPartner in eventPartners)
            {
                if (eventPartner.CreatedByUserId == id)
                {
                    eventPartner.CreatedByUserId = TrashMobUserId;
                }

                if (eventPartner.LastUpdatedByUserId == id)
                {
                    eventPartner.LastUpdatedByUserId = TrashMobUserId;
                }
            }

            var events = mobDbContext.Events.Where(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id).ToList();

            foreach (var mobEvent in events)
            {
                if (mobEvent.CreatedByUserId == id)
                {
                    mobEvent.CreatedByUserId = TrashMobUserId;
                }

                if (mobEvent.LastUpdatedByUserId == id)
                {
                    mobEvent.LastUpdatedByUserId = TrashMobUserId;
                }
            }

            var partners = mobDbContext.Partners.Where(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id).ToList();

            foreach (var partner in partners)
            {
                if (partner.CreatedByUserId == id)
                {
                    partner.CreatedByUserId = TrashMobUserId;
                }

                if (partner.LastUpdatedByUserId == id)
                {
                    partner.LastUpdatedByUserId = TrashMobUserId;
                }
            }

            var partnerUsers = mobDbContext.PartnerUsers.Where(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id || e.UserId == id);

            foreach (var partnerUser in partnerUsers)
            {
                if (partnerUser.UserId == id)
                {
                    mobDbContext.PartnerUsers.Remove(partnerUser);
                }
                else
                {
                    if (partnerUser.CreatedByUserId == id)
                    {
                        partnerUser.CreatedByUserId = TrashMobUserId;
                    }

                    if (partnerUser.LastUpdatedByUserId == id)
                    {
                        partnerUser.LastUpdatedByUserId = TrashMobUserId;
                    }
                }
            }

            // Remove the Partner Locations
            var partnerLocations = mobDbContext.PartnerLocations.Where(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id);

            foreach (var partnerLocation in partnerLocations)
            {
                if (partnerLocation.CreatedByUserId == id)
                {
                    partnerLocation.CreatedByUserId = TrashMobUserId;
                }

                if (partnerLocation.LastUpdatedByUserId == id)
                {
                    partnerLocation.LastUpdatedByUserId = TrashMobUserId;
                }
            }

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            // Remove the user's profile
            var user = await mobDbContext.Users.FindAsync(id).ConfigureAwait(false);
            mobDbContext.Users.Remove(user);

            // Save the changes
            return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<User> GetUserByNameIdentifier(string nameIdentifier, CancellationToken cancellationToken = default)
        {
            return mobDbContext.Users.FirstOrDefaultAsync(u => u.NameIdentifier == nameIdentifier, cancellationToken: cancellationToken);
        }
    }
}
