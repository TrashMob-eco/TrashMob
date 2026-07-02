#!/usr/bin/env python3
"""
Upload a news article markdown file to TrashMob's Strapi v5 CMS.

Replaces the LLM-driven upload-article slash command with a deterministic
script. Handles frontmatter parsing, markdown -> Strapi v5 Blocks
conversion, Strapi URL discovery via az CLI, admin login, API-token
caching, POST/PUT/GET-verify.

Usage:
    python scripts/upload_article.py <markdown-path> [--env dev|prod] [--draft]

Environment variables:
    STRAPI_ADMIN_EMAIL     admin email (prompted if missing)
    STRAPI_ADMIN_PASSWORD  admin password (prompted if missing, NEVER echoed)
    STRAPI_URL             override URL discovery (skip az lookup)

Cached state:
    ~/.cache/trashmob-strapi/{env}-token.json  API token (chmod 600)
"""

from __future__ import annotations

import argparse
import getpass
import json
import mimetypes
import os
import re
import shutil
import subprocess
import sys
import urllib.error
import urllib.parse
import urllib.request
import uuid
from dataclasses import dataclass, field
from pathlib import Path
from typing import Any

CACHE_DIR = Path.home() / ".cache" / "trashmob-strapi"

# --- Category cache (mirrors Planning/NewsArticles/CLAUDE.md "Categories (Dev)") ---
KNOWN_CATEGORIES = {
    "announcements": 1,
    "community stories": 2,
    "tips & guides": 3,
    "tips and guides": 3,
    "features": 4,
}


# ---------------------------------------------------------------------------
# Article parsing
# ---------------------------------------------------------------------------

@dataclass
class Article:
    title: str
    slug: str
    author: str
    category: str
    tags: list[str]
    featured: bool
    estimated_read_time: int
    excerpt: str
    body_markdown: str
    body_blocks: list[dict[str, Any]] = field(default_factory=list)


def parse_article(path: Path) -> Article:
    """Parse a TrashMob news-article markdown file into an Article."""
    text = path.read_text(encoding="utf-8")

    # Title: first H1
    m = re.search(r"^#\s+(.+?)\s*$", text, re.MULTILINE)
    if not m:
        raise ValueError(f"{path}: no H1 title found")
    title = m.group(1).strip()

    # Bold key:value frontmatter lines, e.g. **Slug:** foo
    def field_value(name: str) -> str | None:
        rx = rf"\*\*{re.escape(name)}:\*\*\s*(.*?)\s*$"
        mm = re.search(rx, text, re.MULTILINE)
        return mm.group(1).strip() if mm else None

    slug = field_value("Slug") or ""
    author = field_value("Author") or ""
    category = field_value("Category") or ""
    featured_raw = (field_value("Featured") or "false").strip().lower()
    estimated_raw = field_value("Estimated Read Time") or "0"
    tags_raw = field_value("Tags") or "[]"

    # Tags: JSON array literal
    try:
        tags = json.loads(tags_raw)
        if not isinstance(tags, list):
            raise ValueError("Tags must be a JSON array")
    except json.JSONDecodeError as e:
        raise ValueError(f"{path}: invalid Tags JSON {tags_raw!r}: {e}") from e

    # Excerpt: between ## Excerpt and the next ---
    excerpt = _extract_section(text, "Excerpt")
    if not excerpt:
        raise ValueError(f"{path}: no ## Excerpt section found")

    # Body: between ## Body and the next ## (e.g. ## Social Posts) or end-of-file
    body_md = _extract_section(text, "Body", stop_at_h2=True)
    if not body_md:
        raise ValueError(f"{path}: no ## Body section found")

    article = Article(
        title=title,
        slug=slug,
        author=author,
        category=category,
        tags=tags,
        featured=featured_raw in ("true", "1", "yes"),
        estimated_read_time=int(estimated_raw),
        excerpt=excerpt,
        body_markdown=body_md,
    )
    article.body_blocks = markdown_to_blocks(body_md)
    return article


def _extract_section(text: str, name: str, stop_at_h2: bool = False) -> str:
    """Return the contents of a ## Name section, trimmed."""
    # Find "## Name" then capture until next "---" or "## " or EOF.
    rx = rf"^##\s+{re.escape(name)}\s*$\n(.*?)(?=^---|\Z|^## )" if stop_at_h2 \
        else rf"^##\s+{re.escape(name)}\s*$\n(.*?)(?=^---|\Z)"
    m = re.search(rx, text, re.MULTILINE | re.DOTALL)
    return m.group(1).strip() if m else ""


