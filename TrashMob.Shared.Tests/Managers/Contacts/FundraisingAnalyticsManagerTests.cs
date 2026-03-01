namespace TrashMob.Shared.Tests.Managers.Contacts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Contacts;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class FundraisingAnalyticsManagerTests
    {
        private readonly Mock<IKeyedRepository<Contact>> _contactRepo;
        private readonly Mock<IKeyedRepository<Donation>> _donationRepo;
        private readonly Mock<IKeyedRepository<ContactNote>> _noteRepo;
        private readonly Mock<IKeyedRepository<Grant>> _grantRepo;
        private readonly Mock<IBaseRepository<EventAttendee>> _attendeeRepo;
        private readonly Mock<IKeyedRepository<EventAttendeeMetrics>> _metricsRepo;
        private readonly FundraisingAnalyticsManager _sut;

        public FundraisingAnalyticsManagerTests()
        {
            _contactRepo = new Mock<IKeyedRepository<Contact>>();
            _donationRepo = new Mock<IKeyedRepository<Donation>>();
            _noteRepo = new Mock<IKeyedRepository<ContactNote>>();
            _grantRepo = new Mock<IKeyedRepository<Grant>>();
            _attendeeRepo = new Mock<IBaseRepository<EventAttendee>>();
            _metricsRepo = new Mock<IKeyedRepository<EventAttendeeMetrics>>();
            _sut = new FundraisingAnalyticsManager(
                _contactRepo.Object, _donationRepo.Object, _noteRepo.Object,
                _grantRepo.Object, _attendeeRepo.Object, _metricsRepo.Object);
        }

        private void SetupEmptyRepos()
        {
            _contactRepo.SetupGet(new List<Contact>());
            _donationRepo.SetupGet(new List<Donation>());
            _noteRepo.SetupGet(new List<ContactNote>());
            _grantRepo.SetupGet(new List<Grant>());
            _attendeeRepo.SetupGet(new List<EventAttendee>());
            _metricsRepo.SetupGet(new List<EventAttendeeMetrics>());
        }

        private Contact CreateContact(Guid? id = null, Guid? userId = null)
        {
            return new Contact
            {
                Id = id ?? Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                ContactType = 1,
                IsActive = true,
                UserId = userId,
            };
        }

        private Donation CreateDonation(Guid contactId, decimal amount, DateTimeOffset date)
        {
            return new Donation
            {
                Id = Guid.NewGuid(),
                ContactId = contactId,
                Amount = amount,
                DonationDate = date,
                DonationType = 1,
            };
        }

        [Fact]
        public async Task GetEngagementScores_NoContacts_ReturnsEmpty()
        {
            SetupEmptyRepos();

            var result = await _sut.GetEngagementScoresAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetEngagementScores_ContactWithNoData_ReturnsZeroScore()
        {
            var contact = CreateContact();
            _contactRepo.SetupGet(new List<Contact> { contact });
            _donationRepo.SetupGet(new List<Donation>());
            _noteRepo.SetupGet(new List<ContactNote>());
            _grantRepo.SetupGet(new List<Grant>());
            _attendeeRepo.SetupGet(new List<EventAttendee>());
            _metricsRepo.SetupGet(new List<EventAttendeeMetrics>());

            var result = (await _sut.GetEngagementScoresAsync()).ToList();

            Assert.Single(result);
            Assert.Equal(0, result[0].EngagementScore);
            Assert.Equal("Prospect", result[0].DonorLifecycleStage);
        }

        [Fact]
        public async Task LifecycleStage_OneDonation_FirstTimeDonor()
        {
            var contact = CreateContact();
            var donation = CreateDonation(contact.Id, 100, DateTimeOffset.UtcNow.AddDays(-30));

            _contactRepo.SetupGet(new List<Contact> { contact });
            _donationRepo.SetupGet(new List<Donation> { donation });
            _noteRepo.SetupGet(new List<ContactNote>());
            _grantRepo.SetupGet(new List<Grant>());
            _attendeeRepo.SetupGet(new List<EventAttendee>());
            _metricsRepo.SetupGet(new List<EventAttendeeMetrics>());

            var result = (await _sut.GetEngagementScoresAsync()).ToList();

            Assert.Equal("First-Time Donor", result[0].DonorLifecycleStage);
        }

        [Fact]
        public async Task LifecycleStage_TwoDonations_RepeatDonor()
        {
            var contact = CreateContact();
            var donations = new List<Donation>
            {
                CreateDonation(contact.Id, 100, DateTimeOffset.UtcNow.AddDays(-60)),
                CreateDonation(contact.Id, 200, DateTimeOffset.UtcNow.AddDays(-30)),
            };

            _contactRepo.SetupGet(new List<Contact> { contact });
            _donationRepo.SetupGet(donations);
            _noteRepo.SetupGet(new List<ContactNote>());
            _grantRepo.SetupGet(new List<Grant>());
            _attendeeRepo.SetupGet(new List<EventAttendee>());
            _metricsRepo.SetupGet(new List<EventAttendeeMetrics>());

            var result = (await _sut.GetEngagementScoresAsync()).ToList();

            Assert.Equal("Repeat Donor", result[0].DonorLifecycleStage);
        }

        [Fact]
        public async Task LifecycleStage_Over5000_MajorDonor()
        {
            var contact = CreateContact();
            var donations = new List<Donation>
            {
                CreateDonation(contact.Id, 3000, DateTimeOffset.UtcNow.AddDays(-60)),
                CreateDonation(contact.Id, 3000, DateTimeOffset.UtcNow.AddDays(-30)),
            };

            _contactRepo.SetupGet(new List<Contact> { contact });
            _donationRepo.SetupGet(donations);
            _noteRepo.SetupGet(new List<ContactNote>());
            _grantRepo.SetupGet(new List<Grant>());
            _attendeeRepo.SetupGet(new List<EventAttendee>());
            _metricsRepo.SetupGet(new List<EventAttendeeMetrics>());

            var result = (await _sut.GetEngagementScoresAsync()).ToList();

            Assert.Equal("Major Donor", result[0].DonorLifecycleStage);
        }

        [Fact]
        public async Task LifecycleStage_NoDonationsInYear_Lapsed()
        {
            var contact = CreateContact();
            var donation = CreateDonation(contact.Id, 500, DateTimeOffset.UtcNow.AddMonths(-18));

            _contactRepo.SetupGet(new List<Contact> { contact });
            _donationRepo.SetupGet(new List<Donation> { donation });
            _noteRepo.SetupGet(new List<ContactNote>());
            _grantRepo.SetupGet(new List<Grant>());
            _attendeeRepo.SetupGet(new List<EventAttendee>());
            _metricsRepo.SetupGet(new List<EventAttendeeMetrics>());

            var result = (await _sut.GetEngagementScoresAsync()).ToList();

            Assert.Equal("Lapsed", result[0].DonorLifecycleStage);
        }

        [Fact]
        public async Task LybuntDetection_DonatedLastYearNotThisYear()
        {
            var contact = CreateContact();
            var lastYear = new DateTimeOffset(DateTimeOffset.UtcNow.Year - 1, 6, 15, 0, 0, 0, TimeSpan.Zero);
            var donation = CreateDonation(contact.Id, 200, lastYear);

            _contactRepo.SetupGet(new List<Contact> { contact });
            _donationRepo.SetupGet(new List<Donation> { donation });
            _noteRepo.SetupGet(new List<ContactNote>());
            _grantRepo.SetupGet(new List<Grant>());
            _attendeeRepo.SetupGet(new List<EventAttendee>());
            _metricsRepo.SetupGet(new List<EventAttendeeMetrics>());

            var result = (await _sut.GetEngagementScoresAsync()).ToList();

            Assert.True(result[0].IsLybunt);
        }

        [Fact]
        public async Task LybuntDetection_DonatedThisYear_NotLybunt()
        {
            var contact = CreateContact();
            var thisYear = new DateTimeOffset(DateTimeOffset.UtcNow.Year, 1, 15, 0, 0, 0, TimeSpan.Zero);
            var donation = CreateDonation(contact.Id, 200, thisYear);

            _contactRepo.SetupGet(new List<Contact> { contact });
            _donationRepo.SetupGet(new List<Donation> { donation });
            _noteRepo.SetupGet(new List<ContactNote>());
            _grantRepo.SetupGet(new List<Grant>());
            _attendeeRepo.SetupGet(new List<EventAttendee>());
            _metricsRepo.SetupGet(new List<EventAttendeeMetrics>());

            var result = (await _sut.GetEngagementScoresAsync()).ToList();

            Assert.False(result[0].IsLybunt);
        }

        [Fact]
        public async Task VolunteerPipeline_HighEngagementNoDonations()
        {
            var userId = Guid.NewGuid();
            var contact = CreateContact(userId: userId);

            var attendees = Enumerable.Range(0, 5).Select(_ => new EventAttendee
            {
                EventId = Guid.NewGuid(),
                UserId = userId,
            }).ToList();

            _contactRepo.SetupGet(new List<Contact> { contact });
            _donationRepo.SetupGet(new List<Donation>());
            _noteRepo.SetupGet(new List<ContactNote>
            {
                new() { Id = Guid.NewGuid(), ContactId = contact.Id, Body = "Test", CreatedDate = DateTimeOffset.UtcNow.AddDays(-5) },
            });
            _grantRepo.SetupGet(new List<Grant>());
            _attendeeRepo.SetupGet(attendees);
            _metricsRepo.SetupGet(new List<EventAttendeeMetrics>());

            var result = (await _sut.GetVolunteerToDonorPipelineAsync()).ToList();

            Assert.Single(result);
            Assert.Equal(0, result[0].DonationCount);
            Assert.True(result[0].EngagementScore >= 20);
        }

        [Fact]
        public async Task ScoreComponents_RecentLargeDonation_HighDonationScore()
        {
            var contact = CreateContact();
            var donations = Enumerable.Range(0, 6).Select(i =>
                CreateDonation(contact.Id, 1000, DateTimeOffset.UtcNow.AddDays(-10 * (i + 1)))).ToList();

            _contactRepo.SetupGet(new List<Contact> { contact });
            _donationRepo.SetupGet(donations);
            _noteRepo.SetupGet(new List<ContactNote>());
            _grantRepo.SetupGet(new List<Grant>());
            _attendeeRepo.SetupGet(new List<EventAttendee>());
            _metricsRepo.SetupGet(new List<EventAttendeeMetrics>());

            var result = (await _sut.GetEngagementScoresAsync()).ToList();

            // Without UserId, score is redistributed: donation max is 57
            // Recency: 15pts (within 90 days), Frequency: 15pts (5+ donations), Amount: 10pts ($5000+ lifetime) = 40 raw
            // Scaled to 57: 40 * 57/40 = 57
            Assert.True(result[0].DonationScoreComponent > 0);
            Assert.True(result[0].EngagementScore > 30);
        }

        [Fact]
        public async Task Dashboard_YtdTotals()
        {
            var contact = CreateContact();
            var ytdDonation = CreateDonation(contact.Id, 500, DateTimeOffset.UtcNow.AddDays(-10));

            _contactRepo.SetupGet(new List<Contact> { contact });
            _donationRepo.SetupGet(new List<Donation> { ytdDonation });
            _noteRepo.SetupGet(new List<ContactNote>());
            _grantRepo.SetupGet(new List<Grant>());
            _attendeeRepo.SetupGet(new List<EventAttendee>());
            _metricsRepo.SetupGet(new List<EventAttendeeMetrics>());

            var dashboard = await _sut.GetDashboardAsync();

            Assert.Equal(500, dashboard.TotalRaisedYtd);
            Assert.Equal(1, dashboard.DonorCountYtd);
            Assert.Equal(1, dashboard.DonationCountYtd);
            Assert.Equal(500, dashboard.AverageGiftSizeYtd);
        }

        [Fact]
        public async Task CsvGeneration_ProducesValidOutput()
        {
            var contact = CreateContact();
            _contactRepo.SetupGet(new List<Contact> { contact });
            _donationRepo.SetupGet(new List<Donation>());
            _noteRepo.SetupGet(new List<ContactNote>());
            _grantRepo.SetupGet(new List<Grant>());
            _attendeeRepo.SetupGet(new List<EventAttendee>());
            _metricsRepo.SetupGet(new List<EventAttendeeMetrics>());

            var csv = await _sut.GenerateDonorReportCsvAsync();

            Assert.Contains("Name,Email,Type,Engagement Score", csv);
            Assert.Contains("John Doe", csv);
        }
    }
}
