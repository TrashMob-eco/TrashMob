namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manager for user waiver operations.
    /// </summary>
    public class UserWaiverManager : KeyedManager<UserWaiver>, IUserWaiverManager
    {
        private readonly IKeyedRepository<WaiverVersion> waiverVersionRepository;
        private readonly IBaseRepository<CommunityWaiver> communityWaiverRepository;
        private readonly IBaseRepository<EventPartnerLocationService> eventPartnerLocationServiceRepository;
        private readonly IKeyedRepository<User> userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserWaiverManager"/> class.
        /// </summary>
        /// <param name="repository">The user waiver repository.</param>
        /// <param name="waiverVersionRepository">The waiver version repository.</param>
        /// <param name="communityWaiverRepository">The community waiver repository.</param>
        /// <param name="eventPartnerLocationServiceRepository">The event partner location service repository.</param>
        /// <param name="userRepository">The user repository.</param>
        public UserWaiverManager(
            IKeyedRepository<UserWaiver> repository,
            IKeyedRepository<WaiverVersion> waiverVersionRepository,
            IBaseRepository<CommunityWaiver> communityWaiverRepository,
            IBaseRepository<EventPartnerLocationService> eventPartnerLocationServiceRepository,
            IKeyedRepository<User> userRepository)
            : base(repository)
        {
            this.waiverVersionRepository = waiverVersionRepository;
            this.communityWaiverRepository = communityWaiverRepository;
            this.eventPartnerLocationServiceRepository = eventPartnerLocationServiceRepository;
            this.userRepository = userRepository;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<WaiverVersion>> GetRequiredWaiversForEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var endOfYear = GetEndOfYear();

            // Get the user's current valid waivers
            var userWaivers = await Repo.Get(uw =>
                    uw.UserId == userId &&
                    uw.ExpiryDate >= now)
                .ToListAsync(cancellationToken);

            var signedWaiverVersionIds = userWaivers.Select(uw => uw.WaiverVersionId).ToHashSet();

            // Get the current global waiver
            var globalWaiver = await waiverVersionRepository
                .Get(w =>
                    w.IsActive &&
                    w.Scope == WaiverScope.Global &&
                    w.EffectiveDate <= now &&
                    (w.ExpiryDate == null || w.ExpiryDate > now))
                .OrderByDescending(w => w.EffectiveDate)
                .FirstOrDefaultAsync(cancellationToken);

            // Get partner IDs associated with the event through partner locations
            var partnerIds = await eventPartnerLocationServiceRepository
                .Get(epls => epls.EventId == eventId)
                .Include(epls => epls.PartnerLocation)
                .Select(epls => epls.PartnerLocation.PartnerId)
                .Distinct()
                .ToListAsync(cancellationToken);

            // Get community waivers for those partners
            var communityWaiverVersionIds = await communityWaiverRepository
                .Get(cw => partnerIds.Contains(cw.CommunityId) && cw.IsRequired)
                .Select(cw => cw.WaiverVersionId)
                .ToListAsync(cancellationToken);

            // Get active community waivers
            var communityWaivers = communityWaiverVersionIds.Any()
                ? await waiverVersionRepository.Get(w =>
                        communityWaiverVersionIds.Contains(w.Id) &&
                        w.IsActive &&
                        w.EffectiveDate <= now &&
                        (w.ExpiryDate == null || w.ExpiryDate > now))
                    .ToListAsync(cancellationToken)
                : new List<WaiverVersion>();

            // Combine and filter out already signed waivers
            var requiredWaivers = new List<WaiverVersion>();

            if (globalWaiver != null && !signedWaiverVersionIds.Contains(globalWaiver.Id))
            {
                requiredWaivers.Add(globalWaiver);
            }

            requiredWaivers.AddRange(communityWaivers.Where(w => !signedWaiverVersionIds.Contains(w.Id)));

            return requiredWaivers;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<WaiverVersion>> GetPendingWaiversForUserAsync(Guid userId, Guid? communityId = null, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;

            // Get the user's current valid waivers
            var userWaivers = await Repo.Get(uw =>
                    uw.UserId == userId &&
                    uw.ExpiryDate >= now)
                .ToListAsync(cancellationToken);

            var signedWaiverVersionIds = userWaivers.Select(uw => uw.WaiverVersionId).ToHashSet();

            // Get the current global waiver
            var globalWaiver = await waiverVersionRepository
                .Get(w =>
                    w.IsActive &&
                    w.Scope == WaiverScope.Global &&
                    w.EffectiveDate <= now &&
                    (w.ExpiryDate == null || w.ExpiryDate > now))
                .OrderByDescending(w => w.EffectiveDate)
                .FirstOrDefaultAsync(cancellationToken);

            var requiredWaivers = new List<WaiverVersion>();

            if (globalWaiver != null && !signedWaiverVersionIds.Contains(globalWaiver.Id))
            {
                requiredWaivers.Add(globalWaiver);
            }

            // If a specific community is requested, get those waivers too
            if (communityId.HasValue)
            {
                var communityWaiverVersionIds = await communityWaiverRepository
                    .Get(cw => cw.CommunityId == communityId.Value && cw.IsRequired)
                    .Select(cw => cw.WaiverVersionId)
                    .ToListAsync(cancellationToken);

                if (communityWaiverVersionIds.Any())
                {
                    var communityWaivers = await waiverVersionRepository.Get(w =>
                            communityWaiverVersionIds.Contains(w.Id) &&
                            w.IsActive &&
                            w.EffectiveDate <= now &&
                            (w.ExpiryDate == null || w.ExpiryDate > now))
                        .ToListAsync(cancellationToken);

                    requiredWaivers.AddRange(communityWaivers.Where(w => !signedWaiverVersionIds.Contains(w.Id)));
                }
            }

            return requiredWaivers;
        }

        /// <inheritdoc />
        public async Task<bool> HasValidWaiverForEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default)
        {
            var requiredWaivers = await GetRequiredWaiversForEventAsync(userId, eventId, cancellationToken);
            return !requiredWaivers.Any();
        }

        /// <inheritdoc />
        public async Task<ServiceResult<UserWaiver>> AcceptWaiverAsync(AcceptWaiverRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.TypedLegalName))
            {
                return ServiceResult<UserWaiver>.Failure("Typed legal name is required.");
            }

            // Get the waiver version
            var waiverVersion = await waiverVersionRepository.GetAsync(request.WaiverVersionId, cancellationToken);
            if (waiverVersion == null)
            {
                return ServiceResult<UserWaiver>.Failure("Waiver version not found.");
            }

            if (!waiverVersion.IsActive)
            {
                return ServiceResult<UserWaiver>.Failure("This waiver version is no longer active.");
            }

            var now = DateTimeOffset.UtcNow;
            if (waiverVersion.EffectiveDate > now)
            {
                return ServiceResult<UserWaiver>.Failure("This waiver version is not yet effective.");
            }

            if (waiverVersion.ExpiryDate.HasValue && waiverVersion.ExpiryDate < now)
            {
                return ServiceResult<UserWaiver>.Failure("This waiver version has expired.");
            }

            // Check if user already has a valid waiver for this version
            var existingWaiver = await Repo.Get(uw =>
                    uw.UserId == request.UserId &&
                    uw.WaiverVersionId == request.WaiverVersionId &&
                    uw.ExpiryDate >= now)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingWaiver != null)
            {
                return ServiceResult<UserWaiver>.Failure("You have already signed this waiver version.");
            }

            var userWaiver = new UserWaiver
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                WaiverVersionId = request.WaiverVersionId,
                AcceptedDate = now,
                ExpiryDate = GetEndOfYear(),
                TypedLegalName = request.TypedLegalName,
                WaiverTextSnapshot = waiverVersion.WaiverText,
                SigningMethod = "ESignatureWeb",
                IPAddress = request.IPAddress,
                UserAgent = request.UserAgent,
                IsMinor = request.IsMinor,
                GuardianUserId = request.GuardianUserId,
                GuardianName = request.GuardianName,
                GuardianRelationship = request.GuardianRelationship,
                CreatedByUserId = request.UserId,
                LastUpdatedByUserId = request.UserId,
                CreatedDate = now,
                LastUpdatedDate = now,
            };

            var created = await Repo.AddAsync(userWaiver);
            return ServiceResult<UserWaiver>.Success(created);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserWaiver>> GetUserWaiversAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(uw => uw.UserId == userId)
                .Include(uw => uw.WaiverVersion)
                .OrderByDescending(uw => uw.AcceptedDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetUsersWithExpiringWaiversAsync(int daysUntilExpiry, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var expiryThreshold = now.AddDays(daysUntilExpiry);

            // Get user IDs with waivers expiring within the threshold
            var userIds = await Repo.Get(uw =>
                    uw.ExpiryDate >= now &&
                    uw.ExpiryDate <= expiryThreshold)
                .Select(uw => uw.UserId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!userIds.Any())
            {
                return Enumerable.Empty<User>();
            }

            return await userRepository.Get(u => userIds.Contains(u.Id))
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<UserWaiver> GetUserWaiverWithDetailsAsync(Guid userWaiverId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(uw => uw.Id == userWaiverId)
                .Include(uw => uw.WaiverVersion)
                .Include(uw => uw.User)
                .FirstOrDefaultAsync(cancellationToken);
        }

        private static DateTimeOffset GetEndOfYear()
        {
            var now = DateTimeOffset.UtcNow;
            return new DateTimeOffset(now.Year, 12, 31, 23, 59, 59, TimeSpan.Zero);
        }
    }
}
