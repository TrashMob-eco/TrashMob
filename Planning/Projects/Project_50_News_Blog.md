# Project 50 — News & Blog

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | Project 16 (Content Management / Strapi) |

---

## Business Rationale

TrashMob.eco has no way to communicate news, updates, or stories to visitors beyond the static "What's New" feature list page. There is no blog, no news feed, no way for admins to publish timely content like event recaps, community spotlights, partnership announcements, or platform updates.

A blog/news section serves multiple purposes:
- **SEO:** Fresh, keyword-rich content drives organic search traffic
- **Engagement:** Gives visitors a reason to return and share content
- **Storytelling:** Community success stories, volunteer spotlights, and event recaps humanize the platform
- **Communication:** Platform updates, feature announcements, and seasonal campaigns reach users where they browse

Strapi CMS is already deployed and managing home page content (hero, what-is-trashmob, getting-started). Adding a `news-post` collection type is a natural extension of the existing CMS infrastructure — no new services needed.

---

## Objectives

### Primary Goals
- **Blog-style news page** at `/news` with card-based post listing, pagination, and chronological ordering
- **Individual post pages** at `/news/:slug` with rich text content, images, and metadata
- **CMS-managed content** — site admins create and publish posts via the Strapi admin panel
- **SEO-friendly** — proper meta tags, Open Graph, slugs, and structured data

### Secondary Goals
- Category/tag filtering (e.g., "Community Spotlight", "Platform Update", "Event Recap")
- Featured/pinned post support for important announcements
- RSS feed for subscribers
- "Latest News" section on the home page

---

## Scope

### Phase 1 — Strapi Content Type & API Proxy
- ❌ Create `news-post` collection type in Strapi with fields: title, slug, excerpt, body (rich text), coverImage (media), author, category, tags, publishedAt, isFeatured
- ❌ Create `news-category` collection type: name, slug, description
- ❌ Seed initial categories: "Platform Update", "Community Spotlight", "Event Recap", "Tips & Guides"
- ❌ Add CMS proxy endpoints to `CmsController`: list posts (paginated), get post by slug, list categories
- ❌ Add TypeScript types and service factories in `services/cms.ts`

### Phase 2 — News Listing Page
- ❌ Create `/news` page with responsive card grid layout
- ❌ Each card shows: cover image, category badge, title, excerpt, date, read time
- ❌ Server-side pagination (10 posts per page) with page navigation
- ❌ Category filter tabs/pills at the top
- ❌ Empty state when no posts exist

### Phase 3 — Post Detail Page
- ❌ Create `/news/:slug` page with full article layout
- ❌ Render Strapi rich text (markdown or blocks) to HTML
- ❌ Show cover image, title, author, date, category, estimated read time
- ❌ "Back to News" navigation
- ❌ Related posts section at the bottom (same category, max 3)

### Phase 4 — Navigation & Home Page Integration
- ❌ Add "News" to the main site navigation (top-level nav item)
- ❌ Add "Latest News" section to the home page (3 most recent posts as cards)
- ❌ Add "News" link to the site footer
- ❌ Update sitemap generation to include news posts

### Phase 5 — SEO & Polish
- ❌ Open Graph and Twitter Card meta tags per post
- ❌ Structured data (JSON-LD Article schema) per post
- ❌ RSS feed endpoint (`/news/feed.xml` or `/api/cms/news-feed`)
- ❌ Social sharing buttons on post detail page

---

## Out-of-Scope

- ❌ Comments or user-generated content on posts
- ❌ Newsletter email integration (see Project 19)
- ❌ Multi-author profiles or contributor pages
- ❌ Full-text search within posts (use browser Ctrl+F or future site search)
- ❌ Mobile app news feed (future project)
- ❌ Post scheduling beyond Strapi's built-in draft/publish workflow
- ❌ Seed blog content — this project delivers infrastructure only; content creation is a separate effort

---

## Success Metrics

### Quantitative
- **Posts published:** Target 2-4 per month after launch
- **News page visits:** Track via Feature Usage Metrics
- **Average time on post page:** Target >90 seconds (indicates reading)
- **Organic search traffic from blog posts:** Track via analytics
- **Bounce rate on news pages:** Target <60%

