
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

    public class UserManager : KeyedManager<User>, IUserManager
    {
        private readonly Guid TrashMobUserId = Guid.Empty;
        private readonly IBaseRepository<EventAttendee> eventAttendeesRepository;
        private readonly IKeyedRepository<UserNotification> userNotificationRepository;
        private readonly IKeyedRepository<PartnerRequest> partnerRequestRepository;
        private readonly IBaseRepository<EventSummary> eventSummaryRepository;
        private readonly IBaseRepository<EventPartner> eventPartnerRepository;
        private readonly IKeyedRepository<Event> eventRepository;
        private readonly IKeyedRepository<Partner> partnerRepository;
        private readonly IBaseRepository<PartnerUser> partnerUserRepository;
        private readonly IBaseRepository<PartnerLocation> partnerLocationRepository;
        private readonly IEmailManager emailManager;

        public UserManager(IKeyedRepository<User> repository,
                           IBaseRepository<EventAttendee> eventAttendeesRepository,
                           IKeyedRepository<UserNotification> userNotificationRepository,
                           IKeyedRepository<PartnerRequest> partnerRequestRepository,
                           IBaseRepository<EventSummary> eventSummaryRepository,
                           IBaseRepository<EventPartner> eventPartnerRepository,
                           IKeyedRepository<Event> eventRepository,
                           IKeyedRepository<Partner> partnerRepository,
                           IBaseRepository<PartnerUser> partnerUserRepository,
                           IBaseRepository<PartnerLocation> partnerLocationRepository,
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

        public async Task<User> GetUserByUserName(string userName, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(u => u.NameIdentifier == userName).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User> GetUserByInternalId(Guid id, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(u => u.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public override async Task<User> Update(User user)
        {
            // The IsSiteAdmin flag can only be changed directly in the database, so once set, we need to preserve that, no matter what the user passes in
            var matchedUser = await GetUserByInternalId(user.Id).ConfigureAwait(false);
            user.IsSiteAdmin = matchedUser.IsSiteAdmin;

            return await base.Update(user);
        }

        public override async Task<int> Delete(Guid id)
        {
            // Remove the records where the user is attending an event
            var attendees = eventAttendeesRepository.Get(ea => ea.UserId == id);

            foreach (var attendee in attendees)
            {
                await eventAttendeesRepository.Delete(attendee);
            }

            // Remove the userNotification records where the user created or updated the event
            var userNotifications = userNotificationRepository.Get(e => e.UserId == id);

            foreach (var userNotification in userNotifications)
            {
                await userNotificationRepository.Delete(userNotification);
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

                await partnerRequestRepository.Update(partnerRequest);
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

                await eventSummaryRepository.Update(eventSummary);
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

                await eventPartnerRepository.Update(eventPartner);
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

                await eventRepository.Update(mobEvent);
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

                await partnerRepository.Update(partner);
            }

            var partnerUsers = partnerUserRepository.Get(e => e.CreatedByUserId == id || e.LastUpdatedByUserId == id || e.UserId == id);

            foreach (var partnerUser in partnerUsers)
            {
                if (partnerUser.UserId == id)
                {
                    await partnerUserRepository.Delete(partnerUser);
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

                    await partnerUserRepository.Update(partnerUser);
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

                await partnerLocationRepository.Update(partnerLocation);
            }

            // Remove the user's profile
            var user = await Repo.Get(id).ConfigureAwait(false);
            var result = await Repo.Delete(user);

            return result;
        }

        public override async Task<User> Add(User user)
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

            var addedUser = await base.Add(user);

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

            await emailManager.SendTemplatedEmail(subject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, dynamicTemplateData, recipients, CancellationToken.None).ConfigureAwait(false);

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

            await emailManager.SendTemplatedEmail(welcomeSubject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, userDynamicTemplateData, welcomeRecipients, CancellationToken.None).ConfigureAwait(false);

            return addedUser;
        }

        public async Task<bool> UserExists(Guid id)
        {
            return await Repository.Get(e => e.Id == id).AnyAsync();
        }

        public async Task<User> UserExists(string nameIdentifier)
        {
            return await Repository.Get(u => u.NameIdentifier == nameIdentifier).FirstOrDefaultAsync();
        }
    }
}