# ---------------------------------------------------------------------------
# Markdown -> Strapi v5 Blocks
# ---------------------------------------------------------------------------
#
# Supports the subset used in TrashMob news articles per
# Planning/NewsArticles/CLAUDE.md:
#   - Paragraphs
#   - H2 / H3 headings
#   - Bold (**text**)
#   - Italic (*text* or _text_)
#   - Inline links [text](url)
#   - Unordered lists (- item / * item)
#   - Ordered lists (1. item / 2. item)
#   - Mixed inline formatting within paragraphs and list items
#
# Out of scope: code blocks, blockquotes, tables, images. If the article
# uses one of those, the script errors out so a human handles it.

_INLINE_PATTERN = re.compile(
    r"(\*\*([^*]+)\*\*)"            # 1,2 bold
    r"|(\*([^*]+)\*)"               # 3,4 italic with *
    r"|(_([^_]+)_)"                 # 5,6 italic with _
    r"|(\[([^\]]+)\]\(([^)]+)\))"   # 7,8,9 link [text](url)
)

# For parsing inside a bold/italic span — same as _INLINE_PATTERN but excluding
# the outer wrapper. Used by _inline_to_children for nested formatting.
_LINK_PATTERN = re.compile(r"(\[([^\]]+)\]\(([^)]+)\))")


def markdown_to_blocks(md: str) -> list[dict[str, Any]]:
    blocks: list[dict[str, Any]] = []
    lines = md.splitlines()
    i = 0
    while i < len(lines):
        raw = lines[i]
        line = raw.rstrip()

        if not line.strip():
            i += 1
            continue

        # Reject unsupported block types early.
        if line.startswith("```"):
            raise ValueError(
                "Code blocks aren't supported by the converter — handle "
                "manually or extend the script.")
        if line.startswith(">"):
            raise ValueError(
                "Blockquotes aren't supported by the converter — handle "
                "manually or extend the script.")
        if line.startswith("![") or line.startswith("<img"):
            raise ValueError(
                "Inline images need full Strapi upload metadata — handle "
                "manually via the existing upload flow.")

        # Headings
        m = re.match(r"^(#{2,3})\s+(.*)$", line)
        if m:
            level = len(m.group(1))
            blocks.append({
                "type": "heading",
                "level": level,
                "children": _inline_to_children(m.group(2)),
            })
            i += 1
            continue

        # Unordered list
        if re.match(r"^[-*]\s+\S", line):
            items: list[dict[str, Any]] = []
            while i < len(lines) and re.match(r"^[-*]\s+\S", lines[i].rstrip()):
                text = re.sub(r"^[-*]\s+", "", lines[i].rstrip())
                items.append({
                    "type": "list-item",
                    "children": _inline_to_children(text),
                })
                i += 1
            blocks.append({
                "type": "list",
                "format": "unordered",
                "children": items,
            })
            continue

        # Ordered list
        if re.match(r"^\d+\.\s+\S", line):
            items = []
            while i < len(lines) and re.match(r"^\d+\.\s+\S", lines[i].rstrip()):
                text = re.sub(r"^\d+\.\s+", "", lines[i].rstrip())
                items.append({
                    "type": "list-item",
                    "children": _inline_to_children(text),
                })
                i += 1
            blocks.append({
                "type": "list",
                "format": "ordered",
                "children": items,
            })
            continue

        # Paragraph: combine consecutive non-empty non-block lines
        para_lines = [line]
        i += 1
        while i < len(lines):
            nxt = lines[i].rstrip()
            if not nxt.strip():
                break
            if re.match(r"^(#{1,6}\s|[-*]\s|\d+\.\s|>|```|!\[)", nxt):
                break
            para_lines.append(nxt)
            i += 1
        para = " ".join(para_lines)
        blocks.append({
            "type": "paragraph",
            "children": _inline_to_children(para),
        })

    return blocks


