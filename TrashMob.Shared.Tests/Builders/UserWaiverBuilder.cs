namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using TrashMob.Models;

    /// <summary>
    /// Builder for creating UserWaiver test data with sensible defaults.
    /// </summary>
    public class UserWaiverBuilder
    {
        private readonly UserWaiver _userWaiver;

        public UserWaiverBuilder()
        {
            var userId = Guid.NewGuid();
            var now = DateTimeOffset.UtcNow;
            _userWaiver = new UserWaiver
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                WaiverVersionId = Guid.NewGuid(),
                AcceptedDate = now,
                ExpiryDate = new DateTimeOffset(now.Year, 12, 31, 23, 59, 59, TimeSpan.Zero),
                TypedLegalName = "John Doe",
                WaiverTextSnapshot = "Test waiver text content.",
                SigningMethod = "ESignatureWeb",
                IPAddress = "192.168.1.1",
                UserAgent = "Mozilla/5.0 Test Browser",
                IsMinor = false,
                CreatedByUserId = userId,
                CreatedDate = now,
                LastUpdatedByUserId = userId,
                LastUpdatedDate = now
            };
        }

        public UserWaiverBuilder WithId(Guid id)
        {
            _userWaiver.Id = id;
            return this;
        }

        public UserWaiverBuilder ForUser(Guid userId)
        {
            _userWaiver.UserId = userId;
            _userWaiver.CreatedByUserId = userId;
            _userWaiver.LastUpdatedByUserId = userId;
            return this;
        }

        public UserWaiverBuilder ForWaiverVersion(Guid waiverVersionId)
        {
            _userWaiver.WaiverVersionId = waiverVersionId;
            return this;
        }

        public UserWaiverBuilder WithTypedLegalName(string name)
        {
            _userWaiver.TypedLegalName = name;
            return this;
        }

        public UserWaiverBuilder AcceptedOn(DateTimeOffset date)
        {
            _userWaiver.AcceptedDate = date;
            return this;
        }

        public UserWaiverBuilder ExpiresOn(DateTimeOffset date)
        {
            _userWaiver.ExpiryDate = date;
            return this;
        }

        public UserWaiverBuilder AsValid()
        {
            var now = DateTimeOffset.UtcNow;
            _userWaiver.ExpiryDate = new DateTimeOffset(now.Year, 12, 31, 23, 59, 59, TimeSpan.Zero);
            return this;
        }

        public UserWaiverBuilder AsExpired()
        {
            _userWaiver.ExpiryDate = DateTimeOffset.UtcNow.AddDays(-7);
            return this;
        }

        public UserWaiverBuilder ExpiringIn(int days)
        {
            _userWaiver.ExpiryDate = DateTimeOffset.UtcNow.AddDays(days);
            return this;
        }

        public UserWaiverBuilder AsESignature()
        {
            _userWaiver.SigningMethod = "ESignatureWeb";
            return this;
        }

        public UserWaiverBuilder AsPaperUpload(Guid? uploadedByUserId = null)
        {
            _userWaiver.SigningMethod = "PaperUpload";
            _userWaiver.UploadedByUserId = uploadedByUserId ?? Guid.NewGuid();
            return this;
        }

        public UserWaiverBuilder AsMinor(string guardianName = null, string relationship = null, Guid? guardianUserId = null)
        {
            _userWaiver.IsMinor = true;
            _userWaiver.GuardianName = guardianName ?? "Jane Doe";
            _userWaiver.GuardianRelationship = relationship ?? "Parent";
            _userWaiver.GuardianUserId = guardianUserId;
            return this;
        }

        public UserWaiverBuilder WithIPAddress(string ipAddress)
        {
            _userWaiver.IPAddress = ipAddress;
            return this;
        }

        public UserWaiverBuilder WithUserAgent(string userAgent)
        {
            _userWaiver.UserAgent = userAgent;
            return this;
        }

        public UserWaiverBuilder WithDocumentUrl(string url)
        {
            _userWaiver.DocumentUrl = url;
            return this;
        }

        public UserWaiverBuilder WithUser(User user)
        {
            _userWaiver.UserId = user.Id;
            _userWaiver.User = user;
            return this;
        }

        public UserWaiverBuilder WithWaiverVersion(WaiverVersion version)
        {
            _userWaiver.WaiverVersionId = version.Id;
            _userWaiver.WaiverVersion = version;
            return this;
        }

        public UserWaiver Build() => _userWaiver;
    }
}
