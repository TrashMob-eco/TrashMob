---
model: haiku
---

Upload a news article to the TrashMob Strapi CMS.

## Arguments

`$ARGUMENTS` — path to the markdown article, followed optionally by an environment (`dev` or `prod`, default `dev`) and/or `--draft` to skip publishing.

Examples:
- `/upload-article Planning/NewsArticles/2026-02-24_launch.md`
- `/upload-article Planning/NewsArticles/2026-02-24_launch.md prod`
- `/upload-article Planning/NewsArticles/2026-02-24_launch.md dev --draft`

## What to do

1. **Run** [`scripts/upload_article.py`](../../scripts/upload_article.py) with the supplied arguments. The script handles frontmatter parsing, markdown→Strapi-blocks conversion, URL discovery via `az`, admin login → API token caching, the POST/PUT publish dance, and verification:

   ```bash
   python scripts/upload_article.py <markdown> [--env <env>] [--draft]
   ```

   Convert the user's plain-language args into the script's flag form. The environment defaults to `dev`; the article path is required.

2. **Inspect the script's output** and report success or failure to the user. The script prints structured progress (`-> Parsing ...`, `-> POST ...`, etc.) and exits non-zero on errors.

3. **If the script fails on something it can't handle** — most commonly an article that uses an unsupported markdown construct (code blocks, blockquotes, inline images) — surface the script's error to the user, identify the offending lines in the markdown, and offer to either edit the article into a supported form or extend the script. Don't try to do the upload by hand; the script is the source of truth for the Strapi format.

4. **If admin credentials are needed** (first-time use on a machine, or after the cached token expires), the script reads `STRAPI_ADMIN_EMAIL` and `STRAPI_ADMIN_PASSWORD` from the environment or prompts. Tokens are cached under `~/.cache/trashmob-strapi/{env}-token.json` (chmod 600).

## Out of scope for this command

Cover image upload and inline body images require Strapi `/api/upload` calls with binary multipart form data — that flow still lives in `Planning/NewsArticles/CLAUDE.md`. Use that context separately if the article needs a cover image.

Social posts (the trailing `## Social Posts` section) are read by humans only and don't ship to Strapi.
