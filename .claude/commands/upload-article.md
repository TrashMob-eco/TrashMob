Upload a news article to the TrashMob Strapi CMS.

## Arguments
- `$ARGUMENTS` - Path to the markdown article file (e.g., `Planning/NewsArticles/2026-02-24_article.md`) and target environment (`dev` or `prod`)

## Instructions

1. **Parse the arguments** to get the article file path and environment (default to `dev` if not specified).

2. **Read the article markdown file**. It should have frontmatter in this format:
   ```
   # Title
   **Slug:** article-slug
   **Author:** Author Name
   **Category:** Announcements | Community Stories | Tips & Guides
   **Tags:** ["tag1", "tag2"]
   **Featured:** true/false
   **Estimated Read Time:** N
   ---
   ## Excerpt
   Excerpt text here
   ---
   ## Body
   Article body in markdown...
   ```

3. **Determine the Strapi URL** based on environment:
   - `dev`: Get FQDN via `az containerapp show --name ca-strapi-tm-dev-westus2 --resource-group rg-trashmob-dev-westus2 --query "properties.configuration.ingress.fqdn" -o tsv`
   - `prod`: Get FQDN via `az containerapp show --name ca-strapi-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2 --subscription 5ea21946-8cb1-413f-9005-0ab10bfa839d --query "properties.configuration.ingress.fqdn" -o tsv`

4. **Convert the markdown body to Strapi Blocks format**. The body field uses Strapi's block editor format:
   - Paragraphs → `{ "type": "paragraph", "children": [{ "type": "text", "text": "..." }] }`
   - H2 headings → `{ "type": "heading", "level": 2, "children": [{ "type": "text", "text": "..." }] }`
   - H3 headings → `{ "type": "heading", "level": 3, "children": [{ "type": "text", "text": "..." }] }`
   - Bold text → `{ "type": "text", "bold": true, "text": "..." }`
   - Italic text → `{ "type": "text", "italic": true, "text": "..." }`
   - Links → `{ "type": "link", "url": "https://...", "children": [{ "type": "text", "text": "..." }] }`
   - Mixed formatting in a paragraph: multiple children in the paragraph's children array

5. **Look up the category ID**. Query `GET /api/news-categories` and find the matching category by name. Default categories (auto-seeded):
   - ID 1: "Announcements"
   - ID 2: "Community Stories"
   - ID 3: "Tips & Guides"

6. **Build the JSON payload**:
   ```json
   {
     "data": {
       "title": "...",
       "slug": "...",
       "excerpt": "...",
       "author": "...",
       "isFeatured": true/false,
       "estimatedReadTime": N,
       "tags": ["tag1", "tag2"],
       "category": { "connect": [CATEGORY_ID] },
       "body": [ ...blocks... ]
     }
   }
   ```

7. **Authenticate with Strapi**:
   - Try `POST /admin/login` with admin credentials (ask user if not known)
   - Create or reuse an API token via `POST /admin/api-tokens`
   - Use `Authorization: Bearer <token>` for the upload

8. **Upload the article**:
   ```bash
   curl -X POST "$STRAPI_URL/api/news-posts" \
     -H "Authorization: Bearer $API_TOKEN" \
     -H "Content-Type: application/json" \
     -d @article.json
   ```

9. **Publish the article** (Strapi creates as draft by default in v5):
   - Get the article ID from the POST response
   - `PUT /api/news-posts/{id}` with `{ "data": { "publishedAt": "2026-01-01T00:00:00.000Z" } }` to publish

10. **Save the upload JSON** to `Planning/NewsArticles/` alongside the markdown file for reference (e.g., `strapi-upload.json`).

11. **Verify** by hitting `GET /api/news-posts?filters[slug][$eq]=SLUG` and confirming the article appears.

## Example Usage
```
/upload-article Planning/NewsArticles/2026-02-24_trashmob-2026-launch.md prod
/upload-article Planning/NewsArticles/my-article.md dev
/upload-article Planning/NewsArticles/my-article.md  (defaults to dev)
```