def _inline_to_children(text: str) -> list[dict[str, Any]]:
    """Parse inline markdown into Strapi children array.

    Handles nesting: a link inside a bold span becomes a link child whose
    inner text node carries the bold flag (Strapi treats links as siblings
    of text at the inline level, not as wrappers).
    """
    children: list[dict[str, Any]] = []
    pos = 0
    for m in _INLINE_PATTERN.finditer(text):
        if m.start() > pos:
            children.append({"type": "text", "text": text[pos:m.start()]})
        if m.group(1):  # bold
            children.extend(_styled_children(m.group(2), bold=True))
        elif m.group(3):  # italic *
            children.extend(_styled_children(m.group(4), italic=True))
        elif m.group(5):  # italic _
            children.extend(_styled_children(m.group(6), italic=True))
        elif m.group(7):  # link
            children.append({
                "type": "link",
                "url": m.group(9),
                "children": [{"type": "text", "text": m.group(8)}],
            })
        pos = m.end()
    if pos < len(text):
        children.append({"type": "text", "text": text[pos:]})
    if not children:
        children.append({"type": "text", "text": ""})
    return children


def _styled_children(
    inner: str, *, bold: bool = False, italic: bool = False,
) -> list[dict[str, Any]]:
    """Render the inside of a bold/italic span, surfacing nested links."""
    out: list[dict[str, Any]] = []
    pos = 0
    for m in _LINK_PATTERN.finditer(inner):
        if m.start() > pos:
            out.append(_text_node(inner[pos:m.start()], bold, italic))
        out.append({
            "type": "link",
            "url": m.group(3),
            "children": [_text_node(m.group(2), bold, italic)],
        })
        pos = m.end()
    if pos < len(inner):
        out.append(_text_node(inner[pos:], bold, italic))
    return out


def _text_node(text: str, bold: bool, italic: bool) -> dict[str, Any]:
    node: dict[str, Any] = {"type": "text", "text": text}
    if bold:
        node["bold"] = True
    if italic:
        node["italic"] = True
    return node


# ---------------------------------------------------------------------------
# Strapi URL + auth
# ---------------------------------------------------------------------------

def resolve_strapi_url(env: str) -> str:
    if override := os.environ.get("STRAPI_URL"):
        return override.rstrip("/")
    if env == "dev":
        cmd = [
            "az", "containerapp", "show",
            "--name", "ca-strapi-tm-dev-westus2",
            "--resource-group", "rg-trashmob-dev-westus2",
            "--subscription", "39a254b7-c01a-45ab-bebd-4038ea4adea9",
            "--query", "properties.configuration.ingress.fqdn", "-o", "tsv",
        ]
    elif env == "prod":
        cmd = [
            "az", "containerapp", "show",
            "--name", "ca-strapi-tm-pr-westus2",
            "--resource-group", "rg-trashmob-pr-westus2",
            "--subscription", "5ea21946-8cb1-413f-9005-0ab10bfa839d",
            "--query", "properties.configuration.ingress.fqdn", "-o", "tsv",
        ]
    else:
        raise ValueError(f"Unknown env: {env}")

    # Resolve the az executable ourselves — on Windows `az` ships as `az.cmd`
    # (a batch shim), which bare `subprocess.run(["az", ...])` won't find.
    # `shutil.which` honours PATHEXT and returns the actual path.
    az_path = shutil.which(cmd[0])
    if az_path is None:
        raise FileNotFoundError(
            "Azure CLI ('az') not found on PATH. Install from https://aka.ms/azcli "
            "or ensure the shim is discoverable."
        )
    cmd[0] = az_path
    res = subprocess.run(cmd, capture_output=True, text=True, check=True)
    fqdn = res.stdout.strip()
    return f"https://{fqdn}"


_KV_BY_ENV = {
    "dev": ("kv-tm-dev-westus2", "39a254b7-c01a-45ab-bebd-4038ea4adea9"),
    "prod": ("kv-tm-pr-westus2", "5ea21946-8cb1-413f-9005-0ab10bfa839d"),
}


def _fetch_kv_secret(vault: str, subscription: str, name: str) -> str | None:
    """Return the value of a Key Vault secret, or None if it can't be read."""
    az_path = shutil.which("az")
    if az_path is None:
        return None
    cmd = [
        az_path, "keyvault", "secret", "show",
        "--vault-name", vault,
        "--subscription", subscription,
        "--name", name,
        "--query", "value", "-o", "tsv",
    ]
    try:
        res = subprocess.run(cmd, capture_output=True, text=True, check=True)
    except (subprocess.CalledProcessError, FileNotFoundError):
        return None
    value = res.stdout.strip()
    return value or None


