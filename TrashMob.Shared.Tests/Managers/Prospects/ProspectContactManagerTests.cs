namespace TrashMob.Shared.Tests.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Prospects;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class ProspectContactManagerTests
    {
        private readonly Mock<IKeyedRepository<ProspectContact>> _contactRepo;
        private readonly Mock<IKeyedRepository<ProspectActivity>> _activityRepo;
        private readonly Mock<IKeyedRepository<ProspectOutreachEmail>> _outreachRepo;
        private readonly ProspectContactManager _sut;
        private readonly Guid _userId = Guid.NewGuid();

        public ProspectContactManagerTests()
        {
            _contactRepo = new Mock<IKeyedRepository<ProspectContact>>();
            _activityRepo = new Mock<IKeyedRepository<ProspectActivity>>();
            _outreachRepo = new Mock<IKeyedRepository<ProspectOutreachEmail>>();

            _contactRepo.SetupAddAsync();
            _contactRepo.SetupUpdateAsync();
            _activityRepo.SetupGet(new List<ProspectActivity>());
            _outreachRepo.SetupGet(new List<ProspectOutreachEmail>());

            _sut = new ProspectContactManager(_contactRepo.Object, _activityRepo.Object, _outreachRepo.Object);
        }

        [Fact]
        public async Task GetByProspectAsync_ReturnsPrimaryFirst()
        {
            var prospectId = Guid.NewGuid();
            var contacts = new[]
            {
                MakeContact(prospectId, "Bob", isPrimary: false),
                MakeContact(prospectId, "Alice", isPrimary: true),
                MakeContact(prospectId, "Carla", isPrimary: false),
            };
            _contactRepo.SetupGet(contacts);

            var result = (await _sut.GetByProspectAsync(prospectId)).ToList();

            Assert.Equal(3, result.Count);
            Assert.True(result[0].IsPrimary);
            Assert.Equal("Alice", result[0].Name);
        }

        [Fact]
        public async Task SetPrimaryAsync_DemotesExistingPrimary()
        {
            var prospectId = Guid.NewGuid();
            var oldPrimary = MakeContact(prospectId, "Alice", isPrimary: true);
            var newPrimary = MakeContact(prospectId, "Bob", isPrimary: false);
            _contactRepo.SetupGet(new[] { oldPrimary, newPrimary });
            _contactRepo.SetupGetAsync(new[] { oldPrimary, newPrimary });

            var result = await _sut.SetPrimaryAsync(newPrimary.Id, _userId);

            Assert.NotNull(result);
            Assert.True(result.IsPrimary);
            _contactRepo.Verify(r => r.UpdateAsync(It.Is<ProspectContact>(c => c.Id == oldPrimary.Id && !c.IsPrimary)), Times.Once);
            _contactRepo.Verify(r => r.UpdateAsync(It.Is<ProspectContact>(c => c.Id == newPrimary.Id && c.IsPrimary)), Times.Once);
        }

        [Fact]
        public async Task SetPrimaryAsync_WhenAlreadyPrimary_StillUpdatesLastModified()
        {
            var prospectId = Guid.NewGuid();
            var primary = MakeContact(prospectId, "Alice", isPrimary: true);
            _contactRepo.SetupGet(new[] { primary });
            _contactRepo.SetupGetAsync(new[] { primary });

            var result = await _sut.SetPrimaryAsync(primary.Id, _userId);

            Assert.True(result.IsPrimary);
            // No siblings to demote, but the target itself is updated.
            _contactRepo.Verify(r => r.UpdateAsync(It.Is<ProspectContact>(c => c.Id == primary.Id)), Times.Once);
        }

        [Fact]
        public async Task SetPrimaryAsync_DoesNotTouchContactsOnOtherProspects()
        {
            var prospectId = Guid.NewGuid();
            var otherProspectId = Guid.NewGuid();
            var newPrimary = MakeContact(prospectId, "Bob", isPrimary: false);
            var oldPrimaryOtherProspect = MakeContact(otherProspectId, "Carla", isPrimary: true);
            _contactRepo.SetupGet(new[] { newPrimary, oldPrimaryOtherProspect });
            _contactRepo.SetupGetAsync(new[] { newPrimary, oldPrimaryOtherProspect });

            await _sut.SetPrimaryAsync(newPrimary.Id, _userId);

            // Carla on the other prospect must NOT be demoted.
            _contactRepo.Verify(r => r.UpdateAsync(It.Is<ProspectContact>(c => c.Id == oldPrimaryOtherProspect.Id)), Times.Never);
        }

        [Fact]
        public async Task SetPrimaryAsync_WhenContactNotFound_ReturnsNull()
        {
            _contactRepo.SetupGet(new List<ProspectContact>());
            _contactRepo.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProspectContact)null);

            var result = await _sut.SetPrimaryAsync(Guid.NewGuid(), _userId);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateStatusAsync_PersistsStatusAndAuditFields()
        {
            var contact = MakeContact(Guid.NewGuid(), "Alice", isPrimary: true);
            _contactRepo.SetupGetAsync(contact);

            var result = await _sut.UpdateStatusAsync(contact.Id, (int)ProspectContactStatus.WrongPerson, _userId);

            Assert.Equal((int)ProspectContactStatus.WrongPerson, result.ContactStatus);
            Assert.Equal(_userId, result.LastUpdatedByUserId);
            _contactRepo.Verify(r => r.UpdateAsync(It.Is<ProspectContact>(c => c.ContactStatus == (int)ProspectContactStatus.WrongPerson)), Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_WhenContactNotFound_ReturnsNull()
        {
            _contactRepo.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProspectContact)null);

            var result = await _sut.UpdateStatusAsync(Guid.NewGuid(), (int)ProspectContactStatus.NoResponse, _userId);

            Assert.Null(result);
        }

        [Fact]
        public async Task HasReferencesAsync_FalseWhenNoActivityOrOutreach()
        {
            var contactId = Guid.NewGuid();

            var result = await _sut.HasReferencesAsync(contactId);

            Assert.False(result);
        }

        [Fact]
        public async Task HasReferencesAsync_TrueWhenActivityReferencesContact()
        {
            var contactId = Guid.NewGuid();
            _activityRepo.SetupGet(new[]
            {
                new ProspectActivity
                {
                    Id = Guid.NewGuid(),
                    ProspectId = Guid.NewGuid(),
                    ProspectContactId = contactId,
                    ActivityType = "Call",
                },
            });

            var result = await _sut.HasReferencesAsync(contactId);

            Assert.True(result);
        }

        [Fact]
        public async Task HasReferencesAsync_TrueWhenOutreachEmailReferencesContact()
        {
            var contactId = Guid.NewGuid();
            _outreachRepo.SetupGet(new[]
            {
                new ProspectOutreachEmail
                {
                    Id = Guid.NewGuid(),
                    ProspectId = Guid.NewGuid(),
                    ProspectContactId = contactId,
                    Status = "Sent",
                },
            });

            var result = await _sut.HasReferencesAsync(contactId);

            Assert.True(result);
        }

        private static ProspectContact MakeContact(Guid prospectId, string name, bool isPrimary)
        {
            return new ProspectContact
            {
                Id = Guid.NewGuid(),
                ProspectId = prospectId,
                Name = name,
                Email = $"{name.ToLowerInvariant()}@example.com",
                ContactStatus = (int)ProspectContactStatus.Active,
                IsPrimary = isPrimary,
            };
        }
    }
}
