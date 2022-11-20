
namespace TrashMob.Shared.Managers
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    public class UserManager : KeyedManager<User>, IUserManager
    {
        private readonly Guid TrashMobUserId = Guid.Empty;
        private readonly IBaseRepository<EventAttendee> eventAttendeesRepository;
        private readonly IKeyedRepository<UserNotification> userNotificationRepository;
        private readonly IKeyedRepository<PartnerRequest> partnerRequestRepository;
        private readonly IBaseRepository<EventSummary> eventSummaryRepository;
        private readonly IBaseRepository<EventPartnerLocationService> eventPartnerRepository;
        private readonly IKeyedRepository<Event> eventRepository;
        private readonly IKeyedRepository<Partner> partnerRepository;
        private readonly IBaseRepository<PartnerAdmin> partnerUserRepository;
        private readonly IKeyedRepository<PartnerLocation> partnerLocationRepository;
        private readonly IEmailManager emailManager;

        public UserManager(IKeyedRepository<User> repository,
                           IBaseRepository<EventAttendee> eventAttendeesRepository,
                           IKeyedRepository<UserNotification> userNotificationRepository,
                           IKeyedRepository<PartnerRequest> partnerRequestRepository,
                           IBaseRepository<EventSummary> eventSummaryRepository,
                           IBaseRepository<EventPartnerLocationService> eventPartnerRepository,
                           IKeyedRepository<Event> eventRepository,
                           IKeyedRepository<Partner> partnerRepository,
                           IBaseRepository<PartnerAdmin> partnerUserRepository,
                           IKeyedRepository<PartnerLocation> partnerLocationRepository,
                           IEmailManager emailManager) : base(repository)
        {
            this.eventAttendeesRepository = eventAttendeesRepository;
            this.userNotificationRepository = userNotificationRepository;
            this.partnerRequestRepository = partnerRequestRepository;
            this.eventSummaryRepository = eventSummaryRepository;
            this.eventPartnerRepository = eventPartnerRepository;
            this.eventRepository = eventRepository;
            this.partnerRepository = partnerRepository;
            this.partnerUserRepository = partnerUserRepository;
            this.partnerLocationRepository = partnerLocationRepository;
            this.emailManager = emailManager;
        }      

        public async Task<User> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(u => u.UserName == userName).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(u => u.Email == email).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User> GetUserByInternalIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(u => u.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public override async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            // The IsSiteAdmin flag can only be changed directly in the database, so once set, we need to preserve that, no matter what the user passes in
            var matchedUser = await GetUserByInternalIdAsync(user.Id, cancellationToken).ConfigureAwait(false);
            user.IsSiteAdmin = matchedUser.IsSiteAdmin;

            return await base.UpdateAsync(user, cancellationToken);
        }

        public override async Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Remove the records where the user is attending an event
            var attendees = eventAttendeesRepository.Get(ea => ea.UserId == id);

            foreach (var attendee in attendees)
            {
                await eventAttendeesRepository.DeleteAsync(attendee);
            }

            // Remove the userNotification records where the user created or updated the event
            var userNotifications = userNotificationRepository.Get(e => e.UserId == id);

            foreach (var userNotification in userNotifications)
            {
                await userNotificationRepository.DeleteAsync(userNotification);
            }

            // Remove the Partner Requests
            var partnerRequests = partnerRequestRepository.Get(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id);

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

                await partnerRequestRepository.UpdateAsync(partnerRequest);
            }

            // Remove the records where the user created or updated the event
            var eventSummaries = eventSummaryRepository.Get(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id);

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

                await eventSummaryRepository.UpdateAsync(eventSummary);
            }

            // Remove the event partner records for this event
            var eventPartners = eventPartnerRepository.Get(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id);

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

                await eventPartnerRepository.UpdateAsync(eventPartner);
            }

            var events = eventRepository.Get(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id);

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

                await eventRepository.UpdateAsync(mobEvent);
            }

            var partners = partnerRepository.Get(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id);

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

                await partnerRepository.UpdateAsync(partner);
            }

            var partnerUsers = partnerUserRepository.Get(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id || e.UserId == id);

            foreach (var partnerUser in partnerUsers)
            {
                if (partnerUser.UserId == id)
                {
                    await partnerUserRepository.DeleteAsync(partnerUser);
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

                    await partnerUserRepository.UpdateAsync(partnerUser);
                }
            }

            // Remove the Partner Locations
            var partnerLocations = partnerLocationRepository.Get(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id);

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

                await partnerLocationRepository.UpdateAsync(partnerLocation);
            }

            // Remove the user's profile
            var user = await Repo.GetAsync(id, cancellationToken).ConfigureAwait(false);
            var result = await Repo.DeleteAsync(user);

            return result;
        }

        public override async Task<User> AddAsync(User user, CancellationToken cancellationToken)
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

            var addedUser = await base.AddAsync(user, cancellationToken);

            // Notify Admins that a new user has joined
            var message = $"A new user: {user.Email} has joined TrashMob.eco!";
            var subject = "New User Alert";

            var dynamicTemplateData = new
            {
                username = Constants.TrashMobEmailName,
                emailCopy = message,
                subject = subject,
            };

            var recipients = new List<EmailAddress>
            {
                new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress }
            };

            await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, dynamicTemplateData, recipients, CancellationToken.None).ConfigureAwait(false);

            // Send welcome email to new User
            var welcomeMessage = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.WelcomeToTrashMob.ToString());
            var welcomeSubject = "Welcome to TrashMob.eco!";

            var userDynamicTemplateData = new
            {
                username = user.UserName,
                emailCopy = welcomeMessage,
                subject = welcomeSubject,
            };

            var welcomeRecipients = new List<EmailAddress>
            {
                new EmailAddress { Name = user.UserName, Email = user.Email }
            };

            await emailManager.SendTemplatedEmailAsync(welcomeSubject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, userDynamicTemplateData, welcomeRecipients, CancellationToken.None).ConfigureAwait(false);

            return addedUser;
        }

        public async Task<bool> UserExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Repository.Get(e => e.Id == id).AnyAsync(cancellationToken);
        }

        public async Task<User> UserExistsAsync(string nameIdentifier, CancellationToken cancellationToken = default)
        {
            return await Repository.Get(u => u.NameIdentifier == nameIdentifier).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