def get_admin_credentials(env: str) -> tuple[str, str]:
    """Resolve Strapi admin credentials.

    Order of precedence:
      1. `STRAPI_ADMIN_EMAIL` / `STRAPI_ADMIN_PASSWORD` environment variables
      2. `Strapi-Admin-Email` / `Strapi-Admin-Password` secrets in the env's
         Key Vault (`kv-tm-dev-westus2` for dev, `kv-tm-pr-westus2` for prod).
         Requires `az login` on a principal with `get` on the vault.
      3. Interactive prompt as a last resort.
    """
    email = os.environ.get("STRAPI_ADMIN_EMAIL")
    password = os.environ.get("STRAPI_ADMIN_PASSWORD")

    if (not email or not password) and env in _KV_BY_ENV:
        vault, subscription = _KV_BY_ENV[env]
        if not email:
            email = _fetch_kv_secret(vault, subscription, "Strapi-Admin-Email")
        if not password:
            password = _fetch_kv_secret(vault, subscription, "Strapi-Admin-Password")
        if email and password:
            print(f"-> Loaded admin credentials from Key Vault ({vault})")

    if not email:
        email = input("Strapi admin email: ").strip()
    if not password:
        password = getpass.getpass("Strapi admin password: ")
    return email, password


def get_api_token(strapi_url: str, env: str) -> str:
    """Get a Strapi API token, caching it under ~/.cache/trashmob-strapi/."""
    CACHE_DIR.mkdir(parents=True, exist_ok=True)
    cache_file = CACHE_DIR / f"{env}-token.json"

    if cache_file.exists():
        try:
            cached = json.loads(cache_file.read_text())
            token = cached.get("token")
            if token and _ping_with_token(strapi_url, token):
                return token
        except (json.JSONDecodeError, KeyError):
            pass

    email, password = get_admin_credentials(env)
    print(f"-> Admin login as {email}")
    login = _post_json(f"{strapi_url}/admin/login",
                       {"email": email, "password": password})
    jwt = login["data"]["token"]

    print("-> Creating API token")
    token_res = _post_json(
        f"{strapi_url}/admin/api-tokens",
        {
            "name": f"upload-article-{env}-{os.getpid()}",
            "description": "Created by scripts/upload_article.py",
            "type": "full-access",
            "lifespan": None,
        },
        bearer=jwt,
    )
    token = token_res["data"]["accessKey"]
    cache_file.write_text(json.dumps({"token": token}))
    try:
        cache_file.chmod(0o600)
    except OSError:
        pass
    return token


def _ping_with_token(strapi_url: str, token: str) -> bool:
    """Return True if the token can read /api/news-categories."""
    try:
        _get_json(f"{strapi_url}/api/news-categories", bearer=token)
        return True
    except (urllib.error.HTTPError, urllib.error.URLError):
        return False


# ---------------------------------------------------------------------------
# HTTP helpers (urllib — CLAUDE.md mandates not using curl due to shell escapes)
# ---------------------------------------------------------------------------

def _request(method: str, url: str, body: Any = None, bearer: str | None = None) -> Any:
    headers = {"Accept": "application/json"}
    data: bytes | None = None
    if body is not None:
        data = json.dumps(body).encode("utf-8")
        headers["Content-Type"] = "application/json"
    if bearer:
        headers["Authorization"] = f"Bearer {bearer}"
    req = urllib.request.Request(url, data=data, headers=headers, method=method)
    try:
        with urllib.request.urlopen(req) as resp:  # noqa: S310 — trusted Strapi URL
            raw = resp.read().decode("utf-8")
    except urllib.error.HTTPError as err:
        # Surface the Strapi error body — otherwise the caller only sees
        # "HTTP Error 400: Bad Request" and has no idea which field is off.
        detail = ""
        try:
            detail = err.read().decode("utf-8")
        except Exception:
            pass
        print(f"HTTP {err.code} {err.reason} on {method} {url}", file=sys.stderr)
        if detail:
            print(f"Response body: {detail}", file=sys.stderr)
        raise
    if not raw:
        return {}
    return json.loads(raw)


def _post_json(url: str, body: Any, bearer: str | None = None) -> Any:
    return _request("POST", url, body, bearer)


def _put_json(url: str, body: Any, bearer: str | None = None) -> Any:
    return _request("PUT", url, body, bearer)


def _get_json(url: str, bearer: str | None = None) -> Any:
    return _request("GET", url, None, bearer)


# ---------------------------------------------------------------------------
# Strapi operations
# ---------------------------------------------------------------------------

