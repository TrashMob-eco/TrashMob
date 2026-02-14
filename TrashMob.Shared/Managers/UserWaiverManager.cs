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
    public class UserWaiverManager(
        IKeyedRepository<UserWaiver> repository,
        IKeyedRepository<WaiverVersion> waiverVersionRepository,
        IBaseRepository<CommunityWaiver> communityWaiverRepository,
        IBaseRepository<EventPartnerLocationService> eventPartnerLocationServiceRepository,
        IKeyedRepository<User> userRepository,
        IBaseRepository<EventAttendee> eventAttendeeRepository,
        IWaiverDocumentManager waiverDocumentManager)
        : KeyedManager<UserWaiver>(repository), IUserWaiverManager
    {
        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

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

        /// <inheritdoc />
        public async Task<ServiceResult<UserWaiver>> UploadPaperWaiverAsync(
            PaperWaiverUploadRequest request,
            Guid uploadedByUserId,
            CancellationToken cancellationToken = default)
        {
            // Validate file
            if (request.FormFile == null || request.FormFile.Length == 0)
            {
                return ServiceResult<UserWaiver>.Failure("No file was uploaded.");
            }

            if (request.FormFile.Length > MaxFileSizeBytes)
            {
                return ServiceResult<UserWaiver>.Failure("File size exceeds the 10MB limit.");
            }

            if (!AllowedContentTypes.Contains(request.FormFile.ContentType))
            {
                return ServiceResult<UserWaiver>.Failure("Invalid file type. Allowed types: PDF, JPEG, PNG, WebP.");
            }

            // Validate signer name
            if (string.IsNullOrWhiteSpace(request.SignerName))
            {
                return ServiceResult<UserWaiver>.Failure("Signer name is required.");
            }

            // Validate user exists
            var user = await userRepository.GetAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return ServiceResult<UserWaiver>.Failure("User not found.");
            }

            // Validate waiver version exists and is active
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

            // Check if user already has a valid waiver for this version
            var existingWaiver = await Repo.Get(uw =>
                    uw.UserId == request.UserId &&
                    uw.WaiverVersionId == request.WaiverVersionId &&
                    uw.ExpiryDate >= now)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingWaiver != null)
            {
                return ServiceResult<UserWaiver>.Failure("User has already signed this waiver version.");
            }

            // Create the UserWaiver record
            var userWaiver = new UserWaiver
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                WaiverVersionId = request.WaiverVersionId,
                AcceptedDate = request.DateSigned,
                ExpiryDate = GetEndOfYear(),
                TypedLegalName = request.SignerName,
                WaiverTextSnapshot = waiverVersion.WaiverText,
                SigningMethod = "PaperUpload",
                UploadedByUserId = uploadedByUserId,
                IsMinor = request.IsMinor,
                GuardianName = request.GuardianName,
                GuardianRelationship = request.GuardianRelationship,
                CreatedByUserId = uploadedByUserId,
                LastUpdatedByUserId = uploadedByUserId,
                CreatedDate = now,
                LastUpdatedDate = now,
            };

            // Store the uploaded document
            await using var fileStream = request.FormFile.OpenReadStream();
            var documentUrl = await waiverDocumentManager.StorePaperWaiverAsync(
                userWaiver,
                fileStream,
                request.FormFile.ContentType,
                cancellationToken);

            userWaiver.DocumentUrl = documentUrl;

            var created = await Repo.AddAsync(userWaiver);
            return ServiceResult<UserWaiver>.Success(created);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AttendeeWaiverStatus>> GetEventAttendeeWaiverStatusAsync(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;

            // Get all attendees for the event
            var attendees = await eventAttendeeRepository
                .Get(ea => ea.EventId == eventId)
                .Include(ea => ea.User)
                .ToListAsync(cancellationToken);

            if (!attendees.Any())
            {
                return Enumerable.Empty<AttendeeWaiverStatus>();
            }

            var attendeeUserIds = attendees.Select(a => a.UserId).ToList();

            // Get valid waivers for these users
            var validWaiverUserIds = await Repo
                .Get(uw => attendeeUserIds.Contains(uw.UserId) && uw.ExpiryDate >= now)
                .Select(uw => uw.UserId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var validWaiverUserIdSet = validWaiverUserIds.ToHashSet();

            return attendees.Select(a => new AttendeeWaiverStatus
            {
                UserId = a.UserId,
                UserName = a.User?.UserName ?? "Unknown",
                HasValidWaiver = validWaiverUserIdSet.Contains(a.UserId)
            });
        }

        /// <inheritdoc />
        public async Task<WaiverComplianceSummary> GetComplianceSummaryAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var thirtyDaysFromNow = now.AddDays(30);

            // Get total active users (users who have logged in within last year)
            var oneYearAgo = now.AddYears(-1);
            var totalActiveUsers = await userRepository
                .Get(u => u.DateAgreedToTrashMobWaiver >= oneYearAgo || u.MemberSince >= oneYearAgo)
                .CountAsync(cancellationToken);

            // Get users with valid waivers
            var usersWithValidWaivers = await Repo
                .Get(uw => uw.ExpiryDate >= now)
                .Select(uw => uw.UserId)
                .Distinct()
                .CountAsync(cancellationToken);

            // Get users with expiring waivers (within 30 days)
            var usersWithExpiringWaivers = await Repo
                .Get(uw => uw.ExpiryDate >= now && uw.ExpiryDate <= thirtyDaysFromNow)
                .Select(uw => uw.UserId)
                .Distinct()
                .CountAsync(cancellationToken);

            // Get waiver counts using database-side aggregation instead of loading all records
            var waiverCounts = await Repo.Get()
                .GroupBy(w => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    ESignature = g.Count(w => w.SigningMethod != "PaperUpload"),
                    PaperUpload = g.Count(w => w.SigningMethod == "PaperUpload"),
                    Minor = g.Count(w => w.IsMinor),
                })
                .FirstOrDefaultAsync(cancellationToken);

            var totalSignedWaivers = waiverCounts?.Total ?? 0;
            var eSignatureCount = waiverCounts?.ESignature ?? 0;
            var paperUploadCount = waiverCounts?.PaperUpload ?? 0;
            var minorWaiversCount = waiverCounts?.Minor ?? 0;

            var usersWithoutWaivers = totalActiveUsers - usersWithValidWaivers;
            if (usersWithoutWaivers < 0) usersWithoutWaivers = 0;

            var compliancePercentage = totalActiveUsers > 0
                ? Math.Round((decimal)usersWithValidWaivers / totalActiveUsers * 100, 2)
                : 0;

            return new WaiverComplianceSummary
            {
                TotalActiveUsers = totalActiveUsers,
                UsersWithValidWaivers = usersWithValidWaivers,
                UsersWithExpiringWaivers = usersWithExpiringWaivers,
                UsersWithoutWaivers = usersWithoutWaivers,
                TotalSignedWaivers = totalSignedWaivers,
                ESignatureCount = eSignatureCount,
                PaperUploadCount = paperUploadCount,
                MinorWaiversCount = minorWaiversCount,
                CompliancePercentage = compliancePercentage,
                GeneratedAt = now
            };
        }

        /// <inheritdoc />
        public async Task<UserWaiverListResult> GetUserWaiversFilteredAsync(UserWaiverFilter filter, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var query = Repo.Get()
                .Include(uw => uw.User)
                .Include(uw => uw.WaiverVersion)
                .AsQueryable();

            // Apply filters
            if (filter.WaiverVersionId.HasValue)
            {
                query = query.Where(uw => uw.WaiverVersionId == filter.WaiverVersionId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SigningMethod))
            {
                query = query.Where(uw => uw.SigningMethod == filter.SigningMethod);
            }

            if (filter.IsValid.HasValue)
            {
                if (filter.IsValid.Value)
                {
                    query = query.Where(uw => uw.ExpiryDate >= now);
                }
                else
                {
                    query = query.Where(uw => uw.ExpiryDate < now);
                }
            }

            if (filter.IsMinor.HasValue)
            {
                query = query.Where(uw => uw.IsMinor == filter.IsMinor.Value);
            }

            if (filter.AcceptedDateFrom.HasValue)
            {
                query = query.Where(uw => uw.AcceptedDate >= filter.AcceptedDateFrom.Value);
            }

            if (filter.AcceptedDateTo.HasValue)
            {
                query = query.Where(uw => uw.AcceptedDate <= filter.AcceptedDateTo.Value);
            }

            if (filter.ExpiryDateFrom.HasValue)
            {
                query = query.Where(uw => uw.ExpiryDate >= filter.ExpiryDateFrom.Value);
            }

            if (filter.ExpiryDateTo.HasValue)
            {
                query = query.Where(uw => uw.ExpiryDate <= filter.ExpiryDateTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                // Suppressing CA1862 - ToLower().Contains() is required for EF Core SQL translation (LOWER() function)
#pragma warning disable CA1862
                query = query.Where(uw =>
                    uw.User.UserName.ToLower().Contains(searchTerm) ||
                    uw.User.Email.ToLower().Contains(searchTerm) ||
                    uw.TypedLegalName.ToLower().Contains(searchTerm));
#pragma warning restore CA1862
            }

            // Get total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination and ordering
            var skip = (filter.Page - 1) * filter.PageSize;
            var items = await query
                .OrderByDescending(uw => uw.AcceptedDate)
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(uw => new UserWaiverDetail
                {
                    Id = uw.Id,
                    UserId = uw.UserId,
                    UserName = uw.User.UserName,
                    UserEmail = uw.User.Email,
                    WaiverVersionId = uw.WaiverVersionId,
                    WaiverName = uw.WaiverVersion.Name,
                    WaiverVersion = uw.WaiverVersion.Version,
                    TypedLegalName = uw.TypedLegalName,
                    AcceptedDate = uw.AcceptedDate,
                    ExpiryDate = uw.ExpiryDate,
                    SigningMethod = uw.SigningMethod,
                    IsMinor = uw.IsMinor,
                    GuardianName = uw.GuardianName,
                    IsValid = uw.ExpiryDate >= now,
                    IPAddress = uw.IPAddress,
                    DocumentUrl = uw.DocumentUrl
                })
                .ToListAsync(cancellationToken);

            return new UserWaiverListResult
            {
                Items = items,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<WaiverExportRecord>> GetWaiversForExportAsync(UserWaiverFilter filter, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var query = Repo.Get()
                .Include(uw => uw.User)
                .Include(uw => uw.WaiverVersion)
                .AsQueryable();

            // Apply same filters as GetUserWaiversFilteredAsync (but without pagination)
            if (filter.WaiverVersionId.HasValue)
            {
                query = query.Where(uw => uw.WaiverVersionId == filter.WaiverVersionId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SigningMethod))
            {
                query = query.Where(uw => uw.SigningMethod == filter.SigningMethod);
            }

            if (filter.IsValid.HasValue)
            {
                if (filter.IsValid.Value)
                {
                    query = query.Where(uw => uw.ExpiryDate >= now);
                }
                else
                {
                    query = query.Where(uw => uw.ExpiryDate < now);
                }
            }

            if (filter.IsMinor.HasValue)
            {
                query = query.Where(uw => uw.IsMinor == filter.IsMinor.Value);
            }

            if (filter.AcceptedDateFrom.HasValue)
            {
                query = query.Where(uw => uw.AcceptedDate >= filter.AcceptedDateFrom.Value);
            }

            if (filter.AcceptedDateTo.HasValue)
            {
                query = query.Where(uw => uw.AcceptedDate <= filter.AcceptedDateTo.Value);
            }

            if (filter.ExpiryDateFrom.HasValue)
            {
                query = query.Where(uw => uw.ExpiryDate >= filter.ExpiryDateFrom.Value);
            }

            if (filter.ExpiryDateTo.HasValue)
            {
                query = query.Where(uw => uw.ExpiryDate <= filter.ExpiryDateTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                // Suppressing CA1862 - ToLower().Contains() is required for EF Core SQL translation (LOWER() function)
#pragma warning disable CA1862
                query = query.Where(uw =>
                    uw.User.UserName.ToLower().Contains(searchTerm) ||
                    uw.User.Email.ToLower().Contains(searchTerm) ||
                    uw.TypedLegalName.ToLower().Contains(searchTerm));
#pragma warning restore CA1862
            }

            return await query
                .OrderByDescending(uw => uw.AcceptedDate)
                .Select(uw => new WaiverExportRecord
                {
                    UserWaiverId = uw.Id.ToString(),
                    UserId = uw.UserId.ToString(),
                    UserName = uw.User.UserName,
                    UserEmail = uw.User.Email,
                    TypedLegalName = uw.TypedLegalName,
                    WaiverName = uw.WaiverVersion.Name,
                    WaiverVersion = uw.WaiverVersion.Version,
                    AcceptedDate = uw.AcceptedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    ExpiryDate = uw.ExpiryDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    SigningMethod = uw.SigningMethod,
                    IsMinor = uw.IsMinor ? "Yes" : "No",
                    GuardianName = uw.GuardianName ?? "",
                    GuardianRelationship = uw.GuardianRelationship ?? "",
                    IPAddress = uw.IPAddress ?? "",
                    UserAgent = uw.UserAgent ?? "",
                    DocumentUrl = uw.DocumentUrl ?? ""
                })
                .ToListAsync(cancellationToken);
        }

        private static DateTimeOffset GetEndOfYear()
        {
            var now = DateTimeOffset.UtcNow;
            return new DateTimeOffset(now.Year, 12, 31, 23, 59, 59, TimeSpan.Zero);
        }
    }
}
