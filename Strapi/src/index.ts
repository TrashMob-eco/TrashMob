export default {
  /**
   * An asynchronous register function that runs before
   * your application is initialized.
   *
   * This gives you an opportunity to extend code.
   */
  register(/* { strapi } */) {},

  /**
   * An asynchronous bootstrap function that runs before
   * your application gets started.
   *
   * This gives you an opportunity to set up your data model,
   * run jobs, or perform some special logic.
   */
  async bootstrap({ strapi }) {
    // Set up public read permissions for content types.
    // This runs on every startup to ensure permissions are always configured,
    // especially important when using ephemeral SQLite storage.
    const publicRole = await strapi
      .query("plugin::users-permissions.role")
      .findOne({ where: { type: "public" } });

    if (publicRole) {
      const publicPermissions = [
        "api::news-post.news-post.find",
        "api::news-post.news-post.findOne",
        "api::news-category.news-category.find",
        "api::news-category.news-category.findOne",
        "api::hero-section.hero-section.find",
        "api::what-is-trashmob.what-is-trashmob.find",
        "api::getting-started.getting-started.find",
      ];

      for (const action of publicPermissions) {
        const existing = await strapi
          .query("plugin::users-permissions.permission")
          .findOne({ where: { action, role: publicRole.id } });

        if (!existing) {
          await strapi
            .query("plugin::users-permissions.permission")
            .create({ data: { action, role: publicRole.id, enabled: true } });
        }
      }

      strapi.log.info("Public permissions configured for CMS content types");
    }

    // Seed default news categories if they don't exist
    const defaultCategories = [
      {
        name: "Announcements",
        slug: "announcements",
        description: "Official TrashMob announcements and major releases",
      },
      {
        name: "Community Stories",
        slug: "community-stories",
        description: "Stories and highlights from TrashMob communities",
      },
      {
        name: "Tips & Guides",
        slug: "tips-and-guides",
        description: "How-to guides and best practices for cleanup organizers",
      },
    ];

    for (const cat of defaultCategories) {
      const existing = await strapi
        .documents("api::news-category.news-category")
        .findFirst({ filters: { slug: cat.slug } });

      if (!existing) {
        await strapi
          .documents("api::news-category.news-category")
          .create({ data: cat, status: "published" });
      }
    }

    strapi.log.info("Default news categories seeded");
  },
};