def resolve_category_id(strapi_url: str, token: str, name: str) -> int:
    # Try cache first
    if (cid := KNOWN_CATEGORIES.get(name.lower())) is not None:
        return cid
    res = _get_json(f"{strapi_url}/api/news-categories", bearer=token)
    for row in res.get("data", []):
        attrs = row.get("attributes", row)
        if attrs.get("name", "").lower() == name.lower():
            return row["id"]
    raise ValueError(f"No category named {name!r} on Strapi")


def upload_cover_file(strapi_url: str, token: str, path: Path) -> int:
    """POST a file to Strapi's upload plugin and return the new file id.

    Uses `/api/upload` with a multipart body assembled by hand so we don't
    take on `requests` or `httpx` as a dependency. The response is a JSON
    array; we return the id of the first entry.
    """
    boundary = f"----trashmobcover{uuid.uuid4().hex}"
    mime = mimetypes.guess_type(path.name)[0] or "application/octet-stream"
    header = (
        f"--{boundary}\r\n"
        f'Content-Disposition: form-data; name="files"; filename="{path.name}"\r\n'
        f"Content-Type: {mime}\r\n\r\n"
    ).encode("utf-8")
    footer = f"\r\n--{boundary}--\r\n".encode("utf-8")
    body = header + path.read_bytes() + footer

    req = urllib.request.Request(
        f"{strapi_url}/api/upload",
        data=body,
        headers={
            "Content-Type": f"multipart/form-data; boundary={boundary}",
            "Authorization": f"Bearer {token}",
            "Accept": "application/json",
        },
        method="POST",
    )
    try:
        with urllib.request.urlopen(req) as resp:  # noqa: S310 — trusted Strapi URL
            data = json.loads(resp.read().decode("utf-8"))
    except urllib.error.HTTPError as err:
        detail = ""
        try:
            detail = err.read().decode("utf-8")
        except Exception:
            pass
        print(f"HTTP {err.code} {err.reason} on POST /api/upload", file=sys.stderr)
        if detail:
            print(f"Response body: {detail}", file=sys.stderr)
        raise

    if not isinstance(data, list) or not data:
        raise ValueError(f"unexpected /api/upload response: {data!r}")
    return int(data[0]["id"])


def set_file_alt_text(strapi_url: str, token: str, file_id: int, alt: str) -> None:
    """Set alternativeText on an uploaded file via the upload plugin.

    Strapi's upload plugin accepts a POST to `/api/upload?id={id}` with a
    `fileInfo` multipart field containing JSON. This is documented on the
    plugin's `updateFileInfo` action. Failures here are non-fatal — alt
    text can always be edited in the Strapi admin later.
    """
    boundary = f"----trashmobalt{uuid.uuid4().hex}"
    file_info = json.dumps({"alternativeText": alt}).encode("utf-8")
    body = (
        f"--{boundary}\r\n"
        f'Content-Disposition: form-data; name="fileInfo"\r\n\r\n'
    ).encode("utf-8") + file_info + f"\r\n--{boundary}--\r\n".encode("utf-8")

    req = urllib.request.Request(
        f"{strapi_url}/api/upload?id={file_id}",
        data=body,
        headers={
            "Content-Type": f"multipart/form-data; boundary={boundary}",
            "Authorization": f"Bearer {token}",
            "Accept": "application/json",
        },
        method="POST",
    )
    try:
        with urllib.request.urlopen(req) as resp:  # noqa: S310 — trusted Strapi URL
            resp.read()
    except urllib.error.HTTPError as err:
        print(f"warning: could not set alt text ({err.code} {err.reason}); "
              f"edit it manually in Strapi admin.", file=sys.stderr)


def post_article(strapi_url: str, token: str, article: Article,
                 category_id: int, cover_file_id: int | None = None) -> dict[str, Any]:
    data: dict[str, Any] = {
        "title": article.title,
        "slug": article.slug,
        "excerpt": article.excerpt,
        "author": article.author,
        "isFeatured": article.featured,
        "estimatedReadTime": article.estimated_read_time,
        "tags": article.tags,
        "category": {"connect": [category_id]},
        "body": article.body_blocks,
    }
    if cover_file_id is not None:
        data["coverImage"] = cover_file_id
    res = _post_json(f"{strapi_url}/api/news-posts", {"data": data}, bearer=token)
    return res["data"]


