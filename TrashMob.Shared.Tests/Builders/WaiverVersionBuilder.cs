namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using TrashMob.Models;

    /// <summary>
    /// Builder for creating WaiverVersion test data with sensible defaults.
    /// </summary>
    public class WaiverVersionBuilder
    {
        private readonly WaiverVersion _waiverVersion;

        public WaiverVersionBuilder()
        {
            var userId = Guid.NewGuid();
            _waiverVersion = new WaiverVersion
            {
                Id = Guid.NewGuid(),
                Name = "TrashMob Liability Waiver",
                Version = "1.0",
                WaiverText = "Test waiver text content.",
                EffectiveDate = DateTimeOffset.UtcNow.AddDays(-30),
                ExpiryDate = null,
                IsActive = true,
                Scope = WaiverScope.Global,
                CreatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow.AddDays(-30),
                LastUpdatedByUserId = userId,
                LastUpdatedDate = DateTimeOffset.UtcNow.AddDays(-30)
            };
        }

        public WaiverVersionBuilder WithId(Guid id)
        {
            _waiverVersion.Id = id;
            return this;
        }

        public WaiverVersionBuilder WithName(string name)
        {
            _waiverVersion.Name = name;
            return this;
        }

        public WaiverVersionBuilder WithVersion(string version)
        {
            _waiverVersion.Version = version;
            return this;
        }

        public WaiverVersionBuilder WithWaiverText(string text)
        {
            _waiverVersion.WaiverText = text;
            return this;
        }

        public WaiverVersionBuilder AsGlobal()
        {
            _waiverVersion.Scope = WaiverScope.Global;
            return this;
        }

        public WaiverVersionBuilder AsCommunity()
        {
            _waiverVersion.Scope = WaiverScope.Community;
            return this;
        }

        public WaiverVersionBuilder AsActive()
        {
            _waiverVersion.IsActive = true;
            return this;
        }

        public WaiverVersionBuilder AsInactive()
        {
            _waiverVersion.IsActive = false;
            return this;
        }

        public WaiverVersionBuilder WithEffectiveDate(DateTimeOffset date)
        {
            _waiverVersion.EffectiveDate = date;
            return this;
        }

        public WaiverVersionBuilder WithExpiryDate(DateTimeOffset? date)
        {
            _waiverVersion.ExpiryDate = date;
            return this;
        }

        public WaiverVersionBuilder EffectiveInPast(int days = 30)
        {
            _waiverVersion.EffectiveDate = DateTimeOffset.UtcNow.AddDays(-days);
            return this;
        }

        public WaiverVersionBuilder EffectiveInFuture(int days = 7)
        {
            _waiverVersion.EffectiveDate = DateTimeOffset.UtcNow.AddDays(days);
            return this;
        }

        public WaiverVersionBuilder ExpiredInPast(int days = 7)
        {
            _waiverVersion.ExpiryDate = DateTimeOffset.UtcNow.AddDays(-days);
            return this;
        }

        public WaiverVersion Build() => _waiverVersion;
    }
}
