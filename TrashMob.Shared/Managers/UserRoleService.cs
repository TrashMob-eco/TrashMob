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
    using TrashMob.Shared.Persistence;

    /// <summary>
    /// Read-side accessor for role membership. See <see cref="IUserRoleService"/>
    /// for the contract, including the compatibility bridge that honours the
    /// legacy <see cref="User.IsSiteAdmin"/> boolean.
    /// </summary>
    /// <remarks>
    /// Registered as scoped, so the per-instance cache is effectively a
    /// per-request cache. Concurrent grants / revocations take effect on the
    /// caller's next request, not mid-request. Acceptable for a coarse-role
    /// system.
    /// </remarks>
    public class UserRoleService(MobDbContext db) : IUserRoleService
    {
        private readonly Dictionary<Guid, HashSet<string>> cache = new();

        /// <inheritdoc />
        public async Task<bool> HasRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return false;
            }

            var roles = await GetRoleNamesAsync(userId, cancellationToken);
            return roles.Contains(roleName);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<string>> GetRoleNamesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(userId, out var cached))
            {
                return cached;
            }

            var now = DateTimeOffset.UtcNow;

            var activeRoles = await db.UserRoles
                .Where(ur => ur.UserId == userId
                             && ur.RevokedDate == null
                             && (ur.ExpiryDate == null || ur.ExpiryDate > now))
                .Include(ur => ur.Role)
                .Select(ur => ur.Role.Name)
                .ToListAsync(cancellationToken);

            var set = new HashSet<string>(activeRoles, StringComparer.OrdinalIgnoreCase);

            // Compatibility bridge: during the Project 64 migration window,
            // treat the legacy IsSiteAdmin boolean as an implicit SiteAdmin
            // grant. The backfill migration also inserts real UserRole rows,
            // so most reads never hit this fallback — but any user promoted
            // to admin during a partial deploy (backfill pending) still
            // authorizes correctly. Removed in Phase 4 when the boolean is
            // dropped.
            if (!set.Contains(RoleNames.SiteAdmin))
            {
                var isLegacyAdmin = await db.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.IsSiteAdmin)
                    .FirstOrDefaultAsync(cancellationToken);

                if (isLegacyAdmin)
                {
                    set.Add(RoleNames.SiteAdmin);
                }
            }

            cache[userId] = set;
            return set;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return Array.Empty<User>();
            }

            var now = DateTimeOffset.UtcNow;

            var usersWithGrant = await db.UserRoles
                .Where(ur => ur.Role.Name == roleName
                             && ur.RevokedDate == null
                             && (ur.ExpiryDate == null || ur.ExpiryDate > now))
                .Select(ur => ur.User)
                .ToListAsync(cancellationToken);

            // Compatibility bridge: pull in anyone flagged as legacy SiteAdmin
            // who hasn't yet been backfilled with a UserRole row.
            if (roleName.Equals(RoleNames.SiteAdmin, StringComparison.OrdinalIgnoreCase))
            {
                var alreadyIncluded = usersWithGrant.Select(u => u.Id).ToHashSet();
                var legacyAdmins = await db.Users
                    .Where(u => u.IsSiteAdmin && !alreadyIncluded.Contains(u.Id))
                    .ToListAsync(cancellationToken);
                usersWithGrant.AddRange(legacyAdmins);
            }

            return usersWithGrant;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<Role>> ListRolesAsync(CancellationToken cancellationToken = default)
        {
            return await db.Roles
                .OrderBy(r => r.DisplayOrder)
                .ThenBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<UserRole>> GetActiveGrantsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            return await db.UserRoles
                .Where(ur => ur.UserId == userId
                             && ur.RevokedDate == null
                             && (ur.ExpiryDate == null || ur.ExpiryDate > now))
                .Include(ur => ur.Role)
                .OrderBy(ur => ur.Role.DisplayOrder)
                .ThenBy(ur => ur.Role.Name)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<UserRole> GrantRoleAsync(Guid userId, string roleName, Guid grantedByUserId, DateTimeOffset? expiryDate, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new InvalidOperationException("Role name is required.");
            }

            var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken)
                ?? throw new InvalidOperationException($"Role '{roleName}' does not exist.");

            var now = DateTimeOffset.UtcNow;

            var existing = await db.UserRoles
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(
                    ur => ur.UserId == userId
                          && ur.RoleId == role.Id
                          && ur.RevokedDate == null,
                    cancellationToken);

            if (existing != null)
            {
                // Idempotent: return the current grant. If the caller passed a
                // new expiry, apply it — otherwise leave the existing grant alone.
                if (expiryDate.HasValue && existing.ExpiryDate != expiryDate)
                {
                    existing.ExpiryDate = expiryDate;
                    existing.LastUpdatedByUserId = grantedByUserId;
                    existing.LastUpdatedDate = now;
                    await db.SaveChangesAsync(cancellationToken);
                }

                cache.Remove(userId);
                return existing;
            }

            var grant = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = role.Id,
                GrantedByUserId = grantedByUserId,
                GrantedDate = now,
                ExpiryDate = expiryDate,
                CreatedByUserId = grantedByUserId,
                CreatedDate = now,
                LastUpdatedByUserId = grantedByUserId,
                LastUpdatedDate = now,
                Role = role,
            };

            db.UserRoles.Add(grant);
            await db.SaveChangesAsync(cancellationToken);

            cache.Remove(userId);
            return grant;
        }

        /// <inheritdoc />
        public async Task<UserRole> RevokeRoleAsync(Guid userId, string roleName, Guid revokedByUserId, string revokedReason, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return null;
            }

            var grant = await db.UserRoles
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(
                    ur => ur.UserId == userId
                          && ur.Role.Name == roleName
                          && ur.RevokedDate == null,
                    cancellationToken);

            if (grant == null)
            {
                return null;
            }

            var now = DateTimeOffset.UtcNow;
            grant.RevokedDate = now;
            grant.RevokedByUserId = revokedByUserId;
            grant.RevokedReason = revokedReason;
            grant.LastUpdatedByUserId = revokedByUserId;
            grant.LastUpdatedDate = now;

            await db.SaveChangesAsync(cancellationToken);

            cache.Remove(userId);
            return grant;
        }
    }
}