def publish_article(strapi_url: str, token: str, document_id: str) -> None:
    # Strapi v5: use documentId, not numeric id.
    import datetime
    now = datetime.datetime.now(datetime.timezone.utc).isoformat().replace("+00:00", "Z")
    _put_json(
        f"{strapi_url}/api/news-posts/{document_id}",
        {"data": {"publishedAt": now}},
        bearer=token,
    )


def verify_slug(strapi_url: str, token: str, slug: str) -> bool:
    q = urllib.parse.urlencode({"filters[slug][$eq]": slug})
    res = _get_json(f"{strapi_url}/api/news-posts?{q}", bearer=token)
    return bool(res.get("data"))


# ---------------------------------------------------------------------------
# CLI
# ---------------------------------------------------------------------------

def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[1])
    parser.add_argument("markdown", type=Path, help="Path to the article .md file")
    parser.add_argument("--env", choices=["dev", "prod"], default="dev",
                        help="Strapi environment (default: dev)")
    parser.add_argument("--draft", action="store_true",
                        help="Skip the publish step; leave the article as a draft")
    parser.add_argument("--dry-run", action="store_true",
                        help="Parse + convert + print the JSON payload, do not upload")
    parser.add_argument("--cover", type=Path, default=None,
                        help="Path to a cover image (1200x675 PNG per NewsArticles/CLAUDE.md). "
                             "Defaults to '<article-stem>-cover.png' alongside the article if it "
                             "exists.")
    parser.add_argument("--cover-alt", default=None,
                        help="Alternative text for the cover (defaults to the article title). "
                             "Used for accessibility and social preview cards.")
    parser.add_argument("--no-cover", action="store_true",
                        help="Skip cover upload even if a matching '-cover.png' exists next to "
                             "the article.")
    args = parser.parse_args()

    if not args.markdown.exists():
        print(f"error: {args.markdown} not found", file=sys.stderr)
        return 2

    print(f"-> Parsing {args.markdown}")
    article = parse_article(args.markdown)
    print(f"   title={article.title!r} slug={article.slug!r} category={article.category!r}")
    print(f"   {len(article.body_blocks)} block(s) generated")

    if args.dry_run:
        payload = {
            "data": {
                "title": article.title,
                "slug": article.slug,
                "excerpt": article.excerpt,
                "author": article.author,
                "isFeatured": article.featured,
                "estimatedReadTime": article.estimated_read_time,
                "tags": article.tags,
                "category": "RESOLVED-AT-UPLOAD-TIME",
                "body": article.body_blocks,
            }
        }
        json.dump(payload, sys.stdout, indent=2)
        print()
        return 0

    # Resolve the cover path before we go online, so mistakes fail fast.
    cover_path: Path | None = args.cover
    if cover_path is None and not args.no_cover:
        default_cover = args.markdown.with_name(f"{args.markdown.stem}-cover.png")
        if default_cover.exists():
            cover_path = default_cover
    if cover_path is not None and not cover_path.exists():
        print(f"error: --cover {cover_path} does not exist", file=sys.stderr)
        return 2

    print(f"-> Resolving Strapi URL ({args.env})")
    strapi_url = resolve_strapi_url(args.env)
    print(f"   {strapi_url}")

    token = get_api_token(strapi_url, args.env)

    print(f"-> Resolving category id for {article.category!r}")
    category_id = resolve_category_id(strapi_url, token, article.category)
    print(f"   id={category_id}")

    cover_file_id: int | None = None
    if cover_path is not None:
        print(f"-> POST /api/upload (cover: {cover_path.name})")
        cover_file_id = upload_cover_file(strapi_url, token, cover_path)
        alt_text = args.cover_alt or article.title
        set_file_alt_text(strapi_url, token, cover_file_id, alt_text)
        print(f"   fileId={cover_file_id} alt={alt_text!r}")

    print("-> POST /api/news-posts (creates draft)")
    created = post_article(strapi_url, token, article, category_id, cover_file_id)
    document_id = created.get("documentId") or created.get("attributes", {}).get("documentId")
    if not document_id:
        print("error: created article missing documentId", file=sys.stderr)
        json.dump(created, sys.stderr, indent=2)
        return 1
    print(f"   documentId={document_id}")

    if not args.draft:
        print(f"-> PUT /api/news-posts/{document_id} (publish)")
        publish_article(strapi_url, token, document_id)

    print(f"-> Verifying slug={article.slug!r} is queryable")
    if not verify_slug(strapi_url, token, article.slug):
        print("warning: slug not found via GET — Strapi may need a moment", file=sys.stderr)

    print("Done.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
