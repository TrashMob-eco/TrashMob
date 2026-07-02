# NewsArticles CLAUDE.md

Instructions for writing and uploading TrashMob blog posts.

## Article Format

Articles are markdown files with this structure:

```
# Article Title

**Slug:** kebab-case-slug
**Author:** Joe Beernink
**Category:** Announcements | Community Stories | Tips & Guides | Features
**Tags:** ["tag1", "tag2"]
**Featured:** true/false
**Estimated Read Time:** N

---

## Excerpt

One-paragraph summary for previews and social cards.

---

## Body

Article body in markdown (H3 subheadings, bold, italic, links, lists).

---

## Social Posts

### LinkedIn
### Reddit (r/DeTrashed)
### Bluesky
### Newsletter
```

## File Naming

- Articles: `YYYY-MM-DD_short-description.md`
- Cover images: `YYYY-MM-DD_short-description-cover.png`
- Dimensions: 1200x675 pixels
- Brand colors: `#005B4B` (dark green), `#ABC313` (lime), `#00A3E4` (blue)
- Generate cover images with Python/Pillow using `C:/Windows/Fonts/segoeuib.ttf` (bold) and `segoeui.ttf` (regular)

## Writing Style

- Conversational, direct tone — write like you're explaining to a colleague
- Lead with the user problem, then show how the feature solves it
- Use concrete examples, not abstract descriptions
- Short paragraphs (2-4 sentences max)
- H3 subheadings to break up sections
- Bold key terms on first use
- End with a clear CTA linking to trashmob.eco
- Close with the nonprofit tagline: *TrashMob.eco is a 501(c)(3) nonprofit dedicated to empowering communities to keep their neighborhoods clean.*
- Social posts: LinkedIn (professional, bullet points), Reddit (casual, practical), Bluesky (short, punchy), Newsletter (single flowing paragraph)

## Strapi Upload

### Authentication

Use Python `urllib.request` (not curl) to avoid shell escaping issues with special characters in the password.

1. Admin login: `POST /admin/login` with `{"email": "...", "password": "..."}`
2. Create/regenerate API token: `POST /admin/api-tokens` with admin Bearer token
3. Use API token for all content operations

### Strapi Blocks Format

The body field uses Strapi v5 Blocks format (not markdown). Convert manually:

| Markdown | Blocks |
|----------|--------|
| Paragraph | `{"type": "paragraph", "children": [{"type": "text", "text": "..."}]}` |
| `### Heading` | `{"type": "heading", "level": 3, "children": [{"type": "text", "text": "..."}]}` |
| `**bold**` | `{"type": "text", "bold": true, "text": "..."}` |
| `*italic*` | `{"type": "text", "italic": true, "text": "..."}` |
| `[text](url)` | `{"type": "link", "url": "...", "children": [{"type": "text", "text": "..."}]}` |
| Bullet list | `{"type": "list", "format": "unordered", "children": [{"type": "list-item", "children": [...]}]}` |

Mixed formatting in a paragraph = multiple children in the paragraph's children array.

### Image Upload

**Cover images:** just pass `--cover <path>` to `scripts/upload_article.py`. If a `YYYY-MM-DD_slug-cover.png` file sits alongside the article's `.md`, the script picks it up automatically — no flag needed. It POSTs to `/api/upload`, sets the alt text (defaulting to the article title, overridable with `--cover-alt`), and attaches the returned file id as `coverImage` in the same create call. Pass `--no-cover` to opt out even when a matching file exists.

**Inline body images** are still a manual step. The Strapi Blocks format needs the full file metadata inline, and `upload_article.py` doesn't handle that yet. The manual flow:

1. Upload file: `POST /api/upload` (multipart form data)
2. Get full metadata: `GET /api/upload/files/{id}` — need `hash`, `ext`, `mime`, `width`, `height`, `size`, `formats`, `provider`, `createdAt`, `updatedAt`
3. Splice the metadata into an image block in the body:
   ```json
   {
     "type": "image",
     "image": {
       "name": "...", "alternativeText": "...", "url": "...",
       "hash": "...", "ext": "...", "mime": "...",
       "width": N, "height": N, "size": N,
       "formats": {...}, "provider": "local",
       "createdAt": "...", "updatedAt": "..."
     },
     "children": [{"type": "text", "text": ""}]
   }
   ```

### Publishing (Strapi v5)

Strapi v5 creates articles as drafts. To publish:

1. Create: `POST /api/news-posts` — returns `id` and `documentId`
2. Publish: `PUT /api/news-posts/{documentId}` with `{"data": {"publishedAt": "ISO-8601"}}`
3. Use `documentId` (not numeric `id`) for PUT operations
4. To find drafts: add `?status=draft` query parameter

### Categories (Dev)

| ID | Name |
|----|------|
| 1 | Announcements |
| 2 | Community Stories |
| 3 | Tips & Guides |
| 4 | Features |

When creating a new category, include a `slug` field: `{"data": {"name": "...", "slug": "..."}}`

### Environment URLs

- **Dev:** `az containerapp show --name ca-strapi-tm-dev-westus2 --resource-group rg-trashmob-dev-westus2 --query "properties.configuration.ingress.fqdn" -o tsv`
- **Prod:** `az containerapp show --name ca-strapi-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2 --subscription 5ea21946-8cb1-413f-9005-0ab10bfa839d --query "properties.configuration.ingress.fqdn" -o tsv`

## Published Articles (Dev)

1. TrashMob 2026: A New Era for Community Cleanup Programs (Announcements)
2. What Your Community Page Actually Does (Features)
3. Adopt-a-Location: How TrashMob Turns a Map Into a Managed Cleanup Program (Features)
4. Teams: How TrashMob Turns Solo Volunteers Into Organized Cleanup Crews (Features)
5. Route Tracking: See Where You Cleaned and Where the Hotspots Are (Features)

## Cleanup

Always delete temporary Python upload scripts and JSON records after upload. Do not commit them.