### Qualitative
- Site admins can publish a new post in under 10 minutes via Strapi
- News page feels professional and blog-like
- Posts are shareable on social media with proper previews

---

## Dependencies

### Blockers
- **Project 16 (Content Management)** — ✅ Complete. Strapi is deployed, CMS proxy pattern established.

### Enables
- **Project 19 (Newsletter)** — Newsletter can link to published blog posts
- **Project 36 (Marketing Materials)** — Blog posts support content marketing strategy
- **SEO strategy** — Regular content publishing improves organic search ranking

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Content drought** — no one writes posts | Medium | High | Seed with 3-5 launch posts; establish a lightweight editorial calendar; AI-assisted drafting |
| **Strapi rich text rendering issues** — formatting doesn't match site design | Low | Medium | Use a proven Strapi-to-React renderer; test with varied content before launch |
| **Image performance** — large cover images slow page load | Low | Medium | Strapi supports responsive image formats; use lazy loading and srcset |
| **SEO takes time** — no immediate traffic boost | High | Low | Expected; blog is a long-term investment. Short-term value is engagement for existing users |

---

## Implementation Plan

### Data Model Changes

**Strapi Content Types (no SQL migration needed — Strapi manages its own schema):**

**`news-post` (Collection Type):**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `title` | String (255) | Yes | Post headline |
| `slug` | UID (from title) | Yes | URL-friendly identifier, auto-generated from title |
| `excerpt` | Text (500) | Yes | Card preview text and meta description |
| `body` | Rich Text (Blocks) | Yes | Full post content with formatting, images, embeds |
| `coverImage` | Media (Single Image) | No | Hero image for the post and social sharing |
| `author` | String (100) | Yes | Author display name (e.g., "TrashMob Team") |
| `category` | Relation (news-category) | No | Many-to-one relation |
| `tags` | JSON | No | Array of tag strings for flexible filtering |
| `isFeatured` | Boolean | No | Pin to top of listing (default false) |
| `estimatedReadTime` | Integer | No | Minutes, auto-calculated or manual override |

**`news-category` (Collection Type):**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `name` | String (100) | Yes | Display name |
| `slug` | UID (from name) | Yes | URL-friendly identifier |
| `description` | Text (255) | No | Optional category description |

### API Changes

**New CMS proxy endpoints on `CmsController`:**

```csharp
// List published posts with pagination
[HttpGet("news-posts")]
[AllowAnonymous]
// Query params: ?page=1&pageSize=10&category=community-spotlight
// Returns: { data: [...], meta: { pagination: { page, pageSize, pageCount, total } } }

// Get single post by slug
[HttpGet("news-posts/{slug}")]
[AllowAnonymous]
// Returns: { data: { id, attributes: { title, slug, body, ... } } }

// List categories
[HttpGet("news-categories")]
[AllowAnonymous]
// Returns: { data: [{ id, attributes: { name, slug } }] }
```

### Web UX Changes

**New pages:**
- `/news` — Card grid listing with pagination and category filter
- `/news/:slug` — Full article page with rich text rendering

**Card layout (listing page):**
```
┌─────────────────────────────────────────────┐
│  News                                        │
│                                              │
│  [All] [Platform Updates] [Community] [Tips] │
│                                              │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐     │
│  │ [image]  │ │ [image]  │ │ [image]  │     │
│  │ Category │ │ Category │ │ Category │     │
│  │ Title    │ │ Title    │ │ Title    │     │
│  │ Excerpt  │ │ Excerpt  │ │ Excerpt  │     │
│  │ Date · 5m│ │ Date · 3m│ │ Date · 7m│     │
│  └──────────┘ └──────────┘ └──────────┘     │
│                                              │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐     │
│  │  ...     │ │  ...     │ │  ...     │     │
│  └──────────┘ └──────────┘ └──────────┘     │
│                                              │
│         [← Previous]  [Next →]               │
└─────────────────────────────────────────────┘
```

