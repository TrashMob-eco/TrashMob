namespace TrashMob.Shared.Managers
{
    /// <summary>
    /// Canonical names for the roles seeded in the database. See the
    /// <see cref="TrashMob.Models.Role"/> HasData block in
    /// <see cref="Persistence.MobDbContext"/> for the seed rows themselves.
    /// </summary>
    public static class RoleNames
    {
        /// <summary>
        /// Full administrative access. Manages users, roles, waivers, events,
        /// moderation, and every other site-admin surface.
        /// </summary>
        public const string SiteAdmin = "SiteAdmin";

        /// <summary>
        /// Manages the municipal sales pipeline (prospects, contacts,
        /// activities) and reads sales reports. Cannot administer users,
        /// waivers, or events. See Project 63.
        /// </summary>
        public const string SalesRep = "SalesRep";
    }
}
