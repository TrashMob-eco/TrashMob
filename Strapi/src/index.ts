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
    // Idempotent — only creates permissions that don't already exist.
    // Ensures permissions survive database migrations or fresh deployments.
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

    // Seed default home page single-type content if it doesn't exist.
    // This prevents 404 errors from the CMS controller when content
    // has not yet been created via the Strapi admin panel.

    const heroSection = await strapi
      .documents("api::hero-section.hero-section")
      .findFirst();

    if (!heroSection) {
      await strapi
        .documents("api::hero-section.hero-section")
        .create({
          data: {
            tagline: "Meet up. Clean up. Feel good!",
            primaryButtonText: "Join us today",
            primaryButtonLink: "/gettingstarted",
            secondaryButtonText: "Report Litter",
            secondaryButtonLink: "/litterreports/create",
            googlePlayUrl:
              "https://play.google.com/store/apps/details?id=eco.trashmob.trashmobmobileapp",
            appStoreUrl:
              "https://apps.apple.com/us/app/trashmob/id1599996743?itscg=30200&itsct=apps_box_badge&mttnsubad=1599996743",
          },
          status: "published",
        });
      strapi.log.info("Default hero section content seeded");
    }

    const whatIsTrashmob = await strapi
      .documents("api::what-is-trashmob.what-is-trashmob")
      .findFirst();

    if (!whatIsTrashmob) {
      await strapi
        .documents("api::what-is-trashmob.what-is-trashmob")
        .create({
          data: {
            heading: "What is a TrashMob?",
            description:
              "A TrashMob is a group of citizens who are willing to take an hour or two out of their lives to get together and clean up their communities. Start your impact today.",
            youtubeVideoUrl:
              "https://www.youtube.com/embed/ylOBeVHRtuM?si=5oYDCAMdywNBmp_A",
            primaryButtonText: "Learn more",
            primaryButtonLink: "/aboutus",
            secondaryButtonText: "View Upcoming Events",
            secondaryButtonLink: "/#events",
          },
          status: "published",
        });
      strapi.log.info("Default 'What is TrashMob' content seeded");
    }

    const gettingStarted = await strapi
      .documents("api::getting-started.getting-started")
      .findFirst();

    if (!gettingStarted) {
      await strapi
        .documents("api::getting-started.getting-started")
        .create({
          data: {
            heading: "Getting Started",
            subheading:
              "All you really need to start or join a trash mob are:",
            requirements: [
              { icon: "gloves", label: "Work gloves" },
              { icon: "bucket", label: "A bucket" },
              { icon: "attitude", label: "A good attitude" },
            ],
            ctaButtonText: "Learn more",
            ctaButtonLink: "/gettingstarted",
          },
          status: "published",
        });
      strapi.log.info("Default 'Getting Started' content seeded");
    }
  },
};