**Post detail layout:**
```
┌─────────────────────────────────────────────┐
│  ← Back to News                              │
│                                              │
│  Category Badge          Feb 20, 2026        │
│                                              │
│  Post Title Here                             │
│  By Author Name · 5 min read                 │
│                                              │
│  ┌─────────────────────────────────────────┐ │
│  │         Cover Image (full width)        │ │
│  └─────────────────────────────────────────┘ │
│                                              │
│  Rich text body content rendered here.       │
│  Supports headings, paragraphs, images,      │
│  block quotes, lists, code blocks, etc.      │
│                                              │
│  ─── Share ──────────────────────────────── │
│  [Twitter] [Facebook] [LinkedIn] [Copy Link] │
│                                              │
│  ─── Related Posts ─────────────────────── │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐     │
│  │ Related 1│ │ Related 2│ │ Related 3│     │
│  └──────────┘ └──────────┘ └──────────┘     │
└─────────────────────────────────────────────┘
```

### Mobile App Changes

None in this project. Mobile news feed would be a separate future project.

---

## Implementation Phases

### Phase 1: Strapi Content Type & API Proxy
1. Create `news-category` collection type in Strapi with name, slug, description
2. Create `news-post` collection type with all fields and relation to news-category
3. Configure Strapi permissions: public `find` and `findOne` for both types
4. Seed initial categories via Strapi admin
5. Add proxy endpoints to `CmsController` (list posts paginated, get by slug, list categories)
6. Add TypeScript types and service factories to `services/cms.ts`
7. Create 2-3 seed posts via Strapi admin for testing

### Phase 2: News Listing Page
1. Create `/news` route and lazy-loaded page component
2. Build `NewsCard` component (cover image, category badge, title, excerpt, date, read time)
3. Build responsive card grid (3 columns desktop, 2 tablet, 1 mobile)
4. Add pagination component with page numbers and prev/next
5. Add category filter pills that update the query
6. Wire up React Query with the news posts service factory
7. Add empty state component when no posts exist

### Phase 3: Post Detail Page
1. Create `/news/:slug` route and page component
2. Integrate Strapi rich text (Blocks) renderer — evaluate `@strapi/blocks-react-renderer` or custom renderer
3. Build article layout with cover image, metadata header, body content
4. Add "Back to News" breadcrumb navigation
5. Build related posts section (query same category, exclude current, limit 3)
6. Handle 404 for invalid slugs

### Phase 4: Navigation & Home Page Integration
1. Add "News" as a top-level navigation item in the site header
2. Build "Latest News" home page section showing 3 most recent posts
3. Add "News" to site footer
4. Add news routes to sitemap

### Phase 5: SEO & Polish
1. Add Open Graph and Twitter Card meta tags per post (title, description, image)
2. Add JSON-LD Article structured data
3. Build RSS feed endpoint
4. Add social sharing buttons to post detail page
5. Optimize images with lazy loading and responsive srcset

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Open Questions

1. **Should "News" be a top-level nav item or in a dropdown?**
   **Decision:** Resolved — top-level nav item for maximum visibility.
   **Status:** "News" will be a top-level navigation item, not buried in a dropdown.

2. **Strapi rich text format: Blocks editor or Markdown?**
   **Decision:** Resolved — use Strapi v5's Blocks editor (WYSIWYG).
   **Status:** Blocks provides the best authoring experience for non-technical users. Use `@strapi/blocks-react-renderer` on the frontend.

3. **Should the existing "What's New" page (`/whatsnew`) be replaced or kept?**
   **Decision:** Resolved — keep as-is.
   **Status:** The static feature list page remains unchanged at `/whatsnew`. The blog at `/news` serves a different purpose (storytelling, announcements, community spotlights).

4. **Should the project include seed blog posts?**
   **Decision:** Resolved — build the platform only.
   **Status:** This project delivers the blog infrastructure. Content creation is a separate effort outside this project's scope.

---

## Related Documents

- **[Project 16 - Content Management](./Project_16_Content_Management.md)** — Strapi CMS infrastructure and home page content types
- **[Project 2 - Home Page Improvements](./Project_02_Home_Page.md)** — Home page sections where latest news could appear
- **[Project 19 - Newsletter](./Project_19_Newsletter.md)** — Newsletter can link to blog posts
- **[Project 36 - Marketing Materials](./Project_36_Marketing_Materials.md)** — Content marketing strategy alignment

---

**Last Updated:** February 22, 2026
**Owner:** Product & Engineering Team
**Status:** Not Started
**Next Review:** When ready to begin Phase 1
