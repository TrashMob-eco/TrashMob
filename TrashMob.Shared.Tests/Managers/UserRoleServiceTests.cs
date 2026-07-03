namespace TrashMob.Shared.Tests.Managers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Persistence;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="UserRoleService"/>. Cover the four bits of
    /// non-obvious behaviour: active-grant lookup, revoked / expired filtering,
    /// the compatibility bridge for the legacy <see cref="User.IsSiteAdmin"/>
    /// boolean, and the per-request cache.
    /// </summary>
    public class UserRoleServiceTests : IDisposable
    {
        private readonly MobDbContext db;
        private readonly UserRoleService sut;

        private static readonly int SiteAdminRoleId = 1;
        private static readonly int SalesRepRoleId = 2;

        public UserRoleServiceTests()
        {
            var options = new DbContextOptionsBuilder<MobDbContext>()
                .UseInMemoryDatabase(databaseName: $"UserRoleService_{Guid.NewGuid()}")
                .Options;
            db = new MobDbContext(options);
            SeedRoles();
            sut = new UserRoleService(db);
        }

        private void SeedRoles()
        {
            db.Roles.AddRange(
                new Role { Id = SiteAdminRoleId, Name = RoleNames.SiteAdmin, IsActive = true },
                new Role { Id = SalesRepRoleId, Name = RoleNames.SalesRep, IsActive = true });
            db.SaveChanges();
        }

        private User AddUser(bool isSiteAdmin = false)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "test",
                Email = "test@example.com",
                IsSiteAdmin = isSiteAdmin,
                MemberSince = DateTimeOffset.UtcNow.AddYears(-1),
            };
            db.Users.Add(user);
            db.SaveChanges();
            return user;
        }

        private UserRole AddGrant(Guid userId, int roleId,
            DateTimeOffset? expiryDate = null, DateTimeOffset? revokedDate = null)
        {
            var grant = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                GrantedByUserId = userId,
                GrantedDate = DateTimeOffset.UtcNow.AddDays(-1),
                ExpiryDate = expiryDate,
                RevokedDate = revokedDate,
            };
            db.UserRoles.Add(grant);
            db.SaveChanges();
            return grant;
        }

        [Fact]
        public async Task HasRoleAsync_ReturnsTrue_WhenActiveGrantExists()
        {
            var user = AddUser();
            AddGrant(user.Id, SalesRepRoleId);

            Assert.True(await sut.HasRoleAsync(user.Id, RoleNames.SalesRep));
        }

        [Fact]
        public async Task HasRoleAsync_ReturnsFalse_WhenGrantIsRevoked()
        {
            var user = AddUser();
            AddGrant(user.Id, SalesRepRoleId, revokedDate: DateTimeOffset.UtcNow.AddHours(-1));

            Assert.False(await sut.HasRoleAsync(user.Id, RoleNames.SalesRep));
        }

        [Fact]
        public async Task HasRoleAsync_ReturnsFalse_WhenGrantIsExpired()
        {
            var user = AddUser();
            AddGrant(user.Id, SalesRepRoleId, expiryDate: DateTimeOffset.UtcNow.AddHours(-1));

            Assert.False(await sut.HasRoleAsync(user.Id, RoleNames.SalesRep));
        }

        [Fact]
        public async Task HasRoleAsync_ReturnsTrue_WhenGrantHasFutureExpiry()
        {
            var user = AddUser();
            AddGrant(user.Id, SalesRepRoleId, expiryDate: DateTimeOffset.UtcNow.AddDays(30));

            Assert.True(await sut.HasRoleAsync(user.Id, RoleNames.SalesRep));
        }

        [Fact]
        public async Task HasRoleAsync_IsCaseInsensitive()
        {
            var user = AddUser();
            AddGrant(user.Id, SalesRepRoleId);

            Assert.True(await sut.HasRoleAsync(user.Id, "salesrep"));
            Assert.True(await sut.HasRoleAsync(user.Id, "SALESREP"));
        }

        [Fact]
        public async Task HasRoleAsync_ReturnsFalse_ForEmptyRoleName()
        {
            var user = AddUser();
            AddGrant(user.Id, SiteAdminRoleId);

            Assert.False(await sut.HasRoleAsync(user.Id, ""));
            Assert.False(await sut.HasRoleAsync(user.Id, null!));
        }

        [Fact]
        public async Task HasRoleAsync_CompatibilityBridge_HonoursLegacyIsSiteAdminBoolean()
        {
            // A user whose IsSiteAdmin=true but no UserRole row (e.g. a state
            // during deploy where the backfill migration hasn't run yet)
            // still authorizes as SiteAdmin.
            var user = AddUser(isSiteAdmin: true);

            Assert.True(await sut.HasRoleAsync(user.Id, RoleNames.SiteAdmin));
        }

        [Fact]
        public async Task HasRoleAsync_CompatibilityBridge_DoesNotLeakToOtherRoles()
        {
            // The legacy boolean only grants SiteAdmin — it should not
            // accidentally authorize SalesRep or any future role.
            var user = AddUser(isSiteAdmin: true);

            Assert.False(await sut.HasRoleAsync(user.Id, RoleNames.SalesRep));
        }

        [Fact]
        public async Task GetRoleNamesAsync_ReturnsAllActiveGrants()
        {
            var user = AddUser();
            AddGrant(user.Id, SiteAdminRoleId);
            AddGrant(user.Id, SalesRepRoleId);

            var names = await sut.GetRoleNamesAsync(user.Id);

            Assert.Contains(RoleNames.SiteAdmin, names);
            Assert.Contains(RoleNames.SalesRep, names);
            Assert.Equal(2, names.Count);
        }

        [Fact]
        public async Task GetRoleNamesAsync_ExcludesRevokedGrants()
        {
            var user = AddUser();
            AddGrant(user.Id, SiteAdminRoleId);
            AddGrant(user.Id, SalesRepRoleId, revokedDate: DateTimeOffset.UtcNow.AddHours(-1));

            var names = await sut.GetRoleNamesAsync(user.Id);

            Assert.Contains(RoleNames.SiteAdmin, names);
            Assert.DoesNotContain(RoleNames.SalesRep, names);
        }

        [Fact]
        public async Task GetUsersInRoleAsync_ReturnsGrantedUsersAndLegacyAdmins()
        {
            var granted = AddUser();
            AddGrant(granted.Id, SiteAdminRoleId);
            var legacyAdmin = AddUser(isSiteAdmin: true);
            AddUser(); // control — should not appear

            var users = await sut.GetUsersInRoleAsync(RoleNames.SiteAdmin);

            var ids = System.Linq.Enumerable.ToHashSet(System.Linq.Enumerable.Select(users, u => u.Id));
            Assert.Contains(granted.Id, ids);
            Assert.Contains(legacyAdmin.Id, ids);
            Assert.Equal(2, ids.Count);
        }

        [Fact]
        public async Task GetUsersInRoleAsync_NonSiteAdminRole_DoesNotIncludeLegacyAdmins()
        {
            var granted = AddUser();
            AddGrant(granted.Id, SalesRepRoleId);
            AddUser(isSiteAdmin: true); // must not leak into SalesRep results

            var users = await sut.GetUsersInRoleAsync(RoleNames.SalesRep);

            Assert.Single(users);
            Assert.Equal(granted.Id, System.Linq.Enumerable.Single(users).Id);
        }

        [Fact]
        public async Task GetRoleNamesAsync_CachesPerRequest()
        {
            var user = AddUser();
            AddGrant(user.Id, SalesRepRoleId);

            // Prime the cache
            _ = await sut.GetRoleNamesAsync(user.Id);

            // Mutate the DB directly — cache should still return the original result
            var grant = await db.UserRoles.FirstAsync();
            grant.RevokedDate = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync();

            var second = await sut.GetRoleNamesAsync(user.Id);
            Assert.Contains(RoleNames.SalesRep, second);

            // A new service instance (representing a new request) sees the mutation.
            var freshSut = new UserRoleService(db);
            var third = await freshSut.GetRoleNamesAsync(user.Id);
            Assert.DoesNotContain(RoleNames.SalesRep, third);
        }

        public void Dispose()
        {
            db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
